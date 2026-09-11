using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Loads Turpinverse canon JSON files from the upstream GitHub repository on demand.
/// </summary>
public sealed class TurpinverseCanonClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly TurpinverseCanonOptions _options;
    private readonly ConcurrentDictionary<string, string> _jsonCache = new(StringComparer.Ordinal);

    public TurpinverseCanonClient(HttpClient httpClient, IOptions<TurpinverseCanonOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<T?> LoadJsonAsync<T>(string fileName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var json = await LoadRawJsonAsync(fileName, cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    private async Task<string> LoadRawJsonAsync(string fileName, CancellationToken cancellationToken)
    {
        if (_jsonCache.TryGetValue(fileName, out var cached))
        {
            return cached;
        }

        var repository = string.IsNullOrWhiteSpace(_options.Repository)
            ? "markheydon/turpinverse"
            : _options.Repository.Trim();
        var canonRef = string.IsNullOrWhiteSpace(_options.CanonRef)
            ? "main"
            : _options.CanonRef.Trim();

        var url = $"https://raw.githubusercontent.com/{repository}/{canonRef}/canon/{fileName}";
        using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        _jsonCache[fileName] = json;
        return json;
    }
}
