using FunBooksAndVideos.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FunBooksAndVideos.API.Exceptions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception for {Path}", context.Request.Path);

        var (statusCode, title) = exception switch
        {
            ProductNotFoundException => (
                StatusCodes.Status400BadRequest,
                "Invalid product"),

            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Invalid request"),

            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred")
        };

        // A better, standardized way to structure unhappy response data in .NET Core applications
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = statusCode == StatusCodes.Status500InternalServerError
                ? "Please contact support with the trace ID."
                : exception.Message,
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}