using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="ContactReference"/> as a URI string.
/// </summary>
internal sealed class ContactReferenceJsonConverter : JsonConverter<ContactReference>
{
    public override ContactReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Contact reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing contact reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Contact reference URI cannot be null or empty.");
        }

        return ContactReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, ContactReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
