using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NexaERP.API.Middleware;

/// <summary>
/// Handles FluentValidation validation exceptions.
/// </summary>
public sealed class ValidationExceptionHandler(
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Handle optimistic concurrency conflicts.
        if (exception is DbUpdateConcurrencyException)
        {
            return problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Concurrency Conflict",
                    Detail = "The resource was modified by another user. Please reload the resource and try again."
                }
            });
        }

        // Return a generic 500 Problem Details response.
        return problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An error occurred while processing your request. Please try again."
            }
        });
    }
}
