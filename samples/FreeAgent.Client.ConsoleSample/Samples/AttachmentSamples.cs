using System.Diagnostics.CodeAnalysis;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Attachments endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Attachments")]
internal sealed class AttachmentSamples(SampleContext context) : IConsoleSampleProvider
{
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

    [ConsoleSample(Name = "Delete attachment", ExcludeFromRunAll = true, MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task DeleteAttachmentAsync(CancellationToken cancellationToken)
    {
        var attachmentId = await context.Data.GetFirstAttachmentIdAsync(cancellationToken);
        await context.Client.Attachments.DeleteAttachmentAsync(attachmentId, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader("Deleted attachment");
        SampleOutput.WriteField("Id", attachmentId);
    }
}
