using CartMate.Application.Common.Auth;
using CartMate.Application.Common.Persistence;
using CartMate.Infrastructure.Auth;
using CartMate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CartMate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                // Retries transient failures, e.g. while Neon wakes from scale-to-zero.
                npgsql.EnableRetryOnFailure(maxRetryCount: 3)));

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
