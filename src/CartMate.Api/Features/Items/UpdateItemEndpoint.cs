using CartMate.Api.Auth;
using CartMate.Application.Features.Items;
using CartMate.Contracts.Items;
using FastEndpoints;
using Mediator;

namespace CartMate.Api.Features.Items;

public sealed class UpdateItemEndpoint(IMediator mediator) : Endpoint<UpdateItemRequest, ItemDto>
{
    public override void Configure()
    {
        Put("/lists/{listId:guid}/items/{itemId:guid}");
        Summary(s => s.Summary = "Edit an item (returns 409 if someone else changed it first)");
    }

    public override async Task HandleAsync(UpdateItemRequest req, CancellationToken ct)
    {
        var command = new UpdateItemCommand(
            User.GetUserId(), Route<Guid>("listId"), Route<Guid>("itemId"),
            req.Name, req.Quantity, req.Category, req.Note, req.Version);

        var item = await mediator.Send(command, ct);
        await Send.OkAsync(item, ct);
    }
}
