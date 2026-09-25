namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Loads Turpinverse organisation and persona canon from the upstream GitHub repository.
/// </summary>
public sealed class TurpinverseContactCatalog : IDisposable
{
    public const string TurpinEnterprisesOrganisationId = "turpin-enterprises";

    private readonly TurpinverseCanonClient _canonClient;
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private IReadOnlyList<TurpinverseOrganisation>? _organisations;
    private IReadOnlyDictionary<string, TurpinversePersona>? _personasById;

    public TurpinverseContactCatalog(TurpinverseCanonClient canonClient)
    {
        _canonClient = canonClient ?? throw new ArgumentNullException(nameof(canonClient));
    }

    public async Task EnsureLoadedAsync(
        bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        if (!forceRefresh && _organisations is not null)
        {
            return;
        }

        await _loadLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!forceRefresh && _organisations is not null)
            {
                return;
            }

            if (forceRefresh)
            {
                _canonClient.ClearCache();
                _organisations = null;
                _personasById = null;
            }

            var organisations = await _canonClient.LoadJsonAsync<TurpinverseOrganisation[]>(
                "organisations.json",
                cancellationToken) ?? [];

            var personas = await _canonClient.LoadJsonAsync<TurpinversePersona[]>(
                "personas.json",
                cancellationToken) ?? [];

            _organisations = organisations;
            _personasById = personas.ToDictionary(static persona => persona.Id, StringComparer.Ordinal);
        }
        finally
        {
            _loadLock.Release();
        }
    }

    public IReadOnlyList<TurpinverseOrganisation> Organisations
    {
        get
        {
            ThrowIfNotLoaded();
            return _organisations!;
        }
    }

    public IReadOnlyDictionary<string, TurpinversePersona> PersonasById
    {
        get
        {
            ThrowIfNotLoaded();
            return _personasById!;
        }
    }

    public TurpinverseOrganisation TurpinEnterprises =>
        Organisations.First(static organisation => organisation.Id == TurpinEnterprisesOrganisationId);

    private void ThrowIfNotLoaded()
    {
        if (_organisations is null || _personasById is null)
        {
            throw new InvalidOperationException("Call EnsureLoadedAsync before accessing Turpinverse canon data.");
        }
    }

    /// <summary>Clears in-memory canon without clearing the shared HTTP JSON cache.</summary>
    public void InvalidateInMemory()
    {
        _organisations = null;
        _personasById = null;
    }

    public void Dispose() => _loadLock.Dispose();
}
