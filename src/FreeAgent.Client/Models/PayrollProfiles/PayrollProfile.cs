using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Users;

namespace FreeAgent.Client.Models.PayrollProfiles;

/// <summary>
/// Read-only payroll profile for a user in a tax year.
/// </summary>
public class PayrollProfile
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
    /// Optional internal employee payroll reference.
    /// </summary>
    [JsonPropertyName("payroll_reference")]
    public string? PayrollReference { get; set; }

    /// <summary>
    /// First address line.
    /// </summary>
    [JsonPropertyName("address_line_1")]
    public string? AddressLine1 { get; set; }

    /// <summary>
    /// Second address line.
    /// </summary>
    [JsonPropertyName("address_line_2")]
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Third address line.
    /// </summary>
    [JsonPropertyName("address_line_3")]
    public string? AddressLine3 { get; set; }

    /// <summary>
    /// Fourth address line.
    /// </summary>
    [JsonPropertyName("address_line_4")]
    public string? AddressLine4 { get; set; }

    /// <summary>
    /// UK postcode when present.
    /// </summary>
    [JsonPropertyName("postcode")]
    public string? Postcode { get; set; }

    /// <summary>
    /// Country when present for non-UK addresses.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    /// Title or honourific.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gender as defined by HMRC on the wire.
    /// </summary>
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    /// <summary>
    /// Date of birth.
    /// </summary>
    [JsonPropertyName("date_of_birth")]
    public DateOnly? DateOfBirth { get; set; }

    /// <summary>
    /// Total pay in previous employment during the tax year.
    /// </summary>
    [JsonPropertyName("total_pay_in_previous_employment")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? TotalPayInPreviousEmployment { get; set; }

    /// <summary>
    /// Total tax in previous employment during the tax year.
    /// </summary>
    [JsonPropertyName("total_tax_in_previous_employment")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? TotalTaxInPreviousEmployment { get; set; }

    /// <summary>
    /// Employment start date when the employee started during the tax year.
    /// </summary>
    [JsonPropertyName("employment_starts_on")]
    public DateOnly? EmploymentStartsOn { get; set; }

    /// <summary>
    /// Employment end date when the employee is leaving during the tax year.
    /// </summary>
    [JsonPropertyName("employment_ends_on")]
    public DateOnly? EmploymentEndsOn { get; set; }

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
