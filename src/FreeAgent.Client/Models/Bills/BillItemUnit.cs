using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Unit of measure for a bill line item.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BillItemUnit>))]
public enum BillItemUnit
{
    /// <summary>No unit (default).</summary>
    [JsonStringEnumMemberName("-no unit-")]
    NoUnit,

    /// <summary>Hours.</summary>
    [JsonStringEnumMemberName("Hours")]
    Hours,

    /// <summary>Days.</summary>
    [JsonStringEnumMemberName("Days")]
    Days,

    /// <summary>Weeks.</summary>
    [JsonStringEnumMemberName("Weeks")]
    Weeks,

    /// <summary>Months.</summary>
    [JsonStringEnumMemberName("Months")]
    Months,

    /// <summary>Years.</summary>
    [JsonStringEnumMemberName("Years")]
    Years,

    /// <summary>Products.</summary>
    [JsonStringEnumMemberName("Products")]
    Products,

    /// <summary>Services.</summary>
    [JsonStringEnumMemberName("Services")]
    Services,

    /// <summary>Training.</summary>
    [JsonStringEnumMemberName("Training")]
    Training,

    /// <summary>Stock.</summary>
    [JsonStringEnumMemberName("Stock")]
    Stock,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
