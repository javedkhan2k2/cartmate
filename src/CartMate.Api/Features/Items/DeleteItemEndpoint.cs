using CartMate.Api.Auth;
using CartMate.Application.Features.Items;
using FastEndpoints;
using Mediator;

namespace CartMate.Api.Features.Items;

public sealed class DeleteItemEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/lists/{listId:guid}/items/{itemId:guid}");
        Summary(s => s.Summary = "Delete an item");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await mediator.Send(new DeleteItemCommand(User.GetUserId(), Route<Guid>("listId"), Route<Guid>("itemId")), ct);
        await Send.NoContentAsync(ct);
    }
}
