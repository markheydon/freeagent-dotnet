using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Expenses;

/// <summary>
/// Engine type for a mileage expense when the vehicle is a car or motorcycle.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ExpenseEngineType>))]
public enum ExpenseEngineType
{
    /// <summary>Petrol engine.</summary>
    [JsonStringEnumMemberName("Petrol")]
    Petrol,

    /// <summary>Diesel engine.</summary>
    [JsonStringEnumMemberName("Diesel")]
    Diesel,

    /// <summary>LPG engine.</summary>
    [JsonStringEnumMemberName("LPG")]
    Lpg,

    /// <summary>Electric vehicle.</summary>
    [JsonStringEnumMemberName("Electric")]
    Electric,

    /// <summary>Electric vehicle charged at home.</summary>
    [JsonStringEnumMemberName("Electric (Home charger)")]
    ElectricHomeCharger,

    /// <summary>Electric vehicle charged at a public charger.</summary>
    [JsonStringEnumMemberName("Electric (Public charger)")]
    ElectricPublicCharger,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
