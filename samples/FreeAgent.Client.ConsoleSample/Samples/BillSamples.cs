using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client;
using FreeAgent.Client.Models.Bills;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Bills endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Bills")]
internal sealed class BillSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List bills")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListBillsAsync(CancellationToken cancellationToken)
    {
        var page = await context.Client.Bills.ListAsync(perPage: 25, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Bills (showing {page.Items.Count} of {page.Total})");
        SampleOutput.WriteRows(page.Items, FormatBillRow);

        if (page.HasNextPage)
        {
            Console.WriteLine();
            Console.WriteLine("  (First page only. Use ListAutoPagingAsync to stream all pages.)");
        }
    }

    [ConsoleSample(Name = "List bills filtered by contact")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListBillsFilteredByContactAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var page = await context.Client.Bills.ListAsync(
            perPage: 25,
            contactId: contact.ResourceId,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Bills for {contact.DisplayName} ({page.Items.Count})");
        SampleOutput.WriteRows(page.Items, FormatBillRow);
    }

    [ConsoleSample(Name = "Get bill by id")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetBillByIdAsync(CancellationToken cancellationToken)
    {
        var bill = await context.Data.GetFirstBillAsync(cancellationToken);
        var detail = await context.Client.Bills.GetBillAsync(
            bill.ResourceId,
            new BillGetOptions
            {
                IncludeContact = true,
                IncludeProject = true,
                IncludeRebillToProject = true,
                IncludeRebilledOnInvoice = true
            },
            cancellationToken);

        SampleOutput.WriteHeader("Bill detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Reference", detail.Reference);
        SampleOutput.WriteField("Status", detail.Status);
        SampleOutput.WriteField("Contact", detail.Contact?.OrganisationName);
        SampleOutput.WriteField("Dated on", detail.DatedOn);
        SampleOutput.WriteField("Total value", detail.TotalValue);
        SampleOutput.WriteField("Hire purchase", detail.IsPaidByHirePurchase);
    }

    [ConsoleSample(Name = "Create probe bill and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeBillAndDeleteAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleBillAsync(cancellationToken, isPaidByHirePurchase: false);

        SampleOutput.WriteHeader("Created probe bill");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Reference", created.Reference);
        SampleOutput.WriteField("Status", created.Status);
        SampleOutput.WriteField("Total value", created.TotalValue);
        SampleOutput.WriteField("Line items", created.BillItems?.Count ?? 0);
        SampleOutput.WriteField("Hire purchase", created.IsPaidByHirePurchase);

        await context.Client.Bills.DeleteBillAsync(created.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Deleted probe bill");
        SampleOutput.WriteField("Id", created.ResourceId);
    }

    [ConsoleSample(Name = "Create hire purchase probe bill and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateHirePurchaseProbeBillAndDeleteAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleBillAsync(cancellationToken, isPaidByHirePurchase: true);

        try
        {
            SampleOutput.WriteHeader("Created hire purchase probe bill");
            SampleOutput.WriteField("Id", created.ResourceId);
            SampleOutput.WriteField("Reference", created.Reference);
            SampleOutput.WriteField("Hire purchase", created.IsPaidByHirePurchase);

            var hirePurchases = await context.Client.HirePurchases.ListAsync(cancellationToken: cancellationToken);
            var hirePurchase = hirePurchases.FirstOrDefault(h => h.BillId == created.ResourceId);
            if (hirePurchase is not null)
            {
                var detail = await context.Client.HirePurchases.GetHirePurchaseAsync(
                    hirePurchase.ResourceId,
                    cancellationToken: cancellationToken);
                SampleOutput.WriteField("Hire purchase ID", detail.ResourceId);
                SampleOutput.WriteField("Hire purchase description", detail.Description);
            }
        }
        finally
        {
            await DeleteProbeBillAsync(created.ResourceId, cancellationToken);

            SampleOutput.WriteHeader("Deleted hire purchase probe bill");
            SampleOutput.WriteField("Id", created.ResourceId);
        }
    }

    [ConsoleSample(Name = "Update bill line item", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateBillLineItemAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleBillAsync(cancellationToken, isPaidByHirePurchase: false);

        try
        {
            var itemId = created.BillItems?.FirstOrDefault()?.ItemId
                ?? throw new InvalidOperationException("Created bill did not return a bill item identifier.");

            var updated = await context.Client.Bills.UpdateBillAsync(
                created.ResourceId,
                new Bill
                {
                    BillItems =
                    [
                        new BillItem
                        {
                            Url = BuildBillItemUrl(context.Client.Environment, itemId),
                            Description = "Console probe line item (updated)",
                            TotalValue = 99m
                        }
                    ]
                },
                cancellationToken: cancellationToken);

            SampleOutput.WriteHeader("Updated bill line item");
            SampleOutput.WriteField("Bill ID", updated.ResourceId);
            SampleOutput.WriteField("Line item ID", itemId);
            SampleOutput.WriteField("Total value", updated.TotalValue);
        }
        finally
        {
            await DeleteProbeBillAsync(created.ResourceId, cancellationToken);
        }
    }

    private async Task<Bill> CreateSampleBillAsync(
        CancellationToken cancellationToken,
        bool isPaidByHirePurchase = false)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var nominalCode = await context.Data.GetFirstAdminExpensesCategoryNominalCodeAsync(cancellationToken);

        return await context.Client.Bills.CreateBillAsync(
            new Bill
            {
                ContactId = contact.ResourceId,
                DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                DueOn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
                IsPaidByHirePurchase = isPaidByHirePurchase,
                Reference = $"Console probe {DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}",
                BillItems =
                [
                    new BillItem
                    {
                        Description = "Console probe supplier charge",
                        TotalValue = 50m,
                        SalesTaxRate = PurchaseSalesTaxRate.FromPercentage(20m),
                        SalesTaxStatus = BillSalesTaxStatus.Taxable,
                        CategoryNominalCode = nominalCode
                    }
                ]
            },
            cancellationToken);
    }

    private async Task DeleteProbeBillAsync(long billId, CancellationToken cancellationToken)
    {
        try
        {
            await context.Client.Bills.DeleteBillAsync(billId, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Console.WriteLine($"  Warning: could not delete probe bill {billId}: {ex.Message}");
        }
    }

    private static string FormatBillRow(Bill bill) =>
        $"{bill.ResourceId,6}  {bill.Reference ?? "-",-20}  {bill.Status,-12}  {bill.TotalValue,10:0.00}";

    private static string BuildBillItemUrl(FreeAgentEnvironment environment, long billItemId)
    {
        var sampleContactUri = ContactReference.ForEnvironment(environment, 1).Uri;
        var apiRoot = sampleContactUri[..sampleContactUri.IndexOf("contacts/", StringComparison.Ordinal)];
        return $"{apiRoot}bill_items/{billItemId}";
    }
}
