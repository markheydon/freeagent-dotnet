namespace FreeAgent.Client.Http;

/// <summary>
/// Exception thrown when rate limit is exceeded.
/// </summary>
public class FreeAgentRateLimitException : FreeAgentApiException
{
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
}
