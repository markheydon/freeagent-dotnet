namespace FreeAgent.Client.Infrastructure.Http;

/// <summary>
/// Minimal request surface used by public services.
/// </summary>
internal interface IFreeAgentRequestClient
{
    /// <summary>
    /// Sends a GET request and deserializes the response body.
    /// </summary>
    /// <typeparam name="T">Response payload type.</typeparam>
    /// <param name="endpoint">API endpoint relative to the service base URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Deserialized response payload.</returns>
    Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a GET request and returns deserialized payload with selected response headers.
    /// </summary>
    /// <typeparam name="T">Response payload type.</typeparam>
    /// <param name="endpoint">API endpoint relative to the service base URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Deserialized response payload and response headers.</returns>
    Task<FreeAgentHttpResponse<T>> GetWithMetadataAsync<T>(string endpoint, CancellationToken cancellationToken = default);
}
