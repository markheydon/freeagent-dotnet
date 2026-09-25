using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FreeAgent.Client;
using FreeAgent.Client.Models.CreditNoteReconciliations;

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

    [ConsoleSample(Name = "Create probe credit note reconciliation and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeCreditNoteReconciliationAndDeleteAsync(CancellationToken cancellationToken)
    {
        var invoice = await context.Data.GetFirstInvoiceAsync(cancellationToken);
        var creditNote = await context.Data.GetFirstCreditNoteAsync(cancellationToken);
        var grossValue = ResolveProbeGrossValue(invoice.TotalValue, creditNote.TotalValue);

        var created = await context.Client.CreditNoteReconciliations.CreateCreditNoteReconciliationAsync(
            CreateCreditNoteReconciliationRequest.Create(grossValue, invoiceId: invoice.ResourceId, creditNoteId: creditNote.ResourceId,
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

    [ConsoleSample(Name = "Update credit note reconciliation", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateCreditNoteReconciliationAsync(CancellationToken cancellationToken)
    {
        var invoice = await context.Data.GetFirstInvoiceAsync(cancellationToken);
        var creditNote = await context.Data.GetFirstCreditNoteAsync(cancellationToken);
        var grossValue = ResolveProbeGrossValue(invoice.TotalValue, creditNote.TotalValue);

        var created = await context.Client.CreditNoteReconciliations.CreateCreditNoteReconciliationAsync(
            CreateCreditNoteReconciliationRequest.Create(grossValue, invoiceId: invoice.ResourceId, creditNoteId: creditNote.ResourceId,
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

    private static string FormatReconciliationRow(CreditNoteReconciliation reconciliation) =>
        $"{reconciliation.ResourceId,8}  gross={reconciliation.GrossValue,8}  invoice={reconciliation.InvoiceId,6}  credit_note={reconciliation.CreditNoteId,6}  dated={reconciliation.DatedOn?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "—"}";

    private static decimal ResolveProbeGrossValue(decimal? invoiceTotal, decimal? creditNoteTotal)
    {
        if (invoiceTotal is > 0 && creditNoteTotal is > 0)
        {
            return Math.Min(invoiceTotal.Value, creditNoteTotal.Value);
        }

        return invoiceTotal is > 0 ? invoiceTotal.Value
            : creditNoteTotal is > 0 ? creditNoteTotal.Value
            : 1m;
    }
}
