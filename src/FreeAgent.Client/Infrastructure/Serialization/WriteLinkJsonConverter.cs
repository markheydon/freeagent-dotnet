using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="WriteLink{T}"/> as a URI string, JSON null when cleared, or omits the parent property when null.
/// </summary>
internal sealed class WriteLinkJsonConverter<T> : JsonConverter<WriteLink<T>?>
    where T : struct
{
    public override WriteLink<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException("WriteLink is write-only.");

    public override void Write(Utf8JsonWriter writer, WriteLink<T>? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            throw new JsonException("WriteLink value must not be null when serialising.");
        }

        if (value.IsCleared)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value.Value, options);
    }
}
