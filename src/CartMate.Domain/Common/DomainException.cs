namespace CartMate.Domain.Common;

/// <summary>
/// Thrown when a business rule is violated. <see cref="Code"/> is a stable,
/// machine-readable identifier the API returns to clients (e.g. "list_full").
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException()
        : this("domain_error", "A business rule was violated.")
    {
    }

    public DomainException(string message)
        : this("domain_error", message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
        Code = "domain_error";
    }

    public DomainException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
