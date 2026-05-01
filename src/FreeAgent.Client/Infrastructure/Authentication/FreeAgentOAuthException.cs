namespace FreeAgent.Client.Infrastructure.Authentication;

/// <summary>
/// Exception thrown when OAuth authentication fails.
/// </summary>
public class FreeAgentOAuthException : Exception
{
    /// <summary>
    /// Initializes a new instance of the exception.
    /// </summary>
    /// <param name="message">Error message</param>
    public FreeAgentOAuthException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the exception with an inner exception.
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public FreeAgentOAuthException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
