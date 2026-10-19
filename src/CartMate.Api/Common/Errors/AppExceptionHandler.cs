using CartMate.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace CartMate.Api.Common.Errors;

/// <summary>
/// Turns known exceptions into ProblemDetails. Unknown exceptions are left to the default
/// handler, which returns a generic 500 without leaking stack traces.
/// </summary>
public sealed class AppExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails? problem = exception switch
        {
            ValidationException validation =>
                ValidationProblems.FromFailures(validation.Errors.ToList(), httpContext, StatusCodes.Status400BadRequest),
            DomainException domain => FromDomain(domain),
            DbUpdateConcurrencyException => Create(
                StatusCodes.Status409Conflict,
                "concurrency_conflict",
                "Someone else changed this item. Reload the list and try again."),
            _ => null,
        };

        if (problem is null)
        {
            return false;
        }

        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception,
        });
    }

    private static ProblemDetails FromDomain(DomainException exception)
    {
        var status = exception.Code switch
        {
            // "Not a member" is reported as 404, not 403: outsiders can't learn which lists exist.
            "list_not_found" or "item_not_found" or "invite_not_found" or "not_a_member" => StatusCodes.Status404NotFound,
            "invalid_credentials" => StatusCodes.Status401Unauthorized,
            "not_owner" => StatusCodes.Status403Forbidden,
            "list_full" or "email_taken" => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest,
        };

        var detail = status == StatusCodes.Status404NotFound
            ? "The requested resource was not found."
            : exception.Message;

        return Create(status, exception.Code, detail);
    }

    private static ProblemDetails Create(int status, string code, string detail) => new()
    {
        Status = status,
        Title = ReasonPhrases.GetReasonPhrase(status),
        Detail = detail,
        Extensions = { ["code"] = code },
    };
}
