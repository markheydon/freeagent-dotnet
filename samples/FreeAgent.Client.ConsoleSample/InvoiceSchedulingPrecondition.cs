using System.Net;
using FreeAgent.Client;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Detects sandbox preconditions that prevent the "mark invoice as scheduled" console sample from succeeding.
/// </summary>
internal static class InvoiceSchedulingPrecondition
{
    internal static bool IsFailure(FreeAgentApiException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        if (exception.StatusCode is not (HttpStatusCode.Forbidden or HttpStatusCode.UnprocessableEntity))
        {
            return false;
        }

        if (exception.Message.Contains("cannot be marked as scheduled", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (IsMissingInvoiceEmailTemplateFailure(exception))
        {
            return true;
        }

        return exception.RequestPath?.Contains("mark_as_scheduled", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool IsMissingInvoiceEmailTemplateFailure(FreeAgentApiException exception) =>
        exception.Message.Contains("email template", StringComparison.OrdinalIgnoreCase)
        && exception.Message.Contains("send_new_invoice_emails", StringComparison.OrdinalIgnoreCase)
        && exception.RequestPath?.Contains("invoices", StringComparison.OrdinalIgnoreCase) == true;
}
