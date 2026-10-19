using CartMate.Application.Common.Auth;
using Microsoft.AspNetCore.Identity;

namespace CartMate.Infrastructure.Auth;

/// <summary>
/// Uses ASP.NET Core Identity's PBKDF2 hasher (salted, versioned format) without the rest of
/// Identity. Never write your own password hashing.
/// </summary>
public sealed class IdentityPasswordHasher : IPasswordHasher
{
    // The hasher's TUser parameter is unused by the default implementation.
    private static readonly object _unusedUser = new();
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(_unusedUser, password);

    public bool Verify(string passwordHash, string password) =>
        _hasher.VerifyHashedPassword(_unusedUser, passwordHash, password) != PasswordVerificationResult.Failed;
}
