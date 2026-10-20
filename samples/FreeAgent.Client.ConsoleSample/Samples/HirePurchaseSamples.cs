using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.HirePurchases;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Hire purchases endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Hire purchases")]
internal sealed class HirePurchaseSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List hire purchases")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListHirePurchasesAsync(CancellationToken cancellationToken)
    {
        var hirePurchases = await context.Client.HirePurchases.ListAsync(cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Hire purchases ({hirePurchases.Count})");
        SampleOutput.WriteRows(
            hirePurchases,
            item => $"{item.ResourceId,8}  {item.Description}  (bill: {item.BillId})");
    }

    [ConsoleSample(Name = "Get hire purchase detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetHirePurchaseDetailAsync(CancellationToken cancellationToken)
    {
        var hirePurchase = await context.Data.GetFirstHirePurchaseAsync(cancellationToken);
        var detail = await context.Client.HirePurchases.GetHirePurchaseAsync(hirePurchase.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Hire purchase detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Description", detail.Description);
        SampleOutput.WriteField("Bill id", detail.BillId);
        SampleOutput.WriteField("Liabilities over one year", detail.LiabilitiesOverOneYearCategoryNominalCode);
        SampleOutput.WriteField("Liabilities under one year", detail.LiabilitiesUnderOneYearCategoryNominalCode);
    }
}
