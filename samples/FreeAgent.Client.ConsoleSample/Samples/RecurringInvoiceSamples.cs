using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client;
using FreeAgent.Client.Models.RecurringInvoices;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Recurring invoices endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Recurring invoices")]
internal sealed class RecurringInvoiceSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List recurring invoices")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListRecurringInvoicesAsync(CancellationToken cancellationToken)
    {
        var page = await context.Client.RecurringInvoices.ListAsync(perPage: 25, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Recurring invoices (showing {page.Items.Count} of {page.Total})");
        SampleOutput.WriteRows(page.Items, FormatRecurringInvoiceRow);

        if (page.HasNextPage)
        {
            Console.WriteLine();
            Console.WriteLine("  (First page only. Use ListAutoPagingAsync to stream all pages.)");
        }
    }

    [ConsoleSample(Name = "Stream all recurring invoices", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task StreamAllRecurringInvoicesAsync(CancellationToken cancellationToken)
    {
        var count = 0;
        await foreach (var recurringInvoice in context.Client.RecurringInvoices.ListAutoPagingAsync(cancellationToken: cancellationToken))
        {
            count++;
            if (count <= 5)
            {
                Console.WriteLine($"  {FormatRecurringInvoiceRow(recurringInvoice)}");
            }
        }

        SampleOutput.WriteHeader("Streamed recurring invoices via ListAutoPagingAsync");
        Console.WriteLine($"  Total recurring invoices streamed: {count}");
    }

    [ConsoleSample(Name = "List recurring invoices filtered by contact")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListRecurringInvoicesFilteredByContactAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var page = await context.Client.RecurringInvoices.ListAsync(
            perPage: 25,
            contactId: contact.ResourceId,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Recurring invoices for {contact.DisplayName} ({page.Items.Count})");
        SampleOutput.WriteRows(page.Items, FormatRecurringInvoiceRow);
    }

    [ConsoleSample(Name = "Get recurring invoice by id")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetRecurringInvoiceByIdAsync(CancellationToken cancellationToken)
    {
        var recurringInvoice = await context.Data.GetFirstRecurringInvoiceAsync(cancellationToken);
        var detail = await context.Client.RecurringInvoices.GetRecurringInvoiceAsync(
            recurringInvoice.ResourceId,
            new RecurringInvoiceGetOptions { IncludeContact = true, IncludeProject = true },
            cancellationToken);

        SampleOutput.WriteHeader("Recurring invoice detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Reference", detail.Reference);
        SampleOutput.WriteField("Status", detail.RecurringStatus);
        SampleOutput.WriteField("Frequency", detail.Frequency);
        SampleOutput.WriteField("Contact", detail.ContactName ?? detail.Contact?.OrganisationName);
        SampleOutput.WriteField("Next recurs on", detail.NextRecursOn);
        SampleOutput.WriteField("Net value", detail.NetValue);
    }

    private static string FormatRecurringInvoiceRow(RecurringInvoice recurringInvoice) =>
        $"{recurringInvoice.ResourceId,6}  {recurringInvoice.Reference ?? "-",-8}  {recurringInvoice.RecurringStatus,-8}  {recurringInvoice.Frequency,-12}  {recurringInvoice.ContactName ?? "-"}";
}
