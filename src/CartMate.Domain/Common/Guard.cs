namespace CartMate.Domain.Common;

/// <summary>
/// Small helpers that enforce invariants. Input validation also happens earlier in the
/// API (FluentValidation), but the domain never trusts its callers.
/// </summary>
internal static class Guard
{
    public static string RequiredText(string? value, int maxLength, string field)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            throw new DomainException($"{field}_required", $"The {field} is required.");
        }

        if (trimmed.Length > maxLength)
        {
            throw new DomainException($"{field}_too_long", $"The {field} must be at most {maxLength} characters.");
        }

        return trimmed;
    }

    public static string? OptionalText(string? value, int maxLength, string field)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return null;
        }

        if (trimmed.Length > maxLength)
        {
            throw new DomainException($"{field}_too_long", $"The {field} must be at most {maxLength} characters.");
        }

        return trimmed;
    }

    public static int InRange(int value, int min, int max, string field)
    {
        if (value < min || value > max)
        {
            throw new DomainException($"{field}_out_of_range", $"The {field} must be between {min} and {max}.");
        }

        return value;
    }
}
