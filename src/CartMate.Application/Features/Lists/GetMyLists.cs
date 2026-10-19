using CartMate.Application.Common.Persistence;
using CartMate.Contracts.Lists;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace CartMate.Application.Features.Lists;

public sealed record GetMyListsQuery(Guid UserId) : IQuery<IReadOnlyList<ListSummaryDto>>;

public sealed class GetMyListsHandler(IAppDbContext db)
    : IQueryHandler<GetMyListsQuery, IReadOnlyList<ListSummaryDto>>
{
    public async ValueTask<IReadOnlyList<ListSummaryDto>> Handle(GetMyListsQuery query, CancellationToken cancellationToken) =>
        // Read side: no aggregate loading, just one SQL query shaped exactly like the DTO.
        await db.GroceryLists
            .AsNoTracking()
            .Where(l => l.Members.Any(m => m.UserId == query.UserId))
            .OrderBy(l => l.Name)
            .Select(l => new ListSummaryDto(
                l.Id,
                l.Name,
                l.InviteCode,
                l.Items.Count(i => !i.IsBought),
                l.Members.Count))
            .ToListAsync(cancellationToken);
}
