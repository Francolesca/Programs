using System;
using Microsoft.EntityFrameworkCore;
using Npgsql.Replication;
using Starbucks.Persistence;

namespace Starbucks.Api.Extensions;

public static class DatabaseInitializer
{
    public static async Task ApplyMigration(
        this IApplicationBuilder builder,
        IWebHostEnvironment? environment
    ) {
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var service = scope.ServiceProvider;
            var loggerFactory = service.GetRequiredService<ILoggerFactory>();
            try
            {
                var context = service.GetRequiredService<StarbucksDbContext>();
                await context.Database.MigrateAsync();
            }
            catch (System.Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "Error en la migracion");
            }
        }
    }
}
