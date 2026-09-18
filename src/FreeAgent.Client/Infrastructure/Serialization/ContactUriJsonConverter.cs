using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Deserialises a contact field that may be either a resource URI string or a nested contact object.
/// </summary>
internal sealed class ContactUriJsonConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            var contact = JsonSerializer.Deserialize<Contact>(ref reader, options);
            return contact?.Url;
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing contact URI.");
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value);
    }
}
