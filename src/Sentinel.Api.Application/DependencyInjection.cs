using Microsoft.Extensions.DependencyInjection;

namespace Sentinel.Api.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}