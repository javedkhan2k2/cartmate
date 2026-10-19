using CartMate.Api.Auth;
using CartMate.Application.Features.Lists;
using CartMate.Contracts.Lists;
using FastEndpoints;
using Mediator;

namespace CartMate.Api.Features.Lists;

public sealed class SetShoppingEndpoint(IMediator mediator) : Endpoint<SetShoppingRequest>
{
    public override void Configure()
    {
        Put("/lists/{listId:guid}/shopping");
        Summary(s => s.Summary = "Turn \"I'm shopping now\" mode on or off");
    }

    public override async Task HandleAsync(SetShoppingRequest req, CancellationToken ct)
    {
        await mediator.Send(new SetShoppingCommand(User.GetUserId(), Route<Guid>("listId"), req.IsShopping), ct);
        await Send.NoContentAsync(ct);
    }
}
