using CartMate.Application.Common.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace CartMate.Application.Features.Items;

/// <summary>Returns the removed item IDs (Chapter 8 broadcasts them to the other partner).</summary>
public sealed record ClearBoughtItemsCommand(Guid UserId, Guid ListId) : ICommand<IReadOnlyList<Guid>>;

public sealed class ClearBoughtItemsHandler(IAppDbContext db)
    : ICommandHandler<ClearBoughtItemsCommand, IReadOnlyList<Guid>>
{
    public async ValueTask<IReadOnlyList<Guid>> Handle(ClearBoughtItemsCommand command, CancellationToken cancellationToken)
    {
        var list = await db.GroceryLists
            .Include(l => l.Members)
            .Include(l => l.Items.Where(i => i.IsBought))
            .GetOrThrowAsync(command.ListId, cancellationToken);

        var removed = list.ClearBoughtItems(command.UserId);
        await db.SaveChangesAsync(cancellationToken);
        return removed;
    }
}
