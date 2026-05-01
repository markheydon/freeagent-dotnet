using System.Text.Json;
using FreeAgent.Client.Infrastructure.Configuration;

namespace FreeAgent.Client.Sample.Services;

/// <summary>
/// Fetches raw endpoint payloads to aid debugging when model deserialization or API calls fail.
/// </summary>
public sealed class ApiDiagnosticsService
{
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
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri(FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment))
            };

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "FreeAgent.Client.Sample/1.0");

            using var response = await httpClient.GetAsync(relativeEndpoint, cancellationToken);
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
}
