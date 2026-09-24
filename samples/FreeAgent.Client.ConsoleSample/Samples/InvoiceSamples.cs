using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Invoices endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Invoices")]
internal sealed class InvoiceSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List invoices")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListInvoicesAsync(CancellationToken cancellationToken)
    {
        var page = await context.Client.Invoices.ListAsync(perPage: 25, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Invoices (showing {page.Items.Count} of {page.Total})");
        SampleOutput.WriteRows(page.Items, FormatInvoiceRow);
    }

    [ConsoleSample(Name = "List invoices filtered by contact")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListInvoicesFilteredByContactAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var page = await context.Client.Invoices.ListAsync(
            perPage: 25,
            contactId: contact.ResourceId,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Invoices for {contact.DisplayName} ({page.Items.Count})");
        SampleOutput.WriteRows(page.Items, FormatInvoiceRow);
    }

    [ConsoleSample(Name = "List invoice timeline")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListInvoiceTimelineAsync(CancellationToken cancellationToken)
    {
        var items = await context.Client.Invoices.ListTimelineAsync(cancellationToken);

        SampleOutput.WriteHeader($"Invoice timeline ({items.Count})");
        SampleOutput.WriteRows(
            items,
            item => $"{item.DatedOn:yyyy-MM-dd}  {item.Reference ?? "-",-8}  {item.Summary ?? item.Description ?? "-"}");
    }

    [ConsoleSample(Name = "Get invoice by id")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetInvoiceByIdAsync(CancellationToken cancellationToken)
    {
        var invoice = await context.Data.GetFirstInvoiceAsync(cancellationToken);
        var detail = await context.Client.Invoices.GetInvoiceAsync(
            invoice.ResourceId,
            new InvoiceGetOptions { IncludeContact = true, IncludeProject = true },
            cancellationToken);

        SampleOutput.WriteHeader("Invoice detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Reference", detail.Reference);
        SampleOutput.WriteField("Status", detail.Status);
        SampleOutput.WriteField("Contact", detail.ContactName ?? detail.Contact?.OrganisationName);
        SampleOutput.WriteField("Dated on", detail.DatedOn);
        SampleOutput.WriteField("Due on", detail.DueOn);
        SampleOutput.WriteField("Total", detail.TotalValue);
    }

    [ConsoleSample(Name = "Create draft invoice with one line item")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateDraftInvoiceAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleDraftInvoiceAsync(cancellationToken);

        SampleOutput.WriteHeader("Created draft invoice");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Reference", created.Reference);
        SampleOutput.WriteField("Status", created.Status);
        SampleOutput.WriteField("Total", created.TotalValue);
        SampleOutput.WriteField("Line items", created.InvoiceItems?.Count ?? 0);
    }

    [ConsoleSample(Name = "Update invoice comments")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateInvoiceCommentsAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftInvoiceAsync(cancellationToken);
        draft.Comments = "Updated by console sample";
        draft.OmitInvoiceItemsFromWrite = true;

        var updated = await context.Client.Invoices.UpdateInvoiceAsync(draft.ResourceId, draft, cancellationToken);

        SampleOutput.WriteHeader("Updated invoice comments");
        SampleOutput.WriteField("Id", updated.ResourceId);
        SampleOutput.WriteField("Comments", updated.Comments);
    }

    [ConsoleSample(Name = "Duplicate invoice")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task DuplicateInvoiceAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftInvoiceAsync(cancellationToken);
        var duplicate = await context.Client.Invoices.DuplicateInvoiceAsync(draft.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Duplicated invoice");
        SampleOutput.WriteField("Source ID", draft.ResourceId);
        SampleOutput.WriteField("Duplicate ID", duplicate.ResourceId);
        SampleOutput.WriteField("Status", duplicate.Status);
    }

    [ConsoleSample(Name = "Send invoice email (template)")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task SendInvoiceEmailAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftInvoiceAsync(cancellationToken);
        await context.Client.Invoices.SendInvoiceEmailAsync(
            draft.ResourceId,
            new SendInvoiceEmailRequest
            {
                Email = new InvoiceEmailDetails { UseTemplate = true }
            },
            cancellationToken);

        SampleOutput.WriteHeader("Sent invoice email");
        SampleOutput.WriteField("Invoice ID", draft.ResourceId);
        SampleOutput.WriteField("Template", true);
    }

    [ConsoleSample(Name = "Mark invoice as sent")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkInvoiceAsSentAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftInvoiceAsync(cancellationToken);
        var sent = await context.Client.Invoices.MarkInvoiceAsSentAsync(draft.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Marked invoice as sent");
        SampleOutput.WriteField("Id", sent.ResourceId);
        SampleOutput.WriteField("Reference", sent.Reference);
        SampleOutput.WriteField("Status", sent.Status);
    }

    [ConsoleSample(Name = "Mark invoice as scheduled")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkInvoiceAsScheduledAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftInvoiceAsync(cancellationToken);
        var scheduled = await context.Client.Invoices.MarkInvoiceAsScheduledAsync(draft.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Marked invoice as scheduled");
        SampleOutput.WriteField("Id", scheduled.ResourceId);
        SampleOutput.WriteField("Status", scheduled.Status);
    }

    [ConsoleSample(Name = "Mark invoice as draft")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkInvoiceAsDraftAsync(CancellationToken cancellationToken)
    {
        var sent = await CreateSentSampleInvoiceAsync(cancellationToken);
        var draft = await context.Client.Invoices.MarkInvoiceAsDraftAsync(sent.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Marked invoice as draft");
        SampleOutput.WriteField("Id", draft.ResourceId);
        SampleOutput.WriteField("Status", draft.Status);
    }

    [ConsoleSample(Name = "Mark invoice as cancelled")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkInvoiceAsCancelledAsync(CancellationToken cancellationToken)
    {
        var sent = await CreateSentSampleInvoiceAsync(cancellationToken);
        var cancelled = await context.Client.Invoices.MarkInvoiceAsCancelledAsync(sent.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Marked invoice as cancelled");
        SampleOutput.WriteField("Id", cancelled.ResourceId);
        SampleOutput.WriteField("Status", cancelled.Status);
    }

    [ConsoleSample(Name = "Get invoice PDF")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetInvoicePdfAsync(CancellationToken cancellationToken)
    {
        var invoice = await context.Data.GetFirstInvoiceAsync(cancellationToken);
        var pdfBytes = await context.Client.Invoices.GetInvoicePdfAsync(invoice.ResourceId, cancellationToken);
        var path = Path.Combine(Path.GetTempPath(), $"freeagent-invoice-{invoice.ResourceId}.pdf");
        await File.WriteAllBytesAsync(path, pdfBytes, cancellationToken);

        SampleOutput.WriteHeader("Invoice PDF");
        SampleOutput.WriteField("Invoice ID", invoice.ResourceId);
        SampleOutput.WriteField("PDF path", path);
        SampleOutput.WriteField("Size (bytes)", pdfBytes.Length);
    }

    [ConsoleSample(Name = "Get default additional text")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetDefaultAdditionalTextAsync(CancellationToken cancellationToken)
    {
        var text = await context.Client.Invoices.GetDefaultAdditionalTextAsync(cancellationToken);

        SampleOutput.WriteHeader("Default additional text");
        SampleOutput.WriteField("Text", string.IsNullOrWhiteSpace(text) ? "(not set)" : text);
    }

    [ConsoleSample(Name = "Update default additional text")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateDefaultAdditionalTextAsync(CancellationToken cancellationToken)
    {
        var text = await context.Client.Invoices.UpdateDefaultAdditionalTextAsync(
            "Pay within 14 days (console sample update).",
            cancellationToken);

        SampleOutput.WriteHeader("Updated default additional text");
        SampleOutput.WriteField("Text", text);
    }

    [ConsoleSample(Name = "Delete default additional text")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task DeleteDefaultAdditionalTextAsync(CancellationToken cancellationToken)
    {
        await context.Client.Invoices.DeleteDefaultAdditionalTextAsync(cancellationToken);

        SampleOutput.WriteHeader("Deleted default additional text");
        SampleOutput.WriteField("Result", "Deleted");
    }

    [ConsoleSample(Name = "Delete invoice", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task DeleteInvoiceAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftInvoiceAsync(cancellationToken);
        await context.Client.Invoices.DeleteInvoiceAsync(draft.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Deleted invoice");
        SampleOutput.WriteField("Id", draft.ResourceId);
    }

    [ConsoleSample(Name = "Take direct debit payment", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task TakeDirectDebitPaymentAsync(CancellationToken cancellationToken)
    {
        var invoice = await context.Data.GetFirstInvoiceAsync(cancellationToken);
        await context.Client.Invoices.TakeDirectDebitPaymentAsync(invoice.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Requested direct debit payment");
        SampleOutput.WriteField("Invoice ID", invoice.ResourceId);
    }

    [ConsoleSample(Name = "Convert invoice to credit note", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ConvertToCreditNoteAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var nominalCode = await context.Data.GetFirstIncomeCategoryNominalCodeAsync(cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var draft = await context.Client.Invoices.CreateInvoiceAsync(
            new Invoice
            {
                BillingContact = ContactReference.ForEnvironment(context.Client.Environment, contact.ResourceId),
                DatedOn = today,
                PaymentTermsInDays = 14,
                Reference = $"Console credit-note source {DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}",
                InvoiceItems =
                [
                    new InvoiceItem
                    {
                        Description = "Negative line for credit note conversion",
                        ItemType = InvoiceItemType.Services,
                        Quantity = 1,
                        Price = -100,
                        Category = CategoryReference.ForEnvironment(context.Client.Environment, nominalCode),
                    }
                ]
            },
            cancellationToken);

        var creditNote = await context.Client.Invoices.ConvertToCreditNoteAsync(draft.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Converted invoice to credit note");
        SampleOutput.WriteField("Invoice ID", draft.ResourceId);
        SampleOutput.WriteField("Credit note ID", creditNote.ResourceId);
    }

    private async Task<Invoice> CreateSampleDraftInvoiceAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var nominalCode = await context.Data.GetFirstIncomeCategoryNominalCodeAsync(cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await context.Client.Invoices.CreateInvoiceAsync(
            new Invoice
            {
                BillingContact = ContactReference.ForEnvironment(context.Client.Environment, contact.ResourceId),
                DatedOn = today,
                DueOn = today.AddDays(14),
                PaymentTermsInDays = 14,
                Reference = $"Console sample {DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}",
                InvoiceItems =
                [
                    new InvoiceItem
                    {
                        Description = "Console sample line item",
                        ItemType = InvoiceItemType.Services,
                        Quantity = 1,
                        Price = 100,
                        Category = CategoryReference.ForEnvironment(context.Client.Environment, nominalCode),
                    }
                ]
            },
            cancellationToken);
    }

    private async Task<Invoice> CreateSentSampleInvoiceAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftInvoiceAsync(cancellationToken);
        return await context.Client.Invoices.MarkInvoiceAsSentAsync(draft.ResourceId, cancellationToken);
    }

    private static string FormatInvoiceRow(Invoice invoice) =>
        $"{invoice.ResourceId,8}  {invoice.Reference ?? "-",-20}  {invoice.Status,-12}  {invoice.ContactName ?? "-"}";
}
