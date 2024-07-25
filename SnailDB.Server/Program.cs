using Microsoft.EntityFrameworkCore;
using SnailDB.Server.Data;
using SnailDB.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IRelicService, RelicService>();
builder.Services.AddScoped<IRelicWikiScrape, RelicWikiScrape>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

/*if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}*/
app.UseHttpsRedirection();

/*using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var wikiScrape = services.GetRequiredService<IRelicWikiScrape>();

    // Check if the database is empty and seed if necessary
    if (!context.Relics.Any()) {
        Console.WriteLine("Seeding database with relics...");
        var failedRelics = wikiScrape.SeedRelicsAsync().GetAwaiter().GetResult();
        if (failedRelics.Any()) {
            Console.WriteLine("Failed to seed some relics:");
            foreach (var relic in failedRelics) {
                Console.WriteLine($"- {relic.Item1}: {relic.Item2}");
            }
        } else {
            Console.WriteLine("All relics seeded successfully.");
        }

        Console.WriteLine("Seeding relic images...");
        wikiScrape.SeedRelicImagesAsync().GetAwaiter().GetResult();
        Console.WriteLine("Relic images seeded successfully.");
    }
}*/

app.MapGet("api/Relic/GetRelics", async (IRelicService service) => {
    var relics = await service.GetRelicsAsync();
    return relics != null ? Results.Ok(relics) : Results.StatusCode(500);
})
    .WithName("GetRelics")
    .WithOpenApi();

app.MapGet("api/Relic/GetRelic/{id}", async (int id, IRelicService service) => {
    var relic = await service.GetRelicAsync(id);
    return relic != null ? Results.Ok(relic) : Results.NotFound();
})
    .WithName("GetRelic")
    .WithOpenApi();

app.MapFallbackToFile("/index.html");

app.Run();
