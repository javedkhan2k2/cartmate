using CartMate.Api.Auth;
using CartMate.Api.Features.Lists;
using CartMate.Application.Features.Items;
using CartMate.Contracts.Items;
using FastEndpoints;
using Mediator;

namespace CartMate.Api.Features.Items;

public sealed class AddItemEndpoint(IMediator mediator) : Endpoint<AddItemRequest, ItemDto>
{
    public override void Configure()
    {
        Post("/lists/{listId:guid}/items");
        Summary(s => s.Summary = "Add an item to a list");
    }

    public override async Task HandleAsync(AddItemRequest req, CancellationToken ct)
    {
        var listId = Route<Guid>("listId");
        var command = new AddItemCommand(User.GetUserId(), listId, req.Name, req.Quantity, req.Category, req.Note);

        var item = await mediator.Send(command, ct);
        await Send.CreatedAtAsync<GetListEndpoint>(new { listId }, item, cancellation: ct);
    }
}
