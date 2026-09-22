using System.Diagnostics.CodeAnalysis;

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
}
