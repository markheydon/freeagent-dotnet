using System.Text.Json;
using FreeAgent.Client;

namespace FreeAgent.Client.Sample.Services;

/// <summary>
/// Fetches raw endpoint payloads to aid debugging when model deserialisation or API calls fail.
/// </summary>
public sealed class ApiDiagnosticsService
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initialises the diagnostics service with a DI-managed HTTP client.
    /// </summary>
    /// <param name="httpClient">HTTP client managed by DI.</param>
    public ApiDiagnosticsService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Fetches a raw endpoint payload for diagnostics.
    /// </summary>
    /// <param name="accessToken">OAuth access token.</param>
    /// <param name="environment">Target FreeAgent environment.</param>
    /// <param name="relativeEndpoint">Endpoint path relative to API base (for example: <c>company</c>).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Formatted JSON payload where possible, raw text otherwise.</returns>
    public async Task<string> FetchRawEndpointPayloadAsync(
        string accessToken,
        FreeAgentEnvironment environment,
        string relativeEndpoint,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return "Diagnostics unavailable: access token missing.";
        }

        if (string.IsNullOrWhiteSpace(relativeEndpoint))
        {
            return "Diagnostics unavailable: endpoint path missing.";
        }

        try
        {
            var endpoint = relativeEndpoint.TrimStart('/');
            var requestUri = new Uri($"{GetApiBaseUrl(environment)}{endpoint}");
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
            request.Headers.TryAddWithoutValidation("User-Agent", "FreeAgent.Client.Sample/1.0");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(raw))
            {
                return $"No payload returned. HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            }

            try
            {
                var json = JsonSerializer.Deserialize<JsonElement>(raw);
                return JsonSerializer.Serialize(json, PrettyJsonOptions);
            }
            catch (JsonException)
            {
                return raw;
            }
        }
        catch (Exception ex)
        {
            return $"Failed to capture raw payload: {ex.Message}";
        }
    }

    private static readonly JsonSerializerOptions PrettyJsonOptions = new()
    {
        WriteIndented = true
    };

    private static string GetApiBaseUrl(FreeAgentEnvironment environment)
    {
        return environment == FreeAgentEnvironment.Sandbox
            ? "https://api.sandbox.freeagent.com/v2/"
            : "https://api.freeagent.com/v2/";
    }
}
