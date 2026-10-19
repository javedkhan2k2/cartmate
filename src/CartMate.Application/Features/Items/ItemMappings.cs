using CartMate.Contracts.Items;
using CartMate.Domain.Lists;

namespace CartMate.Application.Features.Items;

internal static class ItemMappings
{
    public static ItemDto ToDto(this GroceryItem item) => new(
        item.Id, item.Name, item.Quantity, item.Category, item.Note, item.IsBought,
        item.AddedByUserId, item.BoughtByUserId, item.CreatedAt, item.Version);
}
