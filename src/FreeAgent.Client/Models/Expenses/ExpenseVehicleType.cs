using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Expenses;

/// <summary>
/// Vehicle type for a mileage expense.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ExpenseVehicleType>))]
public enum ExpenseVehicleType
{
    /// <summary>Car.</summary>
    [JsonStringEnumMemberName("Car")]
    Car,

    /// <summary>Motorcycle.</summary>
    [JsonStringEnumMemberName("Motorcycle")]
    Motorcycle,

    /// <summary>Bicycle.</summary>
    [JsonStringEnumMemberName("Bicycle")]
    Bicycle
}
