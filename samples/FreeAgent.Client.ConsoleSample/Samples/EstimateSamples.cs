using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client;
using FreeAgent.Client.Models.Estimates;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Estimates endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Estimates")]
internal sealed class EstimateSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List estimates")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListEstimatesAsync(CancellationToken cancellationToken)
    {
        var page = await context.Client.Estimates.ListAsync(perPage: 25, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Estimates (showing {page.Items.Count} of {page.Total})");
        SampleOutput.WriteRows(page.Items, FormatEstimateRow);

        if (page.HasNextPage)
        {
            Console.WriteLine();
            Console.WriteLine("  (First page only. Use ListAutoPagingAsync to stream all pages.)");
        }
    }

    [ConsoleSample(Name = "Stream all estimates", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task StreamAllEstimatesAsync(CancellationToken cancellationToken)
    {
        var count = 0;
        await foreach (var estimate in context.Client.Estimates.ListAutoPagingAsync(cancellationToken: cancellationToken))
        {
            count++;
            if (count <= 5)
            {
                Console.WriteLine($"  {FormatEstimateRow(estimate)}");
            }
        }

        SampleOutput.WriteHeader("Streamed estimates via ListAutoPagingAsync");
        Console.WriteLine($"  Total estimates streamed: {count}");
    }

    [ConsoleSample(Name = "List estimates filtered by contact")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListEstimatesFilteredByContactAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var page = await context.Client.Estimates.ListAsync(
            perPage: 25,
            contactId: contact.ResourceId,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Estimates for {contact.DisplayName} ({page.Items.Count})");
        SampleOutput.WriteRows(page.Items, FormatEstimateRow);
    }

    [ConsoleSample(Name = "Get estimate by id")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetEstimateByIdAsync(CancellationToken cancellationToken)
    {
        var estimate = await context.Data.GetFirstEstimateAsync(cancellationToken);
        var detail = await context.Client.Estimates.GetEstimateAsync(
            estimate.ResourceId,
            new EstimateGetOptions { IncludeContact = true, IncludeProject = true },
            cancellationToken);

        SampleOutput.WriteHeader("Estimate detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Reference", detail.Reference);
        SampleOutput.WriteField("Status", detail.Status);
        SampleOutput.WriteField("Contact", detail.ClientContactName ?? detail.Contact?.OrganisationName);
        SampleOutput.WriteField("Dated on", detail.DatedOn);
        SampleOutput.WriteField("Net value", detail.NetValue);
    }

    [ConsoleSample(Name = "Create probe estimate and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeEstimateAndDeleteAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleDraftEstimateAsync(cancellationToken);

        SampleOutput.WriteHeader("Created probe estimate");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Reference", created.Reference);
        SampleOutput.WriteField("Status", created.Status);
        SampleOutput.WriteField("Net value", created.NetValue);
        SampleOutput.WriteField("Line items", created.EstimateItems?.Count ?? 0);

        await context.Client.Estimates.DeleteEstimateAsync(created.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Deleted probe estimate");
        SampleOutput.WriteField("Id", created.ResourceId);
    }

    [ConsoleSample(Name = "Mark estimate as sent", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkEstimateAsSentAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftEstimateAsync(cancellationToken);

        try
        {
            var sent = await context.Client.Estimates.MarkEstimateAsSentAsync(draft.ResourceId, cancellationToken);

            SampleOutput.WriteHeader("Marked estimate as sent");
            SampleOutput.WriteField("Id", sent.ResourceId);
            SampleOutput.WriteField("Reference", sent.Reference);
            SampleOutput.WriteField("Status", sent.Status);
        }
        finally
        {
            await DeleteProbeEstimateAsync(draft.ResourceId, cancellationToken);
        }
    }

    [ConsoleSample(Name = "Get estimate PDF", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetEstimatePdfAsync(CancellationToken cancellationToken)
    {
        var estimate = await context.Data.GetFirstEstimateAsync(cancellationToken);
        var pdfBytes = await context.Client.Estimates.GetEstimatePdfAsync(estimate.ResourceId, cancellationToken);
        var path = Path.Combine(Path.GetTempPath(), $"freeagent-estimate-{estimate.ResourceId}.pdf");
        await File.WriteAllBytesAsync(path, pdfBytes, cancellationToken);

        SampleOutput.WriteHeader("Estimate PDF");
        SampleOutput.WriteField("Estimate ID", estimate.ResourceId);
        SampleOutput.WriteField("PDF path", path);
        SampleOutput.WriteField("Size (bytes)", pdfBytes.Length);
    }

    [ConsoleSample(Name = "Get estimate default additional text")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetDefaultAdditionalTextAsync(CancellationToken cancellationToken)
    {
        var text = await context.Client.Estimates.GetDefaultAdditionalTextAsync(cancellationToken);

        SampleOutput.WriteHeader("Default additional text");
        SampleOutput.WriteField("Text", string.IsNullOrWhiteSpace(text) ? "(not set)" : text);
    }

    [ConsoleSample(Name = "Update estimate comments", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateEstimateCommentsAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftEstimateAsync(cancellationToken);

        try
        {
            draft.Notes = "Updated by console sample";
            var updated = await context.Client.Estimates.UpdateEstimateAsync(
                draft.ResourceId,
                draft,
                new EstimateUpdateOptions { OmitLineItems = true },
                cancellationToken);

            SampleOutput.WriteHeader("Updated estimate comments");
            SampleOutput.WriteField("Id", updated.ResourceId);
            SampleOutput.WriteField("Notes", updated.Notes);
        }
        finally
        {
            await DeleteProbeEstimateAsync(draft.ResourceId, cancellationToken);
        }
    }

    [ConsoleSample(Name = "Mark estimate as draft", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkEstimateAsDraftAsync(CancellationToken cancellationToken)
    {
        var sent = await CreateSentSampleEstimateAsync(cancellationToken);

        try
        {
            var draft = await context.Client.Estimates.MarkEstimateAsDraftAsync(sent.ResourceId, cancellationToken);

            SampleOutput.WriteHeader("Marked estimate as draft");
            SampleOutput.WriteField("Id", draft.ResourceId);
            SampleOutput.WriteField("Status", draft.Status);
        }
        finally
        {
            await DeleteProbeEstimateAsync(sent.ResourceId, cancellationToken);
        }
    }

    [ConsoleSample(Name = "Mark estimate as approved", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkEstimateAsApprovedAsync(CancellationToken cancellationToken)
    {
        var sent = await CreateSentSampleEstimateAsync(cancellationToken);

        try
        {
            var approved = await context.Client.Estimates.MarkEstimateAsApprovedAsync(sent.ResourceId, cancellationToken);

            SampleOutput.WriteHeader("Marked estimate as approved");
            SampleOutput.WriteField("Id", approved.ResourceId);
            SampleOutput.WriteField("Status", approved.Status);
        }
        finally
        {
            await DeleteProbeEstimateAsync(sent.ResourceId, cancellationToken);
        }
    }

    [ConsoleSample(Name = "Mark estimate as rejected", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkEstimateAsRejectedAsync(CancellationToken cancellationToken)
    {
        var sent = await CreateSentSampleEstimateAsync(cancellationToken);

        try
        {
            var rejected = await context.Client.Estimates.MarkEstimateAsRejectedAsync(sent.ResourceId, cancellationToken);

            SampleOutput.WriteHeader("Marked estimate as rejected");
            SampleOutput.WriteField("Id", rejected.ResourceId);
            SampleOutput.WriteField("Status", rejected.Status);
        }
        finally
        {
            await DeleteProbeEstimateAsync(sent.ResourceId, cancellationToken);
        }
    }

    [ConsoleSample(Name = "Send estimate email (template)", ExcludeFromRunAll = true, MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task SendEstimateEmailAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftEstimateAsync(cancellationToken);

        try
        {
            await context.Client.Estimates.SendEstimateEmailAsync(
                draft.ResourceId,
                new SendEstimateEmailRequest
                {
                    Email = new EstimateEmailDetails { UseTemplate = true }
                },
                cancellationToken);

            SampleOutput.WriteHeader("Sent estimate email");
            SampleOutput.WriteField("Estimate ID", draft.ResourceId);
            SampleOutput.WriteField("Template", true);
        }
        finally
        {
            await DeleteProbeEstimateAsync(draft.ResourceId, cancellationToken);
        }
    }

    [ConsoleSample(Name = "Create estimate item", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateEstimateItemAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftEstimateAsync(cancellationToken);

        try
        {
            var item = await context.Client.Estimates.CreateEstimateItemAsync(
                draft.ResourceId,
                new EstimateItem
                {
                    Description = "Console sample additional line item",
                    ItemType = EstimateItemType.Services,
                    Quantity = 1,
                    Price = 50,
                    SalesTaxRate = 20,
                    SalesTaxStatus = InvoiceSalesTaxStatus.Taxable
                },
                cancellationToken);

            SampleOutput.WriteHeader("Created estimate item");
            SampleOutput.WriteField("Estimate ID", draft.ResourceId);
            SampleOutput.WriteField("Item ID", item.ItemId);
            SampleOutput.WriteField("Description", item.Description);
        }
        finally
        {
            await DeleteProbeEstimateAsync(draft.ResourceId, cancellationToken);
        }
    }

    [ConsoleSample(Name = "Update estimate item", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateEstimateItemAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftEstimateAsync(cancellationToken);

        try
        {
            var item = await context.Client.Estimates.CreateEstimateItemAsync(
                draft.ResourceId,
                new EstimateItem
                {
                    Description = "Console sample line item to update",
                    ItemType = EstimateItemType.Services,
                    Quantity = 1,
                    Price = 25,
                    SalesTaxRate = 20,
                    SalesTaxStatus = InvoiceSalesTaxStatus.Taxable
                },
                cancellationToken);

            if (item.ItemId is not > 0)
            {
                SampleContext.Skip("created estimate item has no ID");
            }

            var updatedItem = await context.Client.Estimates.UpdateEstimateItemAsync(
                item.ItemId.Value,
                new EstimateItem
                {
                    Description = "Console sample line item (updated)",
                    ItemType = EstimateItemType.Services,
                    Quantity = 2,
                    Price = 100,
                    SalesTaxRate = 20,
                    SalesTaxStatus = InvoiceSalesTaxStatus.Taxable
                },
                cancellationToken);

            SampleOutput.WriteHeader("Updated estimate item");
            SampleOutput.WriteField("Estimate ID", draft.ResourceId);
            SampleOutput.WriteField("Item ID", updatedItem.ItemId);
            SampleOutput.WriteField("Description", updatedItem.Description);
            SampleOutput.WriteField("Quantity", updatedItem.Quantity);
        }
        finally
        {
            await DeleteProbeEstimateAsync(draft.ResourceId, cancellationToken);
        }
    }

    [ConsoleSample(Name = "Delete estimate item", ExcludeFromRunAll = true, MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task DeleteEstimateItemAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftEstimateAsync(cancellationToken);

        try
        {
            var item = await context.Client.Estimates.CreateEstimateItemAsync(
                draft.ResourceId,
                new EstimateItem
                {
                    Description = "Console sample item to delete",
                    ItemType = EstimateItemType.Services,
                    Quantity = 1,
                    Price = 25,
                    SalesTaxRate = 20,
                    SalesTaxStatus = InvoiceSalesTaxStatus.Taxable
                },
                cancellationToken);

            if (item.ItemId is not > 0)
            {
                SampleContext.Skip("created estimate item has no ID");
            }

            await context.Client.Estimates.DeleteEstimateItemAsync(item.ItemId.Value, cancellationToken);

            SampleOutput.WriteHeader("Deleted estimate item");
            SampleOutput.WriteField("Estimate ID", draft.ResourceId);
            SampleOutput.WriteField("Deleted item ID", item.ItemId);
        }
        finally
        {
            await DeleteProbeEstimateAsync(draft.ResourceId, cancellationToken);
        }
    }

    [ConsoleSample(Name = "Update estimate default additional text", ExcludeFromRunAll = true, MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateDefaultAdditionalTextAsync(CancellationToken cancellationToken)
    {
        var previousText = await context.Client.Estimates.GetDefaultAdditionalTextAsync(cancellationToken);

        try
        {
            var text = await context.Client.Estimates.UpdateDefaultAdditionalTextAsync(
                "Valid for 30 days (console sample update).",
                cancellationToken);

            SampleOutput.WriteHeader("Updated default additional text");
            SampleOutput.WriteField("Text", text);
        }
        finally
        {
            await RestoreDefaultAdditionalTextAsync(previousText, cancellationToken);
        }
    }

    [ConsoleSample(Name = "Delete estimate default additional text", ExcludeFromRunAll = true, MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task DeleteDefaultAdditionalTextAsync(CancellationToken cancellationToken)
    {
        var previousText = await context.Client.Estimates.GetDefaultAdditionalTextAsync(cancellationToken);

        try
        {
            await context.Client.Estimates.DeleteDefaultAdditionalTextAsync(cancellationToken);

            SampleOutput.WriteHeader("Deleted default additional text");
            SampleOutput.WriteField("Result", "Deleted");
        }
        finally
        {
            await RestoreDefaultAdditionalTextAsync(previousText, cancellationToken);
        }
    }

    [ConsoleSample(Name = "Convert estimate to invoice", ExcludeFromRunAll = true, MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ConvertEstimateToInvoiceAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftEstimateAsync(cancellationToken);
        var converted = await context.Client.Estimates.ConvertToInvoiceAsync(draft.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Converted estimate to invoice");
        SampleOutput.WriteField("Estimate ID", draft.ResourceId);
        SampleOutput.WriteField("Invoice ID", converted.InvoiceId);
        SampleOutput.WriteField("Status", converted.Status);
    }

    [ConsoleSample(Name = "Duplicate probe estimate and delete", ExcludeFromRunAll = true, MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task DuplicateProbeEstimateAndDeleteAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftEstimateAsync(cancellationToken);
        var duplicate = await context.Client.Estimates.DuplicateEstimateAsync(draft.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Duplicated probe estimate");
        SampleOutput.WriteField("Source ID", draft.ResourceId);
        SampleOutput.WriteField("Duplicate ID", duplicate.ResourceId);
        SampleOutput.WriteField("Status", duplicate.Status);
        SampleOutput.WriteField("Reference", duplicate.Reference);

        await context.Client.Estimates.DeleteEstimateAsync(duplicate.ResourceId, cancellationToken);
        await context.Client.Estimates.DeleteEstimateAsync(draft.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Deleted probe estimates");
        SampleOutput.WriteField("Source ID", draft.ResourceId);
        SampleOutput.WriteField("Duplicate ID", duplicate.ResourceId);
    }

    private async Task<Estimate> CreateSampleDraftEstimateAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var nominalCode = await context.Data.GetFirstIncomeCategoryNominalCodeAsync(cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await context.Client.Estimates.CreateEstimateAsync(
            new Estimate
            {
                ContactId = contact.ResourceId,
                DatedOn = today,
                EstimateType = EstimateType.Estimate,
                Reference = $"Console sample {DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}",
                EstimateItems =
                [
                    new EstimateItem
                    {
                        Description = "Console sample line item",
                        ItemType = EstimateItemType.Services,
                        Quantity = 1,
                        Price = 100,
                        CategoryNominalCode = nominalCode,
                    }
                ]
            },
            cancellationToken);
    }

    private async Task<Estimate> CreateSentSampleEstimateAsync(CancellationToken cancellationToken)
    {
        var draft = await CreateSampleDraftEstimateAsync(cancellationToken);
        return await context.Client.Estimates.MarkEstimateAsSentAsync(draft.ResourceId, cancellationToken);
    }

    private async Task DeleteProbeEstimateAsync(long estimateId, CancellationToken cancellationToken)
    {
        try
        {
            await context.Client.Estimates.DeleteEstimateAsync(estimateId, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await context.Client.Estimates.MarkEstimateAsDraftAsync(estimateId, cancellationToken);
            await context.Client.Estimates.DeleteEstimateAsync(estimateId, cancellationToken);
        }
    }

    private async Task RestoreDefaultAdditionalTextAsync(string? previousText, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(previousText))
        {
            await context.Client.Estimates.DeleteDefaultAdditionalTextAsync(cancellationToken);
            return;
        }

        await context.Client.Estimates.UpdateDefaultAdditionalTextAsync(previousText, cancellationToken);
    }

    private static string FormatEstimateRow(Estimate estimate) =>
        $"{estimate.ResourceId,8}  {estimate.Reference ?? "-",-20}  {estimate.Status,-12}  {estimate.ClientContactName ?? estimate.Contact?.OrganisationName ?? "-"}";
}
