using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Contacts;

/// <summary>
/// CIS deduction rate band for a subcontractor contact.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<CisDeductionRate>))]
public enum CisDeductionRate
{
    /// <summary>Gross payment (wire value: "cis_gross").</summary>
    [JsonStringEnumMemberName("cis_gross")]
    Gross,

    /// <summary>Standard rate (wire value: "cis_standard").</summary>
    [JsonStringEnumMemberName("cis_standard")]
    Standard,

    /// <summary>Higher rate (wire value: "cis_higher").</summary>
    [JsonStringEnumMemberName("cis_higher")]
    Higher
}
