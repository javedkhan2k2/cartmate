using CartMate.Api.Auth;
using CartMate.Application.Features.Lists;
using CartMate.Contracts.Lists;
using FastEndpoints;
using Mediator;

namespace CartMate.Api.Features.Lists;

public sealed class JoinListEndpoint(IMediator mediator) : Endpoint<JoinListRequest, ListSummaryDto>
{
    public override void Configure()
    {
        Post("/lists/join");
        Summary(s => s.Summary = "Join a list using its invite code");
    }

    public override async Task HandleAsync(JoinListRequest req, CancellationToken ct)
    {
        var list = await mediator.Send(new JoinListCommand(User.GetUserId(), req.InviteCode), ct);
        await Send.OkAsync(list, ct);
    }
}
