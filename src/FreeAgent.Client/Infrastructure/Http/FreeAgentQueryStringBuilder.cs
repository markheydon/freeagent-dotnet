using System.Text;

namespace FreeAgent.Client.Infrastructure.Http;

internal static class FreeAgentQueryStringBuilder
{
    public static string BuildEndpoint(
        string endpoint,
        IEnumerable<KeyValuePair<string, string>> queryParameters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);
        ArgumentNullException.ThrowIfNull(queryParameters);

        var builder = new StringBuilder(endpoint);
        var separator = endpoint.Contains('?', StringComparison.Ordinal) ? '&' : '?';

        foreach (var queryParameter in queryParameters)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(queryParameter.Key);
            if (queryParameter.Value is null)
            {
                throw new ArgumentException("Query parameter value cannot be null.", nameof(queryParameters));
            }

            builder.Append(separator);
            builder.Append(Uri.EscapeDataString(queryParameter.Key));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(queryParameter.Value));

            separator = '&';
        }

        return builder.ToString();
    }
}
