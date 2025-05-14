using Mnf_Portal.Core.Entities;
using System.Text.Json;

namespace Mnf_Portal.Infrastructure.Persistence
{
    public static class MnfDbContextSeed
    {
        public static async Task SeedingAsync(MnfDbContext context)
        {
            if (!context.News.Any())
            {
                var newsData = File.ReadAllText("../Mnf Portal.Infrastructure/DataSeeding/newsData.json");
                var news = JsonSerializer.Deserialize<List<PortalNews>>(newsData);
                if (news is { Count: > 0 }) // Check If News Is Not Null and Count > 0
                {
                    foreach (var item in news)
                    {
                        await context.Set<PortalNews>().AddAsync(item);
                    }
                    await context.SaveChangesAsync();
                }
            }

            if (!context.Translations.Any())
            {
                var newsTransData = File.ReadAllText("../Mnf Portal.Infrastructure/DataSeeding/TranslationData.json");
                var newsTrans = JsonSerializer.Deserialize<List<NewsTranslation>>(newsTransData);
                if (newsTrans is { Count: > 0 }) // Check If Translation Is Not Null and Count > 0
                {
                    foreach (var item in newsTrans)
                    {
                        await context.Set<NewsTranslation>().AddAsync(item);
                    }
                    //await context.Translations.AddRangeAsync(newsTrans);
                    //await context.SaveChangesAsync();
                    await context.SaveChangesAsync();
                }
            }

            if (!context.Gallaries.Any())
            {
                var gallariesData = File.ReadAllText("../Mnf Portal.Infrastructure/DataSeeding/GallaryData.json");
                var gallaries = JsonSerializer.Deserialize<List<NewsGallary>>(gallariesData);
                if (gallaries is { Count: > 0 }) // Check If Gallary Is Not Null and Count > 0
                {
                    foreach (var item in gallaries)
                    {
                        await context.Set<NewsGallary>().AddAsync(item);
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
