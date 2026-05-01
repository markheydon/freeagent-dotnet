using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Company;

/// <summary>
/// Represents company sales tax registration status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(SalesTaxRegistrationStatusJsonConverter))]
public enum SalesTaxRegistrationStatus
{
    /// <summary>Registered for sales tax (wire value: "Registered").</summary>
    Registered,

    /// <summary>Deregistered for sales tax (wire value: "De-registered").</summary>
    Deregistered,

    /// <summary>Unregistered for sales tax (wire value: "Unregistered").</summary>
    Unregistered,

    /// <summary>Fallback for undocumented sales tax status values.</summary>
    Unknown
}

/// <summary>
/// Handles known and undocumented sales tax status wire values without throwing.
/// </summary>
public sealed class SalesTaxRegistrationStatusJsonConverter : JsonConverter<SalesTaxRegistrationStatus>
{
    /// <inheritdoc />
    public override SalesTaxRegistrationStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            return SalesTaxRegistrationStatus.Unknown;
        }

        var raw = reader.GetString();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return SalesTaxRegistrationStatus.Unknown;
        }

        var normalized = raw.Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Trim()
            .ToLowerInvariant();

        return normalized switch
        {
            "registered" => SalesTaxRegistrationStatus.Registered,
            "deregistered" => SalesTaxRegistrationStatus.Deregistered,
            "unregistered" => SalesTaxRegistrationStatus.Unregistered,
            "notregistered" => SalesTaxRegistrationStatus.Unregistered,
            _ => SalesTaxRegistrationStatus.Unknown
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, SalesTaxRegistrationStatus value, JsonSerializerOptions options)
    {
        var wireValue = value switch
        {
            SalesTaxRegistrationStatus.Registered => "Registered",
            SalesTaxRegistrationStatus.Deregistered => "De-registered",
            SalesTaxRegistrationStatus.Unregistered => "Unregistered",
            _ => "Unknown"
        };

        writer.WriteStringValue(wireValue);
    }
}
