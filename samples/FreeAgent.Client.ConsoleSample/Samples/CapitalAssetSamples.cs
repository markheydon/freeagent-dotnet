using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Capital assets endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Capital assets")]
internal sealed class CapitalAssetSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List capital assets")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListCapitalAssetsAsync(CancellationToken cancellationToken)
    {
        var items = await context.Client.CapitalAssets.ListAsync(cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Capital assets ({items.Count})");
        SampleOutput.WriteRows(
            items,
            item => $"{item.ResourceId,8}  {item.Description}  ({item.AssetType})");
    }

    [ConsoleSample(Name = "Get capital asset detail with history")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetCapitalAssetDetailAsync(CancellationToken cancellationToken)
    {
        var items = await context.Client.CapitalAssets.ListAsync(cancellationToken: cancellationToken);

        if (items.Count == 0)
        {
            SampleContext.Skip("no capital assets found in sandbox account (create via expenses or bank explanations)");
        }

        var asset = items[0];
        var detail = await context.Client.CapitalAssets.GetCapitalAssetAsync(
            asset.ResourceId,
            new CapitalAssetGetOptions { IncludeHistory = true },
            cancellationToken);

        SampleOutput.WriteHeader("Capital asset detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Description", detail.Description);
        SampleOutput.WriteField("Asset type", detail.AssetType);
        SampleOutput.WriteField("Purchased on", detail.PurchasedOn);
        SampleOutput.WriteField("Disposed on", detail.DisposedOn);
        SampleOutput.WriteField("History events", detail.CapitalAssetHistory?.Count ?? 0);
    }
}
