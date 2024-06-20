namespace SnailDB.Server.Services {
    public interface IRelicWikiScrape {
        Task<List<(string, string)>> SeedRelicsAsync();
        Task SeedRelicImagesAsync();
    }
}
