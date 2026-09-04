using FreeAgent.Client.Models.Categories;

namespace FreeAgent.Client.BlazorSample.Services;

/// <summary>
/// Documented category write operations exposed by the SDK for sample probing.
/// </summary>
public enum CategoryWriteProbeVariant
{
    CreateIncome,
    CreateCostOfSales,
    CreateAdminExpenses,
    CreateCurrentAsset,
    CreateLiabilities,
    CreateEquity,
    UpdateIncome,
    UpdateCostOfSales,
    UpdateAdminExpenses,
    UpdateCurrentAsset,
    UpdateLiabilities,
    UpdateEquity
}

/// <summary>
/// Executes category write probes against the live API using typed SDK requests.
/// </summary>
public static class CategoryWriteProbeSupport
{
    public static async Task<Category> ExecuteAsync(
        FreeAgentClient client,
        CategoryWriteProbeVariant variant,
        string nominalCode,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);

        return variant switch
        {
            CategoryWriteProbeVariant.CreateIncome => await client.Categories.CreateIncomeCategoryAsync(
                CreateIncomeCategoryRequest.Create("SDK probe income category", nominalCode),
                cancellationToken),
            CategoryWriteProbeVariant.CreateCostOfSales => await client.Categories.CreateCostOfSalesCategoryAsync(
                CreateCostOfSalesCategoryRequest.ForUkLimitedCompany(
                    "SDK probe cost of sales category",
                    nominalCode,
                    UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                cancellationToken),
            CategoryWriteProbeVariant.CreateAdminExpenses => await client.Categories.CreateAdminExpensesCategoryAsync(
                CreateAdminExpensesCategoryRequest.ForUkLimitedCompany(
                    "SDK probe admin expenses category",
                    nominalCode,
                    UkLimitedCompanyAdminExpensesTaxReportingName.ComputerSoftwareCosts,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                cancellationToken),
            CategoryWriteProbeVariant.CreateCurrentAsset => await client.Categories.CreateCurrentAssetCategoryAsync(
                CreateCurrentAssetCategoryRequest.Create(
                    "SDK probe current asset category",
                    nominalCode,
                    CurrentAssetTaxReportingName.Debtors),
                cancellationToken),
            CategoryWriteProbeVariant.CreateLiabilities => await client.Categories.CreateLiabilitiesCategoryAsync(
                CreateLiabilitiesCategoryRequest.ForOtherCompanyTypes(
                    "SDK probe liabilities category",
                    nominalCode,
                    OtherCompanyLiabilitiesTaxReportingName.Creditors),
                cancellationToken),
            CategoryWriteProbeVariant.CreateEquity => await client.Categories.CreateEquityCategoryAsync(
                CreateEquityCategoryRequest.Create("SDK probe equity category", nominalCode),
                cancellationToken),
            CategoryWriteProbeVariant.UpdateIncome => await client.Categories.UpdateIncomeCategoryAsync(
                nominalCode,
                UpdateIncomeCategoryRequest.Create("SDK probe income category (updated)", nominalCode),
                cancellationToken),
            CategoryWriteProbeVariant.UpdateCostOfSales => await client.Categories.UpdateCostOfSalesCategoryAsync(
                nominalCode,
                UpdateCostOfSalesCategoryRequest.ForUkLimitedCompany(
                    "SDK probe cost of sales category (updated)",
                    nominalCode,
                    UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                cancellationToken),
            CategoryWriteProbeVariant.UpdateAdminExpenses => await client.Categories.UpdateAdminExpensesCategoryAsync(
                nominalCode,
                UpdateAdminExpensesCategoryRequest.ForUkLimitedCompany(
                    "SDK probe admin expenses category (updated)",
                    nominalCode,
                    UkLimitedCompanyAdminExpensesTaxReportingName.ComputerSoftwareCosts,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                cancellationToken),
            CategoryWriteProbeVariant.UpdateCurrentAsset => await client.Categories.UpdateCurrentAssetCategoryAsync(
                nominalCode,
                UpdateCurrentAssetCategoryRequest.Create(
                    "SDK probe current asset category (updated)",
                    nominalCode,
                    CurrentAssetTaxReportingName.Debtors),
                cancellationToken),
            CategoryWriteProbeVariant.UpdateLiabilities => await client.Categories.UpdateLiabilitiesCategoryAsync(
                nominalCode,
                UpdateLiabilitiesCategoryRequest.ForUkLimitedCompany(
                    "SDK probe liabilities category (updated)",
                    nominalCode,
                    UkLimitedCompanyLiabilitiesTaxReportingName.Creditors),
                cancellationToken),
            CategoryWriteProbeVariant.UpdateEquity => await client.Categories.UpdateEquityCategoryAsync(
                nominalCode,
                UpdateEquityCategoryRequest.Create("SDK probe equity category (updated)", nominalCode),
                cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(variant), variant, "Unsupported category write probe variant.")
        };
    }

    public static string GetDefaultNominalCode(CategoryWriteProbeVariant variant) => variant switch
    {
        CategoryWriteProbeVariant.CreateIncome or CategoryWriteProbeVariant.UpdateIncome => "048",
        CategoryWriteProbeVariant.CreateCostOfSales or CategoryWriteProbeVariant.UpdateCostOfSales => "101",
        CategoryWriteProbeVariant.CreateAdminExpenses or CategoryWriteProbeVariant.UpdateAdminExpenses => "212",
        CategoryWriteProbeVariant.CreateCurrentAsset or CategoryWriteProbeVariant.UpdateCurrentAsset => "672",
        CategoryWriteProbeVariant.CreateLiabilities or CategoryWriteProbeVariant.UpdateLiabilities => "732",
        CategoryWriteProbeVariant.CreateEquity or CategoryWriteProbeVariant.UpdateEquity => "922",
        _ => "001"
    };

    public static string GetSdkMethodName(CategoryWriteProbeVariant variant) => variant switch
    {
        CategoryWriteProbeVariant.CreateIncome => "CreateIncomeCategoryAsync",
        CategoryWriteProbeVariant.CreateCostOfSales => "CreateCostOfSalesCategoryAsync",
        CategoryWriteProbeVariant.CreateAdminExpenses => "CreateAdminExpensesCategoryAsync",
        CategoryWriteProbeVariant.CreateCurrentAsset => "CreateCurrentAssetCategoryAsync",
        CategoryWriteProbeVariant.CreateLiabilities => "CreateLiabilitiesCategoryAsync",
        CategoryWriteProbeVariant.CreateEquity => "CreateEquityCategoryAsync",
        CategoryWriteProbeVariant.UpdateIncome => "UpdateIncomeCategoryAsync",
        CategoryWriteProbeVariant.UpdateCostOfSales => "UpdateCostOfSalesCategoryAsync",
        CategoryWriteProbeVariant.UpdateAdminExpenses => "UpdateAdminExpensesCategoryAsync",
        CategoryWriteProbeVariant.UpdateCurrentAsset => "UpdateCurrentAssetCategoryAsync",
        CategoryWriteProbeVariant.UpdateLiabilities => "UpdateLiabilitiesCategoryAsync",
        CategoryWriteProbeVariant.UpdateEquity => "UpdateEquityCategoryAsync",
        _ => "CategoryService"
    };
}
