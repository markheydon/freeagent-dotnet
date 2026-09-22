using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Contacts endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Contacts")]
internal sealed class ContactSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List active contacts")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListActiveContactsAsync(CancellationToken cancellationToken)
    {
        var page = await context.Client.Contacts.ListAsync(
            perPage: 25,
            view: ContactViews.Active,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Active contacts (showing {page.Items.Count} of {page.Total})");
        SampleOutput.WriteRows(page.Items, contact => $"{contact.ResourceId,8}  {contact.DisplayName}");

        if (page.HasNextPage)
        {
            Console.WriteLine();
            Console.WriteLine("  (First page only. Use ListAutoPagingAsync to stream all pages.)");
        }
    }

    [ConsoleSample(Name = "Stream all contacts", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task StreamAllContactsAsync(CancellationToken cancellationToken)
    {
        var count = 0;
        await foreach (var contact in context.Client.Contacts.ListAutoPagingAsync(cancellationToken: cancellationToken))
        {
            count++;
            if (count <= 5)
            {
                Console.WriteLine($"  {contact.ResourceId,8}  {contact.DisplayName}");
            }
        }

        SampleOutput.WriteHeader($"Streamed contacts via ListAutoPagingAsync");
        Console.WriteLine($"  Total contacts streamed: {count}");
    }

    [ConsoleSample(Name = "Get contact detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetContactDetailAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var detail = await context.Client.Contacts.GetContactAsync(contact.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Contact detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Display name", detail.DisplayName);
        SampleOutput.WriteField("Email", detail.Email);
        SampleOutput.WriteField("Status", detail.Status);
    }
}
