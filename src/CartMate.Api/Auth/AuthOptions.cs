namespace CartMate.Api.Auth;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    /// <summary>HMAC-SHA256 key. At least 32 characters (256 bits). Never committed for real environments.</summary>
    public string SigningKey { get; init; } = string.Empty;

    public string Issuer { get; init; } = "cartmate-api";

    public string Audience { get; init; } = "cartmate-web";

    public int TokenLifetimeHours { get; init; } = 168;

    public void Validate()
    {
        if (SigningKey.Length < 32)
        {
            throw new InvalidOperationException(
                $"'{SectionName}:SigningKey' must be configured and at least 32 characters long.");
        }

        if (TokenLifetimeHours <= 0)
        {
            throw new InvalidOperationException($"'{SectionName}:TokenLifetimeHours' must be positive.");
        }
    }
}
