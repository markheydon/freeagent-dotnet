using System.Net;

namespace FreeAgent.Client.Infrastructure.Http;

/// <summary>
/// Exception thrown when rate limit is exceeded.
/// </summary>
public class FreeAgentRateLimitException : FreeAgentApiException
{
    /// <summary>
    /// Delay to wait before retrying, when provided by the API.
    /// </summary>
    public TimeSpan? RetryAfter { get; }

    /// <summary>
    /// Initializes a new instance of the exception.
    /// </summary>
    /// <param name="message">Error message</param>
    public FreeAgentRateLimitException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the exception with an inner exception.
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public FreeAgentRateLimitException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the exception with retry and request metadata.
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="requestPath">Request path associated with the failure</param>
    /// <param name="attemptCount">Total attempt count when the failure occurred</param>
    /// <param name="statusCode">Last observed status code</param>
    /// <param name="retryAfter">Delay to wait before retrying</param>
    /// <param name="innerException">Inner exception</param>
    public FreeAgentRateLimitException(
        string message,
        string? requestPath,
        int attemptCount,
        HttpStatusCode? statusCode,
        TimeSpan? retryAfter,
        Exception? innerException = null)
        : base(message, requestPath, attemptCount, statusCode, innerException)
    {
        RetryAfter = retryAfter;
    }
}
