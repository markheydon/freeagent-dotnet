using System.Text.Json;

namespace FreeAgent.Client.Sample.Services.Turpinverse;

/// <summary>
/// Loads Turpinverse canon snapshots bundled with the sample app for contact seeding.
/// </summary>
public sealed class TurpinverseContactCatalog
{
    public const string RichardTurpinPersonaId = "dick-turpin";
    public const string RichardTurpinEmail = "richard.turpin@turpinverse.uk";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly Lazy<IReadOnlyList<TurpinversePersona>> _personas;
    private readonly Lazy<IReadOnlyDictionary<string, TurpinverseOrganisation>> _organisationsById;

    public TurpinverseContactCatalog(IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        _personas = new Lazy<IReadOnlyList<TurpinversePersona>>(() =>
            LoadJson<TurpinversePersona[]>(environment, "turpinverse-personas.json") ?? []);

        _organisationsById = new Lazy<IReadOnlyDictionary<string, TurpinverseOrganisation>>(() =>
            (LoadJson<TurpinverseOrganisation[]>(environment, "turpinverse-organisations.json") ?? [])
            .ToDictionary(static organisation => organisation.Id, StringComparer.Ordinal));
    }

    public IReadOnlyList<TurpinversePersona> Personas => _personas.Value;

    public TurpinversePersona RichardTurpin =>
        Personas.First(static persona => persona.Id == RichardTurpinPersonaId);

    public IReadOnlyDictionary<string, TurpinverseOrganisation> OrganisationsById => _organisationsById.Value;

    private static T? LoadJson<T>(IWebHostEnvironment environment, string fileName)
    {
        var path = Path.Combine(environment.ContentRootPath, "Data", fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Turpinverse canon file not found: {path}");
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }
}
