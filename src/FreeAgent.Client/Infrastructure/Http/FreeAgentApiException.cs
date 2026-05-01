using System.Net;

namespace FreeAgent.Client.Infrastructure.Http;

/// <summary>
/// Exception thrown when FreeAgent API requests fail.
/// </summary>
public class FreeAgentApiException : Exception
{
    /// <summary>
    /// Request path associated with the failure when available.
    /// </summary>
    public string? RequestPath { get; }

    /// <summary>
    /// Total number of attempts made before the failure surfaced.
    /// </summary>
    public int AttemptCount { get; }

    /// <summary>
    /// Last observed HTTP status code when available.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the exception.
    /// </summary>
    /// <param name="message">Error message</param>
    public FreeAgentApiException(string message) : base(message)
    {
        AttemptCount = 1;
    }

    /// <summary>
    /// Initializes a new instance of the exception with an inner exception.
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public FreeAgentApiException(string message, Exception innerException) : base(message, innerException)
    {
        AttemptCount = 1;
    }

    /// <summary>
    /// Initializes a new instance of the exception with request and retry metadata.
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="requestPath">Request path associated with the failure</param>
    /// <param name="attemptCount">Total attempt count when the failure occurred</param>
    /// <param name="statusCode">Last observed status code when available</param>
    /// <param name="innerException">Inner exception</param>
    public FreeAgentApiException(
        string message,
        string? requestPath,
        int attemptCount,
        HttpStatusCode? statusCode,
        Exception? innerException = null)
        : base(message, innerException)
    {
        RequestPath = requestPath;
        AttemptCount = attemptCount;
        StatusCode = statusCode;
    }
}
