using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Contacts;

/// <summary>
/// Sales tax charging behaviour for a contact.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ChargeSalesTax>))]
public enum ChargeSalesTax
{
    /// <summary>Automatic (wire value: "Auto").</summary>
    [JsonStringEnumMemberName("Auto")]
    Auto,

    /// <summary>Always charge (wire value: "Always").</summary>
    [JsonStringEnumMemberName("Always")]
    Always,

    /// <summary>Never charge (wire value: "Never").</summary>
    [JsonStringEnumMemberName("Never")]
    Never
}
