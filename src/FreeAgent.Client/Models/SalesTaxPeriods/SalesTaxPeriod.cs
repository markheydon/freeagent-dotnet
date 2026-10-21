using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Represents a FreeAgent sales tax period.
/// </summary>
public sealed class SalesTaxPeriod : IFreeAgentResource
{
    /// <summary>
    /// Sales tax period resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Name of the sales tax (for example VAT, GST, or Sales Tax).
    /// </summary>
    [JsonPropertyName("sales_tax_name")]
    public string? SalesTaxName { get; set; }

    /// <summary>
    /// Registration status for this sales tax period.
    /// </summary>
    [JsonPropertyName("sales_tax_registration_status")]
    public SalesTaxPeriodRegistrationStatus? SalesTaxRegistrationStatus { get; set; }

    /// <summary>
    /// First rate of the sales tax.
    /// </summary>
    [JsonPropertyName("sales_tax_rate_1")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SalesTaxRate1 { get; set; }

    /// <summary>
    /// Second rate of the sales tax.
    /// </summary>
    [JsonPropertyName("sales_tax_rate_2")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SalesTaxRate2 { get; set; }

    /// <summary>
    /// Third rate of the sales tax.
    /// </summary>
    [JsonPropertyName("sales_tax_rate_3")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SalesTaxRate3 { get; set; }

    /// <summary>
    /// Whether tax paid on purchases can be reclaimed.
    /// </summary>
    [JsonPropertyName("sales_tax_is_value_added")]
    public bool? SalesTaxIsValueAdded { get; set; }

    /// <summary>
    /// Sales tax registration number when required.
    /// </summary>
    [JsonPropertyName("sales_tax_registration_number")]
    public string? SalesTaxRegistrationNumber { get; set; }

    /// <summary>
    /// Date from which this sales tax period takes effect.
    /// </summary>
    [JsonPropertyName("effective_date")]
    public DateOnly? EffectiveDate { get; set; }

    /// <summary>
    /// Whether changes to this period are blocked.
    /// </summary>
    [JsonPropertyName("is_locked")]
    public bool? IsLocked { get; set; }

    /// <summary>
    /// Reason the sales tax period is locked, when provided by FreeAgent.
    /// </summary>
    [JsonPropertyName("locked_reason")]
    public string? LockedReason { get; set; }

    /// <summary>
    /// Name of the second sales tax for universal companies (for example PST).
    /// </summary>
    [JsonPropertyName("second_sales_tax_name")]
    public string? SecondSalesTaxName { get; set; }

    /// <summary>
    /// First rate of the second sales tax.
    /// </summary>
    [JsonPropertyName("second_sales_tax_rate_1")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SecondSalesTaxRate1 { get; set; }

    /// <summary>
    /// Second rate of the second sales tax.
    /// </summary>
    [JsonPropertyName("second_sales_tax_rate_2")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SecondSalesTaxRate2 { get; set; }

    /// <summary>
    /// Third rate of the second sales tax.
    /// </summary>
    [JsonPropertyName("second_sales_tax_rate_3")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SecondSalesTaxRate3 { get; set; }

    /// <summary>
    /// Whether the second sales tax is compounded on the main sales tax.
    /// </summary>
    [JsonPropertyName("second_sales_tax_is_compound")]
    public bool? SecondSalesTaxIsCompound { get; set; }
}
