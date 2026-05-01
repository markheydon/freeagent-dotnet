using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace FreeAgent.Client.Models.Company;

/// <summary>
/// Represents a sales tax rate payload.
/// </summary>
[JsonConverter(typeof(SalesTaxRateJsonConverter))]
public class SalesTaxRate
{
    /// <summary>
    /// Tax rate name when present.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Tax percentage/rate when present.
    /// </summary>
    [JsonPropertyName("rate")]
    public decimal? Rate { get; set; }

    /// <summary>
    /// Captures additional fields so the SDK remains forward-compatible with FreeAgent payload changes.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalData { get; set; } = new();
}

internal sealed class SalesTaxRateJsonConverter : JsonConverter<SalesTaxRate>
{
    public override SalesTaxRate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                return new SalesTaxRate
                {
                    Rate = reader.GetDecimal()
                };

            case JsonTokenType.String:
            {
                var stringValue = reader.GetString();
                if (decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedRate))
                {
                    return new SalesTaxRate
                    {
                        Rate = parsedRate
                    };
                }

                return new SalesTaxRate
                {
                    Name = stringValue
                };
            }

            case JsonTokenType.StartObject:
            {
                using var document = JsonDocument.ParseValue(ref reader);
                var model = new SalesTaxRate();

                foreach (var property in document.RootElement.EnumerateObject())
                {
                    if (property.NameEquals("name") && property.Value.ValueKind == JsonValueKind.String)
                    {
                        model.Name = property.Value.GetString();
                        continue;
                    }

                    if (property.NameEquals("rate"))
                    {
                        if (property.Value.ValueKind == JsonValueKind.Number)
                        {
                            model.Rate = property.Value.GetDecimal();
                            continue;
                        }

                        if (property.Value.ValueKind == JsonValueKind.String)
                        {
                            var rateText = property.Value.GetString();
                            if (decimal.TryParse(rateText, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedRate))
                            {
                                model.Rate = parsedRate;
                                continue;
                            }
                        }
                    }

                    model.AdditionalData[property.Name] = property.Value.Clone();
                }

                return model;
            }

            case JsonTokenType.Null:
                return new SalesTaxRate();

            default:
                throw new JsonException($"Unexpected token {reader.TokenType} when deserializing SalesTaxRate.");
        }
    }

    public override void Write(Utf8JsonWriter writer, SalesTaxRate value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (!string.IsNullOrWhiteSpace(value.Name))
        {
            writer.WriteString("name", value.Name);
        }

        if (value.Rate.HasValue)
        {
            writer.WriteNumber("rate", value.Rate.Value);
        }

        foreach (var additionalProperty in value.AdditionalData)
        {
            writer.WritePropertyName(additionalProperty.Key);
            additionalProperty.Value.WriteTo(writer);
        }

        writer.WriteEndObject();
    }
}
