using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeAgent.Client;

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
            _ => MileageUnit.Unknown
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, MileageUnit value, JsonSerializerOptions options)
    {
        var wireValue = value switch
        {
            MileageUnit.Miles => "miles",
            MileageUnit.Kilometers => "kilometers",
            MileageUnit.Unknown => throw new JsonException($"Cannot serialise unknown {nameof(MileageUnit)} value to a FreeAgent wire value."),
            _ => throw new JsonException($"Unsupported {nameof(MileageUnit)} value: {value}.")
        };

        writer.WriteStringValue(wireValue);
    }
}
