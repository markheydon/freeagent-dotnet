using Microsoft.Extensions.DependencyInjection;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Registers console sample providers and runner services.
/// </summary>
internal static class DependencyInjection
{
    /// <summary>
    /// Registers sample providers discovered via <see cref="ConsoleSamplesAttribute"/>.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddConsoleSamples(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        var providerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.GetCustomAttributes(typeof(ConsoleSamplesAttribute), false).Length > 0)
            .Where(t => typeof(IConsoleSampleProvider).IsAssignableFrom(t));

        foreach (var providerType in providerTypes)
        {
            services.AddScoped(providerType);
            services.AddScoped(typeof(IConsoleSampleProvider), providerType);
        }

        services.AddScoped<ConsoleSampleRunner>();
        return services;
    }
}
