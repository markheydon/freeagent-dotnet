using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises expandable link fields as URI strings and deserialises URI strings or nested objects.
/// </summary>
/// <typeparam name="T">Linked resource type.</typeparam>
internal sealed class ExpandableFieldJsonConverter<T> : JsonConverter<ExpandableField<T>> where T : class, IFreeAgentResource
{
    public override ExpandableField<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            return new ExpandableField<T>(reader.GetString());
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            var value = JsonSerializer.Deserialize<T>(ref reader, options);
            return value is null ? null : new ExpandableField<T>(value.Url, value);
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing expandable field.");
    }

    public override void Write(Utf8JsonWriter writer, ExpandableField<T> value, JsonSerializerOptions options)
    {
        if (value.Uri is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Uri);
    }
}

/// <summary>
/// Factory for <see cref="ExpandableFieldJsonConverter{T}"/> instances.
/// </summary>
internal sealed class ExpandableFieldJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(ExpandableField<>);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var resourceType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(ExpandableFieldJsonConverter<>).MakeGenericType(resourceType);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}
