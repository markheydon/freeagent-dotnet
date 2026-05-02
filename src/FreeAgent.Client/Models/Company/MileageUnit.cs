using FreeAgent.Client.Infrastructure.Serialization;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Company;

/// <summary>
/// Represents the mileage unit system used by a FreeAgent company.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<MileageUnit>))]
public enum MileageUnit
{
    /// <summary>Miles (wire value: "miles").</summary>
    [JsonStringEnumMemberName("miles")]
    Miles,
    /// <summary>Kilometres (wire value: "kilometres").</summary>
    [JsonStringEnumMemberName("kilometres")]
    Kilometers
}
