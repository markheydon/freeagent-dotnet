using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Registers Turpinverse canon loading, seeders, and the bulk seed orchestrator.
/// </summary>
public static class TurpinverseSeedServiceCollectionExtensions
{
    /// <summary>
    /// Adds Turpinverse catalog, seeder, and orchestrator services for sample applications.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Application configuration containing Turpinverse options.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddTurpinverseSeedServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<TurpinverseCanonOptions>(
            configuration.GetSection(TurpinverseCanonOptions.SectionName));
        services.AddHttpClient<TurpinverseCanonClient>();
        services.AddSingleton<TurpinverseContactCatalog>();
        services.AddSingleton<TurpinverseContactSeeder>();
        services.AddSingleton<TurpinverseProjectCatalog>();
        services.AddSingleton<TurpinverseProjectSeeder>();
        services.AddSingleton<TurpinverseInvoiceCatalog>();
        services.AddSingleton<TurpinverseInvoiceSeeder>();
        services.AddSingleton<TurpinverseQuoteCatalog>();
        services.AddSingleton<TurpinverseQuoteSeeder>();
        services.AddSingleton<TurpinverseCreditNoteCatalog>();
        services.AddSingleton<TurpinverseCreditNoteSeeder>();
        services.AddSingleton<TurpinverseTaskSeeder>();
        services.AddSingleton<TurpinverseTimeslipSeeder>();
        services.AddSingleton<TurpinverseNoteSeeder>();
        services.AddSingleton<TurpinverseSeedOrchestrator>();

        return services;
    }
}
