using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="CreditNoteReference"/> as a URI string.
/// </summary>
internal sealed class CreditNoteReferenceJsonConverter : JsonConverter<CreditNoteReference>
{
    public override CreditNoteReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Credit note reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing credit note reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Credit note reference URI cannot be null or empty.");
        }

        return CreditNoteReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, CreditNoteReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
