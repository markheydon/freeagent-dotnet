using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="RecurringInvoiceReference"/> as a URI string.
/// </summary>
internal sealed class RecurringInvoiceReferenceJsonConverter : JsonConverter<RecurringInvoiceReference>
{
    public override RecurringInvoiceReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Recurring invoice reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing recurring invoice reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Recurring invoice reference URI cannot be null or empty.");
        }

        return RecurringInvoiceReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, RecurringInvoiceReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
