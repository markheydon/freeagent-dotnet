using System.Net;

namespace FreeAgent.Client.Infrastructure.Http;

/// <summary>
/// Exception thrown when a network-level failure occurs while calling the FreeAgent API.
/// </summary>
public class FreeAgentNetworkException : FreeAgentTransportException
{
    /// <summary>
    /// Initializes a new instance of the exception.
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="requestPath">Request path associated with the failure</param>
    /// <param name="attemptCount">Total attempt count when the failure occurred</param>
    /// <param name="statusCode">Last observed status code when available</param>
    /// <param name="innerException">Inner exception</param>
    public FreeAgentNetworkException(
        string message,
        string? requestPath = null,
        int attemptCount = 1,
        HttpStatusCode? statusCode = null,
        Exception? innerException = null)
        : base(message, requestPath, attemptCount, statusCode, innerException)
    {
    }
}
