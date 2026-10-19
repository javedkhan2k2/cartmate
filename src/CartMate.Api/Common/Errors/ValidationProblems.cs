using System.Text.Json;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace CartMate.Api.Common.Errors;

/// <summary>
/// Builds the standard 400 response. Used both by FastEndpoints (binding errors) and by
/// AppExceptionHandler (FluentValidation errors), so every validation error looks the same.
/// </summary>
internal static class ValidationProblems
{
    public static ValidationProblemDetails FromFailures(List<ValidationFailure> failures, HttpContext context, int statusCode)
    {
        var errors = failures
            .GroupBy(f => JsonNamingPolicy.CamelCase.ConvertName(f.PropertyName))
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).Distinct().ToArray());

        return new ValidationProblemDetails(errors)
        {
            Status = statusCode,
            Title = "One or more validation errors occurred.",
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Instance = context.Request.Path,
            Extensions =
            {
                ["code"] = "validation_failed",
                ["traceId"] = context.TraceIdentifier,
            },
        };
    }
}
