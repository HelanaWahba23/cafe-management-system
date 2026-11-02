using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Api.Infrastructure.Persistence;

public static class DemoMenuSeeder
{
    public static async Task SeedAsync(CafeDbContext database)
    {
        // Never hide or change an operator's existing catalog.
        if (await database.MenuCategories.AnyAsync())
        {
            Console.WriteLine("Menu already exists; demo seeding skipped.");
            return;
        }

        await using var stream = typeof(DemoMenuSeeder).Assembly
            .GetManifestResourceStream("CafeManagement.DemoMenu.sql")
            ?? throw new InvalidOperationException("The demo menu resource is missing.");
        using var reader = new StreamReader(stream);
        await database.Database.ExecuteSqlRawAsync(await reader.ReadToEndAsync());
        Console.WriteLine("Demo menu installed: 6 categories and 27 dishes.");
    }
}
