using CartMate.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Fail fast at startup if configuration is missing, rather than on the first request.
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'ConnectionStrings:Default' is not configured.");

builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

app.MapGet("/", () => "CartMate API");

app.Run();
