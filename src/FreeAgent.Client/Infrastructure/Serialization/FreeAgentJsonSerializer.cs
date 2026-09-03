using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises request payloads for FreeAgent API calls.
/// </summary>
internal static class FreeAgentJsonSerializer
{
    internal static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = null,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Creates JSON HTTP content for a request body.
    /// </summary>
    public static StringContent CreateContent<T>(T payload)
    {
        var json = JsonSerializer.Serialize(payload, Options);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
