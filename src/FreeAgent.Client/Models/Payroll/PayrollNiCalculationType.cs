using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Payroll;

/// <summary>
/// National Insurance calculation method for a payslip.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<PayrollNiCalculationType>))]
public enum PayrollNiCalculationType
{
    /// <summary>Director NI calculation.</summary>
    [JsonStringEnumMemberName("Director")]
    Director,

    /// <summary>Employee NI calculation.</summary>
    [JsonStringEnumMemberName("Employee")]
    Employee,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
