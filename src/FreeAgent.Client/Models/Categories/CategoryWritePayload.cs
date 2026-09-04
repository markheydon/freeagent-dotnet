using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>
/// Writable category attributes for create and update requests.
/// </summary>
public sealed class CategoryWritePayload
{
    /// <summary>
    /// Category name.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Category nominal code.
    /// </summary>
    [JsonPropertyName("nominal_code")]
    public string? NominalCode { get; set; }

    /// <summary>
    /// Category group. Required when creating a category.
    /// </summary>
    [JsonPropertyName("category_group")]
    public CategoryGroup? CategoryGroup { get; set; }

    /// <summary>
    /// Statutory accounts reporting name wire key.
    /// </summary>
    /// <remarks>
    /// Use documented snake_case values (for example <c>purchases</c>, <c>computer_software_costs</c>).
    /// </remarks>
    [JsonPropertyName("tax_reporting_name")]
    public string? TaxReportingName { get; set; }

    /// <summary>
    /// Whether the cost can be deducted from income when working out tax.
    /// </summary>
    [JsonPropertyName("allowable_for_tax")]
    public bool? AllowableForTax { get; set; }

    /// <summary>
    /// Automatic sales tax rate for the category.
    /// </summary>
    [JsonPropertyName("auto_sales_tax_rate")]
    public CategoryAutoSalesTaxRate? AutoSalesTaxRate { get; set; }
}
