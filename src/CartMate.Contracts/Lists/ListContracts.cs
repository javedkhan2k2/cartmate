using CartMate.Contracts.Items;

namespace CartMate.Contracts.Lists;

public sealed record CreateListRequest
{
    public required string Name { get; init; }
}

public sealed record JoinListRequest
{
    public required string InviteCode { get; init; }
}

public sealed record SetShoppingRequest
{
    public required bool IsShopping { get; init; }
}

public sealed record ListSummaryDto(Guid Id, string Name, string InviteCode, int OpenItemCount, int MemberCount);

public sealed record MemberDto(Guid UserId, string DisplayName, bool IsShopping);

public sealed record ListDetailsDto(
    Guid Id,
    string Name,
    string InviteCode,
    IReadOnlyList<MemberDto> Members,
    IReadOnlyList<ItemDto> Items);
