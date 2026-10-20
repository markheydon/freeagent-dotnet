using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="BankTransactionReference"/> as a URI string.
/// </summary>
internal sealed class BankTransactionReferenceJsonConverter : JsonConverter<BankTransactionReference>
{
    public override BankTransactionReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Bank transaction reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing bank transaction reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Bank transaction reference URI cannot be null or empty.");
        }

        return BankTransactionReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, BankTransactionReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
