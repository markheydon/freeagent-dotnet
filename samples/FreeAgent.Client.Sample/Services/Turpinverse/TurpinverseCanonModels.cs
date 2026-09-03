using System.Text.Json.Serialization;

namespace FreeAgent.Client.Sample.Services.Turpinverse;

public sealed class TurpinversePersona
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("organisationIds")]
    public IReadOnlyList<string> OrganisationIds { get; set; } = [];

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
}

public sealed class TurpinverseOrganisation
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("tradingName")]
    public string TradingName { get; set; } = string.Empty;
}
