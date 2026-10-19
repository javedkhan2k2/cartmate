using CartMate.Api.Auth;
using CartMate.Application.Features.Items;
using CartMate.Contracts.Items;
using FastEndpoints;
using Mediator;

namespace CartMate.Api.Features.Items;

public sealed class SetItemBoughtEndpoint(IMediator mediator) : Endpoint<SetItemBoughtRequest, ItemDto>
{
    public override void Configure()
    {
        Put("/lists/{listId:guid}/items/{itemId:guid}/bought");
        Summary(s => s.Summary = "Mark an item as bought or not bought");
    }

    public override async Task HandleAsync(SetItemBoughtRequest req, CancellationToken ct)
    {
        var command = new SetItemBoughtCommand(User.GetUserId(), Route<Guid>("listId"), Route<Guid>("itemId"), req.IsBought);
        var item = await mediator.Send(command, ct);
        await Send.OkAsync(item, ct);
    }
}
