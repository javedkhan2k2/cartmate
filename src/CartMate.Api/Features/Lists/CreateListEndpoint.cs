using CartMate.Api.Auth;
using CartMate.Application.Features.Lists;
using CartMate.Contracts.Lists;
using FastEndpoints;
using Mediator;

namespace CartMate.Api.Features.Lists;

public sealed class CreateListEndpoint(IMediator mediator) : Endpoint<CreateListRequest, ListSummaryDto>
{
    public override void Configure()
    {
        Post("/lists");
        Summary(s => s.Summary = "Create a new shared grocery list");
    }

    public override async Task HandleAsync(CreateListRequest req, CancellationToken ct)
    {
        var list = await mediator.Send(new CreateListCommand(User.GetUserId(), req.Name), ct);
        await Send.CreatedAtAsync<GetListEndpoint>(new { listId = list.Id }, list, cancellation: ct);
    }
}
