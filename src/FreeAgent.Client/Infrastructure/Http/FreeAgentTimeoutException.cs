using System.Net;

namespace FreeAgent.Client.Infrastructure.Http;

/// <summary>
/// Exception thrown when a request to the FreeAgent API times out.
/// </summary>
public class FreeAgentTimeoutException : FreeAgentTransportException
{
    /// <summary>
    /// Initializes a new instance of the exception.
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="requestPath">Request path associated with the failure</param>
    /// <param name="attemptCount">Total attempt count when the failure occurred</param>
    /// <param name="statusCode">Last observed status code when available</param>
    /// <param name="innerException">Inner exception</param>
    public FreeAgentTimeoutException(
        string message,
        string? requestPath = null,
        int attemptCount = 1,
        HttpStatusCode? statusCode = null,
        Exception? innerException = null)
        : base(message, requestPath, attemptCount, statusCode, innerException)
    {
    }
}
