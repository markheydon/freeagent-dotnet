using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Company;

/// <summary>
/// Represents the mileage unit system used by a FreeAgent company.
/// </summary>
[JsonConverter(typeof(MileageUnitJsonConverter))]
public enum MileageUnit
{
    /// <summary>Miles (wire value: "miles").</summary>
    Miles,

    /// <summary>Kilometres (wire values: "kilometers" or "kilometres").</summary>
    Kilometers
}
