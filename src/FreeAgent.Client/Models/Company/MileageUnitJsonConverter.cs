using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Company;

/// <summary>
/// Handles mileage unit wire values, including US and UK spellings of kilometres.
/// </summary>
public sealed class MileageUnitJsonConverter : JsonConverter<MileageUnit>
{
    /// <inheritdoc />
    public override MileageUnit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string when parsing {nameof(MileageUnit)}.");
        }

        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new JsonException($"Cannot parse empty value for {nameof(MileageUnit)}.");
        }

        return value.ToLowerInvariant() switch
        {
            "miles" => MileageUnit.Miles,
            "kilometres" or "kilometers" => MileageUnit.Kilometers,
            _ => throw new JsonException($"Unknown {nameof(MileageUnit)} value: \"{value}\".")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, MileageUnit value, JsonSerializerOptions options)
    {
        var wireValue = value switch
        {
            MileageUnit.Miles => "miles",
            MileageUnit.Kilometers => "kilometers",
            _ => throw new JsonException($"Unsupported {nameof(MileageUnit)} value: {value}.")
        };

        writer.WriteStringValue(wireValue);
    }
}
