using FreeAgent.Client.Models.Categories;

namespace FreeAgent.Client.Services.Categories;

internal static class CategoryWritePayloadMapper
{
    public static CategoryWritePayload FromCreate(CreateIncomeCategoryRequest request) =>
        new()
        {
            Description = request.Description,
            NominalCode = request.NominalCode,
            CategoryGroup = CategoryGroup.Income
        };

    public static CategoryWritePayload FromCreate(CreateCostOfSalesCategoryRequest request) =>
        new()
        {
            Description = request.Description,
            NominalCode = request.NominalCode,
            TaxReportingName = request.TaxReportingName,
            AllowableForTax = request.AllowableForTax,
            AutoSalesTaxRate = request.AutoSalesTaxRate,
            CategoryGroup = CategoryGroup.CostOfSales
        };

    public static CategoryWritePayload FromCreate(CreateAdminExpensesCategoryRequest request) =>
        new()
        {
            Description = request.Description,
            NominalCode = request.NominalCode,
            TaxReportingName = request.TaxReportingName,
            AllowableForTax = request.AllowableForTax,
            AutoSalesTaxRate = request.AutoSalesTaxRate,
            CategoryGroup = CategoryGroup.AdminExpenses
        };

    public static CategoryWritePayload FromCreate(CreateCurrentAssetCategoryRequest request) =>
        new()
        {
            Description = request.Description,
            NominalCode = request.NominalCode,
            TaxReportingName = request.TaxReportingName,
            CategoryGroup = CategoryGroup.CurrentAssets
        };

    public static CategoryWritePayload FromCreate(CreateLiabilitiesCategoryRequest request) =>
        new()
        {
            Description = request.Description,
            NominalCode = request.NominalCode,
            TaxReportingName = request.TaxReportingName,
            CategoryGroup = CategoryGroup.Liabilities
        };

    public static CategoryWritePayload FromCreate(CreateEquityCategoryRequest request) =>
        new()
        {
            Description = request.Description,
            NominalCode = request.NominalCode,
            CategoryGroup = CategoryGroup.Equities
        };

    public static CategoryWritePayload FromUpdate(UpdateIncomeCategoryRequest request) =>
        new()
        {
            Description = request.Description,
            NominalCode = request.NominalCode
        };

    public static CategoryWritePayload FromUpdate(UpdateCostOfSalesCategoryRequest request) =>
        new()
        {
            Description = request.Description,
            NominalCode = request.NominalCode,
            TaxReportingName = request.TaxReportingName,
            AllowableForTax = request.AllowableForTax,
            AutoSalesTaxRate = request.AutoSalesTaxRate
        };

    public static CategoryWritePayload FromUpdate(UpdateAdminExpensesCategoryRequest request) =>
        new()
        {
            Description = request.Description,
            NominalCode = request.NominalCode,
            TaxReportingName = request.TaxReportingName,
            AllowableForTax = request.AllowableForTax,
            AutoSalesTaxRate = request.AutoSalesTaxRate
        };

    public static CategoryWritePayload FromUpdate(UpdateCurrentAssetCategoryRequest request) =>
        new()
        {
            Description = request.Description,
            NominalCode = request.NominalCode,
            TaxReportingName = request.TaxReportingName
        };

    public static CategoryWritePayload FromUpdate(UpdateLiabilitiesCategoryRequest request) =>
        new()
        {
            Description = request.Description,
            NominalCode = request.NominalCode,
            TaxReportingName = request.TaxReportingName
        };

    public static CategoryWritePayload FromUpdate(UpdateEquityCategoryRequest request) =>
        new()
        {
            Description = request.Description,
            NominalCode = request.NominalCode
        };
}
