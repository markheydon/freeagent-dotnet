namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Loads Turpinverse invoice canon from the upstream GitHub repository.
/// </summary>
public sealed class TurpinverseInvoiceCatalog : IDisposable
{
    public const string HighwayCommissionDraftInvoiceId = "inv-001";

    private readonly TurpinverseCanonClient _canonClient;
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private IReadOnlyList<TurpinverseInvoice>? _invoices;

    public TurpinverseInvoiceCatalog(TurpinverseCanonClient canonClient)
    {
        _canonClient = canonClient ?? throw new ArgumentNullException(nameof(canonClient));
    }

    public async Task EnsureLoadedAsync(
        bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        if (!forceRefresh && _invoices is not null)
        {
            return;
        }

        await _loadLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!forceRefresh && _invoices is not null)
            {
                return;
            }

            if (forceRefresh)
            {
                _canonClient.ClearCache();
                _invoices = null;
            }

            _invoices = await _canonClient.LoadJsonAsync<TurpinverseInvoice[]>(
                "invoices.json",
                cancellationToken) ?? [];
        }
        finally
        {
            _loadLock.Release();
        }
    }

    public IReadOnlyList<TurpinverseInvoice> Invoices
    {
        get
        {
            if (_invoices is null)
            {
                throw new InvalidOperationException("Call EnsureLoadedAsync before accessing Turpinverse invoice canon data.");
            }

            return _invoices;
        }
    }

    public TurpinverseInvoice HighwayCommissionDraft =>
        Invoices.First(static invoice => invoice.InvoiceId == HighwayCommissionDraftInvoiceId);

    public void Dispose() => _loadLock.Dispose();
}
