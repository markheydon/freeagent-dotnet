namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Loads Turpinverse project canon from the upstream GitHub repository.
/// </summary>
public sealed class TurpinverseProjectCatalog : IDisposable
{
    public const string BlackBessRouteOptimiserProjectId = "black-bess-route-optimiser";

    private readonly TurpinverseCanonClient _canonClient;
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private IReadOnlyList<TurpinverseProject>? _projects;

    public TurpinverseProjectCatalog(TurpinverseCanonClient canonClient)
    {
        _canonClient = canonClient ?? throw new ArgumentNullException(nameof(canonClient));
    }

    public async Task EnsureLoadedAsync(
        bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        if (!forceRefresh && _projects is not null)
        {
            return;
        }

        await _loadLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!forceRefresh && _projects is not null)
            {
                return;
            }

            if (forceRefresh)
            {
                _canonClient.ClearCache();
                _projects = null;
            }

            _projects = await _canonClient.LoadJsonAsync<TurpinverseProject[]>(
                "projects.json",
                cancellationToken) ?? [];
        }
        finally
        {
            _loadLock.Release();
        }
    }

    public IReadOnlyList<TurpinverseProject> Projects
    {
        get
        {
            if (_projects is null)
            {
                throw new InvalidOperationException("Call EnsureLoadedAsync before accessing Turpinverse project canon data.");
            }

            return _projects;
        }
    }

    public TurpinverseProject BlackBessRouteOptimiser =>
        Projects.First(static project => project.Id == BlackBessRouteOptimiserProjectId);

    /// <summary>Clears in-memory canon without clearing the shared HTTP JSON cache.</summary>
    public void InvalidateInMemory() => _projects = null;

    public void Dispose() => _loadLock.Dispose();
}
