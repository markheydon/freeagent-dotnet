using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
            IncomeNominalCodeRangeMin,
            IncomeNominalCodeRangeMax,
            (nominalCode, ct) => context.Client.Categories.CreateIncomeCategoryAsync(
                CreateIncomeCategoryRequest.Create("Console probe income category", nominalCode),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Update income category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateIncomeCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunUpdateAndDeleteAsync(
            IncomeNominalCodeRangeMin,
            IncomeNominalCodeRangeMax,
            (nominalCode, ct) => context.Client.Categories.CreateIncomeCategoryAsync(
                CreateIncomeCategoryRequest.Create("Console probe income category", nominalCode),
                ct),
            (nominalCode, ct) => context.Client.Categories.UpdateIncomeCategoryAsync(
                nominalCode,
                UpdateIncomeCategoryRequest.Create("Console probe income category (updated)", nominalCode),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Create cost of sales category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateCostOfSalesCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunCreateAndDeleteAsync(
            CostOfSalesNominalCodeRangeMin,
            CostOfSalesNominalCodeRangeMax,
            (nominalCode, ct) => context.Client.Categories.CreateCostOfSalesCategoryAsync(
                CreateCostOfSalesCategoryRequest.ForUkLimitedCompany(
                    "Console probe cost of sales category",
                    nominalCode,
                    UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Update cost of sales category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateCostOfSalesCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunUpdateAndDeleteAsync(
            CostOfSalesNominalCodeRangeMin,
            CostOfSalesNominalCodeRangeMax,
            (nominalCode, ct) => context.Client.Categories.CreateCostOfSalesCategoryAsync(
                CreateCostOfSalesCategoryRequest.ForUkLimitedCompany(
                    "Console probe cost of sales category",
                    nominalCode,
                    UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                ct),
            (nominalCode, ct) => context.Client.Categories.UpdateCostOfSalesCategoryAsync(
                nominalCode,
                UpdateCostOfSalesCategoryRequest.ForUkLimitedCompany(
                    "Console probe cost of sales category (updated)",
                    nominalCode,
                    UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Create admin expenses category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateAdminExpensesCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunCreateAndDeleteAsync(
            AdminExpensesNominalCodeRangeMin,
            AdminExpensesNominalCodeRangeMax,
            (nominalCode, ct) => context.Client.Categories.CreateAdminExpensesCategoryAsync(
                CreateAdminExpensesCategoryRequest.ForUkLimitedCompany(
                    "Console probe admin expenses category",
                    nominalCode,
                    UkLimitedCompanyAdminExpensesTaxReportingName.ComputerSoftwareCosts,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Update admin expenses category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateAdminExpensesCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunUpdateAndDeleteAsync(
            AdminExpensesNominalCodeRangeMin,
            AdminExpensesNominalCodeRangeMax,
            (nominalCode, ct) => context.Client.Categories.CreateAdminExpensesCategoryAsync(
                CreateAdminExpensesCategoryRequest.ForUkLimitedCompany(
                    "Console probe admin expenses category",
                    nominalCode,
                    UkLimitedCompanyAdminExpensesTaxReportingName.ComputerSoftwareCosts,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                ct),
            (nominalCode, ct) => context.Client.Categories.UpdateAdminExpensesCategoryAsync(
                nominalCode,
                UpdateAdminExpensesCategoryRequest.ForUkLimitedCompany(
                    "Console probe admin expenses category (updated)",
                    nominalCode,
                    UkLimitedCompanyAdminExpensesTaxReportingName.ComputerSoftwareCosts,
                    allowableForTax: true,
                    autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Create current asset category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateCurrentAssetCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunCreateAndDeleteAsync(
            CurrentAssetNominalCodeRangeMin,
            CurrentAssetNominalCodeRangeMax,
            (nominalCode, ct) => context.Client.Categories.CreateCurrentAssetCategoryAsync(
                CreateCurrentAssetCategoryRequest.Create(
                    "Console probe current asset category",
                    nominalCode,
                    CurrentAssetTaxReportingName.Debtors),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Update current asset category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateCurrentAssetCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunUpdateAndDeleteAsync(
            CurrentAssetNominalCodeRangeMin,
            CurrentAssetNominalCodeRangeMax,
            (nominalCode, ct) => context.Client.Categories.CreateCurrentAssetCategoryAsync(
                CreateCurrentAssetCategoryRequest.Create(
                    "Console probe current asset category",
                    nominalCode,
                    CurrentAssetTaxReportingName.Debtors),
                ct),
            (nominalCode, ct) => context.Client.Categories.UpdateCurrentAssetCategoryAsync(
                nominalCode,
                UpdateCurrentAssetCategoryRequest.Create(
                    "Console probe current asset category (updated)",
                    nominalCode,
                    CurrentAssetTaxReportingName.Debtors),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Create liabilities category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateLiabilitiesCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunCreateAndDeleteAsync(
            LiabilitiesNominalCodeRangeMin,
            LiabilitiesNominalCodeRangeMax,
            (nominalCode, ct) => context.Client.Categories.CreateLiabilitiesCategoryAsync(
                CreateLiabilitiesCategoryRequest.ForUkLimitedCompany(
                    "Console probe liabilities category",
                    nominalCode,
                    UkLimitedCompanyLiabilitiesTaxReportingName.Creditors),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Update liabilities category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateLiabilitiesCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunUpdateAndDeleteAsync(
            LiabilitiesNominalCodeRangeMin,
            LiabilitiesNominalCodeRangeMax,
            (nominalCode, ct) => context.Client.Categories.CreateLiabilitiesCategoryAsync(
                CreateLiabilitiesCategoryRequest.ForUkLimitedCompany(
                    "Console probe liabilities category",
                    nominalCode,
                    UkLimitedCompanyLiabilitiesTaxReportingName.Creditors),
                ct),
            (nominalCode, ct) => context.Client.Categories.UpdateLiabilitiesCategoryAsync(
                nominalCode,
                UpdateLiabilitiesCategoryRequest.ForUkLimitedCompany(
                    "Console probe liabilities category (updated)",
                    nominalCode,
                    UkLimitedCompanyLiabilitiesTaxReportingName.Creditors),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Create equity category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateEquityCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunCreateAndDeleteAsync(
            EquityNominalCodeRangeMin,
            EquityNominalCodeRangeMax,
            (nominalCode, ct) => context.Client.Categories.CreateEquityCategoryAsync(
                CreateEquityCategoryRequest.Create("Console probe equity category", nominalCode),
                ct),
            cancellationToken);

    [ConsoleSample(Name = "Update equity category and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateEquityCategoryAndDeleteAsync(CancellationToken cancellationToken) =>
        await RunUpdateAndDeleteAsync(
            EquityNominalCodeRangeMin,
            EquityNominalCodeRangeMax,
            (nominalCode, ct) => context.Client.Categories.CreateEquityCategoryAsync(
                CreateEquityCategoryRequest.Create("Console probe equity category", nominalCode),
                ct),
            (nominalCode, ct) => context.Client.Categories.UpdateEquityCategoryAsync(
                nominalCode,
                UpdateEquityCategoryRequest.Create("Console probe equity category (updated)", nominalCode),
                ct),
            cancellationToken);

    private const int IncomeNominalCodeRangeMin = 1;
    private const int IncomeNominalCodeRangeMax = 49;
    private const int CostOfSalesNominalCodeRangeMin = 96;
    private const int CostOfSalesNominalCodeRangeMax = 199;
    private const int AdminExpensesNominalCodeRangeMin = 200;
    private const int AdminExpensesNominalCodeRangeMax = 399;
    private const int CurrentAssetNominalCodeRangeMin = 671;
    private const int CurrentAssetNominalCodeRangeMax = 720;
    private const int LiabilitiesNominalCodeRangeMin = 731;
    private const int LiabilitiesNominalCodeRangeMax = 780;
    private const int EquityNominalCodeRangeMin = 921;
    private const int EquityNominalCodeRangeMax = 960;

    private async Task RunCreateAndDeleteAsync(
        int minNominalCode,
        int maxNominalCode,
        Func<string, CancellationToken, Task<Category>> create,
        CancellationToken cancellationToken)
    {
        var nominalCode = AllocateProbeNominalCode(minNominalCode, maxNominalCode);
        var created = await create(nominalCode, cancellationToken);

        try
        {
            SampleOutput.WriteHeader("Created category");
            SampleOutput.WriteField("Nominal code", created.NominalCode);
            SampleOutput.WriteField("Description", created.Description);
        }
        finally
        {
            await context.Client.Categories.DeleteCategoryAsync(nominalCode, cancellationToken);

            SampleOutput.WriteHeader("Deleted category");
            SampleOutput.WriteField("Nominal code", nominalCode);
        }
    }

    private async Task RunUpdateAndDeleteAsync(
        int minNominalCode,
        int maxNominalCode,
        Func<string, CancellationToken, Task<Category>> create,
        Func<string, CancellationToken, Task<Category>> update,
        CancellationToken cancellationToken)
    {
        var nominalCode = AllocateProbeNominalCode(minNominalCode, maxNominalCode);
        await create(nominalCode, cancellationToken);

        try
        {
            var updated = await update(nominalCode, cancellationToken);

            SampleOutput.WriteHeader("Updated category");
            SampleOutput.WriteField("Nominal code", updated.NominalCode);
            SampleOutput.WriteField("Description", updated.Description);
        }
        finally
        {
            await context.Client.Categories.DeleteCategoryAsync(nominalCode, cancellationToken);

            SampleOutput.WriteHeader("Deleted category");
            SampleOutput.WriteField("Nominal code", nominalCode);
        }
    }

    private static string AllocateProbeNominalCode(int minInclusive, int maxInclusive)
    {
        var range = maxInclusive - minInclusive + 1;
        var offset = (int)(Math.Abs(DateTime.UtcNow.Ticks % range));
        return (minInclusive + offset).ToString("D3", CultureInfo.InvariantCulture);
    }
}
