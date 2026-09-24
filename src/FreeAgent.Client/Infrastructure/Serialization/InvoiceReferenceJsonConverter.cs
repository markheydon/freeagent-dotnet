using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="InvoiceReference"/> as a URI string.
/// </summary>
internal sealed class InvoiceReferenceJsonConverter : JsonConverter<InvoiceReference>
{
    public override InvoiceReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Invoice reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing invoice reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Invoice reference URI cannot be null or empty.");
        }

        return InvoiceReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, InvoiceReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
