using CartMate.Application.Common.Persistence;
using CartMate.Contracts.Lists;
using CartMate.Domain.Lists;
using FluentValidation;
using Mediator;

namespace CartMate.Application.Features.Lists;

public sealed record CreateListCommand(Guid UserId, string Name) : ICommand<ListSummaryDto>;

public sealed class CreateListValidator : AbstractValidator<CreateListCommand>
{
    public CreateListValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(GroceryList.MaxNameLength);
    }
}

public sealed class CreateListHandler(IAppDbContext db, TimeProvider time)
    : ICommandHandler<CreateListCommand, ListSummaryDto>
{
    public async ValueTask<ListSummaryDto> Handle(CreateListCommand command, CancellationToken cancellationToken)
    {
        var list = GroceryList.Create(command.Name, command.UserId, time.GetUtcNow());
        db.GroceryLists.Add(list);
        await db.SaveChangesAsync(cancellationToken);

        return new ListSummaryDto(list.Id, list.Name, list.InviteCode, OpenItemCount: 0, MemberCount: 1);
    }
}
