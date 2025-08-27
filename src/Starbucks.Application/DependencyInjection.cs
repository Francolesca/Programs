using System;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Core.Mappy.Extensions;
using Core.mediatOR;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Starbucks.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMediatOR(typeof(DependencyInjection).Assembly);
        services.AddMapper();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped(sp =>
        {
            return new TableServiceClient(
                    configuration.GetConnectionString("StorageAzure")
                );
        });
        services.AddScoped(sp =>
        {
            return new BlobServiceClient(
                    configuration.GetConnectionString("StorageAzure")
                );
        });
        return services;
    }
}
