using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models;

/// <summary>
/// Represents a company in FreeAgent.
/// </summary>
public class Company
{
    /// <summary>
    /// Company URL identifier.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Company name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Company subdomain.
    /// </summary>
    [JsonPropertyName("subdomain")]
    public string Subdomain { get; set; } = string.Empty;

    /// <summary>
    /// Company type (e.g., "UkLimitedCompany", "UkSoleTrader", etc.).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Company currency code (e.g., "GBP", "USD").
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Mileage units (e.g., "miles", "kilometres").
    /// </summary>
    [JsonPropertyName("mileage_units")]
    public string? MileageUnits { get; set; }

    /// <summary>
    /// Company registration number.
    /// </summary>
    [JsonPropertyName("company_registration_number")]
    public string? CompanyRegistrationNumber { get; set; }

    /// <summary>
    /// Sales tax registration status.
    /// </summary>
    [JsonPropertyName("sales_tax_registration_status")]
    public string? SalesTaxRegistrationStatus { get; set; }

    /// <summary>
    /// VAT number.
    /// </summary>
    [JsonPropertyName("sales_tax_number")]
    public string? SalesTaxNumber { get; set; }

    /// <summary>
    /// Company start date.
    /// </summary>
    [JsonPropertyName("company_start_date")]
    public DateTime? CompanyStartDate { get; set; }

    /// <summary>
    /// First accounting year end date.
    /// </summary>
    [JsonPropertyName("first_accounting_year_end")]
    public DateTime? FirstAccountingYearEnd { get; set; }

    /// <summary>
    /// Supports auto-sales-tax on expenses.
    /// </summary>
    [JsonPropertyName("supports_auto_sales_tax_on_expenses")]
    public bool SupportsAutoSalesTaxOnExpenses { get; set; }

    /// <summary>
    /// Business type.
    /// </summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }

    /// <summary>
    /// FreeAgent accounting start date.
    /// </summary>
    [JsonPropertyName("freeagent_start_date")]
    public DateTime? FreeAgentStartDate { get; set; }
}
