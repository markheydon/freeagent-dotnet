using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client;
using FreeAgent.Client.Models.Bills;
using FreeAgent.Client.Models.HirePurchases;
using FreeAgent.Client.Models.Shared;

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

    [ConsoleSample(Name = "Get hire purchase detail with probe create and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetHirePurchaseDetailAsync(CancellationToken cancellationToken)
    {
        var hirePurchases = await context.Client.HirePurchases.ListAsync(cancellationToken: cancellationToken);
        long? probeBillId = null;

        if (hirePurchases.Count == 0)
        {
            var bill = await CreateHirePurchaseProbeBillAsync(cancellationToken);
            probeBillId = bill.ResourceId;
            hirePurchases = await context.Client.HirePurchases.ListAsync(cancellationToken: cancellationToken);
        }

        HirePurchase? hirePurchase = null;
        for (var i = 0; i < hirePurchases.Count; i++)
        {
            var candidate = hirePurchases[i];
            if (probeBillId is null || candidate.BillId == probeBillId)
            {
                hirePurchase = candidate;
                break;
            }
        }

        if (hirePurchase is null)
        {
            if (probeBillId is { } billId)
            {
                await TryDeleteProbeBillAsync(billId, cancellationToken);
            }

            SampleContext.Skip("no hire purchases found in sandbox account (UK hire purchase bills only)");
        }

        try
        {
            var detail = await context.Client.HirePurchases.GetHirePurchaseAsync(
                hirePurchase.ResourceId,
                new HirePurchaseGetOptions
                {
                    IncludeBill = true,
                    IncludeLiabilitiesOverOneYearCategory = true,
                    IncludeLiabilitiesUnderOneYearCategory = true
                },
                cancellationToken);

            SampleOutput.WriteHeader("Hire purchase detail");
            SampleOutput.WriteField("Id", detail.ResourceId);
            SampleOutput.WriteField("Description", detail.Description);
            SampleOutput.WriteField("Bill id", detail.BillId);
            SampleOutput.WriteField("Liabilities over one year", detail.LiabilitiesOverOneYearCategoryNominalCode);
            SampleOutput.WriteField("Liabilities under one year", detail.LiabilitiesUnderOneYearCategoryNominalCode);
        }
        finally
        {
            if (probeBillId is { } billId)
            {
                await TryDeleteProbeBillAsync(billId, cancellationToken);
            }
        }
    }

    private async Task<Bill> CreateHirePurchaseProbeBillAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var nominalCode = await context.Data.GetFirstAdminExpensesCategoryNominalCodeAsync(cancellationToken);

        return await context.Client.Bills.CreateBillAsync(
            new Bill
            {
                ContactId = contact.ResourceId,
                DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                DueOn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
                IsPaidByHirePurchase = true,
                Reference = $"Console HP probe {DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}",
                BillItems =
                [
                    new BillItem
                    {
                        Description = "Console hire purchase probe charge",
                        TotalValue = 50m,
                        SalesTaxRate = PurchaseSalesTaxRate.FromPercentage(20m),
                        SalesTaxStatus = BillSalesTaxStatus.Taxable,
                        CategoryNominalCode = nominalCode
                    }
                ]
            },
            cancellationToken);
    }

    private async Task TryDeleteProbeBillAsync(long billId, CancellationToken cancellationToken)
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
}
