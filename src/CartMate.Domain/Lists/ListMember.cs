namespace CartMate.Domain.Lists;

public sealed class ListMember
{
    /// <summary>Shopping mode silently expires so a forgotten toggle doesn't notify forever.</summary>
    public static readonly TimeSpan ShoppingTimeout = TimeSpan.FromHours(3);

    private ListMember()
    {
        // Required by EF Core.
    }

    public Guid ListId { get; private set; }

    public Guid UserId { get; private set; }

    public DateTimeOffset JoinedAt { get; private set; }

    public bool IsShopping { get; private set; }

    public DateTimeOffset? ShoppingSince { get; private set; }

    public bool IsActivelyShopping(DateTimeOffset now) =>
        IsShopping && ShoppingSince is { } since && now - since < ShoppingTimeout;

    internal static ListMember Create(Guid listId, Guid userId, DateTimeOffset now) => new()
    {
        ListId = listId,
        UserId = userId,
        JoinedAt = now,
    };

    internal void StartShopping(DateTimeOffset now)
    {
        IsShopping = true;
        ShoppingSince = now;
    }

    internal void StopShopping()
    {
        IsShopping = false;
        ShoppingSince = null;
    }
}
