using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Company;

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
    /// Company ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

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
    /// Company type (e.g., "UkLimitedCompany", "UkSoleTrader", etc.). May be <see langword="null"/> when omitted by the API.
    /// </summary>
    [JsonPropertyName("type")]
    public CompanyType? Type { get; set; }

    /// <summary>
    /// Company currency code (e.g., "GBP", "USD"). May be <see langword="null"/> when omitted by the API.
    /// </summary>
    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    /// <summary>
    /// Mileage units (e.g., "miles", "kilometres").
    /// </summary>
    [JsonPropertyName("mileage_units")]
    public MileageUnit? MileageUnits { get; set; }

    /// <summary>
    /// Company registration number.
    /// </summary>
    [JsonPropertyName("company_registration_number")]
    public string? CompanyRegistrationNumber { get; set; }

    /// <summary>
    /// Sales tax registration status.
    /// </summary>
    [JsonPropertyName("sales_tax_registration_status")]
    public SalesTaxRegistrationStatus? SalesTaxRegistrationStatus { get; set; }

    /// <summary>
    /// VAT number.
    /// </summary>
    [JsonPropertyName("sales_tax_number")]
    public string? SalesTaxNumber { get; set; }

    /// <summary>
    /// Company start date.
    /// </summary>
    [JsonPropertyName("company_start_date")]
    public DateOnly? CompanyStartDate { get; set; }

    /// <summary>
    /// Trading start date when it differs from <see cref="CompanyStartDate"/>.
    /// </summary>
    [JsonPropertyName("trading_start_date")]
    public DateOnly? TradingStartDate { get; set; }

    /// <summary>
    /// First accounting year end date.
    /// </summary>
    [JsonPropertyName("first_accounting_year_end")]
    public DateOnly? FirstAccountingYearEnd { get; set; }

    /// <summary>
    /// Accounting periods covered by annual accounts.
    /// </summary>
    [JsonPropertyName("annual_accounting_periods")]
    public List<AnnualAccountingPeriod> AnnualAccountingPeriods { get; set; } = new();

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
    /// Company business category.
    /// Valid values are returned by the company business categories endpoint.
    /// </summary>
    [JsonPropertyName("business_category")]
    public string? BusinessCategory { get; set; }

    /// <summary>
    /// Date format preference used throughout the account.
    /// </summary>
    [JsonPropertyName("short_date_format")]
    public string? ShortDateFormat { get; set; }

    /// <summary>
    /// FreeAgent accounting start date.
    /// </summary>
    [JsonPropertyName("freeagent_start_date")]
    public DateOnly? FreeAgentStartDate { get; set; }

    /// <summary>
    /// First address line.
    /// </summary>
    [JsonPropertyName("address1")]
    public string? Address1 { get; set; }

    /// <summary>
    /// Second address line.
    /// </summary>
    [JsonPropertyName("address2")]
    public string? Address2 { get; set; }

    /// <summary>
    /// Third address line.
    /// </summary>
    [JsonPropertyName("address3")]
    public string? Address3 { get; set; }

    /// <summary>
    /// Town.
    /// </summary>
    [JsonPropertyName("town")]
    public string? Town { get; set; }

    /// <summary>
    /// Region or state.
    /// </summary>
    [JsonPropertyName("region")]
    public string? Region { get; set; }

    /// <summary>
    /// Postcode or ZIP code.
    /// </summary>
    [JsonPropertyName("postcode")]
    public string? Postcode { get; set; }

    /// <summary>
    /// Country.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    /// Contact email address.
    /// </summary>
    [JsonPropertyName("contact_email")]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Contact phone number.
    /// </summary>
    [JsonPropertyName("contact_phone")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// Company website.
    /// </summary>
    [JsonPropertyName("website")]
    public string? Website { get; set; }

    /// <summary>
    /// Current sales tax name.
    /// </summary>
    [JsonPropertyName("sales_tax_name")]
    public string? SalesTaxName { get; set; }

    /// <summary>
    /// Current sales tax registration number.
    /// </summary>
    [JsonPropertyName("sales_tax_registration_number")]
    public string? SalesTaxRegistrationNumber { get; set; }

    /// <summary>
    /// Date current sales tax took effect.
    /// </summary>
    [JsonPropertyName("sales_tax_effective_date")]
    public DateOnly? SalesTaxEffectiveDate { get; set; }

    /// <summary>
    /// Current sales tax rates.
    /// </summary>
    [JsonPropertyName("sales_tax_rates")]
    public List<SalesTaxRate> SalesTaxRates { get; set; } = new();

    /// <summary>
    /// Indicates if the main sales tax is value added.
    /// </summary>
    [JsonPropertyName("sales_tax_is_value_added")]
    public bool? SalesTaxIsValueAdded { get; set; }

    /// <summary>
    /// Indicates if Construction Industry Scheme for subcontractors is enabled.
    /// </summary>
    [JsonPropertyName("cis_enabled")]
    public bool? CisEnabled { get; set; }

    /// <summary>
    /// Alias for subcontractor CIS setting.
    /// </summary>
    [JsonPropertyName("cis_subcontractor")]
    public bool? CisSubcontractor { get; set; }

    /// <summary>
    /// Indicates if Construction Industry Scheme for contractors is enabled.
    /// </summary>
    [JsonPropertyName("cis_contractor")]
    public bool? CisContractor { get; set; }

    /// <summary>
    /// Attribute names that cannot be modified.
    /// </summary>
    [JsonPropertyName("locked_attributes")]
    public List<string> LockedAttributes { get; set; } = new();

    /// <summary>
    /// End date for first VAT return period.
    /// </summary>
    [JsonPropertyName("vat_first_return_period_ends_on")]
    public DateOnly? VatFirstReturnPeriodEndsOn { get; set; }

    /// <summary>
    /// VAT accounting basis on registration date.
    /// </summary>
    [JsonPropertyName("initial_vat_basis")]
    public InitialVatBasis? InitialVatBasis { get; set; }

    /// <summary>
    /// Indicates if the company was initially on Flat Rate Scheme.
    /// </summary>
    [JsonPropertyName("initially_on_frs")]
    public bool? InitiallyOnFrs { get; set; }

    /// <summary>
    /// Initial Flat Rate Scheme type.
    /// </summary>
    [JsonPropertyName("initial_vat_frs_type")]
    public string? InitialVatFrsType { get; set; }

    /// <summary>
    /// Sales tax deregistration effective date.
    /// </summary>
    [JsonPropertyName("sales_tax_deregistration_effective_date")]
    public DateOnly? SalesTaxDeregistrationEffectiveDate { get; set; }

    /// <summary>
    /// Current second sales tax name.
    /// </summary>
    [JsonPropertyName("second_sales_tax_name")]
    public string? SecondSalesTaxName { get; set; }

    /// <summary>
    /// Current second sales tax rates.
    /// </summary>
    [JsonPropertyName("second_sales_tax_rates")]
    public List<SalesTaxRate> SecondSalesTaxRates { get; set; } = new();

    /// <summary>
    /// Indicates if second sales tax is compounded on top of main sales tax.
    /// </summary>
    [JsonPropertyName("second_sales_tax_is_compound")]
    public bool? SecondSalesTaxIsCompound { get; set; }

    /// <summary>
    /// Creation timestamp in UTC.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp in UTC.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}
