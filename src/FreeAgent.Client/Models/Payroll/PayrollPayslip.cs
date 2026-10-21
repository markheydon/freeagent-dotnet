using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Users;

namespace FreeAgent.Client.Models.Payroll;

/// <summary>
/// Payslip amounts for a user in a payroll period.
/// </summary>
public class PayrollPayslip
{
    /// <summary>
    /// Wire representation of the user link.
    /// </summary>
    [JsonPropertyName("user")]
    [JsonInclude]
    internal ExpandableField<User>? UserLink { get; set; }

    /// <summary>
    /// User when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public User? User => UserLink?.Value;

    /// <summary>
    /// User identifier when the API returns a user link.
    /// </summary>
    [JsonIgnore]
    public long? UserId => UserLink?.Id;

    /// <summary>
    /// HMRC tax code for the payslip.
    /// </summary>
    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    /// <summary>
    /// Payslip date.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Basic pay this period.
    /// </summary>
    [JsonPropertyName("basic_pay")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? BasicPay { get; set; }

    /// <summary>
    /// Tax deducted this period.
    /// </summary>
    [JsonPropertyName("tax_deducted")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? TaxDeducted { get; set; }

    /// <summary>
    /// Employee NI payable this period.
    /// </summary>
    [JsonPropertyName("employee_ni")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? EmployeeNi { get; set; }

    /// <summary>
    /// Employer NI payable this period.
    /// </summary>
    [JsonPropertyName("employer_ni")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? EmployerNi { get; set; }

    /// <summary>
    /// Other deductions this period.
    /// </summary>
    [JsonPropertyName("other_deductions")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? OtherDeductions { get; set; }

    /// <summary>
    /// Student loan deductions this period.
    /// </summary>
    [JsonPropertyName("student_loan_deduction")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? StudentLoanDeduction { get; set; }

    /// <summary>
    /// Postgraduate loan deductions this period.
    /// </summary>
    [JsonPropertyName("postgrad_loan_deduction")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? PostgradLoanDeduction { get; set; }

    /// <summary>
    /// Overtime paid this period.
    /// </summary>
    [JsonPropertyName("overtime")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Overtime { get; set; }

    /// <summary>
    /// Commission paid this period.
    /// </summary>
    [JsonPropertyName("commission")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Commission { get; set; }

    /// <summary>
    /// Bonus paid this period.
    /// </summary>
    [JsonPropertyName("bonus")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Bonus { get; set; }

    /// <summary>
    /// Allowance paid this period.
    /// </summary>
    [JsonPropertyName("allowance")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Allowance { get; set; }

    /// <summary>
    /// Statutory sick pay this period.
    /// </summary>
    [JsonPropertyName("statutory_sick_pay")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? StatutorySickPay { get; set; }

    /// <summary>
    /// Statutory maternity pay this period.
    /// </summary>
    [JsonPropertyName("statutory_maternity_pay")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? StatutoryMaternityPay { get; set; }

    /// <summary>
    /// Statutory paternity pay this period.
    /// </summary>
    [JsonPropertyName("statutory_paternity_pay")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? StatutoryPaternityPay { get; set; }

    /// <summary>
    /// Statutory adoption pay this period.
    /// </summary>
    [JsonPropertyName("statutory_adoption_pay")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? StatutoryAdoptionPay { get; set; }

    /// <summary>
    /// Statutory parental bereavement pay this period.
    /// </summary>
    [JsonPropertyName("statutory_parental_bereavement_pay")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? StatutoryParentalBereavementPay { get; set; }

    /// <summary>
    /// Statutory neonatal care pay this period.
    /// </summary>
    [JsonPropertyName("statutory_neonatal_care_pay")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? StatutoryNeonatalCarePay { get; set; }

    /// <summary>
    /// Absence payments this period.
    /// </summary>
    [JsonPropertyName("absence_payments")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? AbsencePayments { get; set; }

    /// <summary>
    /// Other payments this period.
    /// </summary>
    [JsonPropertyName("other_payments")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? OtherPayments { get; set; }

    /// <summary>
    /// Employee pension deductions this period.
    /// </summary>
    [JsonPropertyName("employee_pension")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? EmployeePension { get; set; }

    /// <summary>
    /// Employer pension deductions this period.
    /// </summary>
    [JsonPropertyName("employer_pension")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? EmployerPension { get; set; }

    /// <summary>
    /// Employee pension contribution percentage when automatic calculations are enabled.
    /// </summary>
    [JsonPropertyName("employee_pension_percentage")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? EmployeePensionPercentage { get; set; }

    /// <summary>
    /// Employer pension contribution percentage when automatic calculations are enabled.
    /// </summary>
    [JsonPropertyName("employer_pension_percentage")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? EmployerPensionPercentage { get; set; }

    /// <summary>
    /// Attachment deductions this period.
    /// </summary>
    [JsonPropertyName("attachments")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Attachments { get; set; }

    /// <summary>
    /// Payroll giving deductions this period.
    /// </summary>
    [JsonPropertyName("payroll_giving")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? PayrollGiving { get; set; }

    /// <summary>
    /// NI calculation method.
    /// </summary>
    [JsonPropertyName("ni_calc_type")]
    public PayrollNiCalculationType? NiCalcType { get; set; }

    /// <summary>
    /// Pay frequency for the payslip.
    /// </summary>
    [JsonPropertyName("frequency")]
    public string? Frequency { get; set; }

    /// <summary>
    /// Additional statutory paternity pay this period.
    /// </summary>
    [JsonPropertyName("additional_statutory_paternity_pay")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? AdditionalStatutoryPaternityPay { get; set; }

    /// <summary>
    /// Deductions subject to NI but not PAYE this period.
    /// </summary>
    [JsonPropertyName("deductions_subject_to_nic_but_not_paye")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? DeductionsSubjectToNicButNotPaye { get; set; }

    /// <summary>
    /// Other deductions from net pay this period.
    /// </summary>
    [JsonPropertyName("other_deductions_from_net_pay")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? OtherDeductionsFromNetPay { get; set; }

    /// <summary>
    /// Employee pension deductions not under net pay agreement this period.
    /// </summary>
    [JsonPropertyName("employee_pension_not_under_net_pay")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? EmployeePensionNotUnderNetPay { get; set; }

    /// <summary>
    /// Other salary sacrifice deductions excluding pension contributions.
    /// </summary>
    [JsonPropertyName("other_salary_sacrifice_deductions")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? OtherSalarySacrificeDeductions { get; set; }

    /// <summary>
    /// Employee pension contributions under salary sacrifice.
    /// </summary>
    [JsonPropertyName("employee_pension_salary_sacrifice")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? EmployeePensionSalarySacrifice { get; set; }

    /// <summary>
    /// Pensionable earnings used for automatic pension calculations.
    /// </summary>
    [JsonPropertyName("pension_earnings_amount")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? PensionEarningsAmount { get; set; }

    /// <summary>
    /// HMRC NI classification letter.
    /// </summary>
    [JsonPropertyName("ni_letter")]
    public string? NiLetter { get; set; }

    /// <summary>
    /// Whether student loan payments are being deducted.
    /// </summary>
    [JsonPropertyName("deduct_student_loan")]
    public bool? DeductStudentLoan { get; set; }

    /// <summary>
    /// Student loan plan type when deductions apply.
    /// </summary>
    [JsonPropertyName("student_loan_deductions_plan")]
    public PayrollStudentLoanDeductionsPlan? StudentLoanDeductionsPlan { get; set; }

    /// <summary>
    /// Whether postgraduate loan payments are being deducted.
    /// </summary>
    [JsonPropertyName("deduct_postgrad_loan")]
    public bool? DeductPostgradLoan { get; set; }

    /// <summary>
    /// Whether deductions are calculated on a week 1 month 1 basis.
    /// </summary>
    [JsonPropertyName("week_1_month_1_basis")]
    public bool? Week1Month1Basis { get; set; }

    /// <summary>
    /// Pay this period not subject to NI or income tax.
    /// </summary>
    [JsonPropertyName("deduction_free_pay")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? DeductionFreePay { get; set; }

    /// <summary>
    /// Hours worked this period.
    /// </summary>
    [JsonPropertyName("hours_worked")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? HoursWorked { get; set; }

    /// <summary>
    /// Whether this is the final payslip for a leaving employee.
    /// </summary>
    [JsonPropertyName("leaving_payslip")]
    public bool? LeavingPayslip { get; set; }

    /// <summary>
    /// Created timestamp.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Updated timestamp.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}
