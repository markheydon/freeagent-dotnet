using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>
/// Response envelope for a single category returned by get, create, update, or delete.
/// </summary>
public sealed class CategorySingleResponse
{
    /// <summary>
    /// Admin expenses category payload.
    /// </summary>
    [JsonPropertyName("admin_expenses_categories")]
    public Category? AdminExpensesCategory { get; set; }

    /// <summary>
    /// Cost of sales category payload.
    /// </summary>
    [JsonPropertyName("cost_of_sales_categories")]
    public Category? CostOfSalesCategory { get; set; }

    /// <summary>
    /// Income category payload.
    /// </summary>
    [JsonPropertyName("income_categories")]
    public Category? IncomeCategory { get; set; }

    /// <summary>
    /// General category payload.
    /// </summary>
    [JsonPropertyName("general_categories")]
    public Category? GeneralCategory { get; set; }
}
