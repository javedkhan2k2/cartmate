using CartMate.Application.Common.Persistence;
using Mediator;

namespace CartMate.Application.Features.Items;

public sealed record DeleteItemCommand(Guid UserId, Guid ListId, Guid ItemId) : ICommand;

public sealed class DeleteItemHandler(IAppDbContext db) : ICommandHandler<DeleteItemCommand>
{
    public async ValueTask<Unit> Handle(DeleteItemCommand command, CancellationToken cancellationToken)
    {
        var list = await db.GroceryLists
            .WithMembersAndItem(command.ItemId)
            .GetOrThrowAsync(command.ListId, cancellationToken);

        list.RemoveItem(command.UserId, command.ItemId);
        await db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
