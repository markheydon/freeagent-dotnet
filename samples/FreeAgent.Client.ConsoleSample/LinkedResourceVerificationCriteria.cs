using System.Net;
using FreeAgent.Client;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Detects expected API validation responses during linked-resource write verification (GitHub issue #134).
/// </summary>
internal static class LinkedResourceVerificationCriteria
{
    /// <summary>
    /// Returns whether the API rejected a project update with a blank or null billing contact.
    /// </summary>
    internal static bool IsProjectContactIdBlankRejection(FreeAgentApiException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        if (exception.StatusCode is not (HttpStatusCode.UnprocessableEntity or HttpStatusCode.BadRequest))
        {
            return false;
        }

        var message = exception.Message;

        return (message.Contains("contact", StringComparison.OrdinalIgnoreCase)
                || message.Contains("contact_id", StringComparison.OrdinalIgnoreCase))
            && (message.Contains("blank", StringComparison.OrdinalIgnoreCase)
                || message.Contains("can't be", StringComparison.OrdinalIgnoreCase)
                || message.Contains("cannot be", StringComparison.OrdinalIgnoreCase));
    }
}
