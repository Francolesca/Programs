using System;
using Core.mediatOR.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Core.mediatOR;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var handlerType = typeof(IRequestHandler<,>)
                            .MakeGenericType(
                                request.GetType(),
                                typeof(TResponse)
                                );

        dynamic handler = _serviceProvider.GetRequiredService(handlerType);
        if (handler is null)
        {
            throw new InvalidOperationException(
                $"Handler for Request type {request.GetType().Name} not found");
        }
        return await handler.Handle((dynamic)request, cancellationToken);
    }
}
