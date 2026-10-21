using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// EU member state country name for EC VAT MOSS <c>place_of_supply</c> and MOSS sales tax rate lookups.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<EcMossPlaceOfSupply>))]
public enum EcMossPlaceOfSupply
{
    /// <summary>Austria.</summary>
    [JsonStringEnumMemberName("Austria")]
    Austria,

    /// <summary>Belgium.</summary>
    [JsonStringEnumMemberName("Belgium")]
    Belgium,

    /// <summary>Bulgaria.</summary>
    [JsonStringEnumMemberName("Bulgaria")]
    Bulgaria,

    /// <summary>Croatia.</summary>
    [JsonStringEnumMemberName("Croatia")]
    Croatia,

    /// <summary>Cyprus.</summary>
    [JsonStringEnumMemberName("Cyprus")]
    Cyprus,

    /// <summary>Czech Republic.</summary>
    [JsonStringEnumMemberName("Czech Republic")]
    CzechRepublic,

    /// <summary>Denmark.</summary>
    [JsonStringEnumMemberName("Denmark")]
    Denmark,

    /// <summary>Estonia.</summary>
    [JsonStringEnumMemberName("Estonia")]
    Estonia,

    /// <summary>Finland.</summary>
    [JsonStringEnumMemberName("Finland")]
    Finland,

    /// <summary>France.</summary>
    [JsonStringEnumMemberName("France")]
    France,

    /// <summary>Germany.</summary>
    [JsonStringEnumMemberName("Germany")]
    Germany,

    /// <summary>Greece.</summary>
    [JsonStringEnumMemberName("Greece")]
    Greece,

    /// <summary>Hungary.</summary>
    [JsonStringEnumMemberName("Hungary")]
    Hungary,

    /// <summary>Ireland.</summary>
    [JsonStringEnumMemberName("Ireland")]
    Ireland,

    /// <summary>Italy.</summary>
    [JsonStringEnumMemberName("Italy")]
    Italy,

    /// <summary>Latvia.</summary>
    [JsonStringEnumMemberName("Latvia")]
    Latvia,

    /// <summary>Lithuania.</summary>
    [JsonStringEnumMemberName("Lithuania")]
    Lithuania,

    /// <summary>Luxembourg.</summary>
    [JsonStringEnumMemberName("Luxembourg")]
    Luxembourg,

    /// <summary>Malta.</summary>
    [JsonStringEnumMemberName("Malta")]
    Malta,

    /// <summary>Netherlands.</summary>
    [JsonStringEnumMemberName("Netherlands")]
    Netherlands,

    /// <summary>Poland.</summary>
    [JsonStringEnumMemberName("Poland")]
    Poland,

    /// <summary>Portugal.</summary>
    [JsonStringEnumMemberName("Portugal")]
    Portugal,

    /// <summary>Romania.</summary>
    [JsonStringEnumMemberName("Romania")]
    Romania,

    /// <summary>Slovakia.</summary>
    [JsonStringEnumMemberName("Slovakia")]
    Slovakia,

    /// <summary>Slovenia.</summary>
    [JsonStringEnumMemberName("Slovenia")]
    Slovenia,

    /// <summary>Spain.</summary>
    [JsonStringEnumMemberName("Spain")]
    Spain,

    /// <summary>Sweden.</summary>
    [JsonStringEnumMemberName("Sweden")]
    Sweden,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
