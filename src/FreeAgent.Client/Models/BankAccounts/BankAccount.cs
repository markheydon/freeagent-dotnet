using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.BankAccounts;

/// <summary>
/// Minimal bank account resource shape for deserialising bank account URI links on other resources.
/// </summary>
/// <remarks>
/// Full bank account operations are not yet implemented in the SDK.
/// </remarks>
public sealed class BankAccount : IFreeAgentResource
{
    /// <summary>
    /// Bank account resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;
}
