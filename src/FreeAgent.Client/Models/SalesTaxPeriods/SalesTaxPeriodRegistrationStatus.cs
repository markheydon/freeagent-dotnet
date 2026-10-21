using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.SalesTaxPeriods;

/// <summary>
/// Sales tax registration status on a sales tax period.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<SalesTaxPeriodRegistrationStatus>))]
public enum SalesTaxPeriodRegistrationStatus
{
    /// <summary>Not registered for sales tax (wire value: "Not Registered").</summary>
    [JsonStringEnumMemberName("Not Registered")]
    NotRegistered,

    /// <summary>Registered for sales tax (wire value: "Registered").</summary>
    [JsonStringEnumMemberName("Registered")]
    Registered,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
