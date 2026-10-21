using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.CisSettings;

/// <summary>
/// PAYE/NI reporting period for CIS contractor registration.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<CisSettingsPayeNiPeriod>))]
public enum CisSettingsPayeNiPeriod
{
    /// <summary>Monthly reporting (wire value: "Monthly").</summary>
    [JsonStringEnumMemberName("Monthly")]
    Monthly,

    /// <summary>Quarterly reporting (wire value: "Quarterly").</summary>
    [JsonStringEnumMemberName("Quarterly")]
    Quarterly,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
