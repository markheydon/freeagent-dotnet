using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises enums using <see cref="JsonStringEnumMemberNameAttribute"/> values when present,
/// with enum member names as a fallback.
/// </summary>
/// <typeparam name="TEnum">The enum type being converted.</typeparam>
public sealed class JsonStringEnumMemberNameCompatibleConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    private static readonly Dictionary<string, TEnum> NameToValue = BuildNameToValueMap();
    private static readonly Dictionary<TEnum, string> ValueToName = BuildValueToNameMap();

    /// <inheritdoc />
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.String)
        {
            throw new JsonException($"Expected string when parsing {typeof(TEnum).Name}.");
        }

        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new JsonException($"Cannot parse empty value for {typeof(TEnum).Name}.");
        }

        if (NameToValue.TryGetValue(value, out var parsed))
        {
            return parsed;
        }

        if (Enum.TryParse<TEnum>(value, ignoreCase: false, out parsed))
        {
            return parsed;
        }

        throw new JsonException($"Unable to convert \"{value}\" to {typeof(TEnum).Name}.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        if (ValueToName.TryGetValue(value, out var wireName))
        {
            writer.WriteStringValue(wireName);
            return;
        }

        writer.WriteStringValue(value.ToString());
    }

    private static Dictionary<string, TEnum> BuildNameToValueMap()
    {
        var map = new Dictionary<string, TEnum>(StringComparer.Ordinal);

        foreach (var enumValue in Enum.GetValues<TEnum>())
        {
            var name = GetWireName(enumValue);
            map[name] = enumValue;
        }

        return map;
    }

    private static Dictionary<TEnum, string> BuildValueToNameMap()
    {
        var map = new Dictionary<TEnum, string>();

        foreach (var enumValue in Enum.GetValues<TEnum>())
        {
            map[enumValue] = GetWireName(enumValue);
        }

        return map;
    }

    private static string GetWireName(TEnum value)
    {
        var fieldInfo = typeof(TEnum).GetField(value.ToString(), BindingFlags.Public | BindingFlags.Static);
        var customName = fieldInfo?.GetCustomAttribute<JsonStringEnumMemberNameAttribute>()?.Name;

        return string.IsNullOrWhiteSpace(customName) ? value.ToString() : customName;
    }
}
