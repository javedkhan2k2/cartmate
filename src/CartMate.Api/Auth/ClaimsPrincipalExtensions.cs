using System.Security.Claims;

namespace CartMate.Api.Auth;

public static class ClaimsPrincipalExtensions
{
    public const string SubjectClaim = "sub";

    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        // Some JWT handlers remap "sub" to ClaimTypes.NameIdentifier on the way in; accept both.
        var value = user.FindFirstValue(SubjectClaim) ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var id)
            ? id
            : throw new InvalidOperationException("The authenticated user has no valid subject claim.");
    }
}
