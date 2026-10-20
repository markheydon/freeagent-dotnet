using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="BankTransactionExplanationReference"/> as a URI string.
/// </summary>
internal sealed class BankTransactionExplanationReferenceJsonConverter : JsonConverter<BankTransactionExplanationReference>
{
    public override BankTransactionExplanationReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Bank transaction explanation reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing bank transaction explanation reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Bank transaction explanation reference URI cannot be null or empty.");
        }

        return BankTransactionExplanationReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, BankTransactionExplanationReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
