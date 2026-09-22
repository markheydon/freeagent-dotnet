using System.Diagnostics.CodeAnalysis;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Company endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Company")]
internal sealed class CompanySamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "Get company profile")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetCompanyProfileAsync(CancellationToken cancellationToken)
    {
        var company = await context.Client.Company.GetCompanyAsync(cancellationToken);

        SampleOutput.WriteHeader("Company profile");
        SampleOutput.WriteField("Name", company.Name);
        SampleOutput.WriteField("Type", company.Type);
        SampleOutput.WriteField("Currency", company.Currency);
        SampleOutput.WriteField("Country", company.Country);
    }

    [ConsoleSample(Name = "List business categories")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListBusinessCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = await context.Client.Company.ListBusinessCategoriesAsync(cancellationToken);

        SampleOutput.WriteHeader($"Business categories ({categories.Count})");
        SampleOutput.WriteRows(categories, category => category);
    }

    [ConsoleSample(Name = "List tax timeline")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListTaxTimelineAsync(CancellationToken cancellationToken)
    {
        var timeline = await context.Client.Company.ListTaxTimelineAsync(cancellationToken);

        SampleOutput.WriteHeader($"Tax timeline ({timeline.Count})");
        SampleOutput.WriteRows(
            timeline,
            item => $"{item.DatedOn:yyyy-MM-dd}  {item.Nature,-20}  {item.Description}");
    }
}
