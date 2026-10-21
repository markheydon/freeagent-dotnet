using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Represents a FreeAgent capital asset type.
/// </summary>
public sealed class CapitalAssetType : IFreeAgentResource
{
    /// <summary>
    /// Capital asset type resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Display name of the capital asset type.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Whether this is a system default type (<see langword="true"/>) or user-created (<see langword="false"/>).
    /// </summary>
    [JsonPropertyName("system_default")]
    public bool? SystemDefault { get; set; }

    /// <summary>
    /// Creation timestamp (UTC).
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp (UTC).
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}
