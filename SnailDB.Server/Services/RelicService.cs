using Microsoft.EntityFrameworkCore;
using SnailDB.Server.Data;
using SnailDB.Server.Models;

namespace SnailDB.Server.Services {
    public class RelicService : IRelicService {
        private readonly AppDbContext _context;
        private readonly ILogger<RelicService> _logger;

        public RelicService(AppDbContext context, ILogger<RelicService> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Relic>?> GetRelicsAsync() {
            try {
                return await _context.Relics.ToListAsync();
            } catch (Exception ex) {
                _logger.LogError(ex, "Error getting relics");
                return null;
            }
        }

        public async Task<Relic?> GetRelicAsync(int id) {
            try {
                return await _context.Relics.FindAsync(id);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error getting relic");
                return null;
            }
        }
    }
}
