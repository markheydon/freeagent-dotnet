using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>
/// Request attributes for creating an income category (nominal codes 001–049). Uniqueness within the account is validated by FreeAgent.
/// </summary>
public sealed class CreateIncomeCategoryRequest
{
    internal string Description { get; private init; } = string.Empty;

    internal string NominalCode { get; private init; } = string.Empty;

    /// <summary>
    /// Creates a request for a new income category.
    /// </summary>
    /// <param name="description">Category name.</param>
    /// <param name="nominalCode">Unique nominal code from 001 to 049. Uniqueness within the account is validated by FreeAgent.</param>
    /// <returns>A create-income-category request.</returns>
    public static CreateIncomeCategoryRequest Create(string description, string nominalCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
        CategoryNominalCodeValidator.ValidateIncome(nominalCode);

        return new CreateIncomeCategoryRequest
        {
            Description = description,
            NominalCode = nominalCode
        };
    }
}

/// <summary>
/// Request attributes for creating a cost of sales category (nominal codes 096–199). Uniqueness within the account is validated by FreeAgent.
/// </summary>
public sealed class CreateCostOfSalesCategoryRequest
{
    internal string Description { get; private init; } = string.Empty;

    internal string NominalCode { get; private init; } = string.Empty;

    internal string TaxReportingName { get; private init; } = string.Empty;

    internal bool AllowableForTax { get; private init; }

    internal CategoryAutoSalesTaxRate? AutoSalesTaxRate { get; private init; }

    /// <summary>
    /// Creates a request for a UK limited company cost of sales category.
    /// </summary>
    public static CreateCostOfSalesCategoryRequest ForUkLimitedCompany(
        string description,
        string nominalCode,
        UkLimitedCompanyCostOfSalesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate? autoSalesTaxRate = null) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    /// <summary>
    /// Creates a request for a UK sole trader cost of sales category.
    /// </summary>
    public static CreateCostOfSalesCategoryRequest ForUkSoleTrader(
        string description,
        string nominalCode,
        UkSoleTraderCostOfSalesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate? autoSalesTaxRate = null) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    /// <summary>
    /// Creates a request for a UK partnership cost of sales category.
    /// </summary>
    public static CreateCostOfSalesCategoryRequest ForUkPartnership(
        string description,
        string nominalCode,
        UkPartnershipCostOfSalesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate? autoSalesTaxRate = null) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    /// <summary>
    /// Creates a request for a universal or US company cost of sales category.
    /// </summary>
    public static CreateCostOfSalesCategoryRequest ForUniversalAndUsCompany(
        string description,
        string nominalCode,
        UniversalAndUsCostOfSalesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate? autoSalesTaxRate = null) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    private static CreateCostOfSalesCategoryRequest Create(
        string description,
        string nominalCode,
        string taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate? autoSalesTaxRate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
        CategoryNominalCodeValidator.ValidateCostOfSales(nominalCode);

        return new CreateCostOfSalesCategoryRequest
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
/// Request attributes for creating an admin expenses category (nominal codes 200–399). Uniqueness within the account is validated by FreeAgent.
/// </summary>
public sealed class CreateAdminExpensesCategoryRequest
{
    internal string Description { get; private init; } = string.Empty;

    internal string NominalCode { get; private init; } = string.Empty;

    internal string TaxReportingName { get; private init; } = string.Empty;

    internal bool AllowableForTax { get; private init; }

    internal CategoryAutoSalesTaxRate? AutoSalesTaxRate { get; private init; }

    /// <summary>
    /// Creates a request for a UK limited company admin expenses category.
    /// </summary>
    public static CreateAdminExpensesCategoryRequest ForUkLimitedCompany(
        string description,
        string nominalCode,
        UkLimitedCompanyAdminExpensesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate? autoSalesTaxRate = null) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    /// <summary>
    /// Creates a request for a UK sole trader admin expenses category.
    /// </summary>
    public static CreateAdminExpensesCategoryRequest ForUkSoleTrader(
        string description,
        string nominalCode,
        UkSoleTraderAdminExpensesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate? autoSalesTaxRate = null) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    /// <summary>
    /// Creates a request for a UK partnership admin expenses category.
    /// </summary>
    public static CreateAdminExpensesCategoryRequest ForUkPartnership(
        string description,
        string nominalCode,
        UkPartnershipAdminExpensesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate? autoSalesTaxRate = null) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    /// <summary>
    /// Creates a request for a universal or US company admin expenses category.
    /// </summary>
    public static CreateAdminExpensesCategoryRequest ForUniversalAndUsCompany(
        string description,
        string nominalCode,
        UniversalAndUsAdminExpensesTaxReportingName taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate? autoSalesTaxRate = null) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName), allowableForTax, autoSalesTaxRate);

    private static CreateAdminExpensesCategoryRequest Create(
        string description,
        string nominalCode,
        string taxReportingName,
        bool allowableForTax,
        CategoryAutoSalesTaxRate? autoSalesTaxRate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
        CategoryNominalCodeValidator.ValidateAdminExpenses(nominalCode);

        return new CreateAdminExpensesCategoryRequest
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
/// Request attributes for creating a current asset category (nominal codes 671–720). Uniqueness within the account is validated by FreeAgent.
/// </summary>
public sealed class CreateCurrentAssetCategoryRequest
{
    internal string Description { get; private init; } = string.Empty;

    internal string NominalCode { get; private init; } = string.Empty;

    internal string TaxReportingName { get; private init; } = string.Empty;

    /// <summary>
    /// Creates a request for a current asset category.
    /// </summary>
    public static CreateCurrentAssetCategoryRequest Create(
        string description,
        string nominalCode,
        CurrentAssetTaxReportingName taxReportingName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
        CategoryNominalCodeValidator.ValidateCurrentAsset(nominalCode);

        return new CreateCurrentAssetCategoryRequest
        {
            Description = description,
            NominalCode = nominalCode,
            TaxReportingName = EnumWireValue.Get(taxReportingName)
        };
    }
}

/// <summary>
/// Request attributes for creating a liabilities category (nominal codes 731–780). Uniqueness within the account is validated by FreeAgent.
/// </summary>
public sealed class CreateLiabilitiesCategoryRequest
{
    internal string Description { get; private init; } = string.Empty;

    internal string NominalCode { get; private init; } = string.Empty;

    internal string TaxReportingName { get; private init; } = string.Empty;

    /// <summary>
    /// Creates a request for a UK limited company liabilities category.
    /// </summary>
    public static CreateLiabilitiesCategoryRequest ForUkLimitedCompany(
        string description,
        string nominalCode,
        UkLimitedCompanyLiabilitiesTaxReportingName taxReportingName) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName));

    /// <summary>
    /// Creates a request for a liabilities category on all other documented company types.
    /// </summary>
    public static CreateLiabilitiesCategoryRequest ForOtherCompanyTypes(
        string description,
        string nominalCode,
        OtherCompanyLiabilitiesTaxReportingName taxReportingName) =>
        Create(description, nominalCode, EnumWireValue.Get(taxReportingName));

    private static CreateLiabilitiesCategoryRequest Create(
        string description,
        string nominalCode,
        string taxReportingName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
        CategoryNominalCodeValidator.ValidateLiabilities(nominalCode);

        return new CreateLiabilitiesCategoryRequest
        {
            Description = description,
            NominalCode = nominalCode,
            TaxReportingName = taxReportingName
        };
    }
}

/// <summary>
/// Request attributes for creating an equity category (nominal codes 921–960). Uniqueness within the account is validated by FreeAgent.
/// </summary>
public sealed class CreateEquityCategoryRequest
{
    internal string Description { get; private init; } = string.Empty;

    internal string NominalCode { get; private init; } = string.Empty;

    /// <summary>
    /// Creates a request for a new equity category.
    /// </summary>
    public static CreateEquityCategoryRequest Create(string description, string nominalCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
        CategoryNominalCodeValidator.ValidateEquity(nominalCode);

        return new CreateEquityCategoryRequest
        {
            Description = description,
            NominalCode = nominalCode
        };
    }
}
