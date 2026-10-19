using CartMate.Application.Common.Auth;
using CartMate.Application.Common.Persistence;
using CartMate.Contracts.Auth;
using CartMate.Domain.Common;
using CartMate.Domain.Users;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace CartMate.Application.Features.Auth;

public sealed record LoginCommand(string Email, string Password) : ICommand<AuthResponse>;

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(c => c.Email).NotEmpty();
        RuleFor(c => c.Password).NotEmpty();
    }
}

public sealed class LoginHandler(IAppDbContext db, IPasswordHasher passwordHasher, ITokenService tokens)
    : ICommandHandler<LoginCommand, AuthResponse>
{
    public async ValueTask<AuthResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var normalizedEmail = User.NormalizeEmail(command.Email);
        var user = await db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

        // Same error for "no such user" and "wrong password": never reveal which emails exist.
        if (user is null || !passwordHasher.Verify(user.PasswordHash, command.Password))
        {
            throw new DomainException("invalid_credentials", "Invalid email or password.");
        }

        return tokens.CreateToken(user);
    }
}
