using System.Reflection;
using Core.mediatOR.Contracts;
using FluentValidation;
using FluentValidation.Results;
using Starbucks.Application.Exceptions;

namespace Starbucks.Application.Abstractions;

public class ValidationBehavior<TRequest, TResponse>
: IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(
                _validators
                .Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = results.SelectMany(r => r.Errors)
            .Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                var resultType = typeof(TResponse);
                if (resultType.IsGenericType
                    && resultType.GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var validationFailureMethod = resultType
                                    .GetMethod(
                                        "ValidationFailure",
                                        BindingFlags.Public |
                                        BindingFlags.Static
                                    );


                    if (validationFailureMethod is not null)
                    {
                        return (TResponse)validationFailureMethod
                        .Invoke(null, new object[] { failures })!;
                    }
                }

                throw new InvalidOperationException(
                "El TResponse debe ser Result<T> para usar ValidationBehavior"
                );

            }

        }

        return await next();
       
    }
}
