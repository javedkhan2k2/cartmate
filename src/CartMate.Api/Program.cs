using CartMate.Api.Auth;
using CartMate.Api.Common.Errors;
using CartMate.Application;
using CartMate.Application.Common.Auth;
using CartMate.Application.Common.Behaviors;
using CartMate.Infrastructure;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Mediator;

var builder = WebApplication.CreateBuilder(args);

// ── Configuration: read and validate eagerly so misconfiguration fails at startup ──
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'ConnectionStrings:Default' is not configured.");

var authOptions = builder.Configuration.GetSection(AuthOptions.SectionName).Get<AuthOptions>() ?? new AuthOptions();
authOptions.Validate();
builder.Services.AddSingleton(authOptions);

// ── Application layers ──
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddApplication();

builder.Services.AddMediator((MediatorOptions options) =>
{
    // Scoped, because handlers depend on the scoped DbContext.
    options.ServiceLifetime = ServiceLifetime.Scoped;
    options.Assemblies = [typeof(ApplicationAssembly)];
    // Order matters: the first behavior is the outermost wrapper.
    options.PipelineBehaviors = [typeof(LoggingBehavior<,>), typeof(ValidationBehavior<,>)];
});

builder.Services.AddScoped<ITokenService, JwtTokenService>();

// ── HTTP: auth, endpoints, OpenAPI ──
builder.Services
    .AddAuthenticationJwtBearer(
        signing => signing.SigningKey = authOptions.SigningKey,
        bearer =>
        {
            bearer.TokenValidationParameters.ValidateIssuer = true;
            bearer.TokenValidationParameters.ValidIssuer = authOptions.Issuer;
            bearer.TokenValidationParameters.ValidateAudience = true;
            bearer.TokenValidationParameters.ValidAudience = authOptions.Audience;
        })
    .AddAuthorization()
    .AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.Title = "CartMate API";
            s.Version = "v1";
        };
    });

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<AppExceptionHandler>();

var app = builder.Build();

// ── Middleware order matters: errors first, then auth, then endpoints ──
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Errors.ResponseBuilder = ValidationProblems.FromFailures;
});

// API docs locally and on staging; production doesn't advertise its surface.
if (!app.Environment.IsProduction())
{
    app.UseSwaggerGen();
}

app.Run();
