using System.Text.Json.Serialization;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

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

    [JsonPropertyName("address")]
    public TurpinverseAddress? Address { get; set; }
}

public sealed class TurpinverseAddress
{
    [JsonPropertyName("address1")]
    public string? Address1 { get; set; }

    [JsonPropertyName("address2")]
    public string? Address2 { get; set; }

    [JsonPropertyName("address3")]
    public string? Address3 { get; set; }

    [JsonPropertyName("town")]
    public string? Town { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("postcode")]
    public string? Postcode { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }
}

public sealed class TurpinverseOrganisation
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("tradingName")]
    public string TradingName { get; set; } = string.Empty;

    [JsonPropertyName("registeredOffice")]
    public TurpinverseAddress? RegisteredOffice { get; set; }
}
