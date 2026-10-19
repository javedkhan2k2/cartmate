using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CartMate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Mediator itself is registered in the Api project, where its source generator runs.
        services.AddValidatorsFromAssembly(typeof(ApplicationAssembly).Assembly);
        return services;
    }
}
