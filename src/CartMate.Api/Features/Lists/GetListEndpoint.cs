using CartMate.Api.Auth;
using CartMate.Application.Features.Lists;
using CartMate.Contracts.Lists;
using FastEndpoints;
using Mediator;

namespace CartMate.Api.Features.Lists;

public sealed class GetListEndpoint(IMediator mediator) : EndpointWithoutRequest<ListDetailsDto>
{
    public override void Configure()
    {
        Get("/lists/{listId:guid}");
        Summary(s => s.Summary = "Get a list with its members and items");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var list = await mediator.Send(new GetListQuery(User.GetUserId(), Route<Guid>("listId")), ct);
        await Send.OkAsync(list, ct);
    }
}
