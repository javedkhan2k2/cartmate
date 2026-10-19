using System.Globalization;
using CartMate.Application.Common.Auth;
using CartMate.Contracts.Auth;
using CartMate.Domain.Users;
using FastEndpoints.Security;

namespace CartMate.Api.Auth;

public sealed class JwtTokenService(AuthOptions options, TimeProvider time) : ITokenService
{
    public AuthResponse CreateToken(User user)
    {
        var expiresAt = time.GetUtcNow().AddHours(options.TokenLifetimeHours);

        var token = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = options.SigningKey;
            o.Issuer = options.Issuer;
            o.Audience = options.Audience;
            o.ExpireAt = expiresAt.UtcDateTime;
            o.User.Claims.Add((ClaimsPrincipalExtensions.SubjectClaim, user.Id.ToString("D", CultureInfo.InvariantCulture)));
            o.User.Claims.Add(("name", user.DisplayName));
        });

        return new AuthResponse(token, expiresAt, new UserDto(user.Id, user.Email, user.DisplayName));
    }
}
