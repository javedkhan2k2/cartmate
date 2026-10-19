using CartMate.Domain.Common;

namespace CartMate.Domain.Lists;

/// <summary>
/// Aggregate root. Every change to members or items goes through this class,
/// so the business rules live in one place.
/// </summary>
public sealed class GroceryList
{
    public const int MaxNameLength = 60;
    public const int MaxMembers = 6;

    private readonly List<ListMember> _members = [];
    private readonly List<GroceryItem> _items = [];

    private GroceryList()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string InviteCode { get; private set; } = null!;

    public Guid OwnerId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>Concurrency token, mapped to PostgreSQL's xmin system column.</summary>
    public uint Version { get; private set; }

    public IReadOnlyCollection<ListMember> Members => _members.AsReadOnly();

    public IReadOnlyCollection<GroceryItem> Items => _items.AsReadOnly();

    public static GroceryList Create(string name, Guid ownerId, DateTimeOffset now)
    {
        var list = new GroceryList
        {
            Id = Guid.CreateVersion7(now),
            Name = Guard.RequiredText(name, MaxNameLength, "name"),
            InviteCode = InviteCodeGenerator.Next(),
            OwnerId = ownerId,
            CreatedAt = now,
        };

        list._members.Add(ListMember.Create(list.Id, ownerId, now));
        return list;
    }

    public bool IsMember(Guid userId) => _members.Exists(m => m.UserId == userId);

    public void Join(Guid userId, DateTimeOffset now)
    {
        if (IsMember(userId))
        {
            return; // Idempotent: joining twice is harmless.
        }

        if (_members.Count >= MaxMembers)
        {
            throw new DomainException("list_full", $"A list can have at most {MaxMembers} members.");
        }

        _members.Add(ListMember.Create(Id, userId, now));
    }

    public void Rename(Guid userId, string name)
    {
        EnsureMember(userId);
        Name = Guard.RequiredText(name, MaxNameLength, "name");
    }

    public void RegenerateInviteCode(Guid userId)
    {
        if (userId != OwnerId)
        {
            throw new DomainException("not_owner", "Only the list owner can regenerate the invite code.");
        }

        InviteCode = InviteCodeGenerator.Next();
    }

    public GroceryItem AddItem(Guid userId, ItemDetails details, DateTimeOffset now)
    {
        EnsureMember(userId);
        var item = GroceryItem.Create(Id, userId, details, now);
        _items.Add(item);
        return item;
    }

    public GroceryItem UpdateItem(Guid userId, Guid itemId, ItemDetails details, DateTimeOffset now)
    {
        EnsureMember(userId);
        var item = GetItem(itemId);
        item.Update(details, now);
        return item;
    }

    public GroceryItem MarkItemBought(Guid userId, Guid itemId, DateTimeOffset now)
    {
        EnsureMember(userId);
        var item = GetItem(itemId);
        item.MarkBought(userId, now);
        return item;
    }

    public GroceryItem MarkItemUnbought(Guid userId, Guid itemId, DateTimeOffset now)
    {
        EnsureMember(userId);
        var item = GetItem(itemId);
        item.MarkUnbought(now);
        return item;
    }

    public void RemoveItem(Guid userId, Guid itemId)
    {
        EnsureMember(userId);
        _items.Remove(GetItem(itemId));
    }

    /// <summary>Removes all bought items. Callers must load <see cref="Items"/> first.</summary>
    public IReadOnlyList<Guid> ClearBoughtItems(Guid userId)
    {
        EnsureMember(userId);
        var bought = _items.Where(i => i.IsBought).ToList();
        foreach (var item in bought)
        {
            _items.Remove(item);
        }

        return bought.Select(i => i.Id).ToList();
    }

    public void StartShopping(Guid userId, DateTimeOffset now) => GetMember(userId).StartShopping(now);

    public void StopShopping(Guid userId) => GetMember(userId).StopShopping();

    public IReadOnlyList<Guid> ActiveShopperIds(DateTimeOffset now) =>
        _members.Where(m => m.IsActivelyShopping(now)).Select(m => m.UserId).ToList();

    private ListMember GetMember(Guid userId) =>
        _members.Find(m => m.UserId == userId)
        ?? throw new DomainException("not_a_member", "You are not a member of this list.");

    private void EnsureMember(Guid userId) => _ = GetMember(userId);

    private GroceryItem GetItem(Guid itemId) =>
        _items.Find(i => i.Id == itemId)
        ?? throw new DomainException("item_not_found", "The item was not found on this list.");
}
