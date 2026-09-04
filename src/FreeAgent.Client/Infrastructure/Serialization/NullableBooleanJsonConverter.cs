using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Deserialises nullable booleans from JSON booleans or common string forms returned by FreeAgent.
/// </summary>
internal sealed class NullableBooleanJsonConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:
                return true;

            case JsonTokenType.False:
                return false;

            case JsonTokenType.Null:
                return null;

            case JsonTokenType.Number:
                if (reader.TryGetInt32(out var number))
                {
                    return number switch
                    {
                        0 => false,
                        1 => true,
                        _ => throw new JsonException($"Unable to convert numeric value {number} to boolean.")
                    };
                }

                throw new JsonException("Unable to convert numeric JSON value to boolean.");

            case JsonTokenType.String:
                {
                    var text = reader.GetString();
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        return null;
                    }

                    if (bool.TryParse(text, out var parsed))
                    {
                        return parsed;
                    }

                    throw new JsonException($"Unable to convert \"{text}\" to boolean.");
                }

            default:
                throw new JsonException($"Unexpected token {reader.TokenType} when deserializing nullable boolean.");
        }
    }

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteBooleanValue(value.Value);
    }
}
