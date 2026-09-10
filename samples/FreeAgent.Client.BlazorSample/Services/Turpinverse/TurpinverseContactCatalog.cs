using System.Text.Json;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Loads Turpinverse canon snapshots bundled with the sample app for contact seeding.
/// </summary>
public sealed class TurpinverseContactCatalog
{
    public const string TurpinEnterprisesOrganisationId = "turpin-enterprises";
    public const string RichardTurpinPersonaId = "dick-turpin";
    public const string RichardTurpinEmail = "richard.turpin@turpinverse.uk";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly Lazy<IReadOnlyList<TurpinverseOrganisation>> _organisations;
    private readonly Lazy<IReadOnlyDictionary<string, TurpinversePersona>> _personasById;

    public TurpinverseContactCatalog(IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        _organisations = new Lazy<IReadOnlyList<TurpinverseOrganisation>>(() =>
            LoadJson<TurpinverseOrganisation[]>(environment, "organisations.json") ?? []);

        _personasById = new Lazy<IReadOnlyDictionary<string, TurpinversePersona>>(() =>
            (LoadJson<TurpinversePersona[]>(environment, "personas.json") ?? [])
            .ToDictionary(static persona => persona.Id, StringComparer.Ordinal));
    }

    public IReadOnlyList<TurpinverseOrganisation> Organisations => _organisations.Value;

    public IReadOnlyDictionary<string, TurpinversePersona> PersonasById => _personasById.Value;

    public TurpinverseOrganisation TurpinEnterprises =>
        Organisations.First(static organisation => organisation.Id == TurpinEnterprisesOrganisationId);

    public TurpinversePersona RichardTurpin => PersonasById[RichardTurpinPersonaId];

    private static T? LoadJson<T>(IWebHostEnvironment environment, string fileName)
    {
        var path = Path.Combine(environment.ContentRootPath, "canon", fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Turpinverse canon file not found: {path}");
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }
}
