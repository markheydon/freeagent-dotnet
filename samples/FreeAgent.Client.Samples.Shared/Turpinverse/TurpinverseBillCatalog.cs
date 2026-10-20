namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Loads Turpinverse bill canon from the upstream GitHub repository.
/// </summary>
public sealed class TurpinverseBillCatalog : IDisposable
{
    public const string BrazierLegalBillId = "bill-003";

    private readonly TurpinverseCanonClient _canonClient;
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private IReadOnlyList<TurpinverseBill>? _bills;

    public TurpinverseBillCatalog(TurpinverseCanonClient canonClient)
    {
        _canonClient = canonClient ?? throw new ArgumentNullException(nameof(canonClient));
    }

    public async Task EnsureLoadedAsync(
        bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        if (!forceRefresh && _bills is not null)
        {
            return;
        }

        await _loadLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!forceRefresh && _bills is not null)
            {
                return;
            }

            if (forceRefresh)
            {
                _canonClient.ClearCache();
                _bills = null;
            }

            _bills = await _canonClient.LoadJsonAsync<TurpinverseBill[]>(
                "bills.json",
                cancellationToken) ?? [];
        }
        finally
        {
            _loadLock.Release();
        }
    }

    public IReadOnlyList<TurpinverseBill> Bills
    {
        get
        {
            if (_bills is null)
            {
                throw new InvalidOperationException("Call EnsureLoadedAsync before accessing Turpinverse bill canon data.");
            }

            return _bills;
        }
    }

    public TurpinverseBill BrazierLegalBill =>
        Bills.First(static bill => bill.BillId == BrazierLegalBillId);

    /// <summary>Clears in-memory canon without clearing the shared HTTP JSON cache.</summary>
    public void InvalidateInMemory() => _bills = null;

    public void Dispose() => _loadLock.Dispose();
}
