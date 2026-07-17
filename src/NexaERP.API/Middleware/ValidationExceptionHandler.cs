using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace NexaERP.API.Middleware;

/// <summary>
/// Handles FluentValidation validation exceptions.
/// </summary>
public sealed class ValidationExceptionHandler(
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Ignore non-validation exceptions.
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        // Create Problem Details response.
        var context = new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new()
            {
                Detail = "One or more validation error occured",
                Status = StatusCodes.Status400BadRequest,
            }
        };

        // Group validation errors by property.
        var errors = validationException.Errors
             .GroupBy(x => x.PropertyName)
             .ToDictionary(
                 g => g.Key.ToLower(),
                 g => g.Select(e => e.ErrorMessage).ToArray());

        context.ProblemDetails.Extensions.Add("errors", errors);

        return await problemDetailsService.TryWriteAsync(context);
    }
}
