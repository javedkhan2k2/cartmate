using CartMate.Api.Auth;
using CartMate.Application.Features.Lists;
using CartMate.Contracts.Lists;
using FastEndpoints;
using Mediator;

namespace CartMate.Api.Features.Lists;

public sealed class GetMyListsEndpoint(IMediator mediator) : EndpointWithoutRequest<IReadOnlyList<ListSummaryDto>>
{
    public override void Configure()
    {
        Get("/lists");
        Summary(s => s.Summary = "Get all lists the current user is a member of");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var lists = await mediator.Send(new GetMyListsQuery(User.GetUserId()), ct);
        await Send.OkAsync(lists, ct);
    }
}
