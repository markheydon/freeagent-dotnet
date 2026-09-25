namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Loads Turpinverse credit note canon from the upstream GitHub repository.
/// </summary>
public sealed class TurpinverseCreditNoteCatalog : IDisposable
{
    public const string HighwayCommissionCreditNoteId = "crn-002";

    private readonly TurpinverseCanonClient _canonClient;
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private IReadOnlyList<TurpinverseCreditNote>? _creditNotes;

    public TurpinverseCreditNoteCatalog(TurpinverseCanonClient canonClient)
    {
        _canonClient = canonClient ?? throw new ArgumentNullException(nameof(canonClient));
    }

    public async Task EnsureLoadedAsync(
        bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        if (!forceRefresh && _creditNotes is not null)
        {
            return;
        }

        await _loadLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!forceRefresh && _creditNotes is not null)
            {
                return;
            }

            if (forceRefresh)
            {
                _canonClient.ClearCache();
                _creditNotes = null;
            }

            _creditNotes = await _canonClient.LoadJsonAsync<TurpinverseCreditNote[]>(
                "credit-notes.json",
                cancellationToken) ?? [];
        }
        finally
        {
            _loadLock.Release();
        }
    }

    public IReadOnlyList<TurpinverseCreditNote> CreditNotes
    {
        get
        {
            if (_creditNotes is null)
            {
                throw new InvalidOperationException("Call EnsureLoadedAsync before accessing Turpinverse credit note canon data.");
            }

            return _creditNotes;
        }
    }

    public TurpinverseCreditNote HighwayCommissionCreditNote =>
        CreditNotes.First(static creditNote => creditNote.CreditNoteId == HighwayCommissionCreditNoteId);

    public void Dispose() => _loadLock.Dispose();
}
