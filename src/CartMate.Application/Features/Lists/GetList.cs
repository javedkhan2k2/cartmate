using CartMate.Application.Common.Persistence;
using CartMate.Contracts.Items;
using CartMate.Contracts.Lists;
using CartMate.Domain.Common;
using CartMate.Domain.Lists;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace CartMate.Application.Features.Lists;

public sealed record GetListQuery(Guid UserId, Guid ListId) : IQuery<ListDetailsDto>;

public sealed class GetListHandler(IAppDbContext db, TimeProvider time) : IQueryHandler<GetListQuery, ListDetailsDto>
{
    public async ValueTask<ListDetailsDto> Handle(GetListQuery query, CancellationToken cancellationToken)
    {
        // Membership is part of the filter: a non-member gets exactly the same 404 as a missing list.
        var list = await db.GroceryLists
            .AsNoTracking()
            .Where(l => l.Id == query.ListId && l.Members.Any(m => m.UserId == query.UserId))
            .Select(l => new { l.Id, l.Name, l.InviteCode })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new DomainException("list_not_found", "The list was not found.");

        var members = await db.ListMembers
            .AsNoTracking()
            .Where(m => m.ListId == query.ListId)
            .Join(db.Users, m => m.UserId, u => u.Id, (m, u) => new { m.UserId, u.DisplayName, m.IsShopping, m.ShoppingSince })
            .ToListAsync(cancellationToken);

        var items = await db.GroceryItems
            .AsNoTracking()
            .Where(i => i.ListId == query.ListId)
            .OrderBy(i => i.IsBought)
            .ThenBy(i => i.CreatedAt)
            .Select(i => new ItemDto(
                i.Id, i.Name, i.Quantity, i.Category, i.Note, i.IsBought,
                i.AddedByUserId, i.BoughtByUserId, i.CreatedAt, i.Version))
            .ToListAsync(cancellationToken);

        var shoppingCutoff = time.GetUtcNow() - ListMember.ShoppingTimeout;
        var memberDtos = members
            .Select(m => new MemberDto(m.UserId, m.DisplayName, m.IsShopping && m.ShoppingSince > shoppingCutoff))
            .ToList();

        return new ListDetailsDto(list.Id, list.Name, list.InviteCode, memberDtos, items);
    }
}
