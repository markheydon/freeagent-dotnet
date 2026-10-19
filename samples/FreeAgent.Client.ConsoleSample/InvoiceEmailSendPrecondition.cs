using System.Net;
using FreeAgent.Client;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Detects sandbox limits that prevent the "send invoice email (template)" console sample from succeeding.
/// </summary>
internal static class InvoiceEmailSendPrecondition
{
    internal static bool IsFailure(FreeAgentApiException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return exception.StatusCode is HttpStatusCode.UnprocessableEntity
            && exception.Message.Contains("exceed your allowance", StringComparison.OrdinalIgnoreCase)
            && exception.Message.Contains("email", StringComparison.OrdinalIgnoreCase);
    }
}
