using CartMate.Domain.Common;

namespace CartMate.Domain.Lists;

public sealed class GroceryItem
{
    public const int MaxNameLength = 100;
    public const int MaxCategoryLength = 40;
    public const int MaxNoteLength = 300;
    public const int MinQuantity = 1;
    public const int MaxQuantity = 999;

    private GroceryItem()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public Guid ListId { get; private set; }

    public string Name { get; private set; } = null!;

    public int Quantity { get; private set; }

    public string? Category { get; private set; }

    public string? Note { get; private set; }

    public bool IsBought { get; private set; }

    public Guid? BoughtByUserId { get; private set; }

    public DateTimeOffset? BoughtAt { get; private set; }

    public Guid AddedByUserId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>Concurrency token, mapped to PostgreSQL's xmin system column.</summary>
    public uint Version { get; private set; }

    // Internal: items are only changed through the GroceryList aggregate root.
    internal static GroceryItem Create(Guid listId, Guid addedByUserId, ItemDetails details, DateTimeOffset now)
    {
        var item = new GroceryItem
        {
            Id = Guid.CreateVersion7(now),
            ListId = listId,
            AddedByUserId = addedByUserId,
            CreatedAt = now,
        };
        item.Apply(details, now);
        return item;
    }

    internal void Update(ItemDetails details, DateTimeOffset now) => Apply(details, now);

    internal void MarkBought(Guid userId, DateTimeOffset now)
    {
        if (IsBought)
        {
            return; // Idempotent: both partners tapping at once is harmless.
        }

        IsBought = true;
        BoughtByUserId = userId;
        BoughtAt = now;
        UpdatedAt = now;
    }

    internal void MarkUnbought(DateTimeOffset now)
    {
        if (!IsBought)
        {
            return;
        }

        IsBought = false;
        BoughtByUserId = null;
        BoughtAt = null;
        UpdatedAt = now;
    }

    private void Apply(ItemDetails details, DateTimeOffset now)
    {
        Name = Guard.RequiredText(details.Name, MaxNameLength, "name");
        Quantity = Guard.InRange(details.Quantity, MinQuantity, MaxQuantity, "quantity");
        Category = Guard.OptionalText(details.Category, MaxCategoryLength, "category");
        Note = Guard.OptionalText(details.Note, MaxNoteLength, "note");
        UpdatedAt = now;
    }
}
