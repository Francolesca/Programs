using System;
using System.Reflection;
using Core.mediatOR.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Core.mediatOR;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatOR(
        this IServiceCollection services,
        params Assembly[] assemblies
    )
    {
        services.AddScoped<IMediator, Mediator>();

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IRequestHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime()
        );
        return services;
    }
}
