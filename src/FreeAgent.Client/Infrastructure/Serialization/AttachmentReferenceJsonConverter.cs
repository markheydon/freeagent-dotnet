using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="AttachmentReference"/> as a URI string.
/// </summary>
internal sealed class AttachmentReferenceJsonConverter : JsonConverter<AttachmentReference>
{
    public override AttachmentReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Attachment reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing attachment reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Attachment reference URI cannot be null or empty.");
        }

        return AttachmentReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, AttachmentReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
