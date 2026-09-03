namespace FreeAgent.Client.Infrastructure.Http;

/// <summary>
/// Minimal request surface used by public services.
/// </summary>
internal interface IFreeAgentRequestClient
{
    /// <summary>
    /// Sends a GET request and deserializes the response body.
    /// </summary>
    Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a GET request and returns deserialized payload with selected response headers.
    /// </summary>
    Task<FreeAgentHttpResponse<T>> GetWithMetadataAsync<T>(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request and deserializes the response body.
    /// </summary>
    Task<T> PostAsync<T>(string endpoint, HttpContent content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a PUT request and deserializes the response body.
    /// </summary>
    Task<T> PutAsync<T>(string endpoint, HttpContent content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a DELETE request.
    /// </summary>
    Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
}
