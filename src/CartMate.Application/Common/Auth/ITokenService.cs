using CartMate.Contracts.Auth;
using CartMate.Domain.Users;

namespace CartMate.Application.Common.Auth;

public interface ITokenService
{
    AuthResponse CreateToken(User user);
}
