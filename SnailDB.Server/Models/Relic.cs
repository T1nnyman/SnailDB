using System.ComponentModel.DataAnnotations.Schema;

namespace SnailDB.Server.Models {
    [Table("relics")]
    public class Relic {
        public int ID { get; set; }
        public required string Name { get; set; }
        public string? Source { get; set; }
        public required string Description { get; set; }
        public required string ImageUri { get; set; }
        public required string WikiUrl { get; set; }
        public required string Type { get; set; }
        public required string Rank { get; set; }
        public required Dictionary<string, RelicStats> Stats { get; set; }
        public Dictionary<string, RelicBadgeSkill>? Skills { get; set; }
    }

    public class RelicStats {
        public required string Fame { get; set; }
        public required string Art { get; set; }
        public required string Faith { get; set; }
        public required string Civ { get; set; }
        public required string Tech { get; set; }
        public required List<string> Special { get; set; }
    }

    public class RelicBadgeSkill {
        public required string Skill { get; set; }
        public required string BadgeUri { get; set; }
    }
}
