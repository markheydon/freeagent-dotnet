using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>
/// Writable category attributes serialised in create and update request envelopes.
/// </summary>
internal sealed class CategoryWritePayload
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("nominal_code")]
    public string? NominalCode { get; set; }

    [JsonPropertyName("category_group")]
    public CategoryGroup? CategoryGroup { get; set; }

    [JsonPropertyName("tax_reporting_name")]
    public string? TaxReportingName { get; set; }

    [JsonPropertyName("allowable_for_tax")]
    public bool? AllowableForTax { get; set; }

    [JsonPropertyName("auto_sales_tax_rate")]
    public CategoryAutoSalesTaxRate? AutoSalesTaxRate { get; set; }
}
