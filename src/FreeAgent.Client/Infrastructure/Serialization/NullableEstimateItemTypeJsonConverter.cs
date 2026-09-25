using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Estimates;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Deserialises nullable <see cref="EstimateItemType"/> values, treating blank strings as unset.
/// </summary>
internal sealed class NullableEstimateItemTypeJsonConverter : JsonConverter<EstimateItemType?>
{
    private static readonly JsonStringEnumMemberNameCompatibleConverter<EstimateItemType> EnumConverter = new();

    public override EstimateItemType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string when parsing {nameof(EstimateItemType)}.");
        }

        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var encoded = JsonSerializer.Serialize(value);
        var encodedBytes = System.Text.Encoding.UTF8.GetBytes(encoded);
        var enumReader = new Utf8JsonReader(encodedBytes);
        enumReader.Read();
        return EnumConverter.Read(ref enumReader, typeof(EstimateItemType), options);
    }

    public override void Write(Utf8JsonWriter writer, EstimateItemType? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        EnumConverter.Write(writer, value.Value, options);
    }
}
