namespace FreeAgent.Client.Models.Categories;

/// <summary>
/// All category sets returned by the list categories endpoint.
/// </summary>
public sealed class CategorySets
{
    /// <summary>
    /// Admin expenses categories.
    /// </summary>
    public IReadOnlyList<Category> AdminExpensesCategories { get; init; } = [];

    /// <summary>
    /// Cost of sales categories.
    /// </summary>
    public IReadOnlyList<Category> CostOfSalesCategories { get; init; } = [];

    /// <summary>
    /// Income categories.
    /// </summary>
    public IReadOnlyList<Category> IncomeCategories { get; init; } = [];

    /// <summary>
    /// General categories (assets, liabilities, equities, and similar).
    /// </summary>
    public IReadOnlyList<Category> GeneralCategories { get; init; } = [];

    /// <summary>
    /// All categories flattened across every set.
    /// </summary>
    public IReadOnlyList<Category> AllCategories =>
        AdminExpensesCategories
            .Concat(CostOfSalesCategories)
            .Concat(IncomeCategories)
            .Concat(GeneralCategories)
            .ToList();
}
