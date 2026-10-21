using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Payroll period filing status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<PayrollPeriodFilingStatus>))]
public enum PayrollPeriodFilingStatus
{
    /// <summary>Period not yet filed.</summary>
    [JsonStringEnumMemberName("unfiled")]
    Unfiled,

    /// <summary>Submission pending.</summary>
    [JsonStringEnumMemberName("pending")]
    Pending,

    /// <summary>Submission rejected.</summary>
    [JsonStringEnumMemberName("rejected")]
    Rejected,

    /// <summary>Period partially filed.</summary>
    [JsonStringEnumMemberName("partially_filed")]
    PartiallyFiled,

    /// <summary>Period filed.</summary>
    [JsonStringEnumMemberName("filed")]
    Filed,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
