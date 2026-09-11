namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Loads Turpinverse organisation and persona canon from the upstream GitHub repository.
/// </summary>
public sealed class TurpinverseContactCatalog : IDisposable
{
    public const string TurpinEnterprisesOrganisationId = "turpin-enterprises";
    public const string RichardTurpinPersonaId = "dick-turpin";
    public const string RichardTurpinEmail = "richard.turpin@turpinverse.uk";

    private readonly TurpinverseCanonClient _canonClient;
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private IReadOnlyList<TurpinverseOrganisation>? _organisations;
    private IReadOnlyDictionary<string, TurpinversePersona>? _personasById;

    public TurpinverseContactCatalog(TurpinverseCanonClient canonClient)
    {
        _canonClient = canonClient ?? throw new ArgumentNullException(nameof(canonClient));
    }

    public async Task EnsureLoadedAsync(CancellationToken cancellationToken = default)
    {
        if (_organisations is not null)
        {
            return;
        }

        await _loadLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_organisations is not null)
            {
                return;
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

    public TurpinversePersona RichardTurpin => PersonasById[RichardTurpinPersonaId];

    private void ThrowIfNotLoaded()
    {
        if (_organisations is null || _personasById is null)
        {
            throw new InvalidOperationException("Call EnsureLoadedAsync before accessing Turpinverse canon data.");
        }
    }

    public void Dispose() => _loadLock.Dispose();
}
