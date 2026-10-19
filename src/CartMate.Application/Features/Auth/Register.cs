using CartMate.Application.Common.Auth;
using CartMate.Application.Common.Persistence;
using CartMate.Contracts.Auth;
using CartMate.Domain.Common;
using CartMate.Domain.Users;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace CartMate.Application.Features.Auth;

public sealed record RegisterCommand(string Email, string DisplayName, string Password) : ICommand<AuthResponse>;

public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress().MaximumLength(User.MaxEmailLength);
        RuleFor(c => c.DisplayName).NotEmpty().MaximumLength(User.MaxDisplayNameLength);
        RuleFor(c => c.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
    }
}

public sealed class RegisterHandler(
    IAppDbContext db,
    IPasswordHasher passwordHasher,
    ITokenService tokens,
    TimeProvider time)
    : ICommandHandler<RegisterCommand, AuthResponse>
{
    public async ValueTask<AuthResponse> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var normalizedEmail = User.NormalizeEmail(command.Email);
        if (await db.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken))
        {
            throw new DomainException("email_taken", "An account with this email already exists.");
        }

        var user = User.Create(command.Email, command.DisplayName, passwordHasher.Hash(command.Password), time.GetUtcNow());
        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return tokens.CreateToken(user);
    }
}
