using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>
/// Request attributes for updating an income category (nominal codes 001–049).
/// </summary>
public sealed class UpdateIncomeCategoryRequest
{
    internal string Description { get; private init; } = string.Empty;

    internal string NominalCode { get; private init; } = string.Empty;

    /// <summary>
    /// Creates a request to update an income category.
    /// </summary>
    public static UpdateIncomeCategoryRequest Create(string description, string nominalCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
        CategoryNominalCodeValidator.ValidateIncome(nominalCode);

        return new UpdateIncomeCategoryRequest
        {
            Description = description,
            NominalCode = nominalCode
        };
    }
}

/// <summary>
/// Request attributes for updating a cost of sales category (nominal codes 096–199).
/// </summary>
public sealed class UpdateCostOfSalesCategoryRequest
{
    internal string Description { get; private init; } = string.Empty;

    internal string NominalCode { get; private init; } = string.Empty;

    internal string TaxReportingName { get; private init; } = string.Empty;

    internal bool AllowableForTax { get; private init; }

    internal CategoryAutoSalesTaxRate AutoSalesTaxRate { get; private init; }

    /// <summary>
    /// Creates an update request for a UK limited company cost of sales category.
    /// </summary>
    public static UpdateCostOfSalesCategoryRequest ForUkLimitedCompany(
        string description,
        string nominalCode,
        UkLimitedCompanyCostOfSalesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate autoSalesTaxRate) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    /// <summary>
    /// Creates an update request for a UK sole trader cost of sales category.
    /// </summary>
    public static UpdateCostOfSalesCategoryRequest ForUkSoleTrader(
        string description,
        string nominalCode,
        UkSoleTraderCostOfSalesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate autoSalesTaxRate) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    /// <summary>
    /// Creates an update request for a UK partnership cost of sales category.
    /// </summary>
    public static UpdateCostOfSalesCategoryRequest ForUkPartnership(
        string description,
        string nominalCode,
        UkPartnershipCostOfSalesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate autoSalesTaxRate) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    /// <summary>
    /// Creates an update request for a universal or US company cost of sales category.
    /// </summary>
    public static UpdateCostOfSalesCategoryRequest ForUniversalAndUsCompany(
        string description,
        string nominalCode,
        UniversalAndUsCostOfSalesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate autoSalesTaxRate) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    private static UpdateCostOfSalesCategoryRequest Create(
        string description,
        string nominalCode,
        string taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate autoSalesTaxRate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
        CategoryNominalCodeValidator.ValidateCostOfSales(nominalCode);

        return new UpdateCostOfSalesCategoryRequest
        {
            Description = description,
            NominalCode = nominalCode,
            TaxReportingName = taxReportingName,
            AllowableForTax = allowableForTax,
            AutoSalesTaxRate = autoSalesTaxRate
        };
    }
}

/// <summary>
/// Request attributes for updating an admin expenses category (nominal codes 200–399).
/// </summary>
public sealed class UpdateAdminExpensesCategoryRequest
{
    internal string Description { get; private init; } = string.Empty;

    internal string NominalCode { get; private init; } = string.Empty;

    internal string TaxReportingName { get; private init; } = string.Empty;

    internal bool AllowableForTax { get; private init; }

    internal CategoryAutoSalesTaxRate AutoSalesTaxRate { get; private init; }

    /// <summary>
    /// Creates an update request for a UK limited company admin expenses category.
    /// </summary>
    public static UpdateAdminExpensesCategoryRequest ForUkLimitedCompany(
        string description,
        string nominalCode,
        UkLimitedCompanyAdminExpensesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate autoSalesTaxRate) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    /// <summary>
    /// Creates an update request for a UK sole trader admin expenses category.
    /// </summary>
    public static UpdateAdminExpensesCategoryRequest ForUkSoleTrader(
        string description,
        string nominalCode,
        UkSoleTraderAdminExpensesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate autoSalesTaxRate) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    /// <summary>
    /// Creates an update request for a UK partnership admin expenses category.
    /// </summary>
    public static UpdateAdminExpensesCategoryRequest ForUkPartnership(
        string description,
        string nominalCode,
        UkPartnershipAdminExpensesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate autoSalesTaxRate) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    /// <summary>
    /// Creates an update request for a universal or US company admin expenses category.
    /// </summary>
    public static UpdateAdminExpensesCategoryRequest ForUniversalAndUsCompany(
        string description,
        string nominalCode,
        UniversalAndUsAdminExpensesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate autoSalesTaxRate) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    private static UpdateAdminExpensesCategoryRequest Create(
        string description,
        string nominalCode,
        string taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate autoSalesTaxRate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
        CategoryNominalCodeValidator.ValidateAdminExpenses(nominalCode);

        return new UpdateAdminExpensesCategoryRequest
        {
            Description = description,
            NominalCode = nominalCode,
            TaxReportingName = taxReportingName,
            AllowableForTax = allowableForTax,
            AutoSalesTaxRate = autoSalesTaxRate
        };
    }
}

/// <summary>
/// Request attributes for updating a current asset category (nominal codes 671–720).
/// </summary>
public sealed class UpdateCurrentAssetCategoryRequest
{
    internal string Description { get; private init; } = string.Empty;

    internal string NominalCode { get; private init; } = string.Empty;

    internal string TaxReportingName { get; private init; } = string.Empty;

    /// <summary>
    /// Creates an update request for a current asset category.
    /// </summary>
    public static UpdateCurrentAssetCategoryRequest Create(
        string description,
        string nominalCode,
        CurrentAssetTaxReportingName taxReportingName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
        CategoryNominalCodeValidator.ValidateCurrentAsset(nominalCode);

        return new UpdateCurrentAssetCategoryRequest
        {
            Description = description,
            NominalCode = nominalCode,
            TaxReportingName = EnumWireValue.Get(taxReportingName)
        };
    }
}

/// <summary>
/// Request attributes for updating a liabilities category (nominal codes 731–780).
/// </summary>
public sealed class UpdateLiabilitiesCategoryRequest
{
    internal string Description { get; private init; } = string.Empty;

    internal string NominalCode { get; private init; } = string.Empty;

    internal string TaxReportingName { get; private init; } = string.Empty;

    /// <summary>
    /// Creates an update request for a UK limited company liabilities category.
    /// </summary>
    public static UpdateLiabilitiesCategoryRequest ForUkLimitedCompany(
        string description,
        string nominalCode,
        UkLimitedCompanyLiabilitiesTaxReportingName taxReportingName) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName));

    /// <summary>
    /// Creates an update request for a liabilities category on all other documented company types.
    /// </summary>
    public static UpdateLiabilitiesCategoryRequest ForOtherCompanyTypes(
        string description,
        string nominalCode,
        OtherCompanyLiabilitiesTaxReportingName taxReportingName) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName));

    private static UpdateLiabilitiesCategoryRequest Create(
        string description,
        string nominalCode,
        string taxReportingName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
        CategoryNominalCodeValidator.ValidateLiabilities(nominalCode);

        return new UpdateLiabilitiesCategoryRequest
        {
            Description = description,
            NominalCode = nominalCode,
            TaxReportingName = taxReportingName
        };
    }
}

/// <summary>
/// Request attributes for updating an equity category (nominal codes 921–960).
/// </summary>
public sealed class UpdateEquityCategoryRequest
{
    internal string Description { get; private init; } = string.Empty;

    internal string NominalCode { get; private init; } = string.Empty;

    /// <summary>
    /// Creates an update request for an equity category.
    /// </summary>
    public static UpdateEquityCategoryRequest Create(string description, string nominalCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
        CategoryNominalCodeValidator.ValidateEquity(nominalCode);

        return new UpdateEquityCategoryRequest
        {
            Description = description,
            NominalCode = nominalCode
        };
    }
}
