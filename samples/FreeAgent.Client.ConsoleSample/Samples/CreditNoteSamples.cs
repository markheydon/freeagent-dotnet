using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client;
using FreeAgent.Client.Models.CreditNotes;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Credit notes endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Credit notes")]
internal sealed class CreditNoteSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List credit notes")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListCreditNotesAsync(CancellationToken cancellationToken)
    {
        var page = await context.Client.CreditNotes.ListAsync(perPage: 25, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Credit notes (showing {page.Items.Count} of {page.Total})");
        SampleOutput.WriteRows(page.Items, FormatCreditNoteRow);

        if (page.HasNextPage)
        {
            Console.WriteLine();
            Console.WriteLine("  (First page only. Use ListAutoPagingAsync to stream all pages.)");
        }
    }

    [ConsoleSample(Name = "Stream all credit notes", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task StreamAllCreditNotesAsync(CancellationToken cancellationToken)
    {
        var count = 0;
        await foreach (var creditNote in context.Client.CreditNotes.ListAutoPagingAsync(cancellationToken: cancellationToken))
        {
            count++;
            if (count <= 5)
            {
                Console.WriteLine($"  {FormatCreditNoteRow(creditNote)}");
            }
        }

        SampleOutput.WriteHeader("Streamed credit notes via ListAutoPagingAsync");
        Console.WriteLine($"  Total credit notes streamed: {count}");
    }

    [ConsoleSample(Name = "List credit notes filtered by contact")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListCreditNotesFilteredByContactAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var page = await context.Client.CreditNotes.ListAsync(
            perPage: 25,
            contactId: contact.ResourceId,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Credit notes for {contact.DisplayName} ({page.Items.Count})");
        SampleOutput.WriteRows(page.Items, FormatCreditNoteRow);
    }

    [ConsoleSample(Name = "Get credit note by id")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetCreditNoteByIdAsync(CancellationToken cancellationToken)
    {
        var creditNote = await context.Data.GetFirstCreditNoteAsync(cancellationToken);
        var detail = await context.Client.CreditNotes.GetCreditNoteAsync(
            creditNote.ResourceId,
            new CreditNoteGetOptions { IncludeContact = true, IncludeProject = true },
            cancellationToken);

        SampleOutput.WriteHeader("Credit note detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Reference", detail.Reference);
        SampleOutput.WriteField("Status", detail.Status);
        SampleOutput.WriteField("Contact", detail.ContactName ?? detail.Contact?.OrganisationName);
        SampleOutput.WriteField("Dated on", detail.DatedOn);
        SampleOutput.WriteField("Total value", detail.TotalValue);
    }

    [ConsoleSample(Name = "Create probe credit note and delete")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeCreditNoteAndDeleteAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleDraftCreditNoteAsync(cancellationToken);

        SampleOutput.WriteHeader("Created probe credit note");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Reference", created.Reference);
        SampleOutput.WriteField("Status", created.Status);
        SampleOutput.WriteField("Total value", created.TotalValue);
        SampleOutput.WriteField("Line items", created.CreditNoteItems?.Count ?? 0);

        await context.Client.CreditNotes.DeleteCreditNoteAsync(created.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Deleted probe credit note");
        SampleOutput.WriteField("Id", created.ResourceId);
    }

    [ConsoleSample(Name = "Mark credit note as sent")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkCreditNoteAsSentAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftCreditNoteAsync(cancellationToken);

        try
        {
            var sent = await context.Client.CreditNotes.MarkCreditNoteAsSentAsync(draft.ResourceId, cancellationToken);

            SampleOutput.WriteHeader("Marked credit note as sent");
            SampleOutput.WriteField("Id", sent.ResourceId);
            SampleOutput.WriteField("Reference", sent.Reference);
            SampleOutput.WriteField("Status", sent.Status);
        }
        finally
        {
            await DeleteProbeCreditNoteAsync(draft.ResourceId, cancellationToken);
        }
    }

    [ConsoleSample(Name = "Update credit note comments")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateCreditNoteCommentsAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftCreditNoteAsync(cancellationToken);

        try
        {
            draft.Comments = "Updated by console sample";
            var updated = await context.Client.CreditNotes.UpdateCreditNoteAsync(
                draft.ResourceId,
                draft,
                new CreditNoteUpdateOptions { OmitLineItems = true },
                cancellationToken);

            SampleOutput.WriteHeader("Updated credit note comments");
            SampleOutput.WriteField("Id", updated.ResourceId);
            SampleOutput.WriteField("Comments", updated.Comments);
        }
        finally
        {
            await DeleteProbeCreditNoteAsync(draft.ResourceId, cancellationToken);
        }
    }

    [ConsoleSample(Name = "Mark credit note as draft")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkCreditNoteAsDraftAsync(CancellationToken cancellationToken)
    {
        var sent = await CreateSentSampleCreditNoteAsync(cancellationToken);

        try
        {
            var draft = await context.Client.CreditNotes.MarkCreditNoteAsDraftAsync(sent.ResourceId, cancellationToken);

            SampleOutput.WriteHeader("Marked credit note as draft");
            SampleOutput.WriteField("Id", draft.ResourceId);
            SampleOutput.WriteField("Status", draft.Status);
        }
        finally
        {
            await DeleteProbeCreditNoteAsync(sent.ResourceId, cancellationToken);
        }
    }

    [ConsoleSample(Name = "Get credit note PDF", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetCreditNotePdfAsync(CancellationToken cancellationToken)
    {
        var creditNote = await context.Data.GetFirstCreditNoteAsync(cancellationToken);
        var pdfBytes = await context.Client.CreditNotes.GetCreditNotePdfAsync(creditNote.ResourceId, cancellationToken);
        var path = Path.Combine(Path.GetTempPath(), $"freeagent-credit-note-{creditNote.ResourceId}.pdf");
        await File.WriteAllBytesAsync(path, pdfBytes, cancellationToken);

        SampleOutput.WriteHeader("Credit note PDF");
        SampleOutput.WriteField("Credit note ID", creditNote.ResourceId);
        SampleOutput.WriteField("PDF path", path);
        SampleOutput.WriteField("Size (bytes)", pdfBytes.Length);
    }

    [ConsoleSample(Name = "Send credit note email (template)", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task SendCreditNoteEmailAsync(CancellationToken cancellationToken)
    {
        var creditNote = await context.Data.GetFirstCreditNoteAsync(cancellationToken);
        await context.Client.CreditNotes.SendCreditNoteEmailAsync(
            creditNote.ResourceId,
            new SendCreditNoteEmailRequest
            {
                Email = new CreditNoteEmailDetails { UseTemplate = true }
            },
            cancellationToken);

        SampleOutput.WriteHeader("Sent credit note email");
        SampleOutput.WriteField("Credit note ID", creditNote.ResourceId);
        SampleOutput.WriteField("Template", "default");
    }

    private async Task<CreditNote> CreateSampleDraftCreditNoteAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var nominalCode = await context.Data.GetFirstIncomeCategoryNominalCodeAsync(cancellationToken);

        return await context.Client.CreditNotes.CreateCreditNoteAsync(
            new CreditNote
            {
                ContactId = contact.ResourceId,
                DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                DueOn = DateOnly.FromDateTime(DateTime.UtcNow),
                PaymentTermsInDays = 0,
                Reference = $"Console probe {DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}",
                CreditNoteItems =
                [
                    new CreditNoteItem
                    {
                        Description = "Console probe credit",
                        ItemType = InvoiceItemType.Services,
                        Quantity = 1,
                        Price = -25m,
                        SalesTaxRate = 20m,
                        SalesTaxStatus = InvoiceSalesTaxStatus.Taxable,
                        CategoryNominalCode = nominalCode
                    }
                ]
            },
            cancellationToken);
    }

    private async Task<CreditNote> CreateSentSampleCreditNoteAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftCreditNoteAsync(cancellationToken);
        return await context.Client.CreditNotes.MarkCreditNoteAsSentAsync(draft.ResourceId, cancellationToken);
    }

    private async Task DeleteProbeCreditNoteAsync(long creditNoteId, CancellationToken cancellationToken)
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

    private static string FormatCreditNoteRow(CreditNote creditNote) =>
        $"{creditNote.ResourceId,6}  {creditNote.Reference ?? "-",-20}  {creditNote.Status,-12}  {creditNote.ContactName ?? "-",-24}  {creditNote.TotalValue,10:0.00}";
}
