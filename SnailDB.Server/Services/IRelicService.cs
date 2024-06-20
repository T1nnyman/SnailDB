using SnailDB.Server.Models;

namespace SnailDB.Server.Services {
    public interface IRelicService {
        Task<List<Relic>?> GetRelicsAsync();
        Task<Relic?> GetRelicAsync(int id);
    }
}
