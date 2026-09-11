using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Loads Turpinverse canon JSON files from the upstream GitHub repository on demand,
/// with optional bundled fallback for offline development.
/// </summary>
public sealed class TurpinverseCanonClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string FallbackDirectoryName = "canon-fallback";

    private readonly HttpClient _httpClient;
    private readonly IWebHostEnvironment _environment;
    private readonly TurpinverseCanonOptions _options;
    private readonly ConcurrentDictionary<string, string> _jsonCache = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _loadLocks = new(StringComparer.Ordinal);

    public TurpinverseCanonClient(
        HttpClient httpClient,
        IWebHostEnvironment environment,
        IOptions<TurpinverseCanonOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<T?> LoadJsonAsync<T>(string fileName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var json = await LoadRawJsonAsync(fileName, cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    public void ClearCache()
    {
        _jsonCache.Clear();
    }

    private async Task<string> LoadRawJsonAsync(string fileName, CancellationToken cancellationToken)
    {
        var cacheKey = BuildCacheKey(fileName);
        if (_jsonCache.TryGetValue(cacheKey, out var cached))
        {
            return cached;
        }

        var loadLock = _loadLocks.GetOrAdd(cacheKey, static _ => new SemaphoreSlim(1, 1));
        await loadLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_jsonCache.TryGetValue(cacheKey, out cached))
            {
                return cached;
            }

            var json = await LoadRawJsonUncachedAsync(fileName, cancellationToken).ConfigureAwait(false);
            _jsonCache[cacheKey] = json;
            return json;
        }
        finally
        {
            loadLock.Release();
        }
    }

    private async Task<string> LoadRawJsonUncachedAsync(string fileName, CancellationToken cancellationToken)
    {
        if (_options.PreferLocalFallback
            && TryReadLocalFallback(fileName, out var preferredLocalJson))
        {
            return preferredLocalJson;
        }

        var url = BuildGitHubRawUrl(fileName);
        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            }

            if (TryReadLocalFallback(fileName, out var fallbackAfterHttpError))
            {
                return fallbackAfterHttpError;
            }

            throw CreateFetchException(
                fileName,
                url,
                $"GitHub returned {(int)response.StatusCode} {response.StatusCode}.");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            if (TryReadLocalFallback(fileName, out var fallbackJson))
            {
                return fallbackJson;
            }

            throw CreateFetchException(
                fileName,
                url,
                "Could not reach GitHub and no bundled fallback canon is available.",
                ex);
        }
    }

    private string BuildCacheKey(string fileName)
    {
        var repository = ResolveRepository();
        var canonRef = ResolveCanonRef();
        return $"{repository}:{canonRef}:{fileName}";
    }

    private string BuildGitHubRawUrl(string fileName)
    {
        var repository = ResolveRepository();
        var canonRef = ResolveCanonRef();
        return $"https://raw.githubusercontent.com/{repository}/{canonRef}/canon/{fileName}";
    }

    private string ResolveRepository() =>
        string.IsNullOrWhiteSpace(_options.Repository)
            ? "markheydon/turpinverse"
            : _options.Repository.Trim();

    private string ResolveCanonRef() =>
        string.IsNullOrWhiteSpace(_options.CanonRef)
            ? "main"
            : _options.CanonRef.Trim();

    private bool TryReadLocalFallback(string fileName, out string json)
    {
        var path = Path.Combine(_environment.ContentRootPath, "Data", FallbackDirectoryName, fileName);
        if (!File.Exists(path))
        {
            json = string.Empty;
            return false;
        }

        json = File.ReadAllText(path);
        return true;
    }

    private static TurpinverseCanonFetchException CreateFetchException(
        string fileName,
        string url,
        string message,
        Exception? innerException = null) =>
        new($"Failed to load Turpinverse canon file '{fileName}' from '{url}'. {message}", innerException);
}

public sealed class TurpinverseCanonFetchException : Exception
{
    public TurpinverseCanonFetchException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
