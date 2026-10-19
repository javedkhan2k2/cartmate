using CartMate.Api.Auth;
using CartMate.Application.Features.Items;
using FastEndpoints;
using Mediator;

namespace CartMate.Api.Features.Items;

public sealed class ClearBoughtItemsEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override void Configure()
    {
        // The :guid constraint on the item routes keeps "bought" from matching {itemId}.
        Delete("/lists/{listId:guid}/items/bought");
        Summary(s => s.Summary = "Remove all bought items from the list");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await mediator.Send(new ClearBoughtItemsCommand(User.GetUserId(), Route<Guid>("listId")), ct);
        await Send.NoContentAsync(ct);
    }
}
