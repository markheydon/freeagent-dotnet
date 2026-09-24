using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.CreditNotes;

/// <summary>
/// Credit note resource returned when converting a draft negative invoice.
/// </summary>
/// <remarks>
/// Full credit note operations are not yet implemented in the SDK.
/// </remarks>
public sealed class CreditNote : IFreeAgentResource
{
    /// <summary>
    /// Credit note resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;
}
