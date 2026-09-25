using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// Estimate line item type values.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<EstimateItemType>))]
public enum EstimateItemType
{
    /// <summary>Hours unit.</summary>
    [JsonStringEnumMemberName("Hours")]
    Hours,

    /// <summary>Days unit.</summary>
    [JsonStringEnumMemberName("Days")]
    Days,

    /// <summary>Weeks unit.</summary>
    [JsonStringEnumMemberName("Weeks")]
    Weeks,

    /// <summary>Months unit.</summary>
    [JsonStringEnumMemberName("Months")]
    Months,

    /// <summary>Years unit.</summary>
    [JsonStringEnumMemberName("Years")]
    Years,

    /// <summary>No unit.</summary>
    [JsonStringEnumMemberName("-no unit-")]
    NoUnit,

    /// <summary>Products unit.</summary>
    [JsonStringEnumMemberName("Products")]
    Products,

    /// <summary>Services unit.</summary>
    [JsonStringEnumMemberName("Services")]
    Services,

    /// <summary>Training unit.</summary>
    [JsonStringEnumMemberName("Training")]
    Training,

    /// <summary>Expenses unit.</summary>
    [JsonStringEnumMemberName("Expenses")]
    Expenses,

    /// <summary>Comment line.</summary>
    [JsonStringEnumMemberName("Comments")]
    Comments,

    /// <summary>Bills unit.</summary>
    [JsonStringEnumMemberName("Bills")]
    Bills,

    /// <summary>Discount line.</summary>
    [JsonStringEnumMemberName("Discount")]
    Discount,

    /// <summary>Credit line.</summary>
    [JsonStringEnumMemberName("Credit")]
    Credit
}
