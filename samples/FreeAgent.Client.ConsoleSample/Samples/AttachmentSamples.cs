using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.Bills;
using FreeAgent.Client.Models.Expenses;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Attachments endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Attachments")]
internal sealed class AttachmentSamples(SampleContext context) : IConsoleSampleProvider
{
    /// <summary>Minimal 1×1 PNG for sandbox attachment upload probes.</summary>
    private const string ProbePngBase64 =
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==";

    [ConsoleSample(Name = "Get attachment detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetAttachmentDetailAsync(CancellationToken cancellationToken)
    {
        var attachmentId = await context.Data.GetFirstAttachmentIdAsync(cancellationToken);
        var attachment = await context.Client.Attachments.GetAttachmentAsync(
            attachmentId,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader("Attachment detail");
        SampleOutput.WriteField("Id", attachment.ResourceId);
        SampleOutput.WriteField("File name", attachment.FileName);
        SampleOutput.WriteField("Content type", attachment.ContentType);
        SampleOutput.WriteField("File size", attachment.FileSize);
        SampleOutput.WriteField("Expires at", attachment.ExpiresAt);
        SampleOutput.WriteField("Description", attachment.Description);
    }

    [ConsoleSample(Name = "Create expense with attachment, get and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateExpenseWithAttachmentGetAndDeleteAsync(CancellationToken cancellationToken)
    {
        var user = await context.Data.GetFirstUserAsync(cancellationToken);
        var nominalCode = await context.Data.GetFirstAdminExpensesCategoryNominalCodeAsync(cancellationToken);

        var created = await context.Client.Expenses.CreateExpenseAsync(
            new Expense
            {
                UserId = user.ResourceId,
                CategoryNominalCode = nominalCode,
                DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                GrossValue = -1m,
                Description = $"SDK probe attachment expense {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss}",
                AttachmentWrite = new ExpenseAttachmentWritePayload
                {
                    Data = ProbePngBase64,
                    FileName = "probe.png",
                    ContentType = BillAttachmentContentType.Png,
                    Description = "Console sample probe"
                }
            },
            cancellationToken);

        var expenseId = created.ResourceId;
        var expenseDeleted = false;
        try
        {
            var detail = await context.Client.Expenses.GetExpenseAsync(expenseId, cancellationToken: cancellationToken);
            if (detail.Attachment?.Url is null)
            {
                SampleContext.Skip("created expense has no attachment URL in response");
            }

            var attachmentId = AttachmentReference.Parse(detail.Attachment.Url).Id;
            var attachment = await context.Client.Attachments.GetAttachmentAsync(attachmentId, cancellationToken: cancellationToken);

            SampleOutput.WriteHeader("Created expense with attachment");
            SampleOutput.WriteField("Expense ID", expenseId);
            SampleOutput.WriteField("Attachment ID", attachment.ResourceId);
            SampleOutput.WriteField("File name", attachment.FileName);

            await context.Client.Attachments.DeleteAttachmentAsync(attachmentId, cancellationToken: cancellationToken);
            await context.Client.Expenses.DeleteExpenseAsync(expenseId, cancellationToken);
            expenseDeleted = true;

            SampleOutput.WriteHeader("Deleted attachment and expense");
            SampleOutput.WriteField("Attachment ID", attachmentId);
            SampleOutput.WriteField("Expense ID", expenseId);
        }
        finally
        {
            if (!expenseDeleted)
            {
                await context.Client.Expenses.DeleteExpenseAsync(expenseId, cancellationToken);
            }
        }
    }
}
