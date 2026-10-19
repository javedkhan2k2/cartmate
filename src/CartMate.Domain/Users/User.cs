using CartMate.Domain.Common;

namespace CartMate.Domain.Users;

public sealed class User
{
    public const int MaxEmailLength = 254;
    public const int MaxDisplayNameLength = 50;

    private User()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public string Email { get; private set; } = null!;

    /// <summary>Upper-cased email used for case-insensitive, unique lookups.</summary>
    public string NormalizedEmail { get; private set; } = null!;

    public string DisplayName { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public static User Create(string email, string displayName, string passwordHash, DateTimeOffset now)
    {
        var trimmedEmail = Guard.RequiredText(email, MaxEmailLength, "email");

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("password_hash_required", "A password hash is required.");
        }

        return new User
        {
            Id = Guid.CreateVersion7(now),
            Email = trimmedEmail,
            NormalizedEmail = NormalizeEmail(trimmedEmail),
            DisplayName = Guard.RequiredText(displayName, MaxDisplayNameLength, "display_name"),
            PasswordHash = passwordHash,
            CreatedAt = now,
        };
    }

    public static string NormalizeEmail(string email) => email.Trim().ToUpperInvariant();
}
