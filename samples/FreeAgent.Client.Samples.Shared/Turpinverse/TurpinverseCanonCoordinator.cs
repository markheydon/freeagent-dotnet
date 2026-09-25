namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Loads all Turpinverse canon files once per bulk seed run.
/// </summary>
public sealed class TurpinverseCanonCoordinator
{
    private readonly TurpinverseCanonClient _canonClient;
    private readonly TurpinverseContactCatalog _contactCatalog;
    private readonly TurpinverseProjectCatalog _projectCatalog;
    private readonly TurpinverseInvoiceCatalog _invoiceCatalog;
    private readonly TurpinverseQuoteCatalog _quoteCatalog;
    private readonly TurpinverseCreditNoteCatalog _creditNoteCatalog;

    public TurpinverseCanonCoordinator(
        TurpinverseCanonClient canonClient,
        TurpinverseContactCatalog contactCatalog,
        TurpinverseProjectCatalog projectCatalog,
        TurpinverseInvoiceCatalog invoiceCatalog,
        TurpinverseQuoteCatalog quoteCatalog,
        TurpinverseCreditNoteCatalog creditNoteCatalog)
    {
        _canonClient = canonClient ?? throw new ArgumentNullException(nameof(canonClient));
        _contactCatalog = contactCatalog ?? throw new ArgumentNullException(nameof(contactCatalog));
        _projectCatalog = projectCatalog ?? throw new ArgumentNullException(nameof(projectCatalog));
        _invoiceCatalog = invoiceCatalog ?? throw new ArgumentNullException(nameof(invoiceCatalog));
        _quoteCatalog = quoteCatalog ?? throw new ArgumentNullException(nameof(quoteCatalog));
        _creditNoteCatalog = creditNoteCatalog ?? throw new ArgumentNullException(nameof(creditNoteCatalog));
    }

    /// <summary>
    /// Clears cached canon JSON and loads every catalog required by the bulk seed orchestrator.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task PrepareForBulkSeedAsync(CancellationToken cancellationToken = default)
    {
        _canonClient.ClearCache();
        _contactCatalog.InvalidateInMemory();
        _projectCatalog.InvalidateInMemory();
        _invoiceCatalog.InvalidateInMemory();
        _quoteCatalog.InvalidateInMemory();
        _creditNoteCatalog.InvalidateInMemory();

        await _contactCatalog.EnsureLoadedAsync(forceRefresh: false, cancellationToken).ConfigureAwait(false);
        await _projectCatalog.EnsureLoadedAsync(forceRefresh: false, cancellationToken).ConfigureAwait(false);
        await _invoiceCatalog.EnsureLoadedAsync(forceRefresh: false, cancellationToken).ConfigureAwait(false);
        await _quoteCatalog.EnsureLoadedAsync(forceRefresh: false, cancellationToken).ConfigureAwait(false);
        await _creditNoteCatalog.EnsureLoadedAsync(forceRefresh: false, cancellationToken).ConfigureAwait(false);
    }
}
