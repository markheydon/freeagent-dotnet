using System.Diagnostics.CodeAnalysis;
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

    [ConsoleSample(Name = "Get invoice by id")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetInvoiceByIdAsync(CancellationToken cancellationToken)
    {
        var invoice = await context.Data.GetFirstInvoiceAsync(cancellationToken);
        var detail = await context.Client.Invoices.GetInvoiceAsync(invoice.ResourceId, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader("Invoice detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Reference", detail.Reference);
        SampleOutput.WriteField("Status", detail.Status);
        SampleOutput.WriteField("Contact", detail.ContactName);
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

    private static string FormatInvoiceRow(Invoice invoice) =>
        $"{invoice.ResourceId,8}  {invoice.Reference ?? "-",-20}  {invoice.Status,-12}  {invoice.ContactName ?? "-"}";
}
