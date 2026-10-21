using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Contact status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ContactStatus>))]
public enum ContactStatus
{
    /// <summary>Active contact (wire value: "Active").</summary>
    [JsonStringEnumMemberName("Active")]
    Active,

    /// <summary>Hidden contact (wire value: "Hidden").</summary>
    [JsonStringEnumMemberName("Hidden")]
    Hidden,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
