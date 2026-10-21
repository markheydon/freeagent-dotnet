using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
namespace FreeAgent.Client.Services.Attachments;

/// <summary>
/// Service for interacting with FreeAgent attachments.
/// </summary>
/// <remarks>
/// Attachments are polymorphic: access level matches the parent resource (for example an expense or bank explanation).
/// </remarks>
public sealed class AttachmentsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the attachments service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal AttachmentsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Gets a single attachment by identifier.
    /// </summary>
    /// <param name="attachmentId">Attachment identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Attachment metadata and download URLs</returns>
    public async Task<Attachment> GetAttachmentAsync(long attachmentId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(attachmentId);

        var response = await _requestClient.GetAsync<AttachmentResponse>(
            $"attachments/{attachmentId}",
            cancellationToken);

        if (response.Attachment is null)
        {
            throw new FreeAgentApiException("Attachment data missing from API response");
        }

        return response.Attachment;
    }

    /// <summary>
    /// Deletes a single attachment by identifier.
    /// </summary>
    /// <param name="attachmentId">Attachment identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task DeleteAttachmentAsync(long attachmentId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(attachmentId);

        return _requestClient.DeleteAsync($"attachments/{attachmentId}", cancellationToken);
    }
}
