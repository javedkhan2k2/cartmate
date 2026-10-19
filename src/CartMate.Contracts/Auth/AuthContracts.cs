namespace CartMate.Contracts.Auth;

public sealed record RegisterRequest
{
    public required string Email { get; init; }

    public required string DisplayName { get; init; }

    public required string Password { get; init; }
}

public sealed record LoginRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }
}

public sealed record UserDto(Guid Id, string Email, string DisplayName);

public sealed record AuthResponse(string AccessToken, DateTimeOffset ExpiresAt, UserDto User);
