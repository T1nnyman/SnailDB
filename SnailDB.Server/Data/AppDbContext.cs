using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using SnailDB.Server.Models;

namespace SnailDB.Server.Data {
    public class AppDbContext : DbContext {
        private readonly IConfiguration _config;

        public AppDbContext(IConfiguration config, DbContextOptions<AppDbContext> options) : base(options) {
            _config = config;
        }

        public DbSet<Relic> Relics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite(_config.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<Relic>().HasIndex(r => r.Name).IsUnique();

            // Comparer for Dictionary<string, RelicStats>
            var statsComparer = new ValueComparer<Dictionary<string, RelicStats>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToDictionary(entry => entry.Key, entry => entry.Value));

            var skillsComparer = new ValueComparer<Dictionary<string, RelicBadgeSkill>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToDictionary(entry => entry.Key, entry => entry.Value));

            builder.Entity<Relic>().Property(e => e.Stats).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<string, RelicStats>>(v)!)
            .Metadata
            .SetValueComparer(statsComparer);

            builder.Entity<Relic>().Property(e => e.Skills).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<string, RelicBadgeSkill>>(v)!)
            .Metadata
            .SetValueComparer(skillsComparer);
        }
    }
}
