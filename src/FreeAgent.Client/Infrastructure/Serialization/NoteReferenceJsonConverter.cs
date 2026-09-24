using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="NoteReference"/> as a URI string.
/// </summary>
internal sealed class NoteReferenceJsonConverter : JsonConverter<NoteReference>
{
    public override NoteReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Note reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing note reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Note reference URI cannot be null or empty.");
        }

        return NoteReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, NoteReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
