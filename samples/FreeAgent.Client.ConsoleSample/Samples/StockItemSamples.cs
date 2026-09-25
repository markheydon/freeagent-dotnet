using System.Diagnostics.CodeAnalysis;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Stock items endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Stock items")]
internal sealed class StockItemSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List stock items")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListStockItemsAsync(CancellationToken cancellationToken)
    {
        var stockItems = await context.Client.StockItems.ListAsync(cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Stock items ({stockItems.Count})");
        SampleOutput.WriteRows(stockItems, item => $"{item.ResourceId,8}  {item.Description}  (on hand: {item.StockOnHand})");
    }

    [ConsoleSample(Name = "Get stock item detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetStockItemDetailAsync(CancellationToken cancellationToken)
    {
        var stockItem = await context.Data.GetFirstStockItemAsync(cancellationToken);
        var detail = await context.Client.StockItems.GetStockItemAsync(stockItem.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Stock item detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Description", detail.Description);
        SampleOutput.WriteField("Stock on hand", detail.StockOnHand);
        SampleOutput.WriteField("Cost of sale category", detail.CostOfSaleCategoryNominalCode);
    }
}
