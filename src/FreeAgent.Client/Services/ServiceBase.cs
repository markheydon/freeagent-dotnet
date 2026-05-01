using FreeAgent.Client.Infrastructure.Http;

namespace FreeAgent.Client.Services;

/// <summary>
/// Base type for API services that require the shared FreeAgent HTTP client.
/// </summary>
public abstract class ServiceBase
{
    /// <summary>
    /// Initializes a new service base with the shared HTTP client.
    /// </summary>
    /// <param name="httpClient">FreeAgent HTTP client.</param>
    protected ServiceBase(FreeAgentHttpClient httpClient)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Shared FreeAgent HTTP client used by derived services.
    /// </summary>
    protected FreeAgentHttpClient HttpClient { get; }
}
