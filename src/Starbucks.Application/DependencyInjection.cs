using System;
using Core.Mappy.Extensions;
using Core.mediatOR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Starbucks.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services
    )
    {
        services.AddMediatOR(typeof(DependencyInjection).Assembly);
        services.AddMapper();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}
