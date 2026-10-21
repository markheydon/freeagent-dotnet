using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Payroll;

/// <summary>
/// Student loan plan type on a payslip.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<PayrollStudentLoanDeductionsPlan>))]
public enum PayrollStudentLoanDeductionsPlan
{
    /// <summary>Plan 1 student loan.</summary>
    [JsonStringEnumMemberName("Plan 1")]
    Plan1,

    /// <summary>Plan 2 student loan.</summary>
    [JsonStringEnumMemberName("Plan 2")]
    Plan2,
}
