using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using SnailDB.Server.Data;
using SnailDB.Server.Models;
using System.Diagnostics;

namespace SnailDB.Server.Services {
    public class RelicWikiScrape : IRelicWikiScrape {
        private readonly AppDbContext _context;
        private readonly ILogger<RelicWikiScrape> _logger;

        public RelicWikiScrape(AppDbContext context, ILogger<RelicWikiScrape> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task<List<(string, string)>> SeedRelicsAsync() {
            List<string> relicWikiUrls = GetRelicWikiUrls();
            List<(string, string)> failedRelics = new List<(string, string)>();
            Stopwatch totalTime = Stopwatch.StartNew();

            for (int i = 0; i < relicWikiUrls.Count; i++) {
                string url = relicWikiUrls[i];

                if (url == "https://supersnail.wiki.gg/wiki/Resort_Postcard")
                    url = "https://supersnail.wiki.gg/wiki/Resort_postcard"; // the wiki cant capitalize...

                // Check if the relic already exists in the database
                if (await _context.Relics.AnyAsync(r => r.WikiUrl == url) == true) {
                    Console.WriteLine($"Skipping Relic {i + 1}/{relicWikiUrls.Count} as it already exists.");
                    continue;
                }

                Stopwatch stopwatch = Stopwatch.StartNew();
                HtmlDocument doc = LoadHtmlDocument(url);

                // Get the relic name, image, rank, affct, source, and description
                string? name = GetBasicRelicInfo(doc, "name");
                string? imageUri = GetBasicRelicInfo(doc, "image");
                string? rank = GetBasicRelicInfo(doc, "rank");
                string? type = GetBasicRelicInfo(doc, "affct");
                string source = GetRelicContent(doc, "Source");
                string description = GetRelicContent(doc, "Description");

                // If any of the relic info is missing, skip the relic and increment the failed count
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(imageUri) || string.IsNullOrEmpty(rank) || string.IsNullOrEmpty(type)) {
                    failedRelics.Add((url, "Step 1: Info"));
                    continue;
                }

                // Get the relic stats
                Dictionary<string, RelicStats>? stats = GetRelicStats(doc);

                // If the stats are missing, skip the relic and increment the failed count
                if (stats == null || stats.Count == 0) {
                    failedRelics.Add((url, "Step 2: Stats"));
                    continue;
                }

                // Get the relic skills if the rank is not Green
                Dictionary<string, RelicBadgeSkill>? skills = null;
                if (rank != "Green") {
                    skills = GetRelicSkills(doc);

                    // If the skills are missing, skip the relic and increment the failed count
                    if (skills == null || skills.Count == 0) {
                        failedRelics.Add((url, "Step 3: Skills"));
                        continue;
                    }
                }

                try {
                    await _context.Relics.AddAsync(new Relic {
                        Name = name,
                        Description = description,
                        Source = source,
                        ImageUri = imageUri,
                        WikiUrl = url,
                        Type = type,
                        Rank = rank,
                        Stats = stats,
                        Skills = skills
                    });
                    await _context.SaveChangesAsync();
                } catch (Exception ex) {
                    _logger.LogError(ex, $"Failed to seed relic {name}");
                    failedRelics.Add((url, "Database Error"));
                }
                stopwatch.Stop();
                Console.WriteLine($"Processed {i + 1}/{relicWikiUrls.Count} relics. Time Taken: {stopwatch.ElapsedMilliseconds}ms");
            }

            totalTime.Stop();
            Console.WriteLine($"Finished Processing Relics - Time: {totalTime.ElapsedMilliseconds}ms");

            return failedRelics;
        }

        public async Task SeedRelicImagesAsync() {
            string baseUrl = "https://supersnail.wiki.gg";
            List<Relic>? relics = await _context.Relics.ToListAsync();
            if (relics == null)
                throw new Exception("No relics found in the database.");

            Stopwatch stopwatch = Stopwatch.StartNew();

            // Updates the image URI for each relic in the database to add the base URL
            for (int i = 0; i < relics.Count; i++) {
                Relic relic = relics[i];

                if (!string.IsNullOrEmpty(relic.ImageUri))
                    relic.ImageUri = baseUrl + relic.ImageUri;

                if (relic.Skills != null) {
                    for (int j = 0; j < relic.Skills.Count; j++) {
                        if (!string.IsNullOrEmpty(relic.Skills[j.ToString()].BadgeUri))
                            relic.Skills[j.ToString()].BadgeUri = baseUrl + relic.Skills[j.ToString()].BadgeUri;
                    }
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Time taken to update relic image URIs: {stopwatch.ElapsedMilliseconds}ms");
            stopwatch.Restart();

            // Downloads the images for each relic and the skills badges
            using (HttpClient client = new HttpClient()) {
                for (int i = 0; i < relics.Count; i++) {
                    Relic relic = relics[i];

                    if (!string.IsNullOrEmpty(relic.ImageUri))
                        await DownloadImageAsync(client, relic.ImageUri);

                    if (relic.Skills != null) {
                        for (int j = 0; j < relic.Skills.Count; j++) {
                            if (!string.IsNullOrEmpty(relic.Skills[j.ToString()].BadgeUri))
                                await DownloadImageAsync(client, relic.Skills[j.ToString()].BadgeUri);
                        }
                    }
                    Console.WriteLine($"Downloaded images for relic {i + 1}/{relics.Count}");
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Time taken to download relic images: {stopwatch.ElapsedMilliseconds}ms");
        }

        private List<string> GetRelicWikiUrls() {
            string url = "https://supersnail.wiki.gg/wiki/Relics";
            List<string> relicWikiUrls = new List<string>();
            HtmlDocument doc = LoadHtmlDocument(url);
            if (doc != null) {
                HtmlNodeCollection relicRows = doc.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable sortable')]/tbody/tr");
                if (relicRows != null) {
                    for (int i = 0; i < relicRows.Count; i++) {
                        HtmlNode row = relicRows[i];
                        HtmlNodeCollection cells = row.SelectNodes(".//td");
                        if (cells != null && cells.Any()) {
                            string relicWikiUrl = "https://supersnail.wiki.gg" + cells[0].SelectSingleNode(".//a").GetAttributeValue("href", "");
                            relicWikiUrls.Add(relicWikiUrl);
                        }
                    }
                }
            }
            return relicWikiUrls;
        }

        private string? GetBasicRelicInfo(HtmlDocument doc, string dataId) {
            switch (dataId) {
                case "image":
                    HtmlNode imageNode = doc.DocumentNode.SelectSingleNode($"//aside[contains(@class, 'portable-infobox')]/figure[@data-source='{dataId}']/a/img");
                    // Constructs and returns the image URI if the node is found, otherwise returns null
                    return imageNode != null ? imageNode.GetAttributeValue("src", null).Replace("%26", "&").Replace("%2C", ",") : null;
                case "name":
                    HtmlNode nameNode = doc.DocumentNode.SelectSingleNode($"//aside[contains(@class, 'portable-infobox')]/h2");
                    // Returns the inner text if found, otherwise returns null
                    return nameNode != null ? nameNode.InnerText.Trim().Replace("&amp;", "&") : null;
                default: // Used for Rank and Type
                    HtmlNode dataNode = doc.DocumentNode.SelectSingleNode($"//aside[contains(@class, 'portable-infobox')]/div[@data-source='{dataId}']/div");
                    // Returns the inner text if found, otherwise returns null
                    return dataNode != null ? dataNode.InnerText.Trim() : null;
            }
        }

        private string GetRelicContent(HtmlDocument doc, string id) {
            string content = "";
            HtmlNode? spanNode = doc.DocumentNode.SelectSingleNode($"//span[@id='{id}']");
            HtmlNode? h2Node = spanNode?.ParentNode;
            HtmlNode? node = h2Node?.SelectSingleNode("./following-sibling");
            // If the node is a paragraph, return the inner text, otherwise return the inner text of each list item separated by a comma
            if (node != null && node.Name == "p")
                content = node.InnerText.Trim().Replace("&amp;", "&").Replace("&gt;", ">");
            else if (node != null && node.Name == "ul") {
                var liNodes = node.SelectNodes(".//li");
                if (liNodes != null) {
                    foreach (var liNode in liNodes) {
                        if (!string.IsNullOrEmpty(content)) {
                            content += ", ";
                        }
                        content += liNode.InnerText.Trim().Replace("&amp;", "&").Replace("&gt;", ">");
                    }
                }
            }

            return content;
        }

        private Dictionary<string, RelicStats>? GetRelicStats(HtmlDocument doc) {
            Dictionary<string, RelicStats>? statsDict = new Dictionary<string, RelicStats>();
            HtmlNode? statsTableNode = doc.DocumentNode.SelectSingleNode("//span[@id='Stats']/following::table[@class='wikitable']");

            if (statsTableNode != null) {
                // Selects all rows except the first and any with a display:none style
                var rows = statsTableNode.SelectNodes(".//tr[position() > 1 and not(@style='display: none;')]");
                if (rows != null) {
                    for (int i = 0; i < rows.Count; i++) {
                        var row = rows[i];
                        var cells = row.SelectNodes(".//td");
                        if (cells != null && cells.Count == 7) {
                            string stars = Convert.ToString(CountFilledStars(cells[0].InnerText.Trim()));
                            var stats = new RelicStats {
                                Fame = cells[1].InnerText.Trim(),
                                Art = cells[2].InnerText.Trim(),
                                Faith = cells[3].InnerText.Trim(),
                                Civ = cells[4].InnerText.Trim(),
                                Tech = cells[5].InnerText.Trim(),
                                Special = cells[6].InnerHtml.Trim().Split(new[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(stat => stat.Trim().Replace("&amp;", "&").Replace("&gt;", ">")).ToList()
                            };

                            statsDict.Add(stars, stats);
                        }
                    }
                }
            }
            // Returns the stats dictionary if it contains any entries, otherwise returns an empty dictionary
            return statsDict.Count > 0 ? statsDict : new Dictionary<string, RelicStats>();
        }

        private Dictionary<string, RelicBadgeSkill>? GetRelicSkills(HtmlDocument doc) {
            Dictionary<string, RelicBadgeSkill> skills = new Dictionary<string, RelicBadgeSkill>();
            HtmlNode? spanNode = doc.DocumentNode.SelectSingleNode("//span[@id='Skills']");
            HtmlNode? h2Node = spanNode?.ParentNode;
            HtmlNode? currentNode = h2Node?.NextSibling;
            int current = 0;

            // Iterates through the sibling nodes until an <h2> node is found
            while (currentNode != null && currentNode.Name != "h2") {
                if (currentNode.Name == "p") {
                    string skillText = currentNode.InnerText.Trim();
                    string badgeUri = currentNode.SelectSingleNode(".//a/img").GetAttributeValue("src", "").Replace("%26", "&");

                    RelicBadgeSkill skill = new RelicBadgeSkill {
                        Skill = skillText,
                        BadgeUri = badgeUri
                    };
                    skills.Add(Convert.ToString(current++), skill);
                }
                // Moves to the next sibling node
                currentNode = currentNode.NextSibling;
            }

            // Returns the Dictionary of skills if any where found, otherwise returns and empty Dict
            return skills.Count > 0 ? skills : new Dictionary<string, RelicBadgeSkill>();
        }

        private async Task DownloadImageAsync(HttpClient client, string imageUrl) {
            string imagePath = GetImageUri(imageUrl);
            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "relics", Path.GetDirectoryName(imagePath) ?? string.Empty);
            string fullImagePath = Path.Combine(directory, Path.GetFileName(imagePath));

            try {
                // Ensure the directory exists
                if (!Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }
                if (!File.Exists(fullImagePath)) {
                    var response = await client.GetAsync(imageUrl);
                    if (response.IsSuccessStatusCode) {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(fullImagePath, FileMode.Create, FileAccess.Write, FileShare.None)) {
                            await stream.CopyToAsync(fileStream);
                        }
                    }
                }
            } catch (Exception ex) {
                _logger.LogWarning($"Failed to download {imageUrl}: {ex.Message}");
            }
        }

        private string GetImageUri(string imageUrl) {
            Uri uri = new Uri(imageUrl);
            return uri.LocalPath.TrimStart('/').Replace("/", "\\");
        }

        private HtmlDocument LoadHtmlDocument(string url) {
            HtmlWeb web = new HtmlWeb();
            return web.Load(url);
        }

        private string CountFilledStars(string stars) {
            if (stars == "Awakened")
                return "Awakened";
            return stars.Count(c => c == '★').ToString();
        }
    }
}
