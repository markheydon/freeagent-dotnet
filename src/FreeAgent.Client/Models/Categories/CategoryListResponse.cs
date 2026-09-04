using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>
/// Response envelope for listing categories.
/// </summary>
public sealed class CategoryListResponse
{
    /// <summary>
    /// Admin expenses categories.
    /// </summary>
    [JsonPropertyName("admin_expenses_categories")]
    public List<Category>? AdminExpensesCategories { get; set; }

    /// <summary>
    /// Cost of sales categories.
    /// </summary>
    [JsonPropertyName("cost_of_sales_categories")]
    public List<Category>? CostOfSalesCategories { get; set; }

    /// <summary>
    /// Income categories.
    /// </summary>
    [JsonPropertyName("income_categories")]
    public List<Category>? IncomeCategories { get; set; }

    /// <summary>
    /// General categories.
    /// </summary>
    [JsonPropertyName("general_categories")]
    public List<Category>? GeneralCategories { get; set; }
}
