namespace FreeAgent.Client.Infrastructure.Http;

/// <summary>
/// Wraps a deserialized API response with selected HTTP header metadata.
/// </summary>
/// <typeparam name="T">Response payload type</typeparam>
public sealed class FreeAgentHttpResponse<T>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="data">Deserialized response payload</param>
    /// <param name="headers">Response headers as a case-insensitive map</param>
    public FreeAgentHttpResponse(T data, IReadOnlyDictionary<string, IReadOnlyList<string>> headers)
    {
        Data = data;
        Headers = headers;
    }

    /// <summary>
    /// Deserialized response payload.
    /// </summary>
    public T Data { get; }

    /// <summary>
    /// Response headers available for pagination and diagnostics.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers { get; }

    /// <summary>
    /// Gets a response header by name.
    /// </summary>
    /// <param name="name">Header name</param>
    /// <returns>Header values when present</returns>
    public IReadOnlyList<string>? GetHeaderValues(string name)
    {
        return Headers.TryGetValue(name, out var values) ? values : null;
    }
}
