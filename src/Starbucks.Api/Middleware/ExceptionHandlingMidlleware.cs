using System;
using Starbucks.Application.Abstractions;
using Starbucks.Domain.Abstractions;

namespace Starbucks.Api.Middleware;

public class ExceptionHandlingMidlleware
{
    private readonly RequestDelegate _next;


    private readonly ILogger<ExceptionHandlingMidlleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMidlleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMidlleware> logger,
        IHostEnvironment env
        )
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "This error is a exception.");

            if (e is Application.Exceptions.ValidationException validationEx)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(validationEx.Errors);
                return;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var error = new Error(
                "UnexpectedError",
                _env.IsDevelopment()
                ? e.ToString()
                : "An unexpected error has ocurred."
            );

            await context.Response.WriteAsJsonAsync(error);
        }
    }

}
