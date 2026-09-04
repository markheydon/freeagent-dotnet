using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>
/// Represents a FreeAgent chart-of-accounts category.
/// </summary>
public class Category
{
    /// <summary>
    /// Category resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

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
    /// Name of the group to which the category belongs.
    /// </summary>
    [JsonPropertyName("group_description")]
    public string? GroupDescription { get; set; }

    /// <summary>
    /// Whether the cost can be deducted from income when working out tax.
    /// </summary>
    [JsonPropertyName("allowable_for_tax")]
    [JsonConverter(typeof(NullableBooleanJsonConverter))]
    public bool? AllowableForTax { get; set; }

    /// <summary>
    /// Where the category is reported in statutory accounts.
    /// </summary>
    /// <remarks>
    /// Responses return a display label (for example <c>Purchases</c>).
    /// Create and update requests use documented snake_case wire keys (for example <c>purchases</c>).
    /// </remarks>
    [JsonPropertyName("tax_reporting_name")]
    public string? TaxReportingName { get; set; }

    /// <summary>
    /// Automatic sales tax rate for the category.
    /// </summary>
    [JsonPropertyName("auto_sales_tax_rate")]
    public CategoryAutoSalesTaxRate? AutoSalesTaxRate { get; set; }

    /// <summary>
    /// Bank account represented by the sub account.
    /// </summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>
    /// Capital asset type represented by the sub account.
    /// </summary>
    [JsonPropertyName("capital_asset_type")]
    public string? CapitalAssetType { get; set; }

    /// <summary>
    /// Stock item represented by the sub account.
    /// </summary>
    [JsonPropertyName("stock_item")]
    public string? StockItem { get; set; }

    /// <summary>
    /// Hire purchase represented by the sub account.
    /// </summary>
    [JsonPropertyName("hire_purchase")]
    public string? HirePurchase { get; set; }

    /// <summary>
    /// User represented by the sub account.
    /// </summary>
    [JsonPropertyName("user")]
    public string? User { get; set; }
}
