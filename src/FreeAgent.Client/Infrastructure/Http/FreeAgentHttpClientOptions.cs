namespace FreeAgent.Client.Infrastructure.Http;

/// <summary>
/// Configures retry and rate-limit behavior for <see cref="FreeAgentHttpClient"/>.
/// </summary>
public class FreeAgentHttpClientOptions
{
    /// <summary>
    /// Maximum number of retry attempts for transient failures after the initial request.
    /// </summary>
    public int MaxNetworkRetries { get; set; } = 2;

    /// <summary>
    /// Base delay used for exponential backoff when retrying transient failures.
    /// </summary>
    public TimeSpan BaseRetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Maximum delay allowed for a single retry backoff interval.
    /// </summary>
    public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Whether retry backoff should include jitter to reduce retry synchronization.
    /// </summary>
    public bool UseRetryJitter { get; set; } = true;

    /// <summary>
    /// Fractional jitter range applied to retry backoff (for example, 0.2 = +/-20%).
    /// </summary>
    public double RetryJitterFactor { get; set; } = 0.2d;

    /// <summary>
    /// Adds a minimum delay between outbound requests as a conservative baseline rate-limit guard.
    /// </summary>
    public TimeSpan MinimumRequestSpacing { get; set; } = TimeSpan.FromMilliseconds(1000);

    /// <summary>
    /// Additional HTTP methods to allow retry for, intended only when the caller guarantees operation safety.
    /// </summary>
    public ISet<string> AdditionalRetriableMethods { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    internal static bool IsMethodRetriableByDefault(HttpMethod method)
    {
        return method == HttpMethod.Get || method == HttpMethod.Delete;
    }

    internal bool IsMethodRetriable(HttpMethod method)
    {
        if (IsMethodRetriableByDefault(method))
        {
            return true;
        }

        return AdditionalRetriableMethods.Contains(method.Method);
    }
}
