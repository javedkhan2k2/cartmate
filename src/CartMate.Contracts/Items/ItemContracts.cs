namespace CartMate.Contracts.Items;

public sealed record AddItemRequest
{
    public required string Name { get; init; }

    public required int Quantity { get; init; }

    public string? Category { get; init; }

    public string? Note { get; init; }
}

public sealed record UpdateItemRequest
{
    public required string Name { get; init; }

    public required int Quantity { get; init; }

    public string? Category { get; init; }

    public string? Note { get; init; }

    /// <summary>The Version the client last saw. A mismatch means someone else edited the item: 409.</summary>
    public required uint Version { get; init; }
}

public sealed record SetItemBoughtRequest
{
    public required bool IsBought { get; init; }
}

public sealed record ItemDto(
    Guid Id,
    string Name,
    int Quantity,
    string? Category,
    string? Note,
    bool IsBought,
    Guid AddedByUserId,
    Guid? BoughtByUserId,
    DateTimeOffset CreatedAt,
    uint Version);
