using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Minimal invoice resource shape for deserialising invoice URI links on other resources.
/// </summary>
/// <remarks>
/// Full invoice operations are not yet implemented in the SDK.
/// </remarks>
public sealed class Invoice : IFreeAgentResource
{
    /// <summary>
    /// Invoice resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;
}
