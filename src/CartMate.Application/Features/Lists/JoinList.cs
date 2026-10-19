using CartMate.Application.Common.Persistence;
using CartMate.Contracts.Lists;
using CartMate.Domain.Common;
using CartMate.Domain.Lists;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace CartMate.Application.Features.Lists;

public sealed record JoinListCommand(Guid UserId, string InviteCode) : ICommand<ListSummaryDto>;

public sealed class JoinListValidator : AbstractValidator<JoinListCommand>
{
    public JoinListValidator()
    {
        RuleFor(c => c.InviteCode).NotEmpty().Length(InviteCodeGenerator.Length);
    }
}

public sealed class JoinListHandler(IAppDbContext db, TimeProvider time)
    : ICommandHandler<JoinListCommand, ListSummaryDto>
{
    public async ValueTask<ListSummaryDto> Handle(JoinListCommand command, CancellationToken cancellationToken)
    {
        // Codes are shown upper-case; accept " abcd2345 " typed on a phone.
        var code = command.InviteCode.Trim().ToUpperInvariant();

        var list = await db.GroceryLists
            .Include(l => l.Members)
            .FirstOrDefaultAsync(l => l.InviteCode == code, cancellationToken)
            ?? throw new DomainException("invite_not_found", "No list matches this invite code.");

        list.Join(command.UserId, time.GetUtcNow());
        await db.SaveChangesAsync(cancellationToken);

        var openItems = await db.GroceryItems
            .CountAsync(i => i.ListId == list.Id && !i.IsBought, cancellationToken);

        return new ListSummaryDto(list.Id, list.Name, list.InviteCode, openItems, list.Members.Count);
    }
}
