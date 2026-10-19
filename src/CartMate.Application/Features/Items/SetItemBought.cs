using CartMate.Application.Common.Persistence;
using CartMate.Contracts.Items;
using Mediator;

namespace CartMate.Application.Features.Items;

public sealed record SetItemBoughtCommand(Guid UserId, Guid ListId, Guid ItemId, bool IsBought) : ICommand<ItemDto>;

public sealed class SetItemBoughtHandler(IAppDbContext db, TimeProvider time)
    : ICommandHandler<SetItemBoughtCommand, ItemDto>
{
    public async ValueTask<ItemDto> Handle(SetItemBoughtCommand command, CancellationToken cancellationToken)
    {
        var list = await db.GroceryLists
            .WithMembersAndItem(command.ItemId)
            .GetOrThrowAsync(command.ListId, cancellationToken);

        // Setting an explicit state (not "toggle") makes the call idempotent:
        // a retried request after a flaky in-store connection can't flip it back.
        var now = time.GetUtcNow();
        var item = command.IsBought
            ? list.MarkItemBought(command.UserId, command.ItemId, now)
            : list.MarkItemUnbought(command.UserId, command.ItemId, now);

        await db.SaveChangesAsync(cancellationToken);
        return item.ToDto();
    }
}
