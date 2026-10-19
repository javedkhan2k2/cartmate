using CartMate.Application.Features.Auth;
using CartMate.Contracts.Auth;
using FastEndpoints;
using Mediator;

namespace CartMate.Api.Features.Auth;

public sealed class LoginEndpoint(IMediator mediator) : Endpoint<LoginRequest, AuthResponse>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
        Summary(s => s.Summary = "Exchange email and password for an access token");
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var response = await mediator.Send(new LoginCommand(req.Email, req.Password), ct);
        await Send.OkAsync(response, ct);
    }
}
