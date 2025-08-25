using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Starbucks.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<StarbucksDbContext>(opt =>
        {
            //opt.UseSqlite(configuration.GetConnectionString("SqlLiteDatabase"));
            opt.UseNpgsql(configuration.GetConnectionString("PostgresDatabase"))
                .UseSnakeCaseNamingConvention();

        });

        return services;
    }
}
