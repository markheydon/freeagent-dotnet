using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises enums as JSON numbers using their underlying integral values.
/// </summary>
/// <typeparam name="TEnum">Enum type.</typeparam>
internal sealed class JsonInt32EnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException($"Cannot parse null as {typeof(TEnum).Name}.");
        }

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var value))
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }

        throw new JsonException($"Expected number when parsing {typeof(TEnum).Name}.");
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture));
    }
}
