using System;
using Core.mediatOR;
using Microsoft.Extensions.DependencyInjection;

namespace Starbucks.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services
    )
    {
        services.AddMediatOR(typeof(DependencyInjection).Assembly);

        return services;
    }
}
