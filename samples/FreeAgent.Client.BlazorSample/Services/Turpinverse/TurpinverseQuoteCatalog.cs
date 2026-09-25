namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Loads Turpinverse quote canon from the upstream GitHub repository.
/// </summary>
public sealed class TurpinverseQuoteCatalog : IDisposable
{
    public const string HighwayCommissionDraftQuoteId = "quote-001";

    private readonly TurpinverseCanonClient _canonClient;
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private IReadOnlyList<TurpinverseQuote>? _quotes;

    public TurpinverseQuoteCatalog(TurpinverseCanonClient canonClient)
    {
        _canonClient = canonClient ?? throw new ArgumentNullException(nameof(canonClient));
    }

    public async Task EnsureLoadedAsync(
        bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        if (!forceRefresh && _quotes is not null)
        {
            return;
        }

        await _loadLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!forceRefresh && _quotes is not null)
            {
                return;
            }

            if (forceRefresh)
            {
                _canonClient.ClearCache();
                _quotes = null;
            }

            _quotes = await _canonClient.LoadJsonAsync<TurpinverseQuote[]>(
                "quotes.json",
                cancellationToken) ?? [];
        }
        finally
        {
            _loadLock.Release();
        }
    }

    public IReadOnlyList<TurpinverseQuote> Quotes
    {
        get
        {
            if (_quotes is null)
            {
                throw new InvalidOperationException("Call EnsureLoadedAsync before accessing Turpinverse quote canon data.");
            }

            return _quotes;
        }
    }

    public TurpinverseQuote HighwayCommissionDraft =>
        Quotes.First(static quote => quote.QuoteId == HighwayCommissionDraftQuoteId);

    public void Dispose() => _loadLock.Dispose();
}
