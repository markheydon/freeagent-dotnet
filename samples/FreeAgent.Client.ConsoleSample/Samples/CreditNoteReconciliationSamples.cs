using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FreeAgent.Client;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Credit note reconciliation endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Credit note reconciliations")]
internal sealed class CreditNoteReconciliationSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List credit note reconciliations")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListCreditNoteReconciliationsAsync(CancellationToken cancellationToken)
    {
        var reconciliations = await context.Client.CreditNoteReconciliations.ListAsync(cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Credit note reconciliations ({reconciliations.Count})");
        SampleOutput.WriteRows(reconciliations, FormatReconciliationRow);
    }

    [ConsoleSample(Name = "List credit note reconciliations filtered by date")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListCreditNoteReconciliationsFilteredAsync(CancellationToken cancellationToken)
    {
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));
        var reconciliations = await context.Client.CreditNoteReconciliations.ListAsync(
            fromDate: fromDate,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Credit note reconciliations from {fromDate:yyyy-MM-dd} ({reconciliations.Count})");
        SampleOutput.WriteRows(reconciliations, FormatReconciliationRow);
    }

    [ConsoleSample(Name = "Get credit note reconciliation by id")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetCreditNoteReconciliationByIdAsync(CancellationToken cancellationToken)
    {
        var reconciliation = await context.Data.GetFirstCreditNoteReconciliationAsync(cancellationToken);
        var detail = await context.Client.CreditNoteReconciliations.GetCreditNoteReconciliationAsync(
            reconciliation.ResourceId,
            new CreditNoteReconciliationGetOptions
            {
                IncludeInvoice = true,
                IncludeCreditNote = true
            },
            cancellationToken);

        SampleOutput.WriteHeader("Credit note reconciliation detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Gross value", detail.GrossValue);
        SampleOutput.WriteField("Dated on", detail.DatedOn);
        SampleOutput.WriteField("Invoice ID", detail.InvoiceId);
        SampleOutput.WriteField("Invoice reference", detail.Invoice?.Reference);
        SampleOutput.WriteField("Credit note ID", detail.CreditNoteId);
        SampleOutput.WriteField("Credit note reference", detail.CreditNote?.Reference);
    }

    [ConsoleSample(Name = "Create probe credit note reconciliation and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeCreditNoteReconciliationAndDeleteAsync(CancellationToken cancellationToken)
    {
        var (invoice, creditNote) = await CreateSentReconciliationProbeDocumentsAsync(cancellationToken);
        try
        {
            var grossValue = ResolveProbeGrossValue(invoice.TotalValue, creditNote.TotalValue);

            var created = await context.Client.CreditNoteReconciliations.CreateCreditNoteReconciliationAsync(
                CreateCreditNoteReconciliationRequest.Create(
                    grossValue,
                    invoiceId: invoice.ResourceId,
                    creditNoteId: creditNote.ResourceId,
                    datedOn: DateOnly.FromDateTime(DateTime.UtcNow)),
                cancellationToken);

            SampleOutput.WriteHeader("Created credit note reconciliation");
            SampleOutput.WriteField("Id", created.ResourceId);
            SampleOutput.WriteField("Gross value", created.GrossValue);
            SampleOutput.WriteField("Invoice ID", created.InvoiceId);
            SampleOutput.WriteField("Credit note ID", created.CreditNoteId);

            await context.Client.CreditNoteReconciliations.DeleteCreditNoteReconciliationAsync(
                created.ResourceId,
                cancellationToken);

            Console.WriteLine();
            Console.WriteLine($"  Deleted reconciliation {created.ResourceId}.");
        }
        finally
        {
            await DeleteSentReconciliationProbeDocumentsAsync(
                invoice.ResourceId,
                creditNote.ResourceId,
                cancellationToken);
        }
    }

    [ConsoleSample(Name = "Update credit note reconciliation", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateCreditNoteReconciliationAsync(CancellationToken cancellationToken)
    {
        var (invoice, creditNote) = await CreateSentReconciliationProbeDocumentsAsync(cancellationToken);
        try
        {
            var grossValue = ResolveProbeGrossValue(invoice.TotalValue, creditNote.TotalValue);

            var created = await context.Client.CreditNoteReconciliations.CreateCreditNoteReconciliationAsync(
                CreateCreditNoteReconciliationRequest.Create(
                    grossValue,
                    invoiceId: invoice.ResourceId,
                    creditNoteId: creditNote.ResourceId,
                    datedOn: DateOnly.FromDateTime(DateTime.UtcNow)),
                cancellationToken);

            if (grossValue <= 1m)
            {
                await context.Client.CreditNoteReconciliations.DeleteCreditNoteReconciliationAsync(
                    created.ResourceId,
                    cancellationToken);
                SampleContext.Skip("probe gross value cannot be decremented for update test");
            }

            var updatedGrossValue = grossValue - 1m;
            var updated = await context.Client.CreditNoteReconciliations.UpdateCreditNoteReconciliationAsync(
                created.ResourceId,
                UpdateCreditNoteReconciliationRequest.Create(
                    grossValue: updatedGrossValue,
                    datedOn: DateOnly.FromDateTime(DateTime.UtcNow)),
                cancellationToken);

            SampleOutput.WriteHeader("Updated credit note reconciliation");
            SampleOutput.WriteField("Id", updated.ResourceId);
            SampleOutput.WriteField("Original gross value", grossValue);
            SampleOutput.WriteField("Updated gross value", updated.GrossValue);
            SampleOutput.WriteField("Dated on", updated.DatedOn);

            await context.Client.CreditNoteReconciliations.DeleteCreditNoteReconciliationAsync(
                created.ResourceId,
                cancellationToken);

            Console.WriteLine();
            Console.WriteLine($"  Deleted reconciliation {created.ResourceId}.");
        }
        finally
        {
            await DeleteSentReconciliationProbeDocumentsAsync(
                invoice.ResourceId,
                creditNote.ResourceId,
                cancellationToken);
        }
    }

    private async Task<(Invoice Invoice, CreditNote CreditNote)> CreateSentReconciliationProbeDocumentsAsync(
        CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var nominalCode = await context.Data.GetFirstIncomeCategoryNominalCodeAsync(cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var referenceSuffix = DateTimeOffset.UtcNow.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);

        var invoiceDraft = await context.Client.Invoices.CreateInvoiceAsync(
            new Invoice
            {
                ContactId = contact.ResourceId,
                DatedOn = today,
                DueOn = today.AddDays(14),
                PaymentTermsInDays = 14,
                Reference = $"Console reconciliation invoice {referenceSuffix}",
                InvoiceItems =
                [
                    new InvoiceItem
                    {
                        Description = "Console reconciliation probe invoice line",
                        ItemType = InvoiceItemType.Services,
                        Quantity = 1,
                        Price = 100m,
                        CategoryNominalCode = nominalCode,
                    }
                ]
            },
            cancellationToken);

        var invoice = await context.Client.Invoices.MarkInvoiceAsSentAsync(invoiceDraft.ResourceId, cancellationToken);

        var creditNoteDraft = await context.Client.CreditNotes.CreateCreditNoteAsync(
            new CreditNote
            {
                ContactId = contact.ResourceId,
                DatedOn = today,
                DueOn = today,
                PaymentTermsInDays = 0,
                Reference = $"Console reconciliation credit {referenceSuffix}",
                CreditNoteItems =
                [
                    new CreditNoteItem
                    {
                        Description = "Console reconciliation probe credit line",
                        ItemType = InvoiceItemType.Services,
                        Quantity = 1,
                        Price = -50m,
                        SalesTaxRate = 20m,
                        SalesTaxStatus = InvoiceSalesTaxStatus.Taxable,
                        CategoryNominalCode = nominalCode
                    }
                ]
            },
            cancellationToken);

        var creditNote = await context.Client.CreditNotes.MarkCreditNoteAsSentAsync(
            creditNoteDraft.ResourceId,
            cancellationToken);

        return (invoice, creditNote);
    }

    private async Task DeleteSentReconciliationProbeDocumentsAsync(
        long invoiceId,
        long creditNoteId,
        CancellationToken cancellationToken)
    {
        await DeleteSentCreditNoteAsync(creditNoteId, cancellationToken);
        await DeleteSentInvoiceAsync(invoiceId, cancellationToken);
    }

    private async Task DeleteSentInvoiceAsync(long invoiceId, CancellationToken cancellationToken)
    {
        try
        {
            await context.Client.Invoices.DeleteInvoiceAsync(invoiceId, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await context.Client.Invoices.MarkInvoiceAsDraftAsync(invoiceId, cancellationToken);
            await context.Client.Invoices.DeleteInvoiceAsync(invoiceId, cancellationToken);
        }
    }

    private async Task DeleteSentCreditNoteAsync(long creditNoteId, CancellationToken cancellationToken)
    {
        try
        {
            await context.Client.CreditNotes.DeleteCreditNoteAsync(creditNoteId, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await context.Client.CreditNotes.MarkCreditNoteAsDraftAsync(creditNoteId, cancellationToken);
            await context.Client.CreditNotes.DeleteCreditNoteAsync(creditNoteId, cancellationToken);
        }
    }

    private static string FormatReconciliationRow(CreditNoteReconciliation reconciliation) =>
        $"{reconciliation.ResourceId,8}  gross={reconciliation.GrossValue,8}  invoice={reconciliation.InvoiceId,6}  credit_note={reconciliation.CreditNoteId,6}  dated={reconciliation.DatedOn?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "—"}";

    private static decimal ResolveProbeGrossValue(decimal? invoiceTotal, decimal? creditNoteTotal)
    {
        var invoiceAmount = invoiceTotal is > 0 ? invoiceTotal.Value : 0m;
        var creditNoteAmount = creditNoteTotal is { } total && total != 0 ? Math.Abs(total) : 0m;

        if (invoiceAmount > 0 && creditNoteAmount > 0)
        {
            return Math.Min(invoiceAmount, creditNoteAmount);
        }

        return invoiceAmount > 0 ? invoiceAmount
            : creditNoteAmount > 0 ? creditNoteAmount
            : 1m;
    }
}
