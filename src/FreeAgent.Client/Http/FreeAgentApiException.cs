namespace FreeAgent.Client.Http;

/// <summary>
/// Exception thrown when FreeAgent API requests fail.
/// </summary>
public class FreeAgentApiException : Exception
{
    /// <summary>
    /// Initializes a new instance of the exception.
    /// </summary>
    /// <param name="message">Error message</param>
    public FreeAgentApiException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the exception with an inner exception.
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public FreeAgentApiException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
