using Microsoft.Extensions.DependencyInjection;

namespace EbikeRental.Shared.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        // Add any shared services here if needed
        return services;
    }
}
