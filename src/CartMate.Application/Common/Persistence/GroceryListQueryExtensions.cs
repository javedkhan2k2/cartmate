using CartMate.Domain.Common;
using CartMate.Domain.Lists;
using Microsoft.EntityFrameworkCore;

namespace CartMate.Application.Common.Persistence;

public static class GroceryListQueryExtensions
{
    /// <summary>Loads the list's members plus a single item: all a per-item command needs.</summary>
    public static IQueryable<GroceryList> WithMembersAndItem(this IQueryable<GroceryList> lists, Guid itemId) =>
        lists
            .Include(l => l.Members)
            .Include(l => l.Items.Where(i => i.Id == itemId));

    public static async Task<GroceryList> GetOrThrowAsync(
        this IQueryable<GroceryList> lists,
        Guid listId,
        CancellationToken cancellationToken) =>
        await lists.FirstOrDefaultAsync(l => l.Id == listId, cancellationToken)
        ?? throw new DomainException("list_not_found", "The list was not found.");
}
