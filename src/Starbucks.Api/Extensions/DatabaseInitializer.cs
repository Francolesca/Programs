using System;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql.Internal;
using Npgsql.Replication;
using Starbucks.Api.Resources;
using Starbucks.Domain;
using Starbucks.Persistence;

namespace Starbucks.Api.Extensions;

public static class DatabaseInitializer
{
    public static async Task ApplyMigration(
        this IApplicationBuilder builder,
        IWebHostEnvironment? environment
    )
    {
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var service = scope.ServiceProvider;
            var loggerFactory = service.GetRequiredService<ILoggerFactory>();
            try
            {
                var context = service.GetRequiredService<StarbucksDbContext>();
                await context.Database.MigrateAsync();
                await SeedData (context, environment);
            }
            catch (System.Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "Error en la migracion");
            }
        }
    }
    private static async Task SeedData(
        StarbucksDbContext context,
        IWebHostEnvironment? environment
    )
    {
        if (context.Coffes.Any())
        {
            return;
        }
        if (environment is null)
        {
            throw new Exception("Enviroment is null");
        }

        var rootPath = environment.ContentRootPath;
        var fullPathCoffe = Path.Combine(rootPath, "Resources/Coffe.json");
        var coffeDataText = await File.ReadAllTextAsync(fullPathCoffe);
        var data = JsonConvert.DeserializeObject<List<CoffeJson>>(coffeDataText)
            ?? Enumerable.Empty<CoffeJson>();

        var coffes = data.Select(json => new Coffe
        {
            Id = json.CoffeId,
            Name = json.Title!,
            Description = json.Description,
            Price = 10,
            CategoryId = json.Category,
            Imagen = json.Image,
        }).ToArray();

        await context.Coffes.AddRangeAsync(coffes);
        await context.SaveChangesAsync();
    }
}
