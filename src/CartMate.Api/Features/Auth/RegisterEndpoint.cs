using CartMate.Application.Features.Auth;
using CartMate.Contracts.Auth;
using FastEndpoints;
using Mediator;

namespace CartMate.Api.Features.Auth;

public sealed class RegisterEndpoint(IMediator mediator) : Endpoint<RegisterRequest, AuthResponse>
{
    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
        Summary(s => s.Summary = "Create an account and receive an access token");
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var response = await mediator.Send(new RegisterCommand(req.Email, req.DisplayName, req.Password), ct);
        await Send.OkAsync(response, ct);
    }
}
