namespace FreeAgent.Client.Models.Categories;

/// <summary>
/// Request attributes for updating an income category (nominal codes 001–049).
/// </summary>
public sealed class UpdateIncomeCategoryRequest
{
    /// <summary>
    /// Category name.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Unique nominal code from 001 to 049.
    /// </summary>
    public required string NominalCode { get; init; }
}

/// <summary>
/// Request attributes for updating a cost of sales category (nominal codes 096–199).
/// </summary>
public sealed class UpdateCostOfSalesCategoryRequest
{
    /// <summary>
    /// Category name.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Unique nominal code from 096 to 199.
    /// </summary>
    public required string NominalCode { get; init; }

    /// <summary>
    /// Statutory accounts reporting name wire key (for example <c>purchases</c>).
    /// </summary>
    /// <remarks>Valid values depend on company type — see the FreeAgent Categories API docs.</remarks>
    public required string TaxReportingName { get; init; }

    /// <summary>
    /// Whether the cost can be deducted from income when working out tax.
    /// </summary>
    public required bool AllowableForTax { get; init; }

    /// <summary>
    /// Automatic sales tax rate.
    /// </summary>
    public required CategoryAutoSalesTaxRate AutoSalesTaxRate { get; init; }
}

/// <summary>
/// Request attributes for updating an admin expenses category (nominal codes 200–399).
/// </summary>
public sealed class UpdateAdminExpensesCategoryRequest
{
    /// <summary>
    /// Category name.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Unique nominal code from 200 to 399.
    /// </summary>
    public required string NominalCode { get; init; }

    /// <summary>
    /// Statutory accounts reporting name wire key (for example <c>computer_software_costs</c>).
    /// </summary>
    /// <remarks>Valid values depend on company type — see the FreeAgent Categories API docs.</remarks>
    public required string TaxReportingName { get; init; }

    /// <summary>
    /// Whether the cost can be deducted from income when working out tax.
    /// </summary>
    public required bool AllowableForTax { get; init; }

    /// <summary>
    /// Automatic sales tax rate.
    /// </summary>
    public required CategoryAutoSalesTaxRate AutoSalesTaxRate { get; init; }
}

/// <summary>
/// Request attributes for updating a current asset category (nominal codes 671–720).
/// </summary>
public sealed class UpdateCurrentAssetCategoryRequest
{
    /// <summary>
    /// Category name.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Unique nominal code from 671 to 720.
    /// </summary>
    public required string NominalCode { get; init; }

    /// <summary>
    /// Statutory accounts reporting name wire key (for example <c>debtors</c>).
    /// </summary>
    public required string TaxReportingName { get; init; }
}

/// <summary>
/// Request attributes for updating a liabilities category (nominal codes 731–780).
/// </summary>
public sealed class UpdateLiabilitiesCategoryRequest
{
    /// <summary>
    /// Category name.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Unique nominal code from 731 to 780.
    /// </summary>
    public required string NominalCode { get; init; }

    /// <summary>
    /// Statutory accounts reporting name wire key (for example <c>creditors</c>).
    /// </summary>
    public required string TaxReportingName { get; init; }
}

/// <summary>
/// Request attributes for updating an equity category (nominal codes 921–960).
/// </summary>
public sealed class UpdateEquityCategoryRequest
{
    /// <summary>
    /// Category name.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Unique nominal code from 921 to 960.
    /// </summary>
    public required string NominalCode { get; init; }
}
