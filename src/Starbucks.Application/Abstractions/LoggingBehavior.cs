using System;
using Core.mediatOR.Contracts;
using Microsoft.Extensions.Logging;

namespace Starbucks.Application.Abstractions;

public class LoggingBehavior<TRequest, TResponse>
: IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
{

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger
    )
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request,
    CancellationToken cancellationToken,
    RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogInformation(
            "Starting the operation/consultation {RequestName}",
            typeof(TRequest).Name
            );
        var res = await next();

        _logger.LogInformation(
            "Finishing the operation/consultation {RequestName}",
            typeof(TRequest).Name
            );
        return res;
    }
}
