using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.Categories;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Categories endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Categories")]
internal sealed class CategorySamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List category sets")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListCategorySetsAsync(CancellationToken cancellationToken)
    {
        var sets = await context.Client.Categories.ListAsync(cancellationToken: cancellationToken);

        SampleOutput.WriteHeader("Category sets");
        SampleOutput.WriteField("Income", sets.IncomeCategories.Count);
        SampleOutput.WriteField("Cost of sales", sets.CostOfSalesCategories.Count);
        SampleOutput.WriteField("Admin expenses", sets.AdminExpensesCategories.Count);
        SampleOutput.WriteField("General", sets.GeneralCategories.Count);
        SampleOutput.WriteField("Total", sets.AllCategories.Count);

        SampleOutput.WriteRows(
            sets.AllCategories,
            category => $"{category.NominalCode,-8}  {category.Description}");
    }

    [ConsoleSample(Name = "Get category by nominal code")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetCategoryByNominalCodeAsync(CancellationToken cancellationToken)
    {
        var sets = await context.Client.Categories.ListAsync(cancellationToken: cancellationToken);
        if (sets.AllCategories.Count == 0)
        {
            SampleContext.Skip("no categories found in sandbox account");
        }

        var nominalCode = sets.AllCategories[0].NominalCode;
        if (string.IsNullOrWhiteSpace(nominalCode))
        {
            SampleContext.Skip("first category has no nominal code");
        }

        var category = await context.Client.Categories.GetCategoryAsync(nominalCode, cancellationToken);

        SampleOutput.WriteHeader("Category detail");
        SampleOutput.WriteField("Nominal code", category.NominalCode);
        SampleOutput.WriteField("Description", category.Description);
        SampleOutput.WriteField("Group", category.GroupDescription);
    }

    [ConsoleSample(Name = "Create income category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateIncomeCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunCreateAndDeleteAsync(
            "048",
            ct => context.Client.Categories.CreateIncomeCategoryAsync(
                CreateIncomeCategoryRequest.Create("Console probe income category", "048"),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Update income category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateIncomeCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunUpdateAndDeleteAsync(
            "048",
            ct => context.Client.Categories.CreateIncomeCategoryAsync(
                CreateIncomeCategoryRequest.Create("Console probe income category", "048"),
                ct),
            ct => context.Client.Categories.UpdateIncomeCategoryAsync(
                "048",
                UpdateIncomeCategoryRequest.Create("Console probe income category (updated)", "048"),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Create cost of sales category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateCostOfSalesCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunCreateAndDeleteAsync(
            "101",
            ct => context.Client.Categories.CreateCostOfSalesCategoryAsync(
                CreateCostOfSalesCategoryRequest.ForUkLimitedCompany(
                    "Console probe cost of sales category",
                    "101",
                    UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Update cost of sales category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateCostOfSalesCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunUpdateAndDeleteAsync(
            "101",
            ct => context.Client.Categories.CreateCostOfSalesCategoryAsync(
                CreateCostOfSalesCategoryRequest.ForUkLimitedCompany(
                    "Console probe cost of sales category",
                    "101",
                    UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                ct),
            ct => context.Client.Categories.UpdateCostOfSalesCategoryAsync(
                "101",
                UpdateCostOfSalesCategoryRequest.ForUkLimitedCompany(
                    "Console probe cost of sales category (updated)",
                    "101",
                    UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Create admin expenses category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateAdminExpensesCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunCreateAndDeleteAsync(
            "212",
            ct => context.Client.Categories.CreateAdminExpensesCategoryAsync(
                CreateAdminExpensesCategoryRequest.ForUkLimitedCompany(
                    "Console probe admin expenses category",
                    "212",
                    UkLimitedCompanyAdminExpensesTaxReportingName.ComputerSoftwareCosts,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Update admin expenses category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateAdminExpensesCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunUpdateAndDeleteAsync(
            "212",
            ct => context.Client.Categories.CreateAdminExpensesCategoryAsync(
                CreateAdminExpensesCategoryRequest.ForUkLimitedCompany(
                    "Console probe admin expenses category",
                    "212",
                    UkLimitedCompanyAdminExpensesTaxReportingName.ComputerSoftwareCosts,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                ct),
            ct => context.Client.Categories.UpdateAdminExpensesCategoryAsync(
                "212",
                UpdateAdminExpensesCategoryRequest.ForUkLimitedCompany(
                    "Console probe admin expenses category (updated)",
                    "212",
                    UkLimitedCompanyAdminExpensesTaxReportingName.ComputerSoftwareCosts,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Create current asset category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateCurrentAssetCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunCreateAndDeleteAsync(
            "672",
            ct => context.Client.Categories.CreateCurrentAssetCategoryAsync(
                CreateCurrentAssetCategoryRequest.Create(
                    "Console probe current asset category",
                    "672",
                    CurrentAssetTaxReportingName.Debtors),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Update current asset category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateCurrentAssetCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunUpdateAndDeleteAsync(
            "672",
            ct => context.Client.Categories.CreateCurrentAssetCategoryAsync(
                CreateCurrentAssetCategoryRequest.Create(
                    "Console probe current asset category",
                    "672",
                    CurrentAssetTaxReportingName.Debtors),
                ct),
            ct => context.Client.Categories.UpdateCurrentAssetCategoryAsync(
                "672",
                UpdateCurrentAssetCategoryRequest.Create(
                    "Console probe current asset category (updated)",
                    "672",
                    CurrentAssetTaxReportingName.Debtors),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Create liabilities category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateLiabilitiesCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunCreateAndDeleteAsync(
            "732",
            ct => context.Client.Categories.CreateLiabilitiesCategoryAsync(
                CreateLiabilitiesCategoryRequest.ForOtherCompanyTypes(
                    "Console probe liabilities category",
                    "732",
                    OtherCompanyLiabilitiesTaxReportingName.Creditors),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Update liabilities category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateLiabilitiesCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunUpdateAndDeleteAsync(
            "732",
            ct => context.Client.Categories.CreateLiabilitiesCategoryAsync(
                CreateLiabilitiesCategoryRequest.ForOtherCompanyTypes(
                    "Console probe liabilities category",
                    "732",
                    OtherCompanyLiabilitiesTaxReportingName.Creditors),
                ct),
            ct => context.Client.Categories.UpdateLiabilitiesCategoryAsync(
                "732",
                UpdateLiabilitiesCategoryRequest.ForUkLimitedCompany(
                    "Console probe liabilities category (updated)",
                    "732",
                    UkLimitedCompanyLiabilitiesTaxReportingName.Creditors),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Create equity category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateEquityCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunCreateAndDeleteAsync(
            "922",
            ct => context.Client.Categories.CreateEquityCategoryAsync(
                CreateEquityCategoryRequest.Create("Console probe equity category", "922"),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Update equity category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateEquityCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunUpdateAndDeleteAsync(
            "922",
            ct => context.Client.Categories.CreateEquityCategoryAsync(
                CreateEquityCategoryRequest.Create("Console probe equity category", "922"),
                ct),
            ct => context.Client.Categories.UpdateEquityCategoryAsync(
                "922",
                UpdateEquityCategoryRequest.Create("Console probe equity category (updated)", "922"),
                ct),
            cancellationToken);

    private async Task RunCreateAndDeleteAsync(
        string nominalCode,
        Func<CancellationToken, Task<Category>> create,
        CancellationToken cancellationToken)
    {
        var created = await create(cancellationToken);

        SampleOutput.WriteHeader("Created category");
        SampleOutput.WriteField("Nominal code", created.NominalCode);
        SampleOutput.WriteField("Description", created.Description);

        await context.Client.Categories.DeleteCategoryAsync(nominalCode, cancellationToken);

        SampleOutput.WriteHeader("Deleted category");
        SampleOutput.WriteField("Nominal code", nominalCode);
    }

    private async Task RunUpdateAndDeleteAsync(
        string nominalCode,
        Func<CancellationToken, Task<Category>> create,
        Func<CancellationToken, Task<Category>> update,
        CancellationToken cancellationToken)
    {
        await create(cancellationToken);
        var updated = await update(cancellationToken);

        SampleOutput.WriteHeader("Updated category");
        SampleOutput.WriteField("Nominal code", updated.NominalCode);
        SampleOutput.WriteField("Description", updated.Description);

        await context.Client.Categories.DeleteCategoryAsync(nominalCode, cancellationToken);

        SampleOutput.WriteHeader("Deleted category");
        SampleOutput.WriteField("Nominal code", nominalCode);
    }
}
