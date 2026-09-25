using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.PriceListItems;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Price list items endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Price list items")]
internal sealed class PriceListItemSamples(SampleContext context) : IConsoleSampleProvider
{
    private const string ProbeCode = "SDK-CONSOLE-PLI";

    [ConsoleSample(Name = "List price list items")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListPriceListItemsAsync(CancellationToken cancellationToken)
    {
        var items = await context.Client.PriceListItems.ListAsync(cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Price list items ({items.Count})");
        SampleOutput.WriteRows(items, item => $"{item.ResourceId,8}  {item.Code}  {item.ItemType}  {item.Price}");
    }

    [ConsoleSample(Name = "Get price list item detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetPriceListItemDetailAsync(CancellationToken cancellationToken)
    {
        var item = await context.Data.GetFirstPriceListItemAsync(cancellationToken);
        var detail = await context.Client.PriceListItems.GetPriceListItemAsync(item.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Price list item detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Code", detail.Code);
        SampleOutput.WriteField("Item type", detail.ItemType);
        SampleOutput.WriteField("Price", detail.Price);
        SampleOutput.WriteField("VAT status", detail.VatStatus);
    }

    [ConsoleSample(Name = "Create probe price list item and delete")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbePriceListItemAsync(CancellationToken cancellationToken)
    {
        var created = await context.Client.PriceListItems.CreatePriceListItemAsync(new CreatePriceListItemRequest
        {
            Code = ProbeCode,
            Description = "Console sample probe item",
            Quantity = 1,
            Price = 4.99m,
            ItemType = InvoiceItemType.Products,
            VatStatus = PriceListItemVatStatus.Standard
        }, cancellationToken);

        SampleOutput.WriteHeader("Created price list item");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Code", created.Code);

        await context.Client.PriceListItems.DeletePriceListItemAsync(created.ResourceId, cancellationToken);
        Console.WriteLine();
        Console.WriteLine($"  Deleted probe price list item {created.ResourceId}.");
    }
}
