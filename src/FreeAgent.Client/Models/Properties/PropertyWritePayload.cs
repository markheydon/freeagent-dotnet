using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Properties;

/// <summary>
/// Writable property attributes for create and update requests.
/// </summary>
internal sealed class PropertyWritePayload
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

    public static PropertyWritePayload FromCreate(CreatePropertyRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new PropertyWritePayload
        {
            Address1 = request.Address1,
            Address2 = request.Address2,
            Address3 = request.Address3,
            Town = request.Town,
            Region = request.Region,
            Postcode = request.Postcode
        };
    }

    public static PropertyWritePayload FromUpdate(UpdatePropertyRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new PropertyWritePayload
        {
            Address1 = request.Address1,
            Address2 = request.Address2,
            Address3 = request.Address3,
            Town = request.Town,
            Region = request.Region,
            Postcode = request.Postcode,
            Country = request.Country
        };
    }
}
