using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Sample.Services;

/// <summary>
/// Status of a wire-to-model field comparison in sample diagnostics.
/// </summary>
public enum MappingCheckStatus
{
    Match,
    Mismatch,
    MissingInModel,
    ModelOnly,
    NotReturned
}

/// <summary>
/// One row in the all-model-fields diagnostics table.
/// </summary>
public sealed record ModelFieldRow(string PropertyName, string JsonFieldName, string TypeName, string Value);

/// <summary>
/// One row in the wire-to-model mapping diagnostics table.
/// </summary>
public sealed record ModelMappingRow(
    string PropertyName,
    string JsonFieldName,
    string ModelTypeName,
    string ModelValue,
    string RawValue,
    string RawValueKind,
    MappingCheckStatus Status);

/// <summary>
/// Diagnostics snapshot for a deserialized SDK model and its wire payload.
/// </summary>
public sealed record ModelProbeSnapshot(
    string ModelJson,
    IReadOnlyList<ModelFieldRow> FieldRows,
    IReadOnlyList<ModelMappingRow> MappingRows)
{
    public int MatchCount => MappingRows.Count(static row => row.Status == MappingCheckStatus.Match);

    public int MismatchCount => MappingRows.Count(static row => row.Status == MappingCheckStatus.Mismatch);

    public int MissingInModelCount => MappingRows.Count(static row => row.Status == MappingCheckStatus.MissingInModel);

    public int ModelOnlyCount => MappingRows.Count(static row => row.Status == MappingCheckStatus.ModelOnly);

    public int NotReturnedCount => MappingRows.Count(static row => row.Status == MappingCheckStatus.NotReturned);
}

/// <summary>
/// Builds wire-to-model diagnostics for sample endpoint probes.
/// </summary>
public static class ModelWireDiagnostics
{
    private static readonly JsonSerializerOptions PrettyJsonOptions = new()
    {
        WriteIndented = true
    };

    private static readonly JsonSerializerOptions MappingComparisonJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    /// <summary>
    /// Builds diagnostics from a model and raw API JSON, optionally extracting a named envelope property.
    /// </summary>
    public static ModelProbeSnapshot Build<T>(T model, string rawPayload, string? envelopeProperty = null)
    {
        if (string.IsNullOrWhiteSpace(rawPayload))
        {
            return Build(model, default);
        }

        try
        {
            using var document = JsonDocument.Parse(rawPayload);
            var wireElement = ExtractEnvelope(document.RootElement, envelopeProperty);
            return Build(model, wireElement);
        }
        catch (JsonException)
        {
            return Build(model, default);
        }
    }

    /// <summary>
    /// Builds diagnostics from a model and a wire JSON element.
    /// </summary>
    public static ModelProbeSnapshot Build<T>(T model, JsonElement wireElement)
    {
        var modelType = typeof(T);
        return new ModelProbeSnapshot(
            JsonSerializer.Serialize(model, PrettyJsonOptions),
            BuildFieldRows(model, modelType),
            BuildMappingRows(model, wireElement, modelType));
    }

    /// <summary>
    /// Attempts to read one item from a named array property in a raw API payload.
    /// </summary>
    public static bool TryGetArrayItem(string rawPayload, string arrayPropertyName, int index, out JsonElement item)
    {
        item = default;

        if (string.IsNullOrWhiteSpace(rawPayload))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(rawPayload);
            if (!document.RootElement.TryGetProperty(arrayPropertyName, out var arrayElement)
                || arrayElement.ValueKind != JsonValueKind.Array)
            {
                return false;
            }

            var position = 0;
            foreach (var element in arrayElement.EnumerateArray())
            {
                if (position == index)
                {
                    item = element.Clone();
                    return true;
                }

                position++;
            }
        }
        catch (JsonException)
        {
            return false;
        }

        return false;
    }

    public static string GetStatusLabel(MappingCheckStatus status) =>
        status switch
        {
            MappingCheckStatus.Match => "Match",
            MappingCheckStatus.Mismatch => "Mismatch",
            MappingCheckStatus.MissingInModel => "Missing in model",
            MappingCheckStatus.ModelOnly => "Model-only",
            MappingCheckStatus.NotReturned => "Not returned",
            _ => "Not returned"
        };

    private static JsonElement ExtractEnvelope(JsonElement rootElement, string? envelopeProperty)
    {
        if (!string.IsNullOrWhiteSpace(envelopeProperty)
            && rootElement.ValueKind == JsonValueKind.Object
            && rootElement.TryGetProperty(envelopeProperty, out var envelopeNode))
        {
            return envelopeNode.Clone();
        }

        return rootElement.Clone();
    }

    private static List<ModelFieldRow> BuildFieldRows<T>(T model, Type modelType)
    {
        var rows = new List<ModelFieldRow>();

        foreach (var property in modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
            {
                continue;
            }

            var rawValue = property.GetValue(model);
            var jsonName = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;
            var stringValue = rawValue switch
            {
                null => "Not returned",
                string text when string.IsNullOrWhiteSpace(text) => "Not returned",
                string text => text,
                DateOnly date => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                IEnumerable<string> stringCollection => string.Join(", ", stringCollection),
                _ => JsonSerializer.Serialize(rawValue, PrettyJsonOptions)
            };

            rows.Add(new ModelFieldRow(
                property.Name,
                jsonName,
                FormatTypeName(property.PropertyType),
                stringValue));
        }

        return rows;
    }

    private static List<ModelMappingRow> BuildMappingRows<T>(T model, JsonElement wireElement, Type modelType)
    {
        var rows = new List<ModelMappingRow>();

        foreach (var property in modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
            {
                continue;
            }

            var jsonName = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;
            var modelRawValue = property.GetValue(model);
            JsonElement rawElement = default;
            var hasRawValue = wireElement.ValueKind == JsonValueKind.Object
                && wireElement.TryGetProperty(jsonName, out rawElement);

            var status = DetermineMappingStatus(
                hasRawValue,
                hasRawValue ? rawElement : default,
                modelRawValue,
                property.PropertyType);

            rows.Add(new ModelMappingRow(
                property.Name,
                jsonName,
                FormatTypeName(property.PropertyType),
                FormatModelValue(modelRawValue),
                hasRawValue ? FormatJsonValue(rawElement) : "Not returned",
                hasRawValue ? rawElement.ValueKind.ToString() : "Not returned",
                status));
        }

        return rows;
    }

    private static MappingCheckStatus DetermineMappingStatus(
        bool hasRawValue,
        JsonElement rawValue,
        object? modelValue,
        Type propertyType)
    {
        var hasModelValue = HasMeaningfulValue(modelValue);

        if (!hasRawValue)
        {
            return hasModelValue ? MappingCheckStatus.ModelOnly : MappingCheckStatus.NotReturned;
        }

        if (!hasModelValue)
        {
            return MappingCheckStatus.MissingInModel;
        }

        return ValuesEquivalent(rawValue, modelValue!, propertyType)
            ? MappingCheckStatus.Match
            : MappingCheckStatus.Mismatch;
    }

    private static bool HasMeaningfulValue(object? value)
    {
        switch (value)
        {
            case null:
                return false;
            case string stringValue:
                return !string.IsNullOrWhiteSpace(stringValue);
            case IEnumerable enumerable when value is not string:
            {
                var enumerator = enumerable.GetEnumerator();
                try
                {
                    return enumerator.MoveNext();
                }
                finally
                {
                    (enumerator as IDisposable)?.Dispose();
                }
            }
            default:
                return true;
        }
    }

    private static bool ValuesEquivalent(JsonElement rawValue, object modelValue, Type propertyType)
    {
        try
        {
            var wireDeserialized = JsonSerializer.Deserialize(rawValue.GetRawText(), propertyType, MappingComparisonJsonOptions);
            return SemanticJsonEquals(modelValue, wireDeserialized, propertyType);
        }
        catch (JsonException)
        {
            return false;
        }
        catch (NotSupportedException)
        {
            return false;
        }
    }

    private static bool SemanticJsonEquals(object? modelValue, object? wireValue, Type propertyType)
    {
        if (modelValue is null && wireValue is null)
        {
            return true;
        }

        if (modelValue is null || wireValue is null)
        {
            return false;
        }

        var modelJson = JsonSerializer.Serialize(modelValue, propertyType, MappingComparisonJsonOptions);
        var wireJson = JsonSerializer.Serialize(wireValue, propertyType, MappingComparisonJsonOptions);

        using var modelDocument = JsonDocument.Parse(modelJson);
        using var wireDocument = JsonDocument.Parse(wireJson);
        return JsonElementsEqual(modelDocument.RootElement, wireDocument.RootElement);
    }

    private static bool JsonElementsEqual(JsonElement left, JsonElement right)
    {
#if NET9_0_OR_GREATER
        return JsonElement.DeepEquals(left, right);
#else
        return left.GetRawText() == right.GetRawText();
#endif
    }

    private static string FormatJsonValue(JsonElement value) =>
        value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.Number => value.GetRawText(),
            JsonValueKind.True => bool.TrueString,
            JsonValueKind.False => bool.FalseString,
            JsonValueKind.Null => "Not returned",
            JsonValueKind.Array or JsonValueKind.Object => JsonSerializer.Serialize(value, PrettyJsonOptions),
            _ => value.GetRawText()
        };

    private static string FormatModelValue(object? value) =>
        value switch
        {
            null => "Not returned",
            string stringValue when string.IsNullOrWhiteSpace(stringValue) => "Not returned",
            string stringValue => stringValue,
            DateOnly dateOnlyValue => dateOnlyValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            DateTimeOffset dateTimeOffsetValue => dateTimeOffsetValue.ToString("O", CultureInfo.InvariantCulture),
            bool boolValue => boolValue.ToString(CultureInfo.InvariantCulture),
            int intValue => intValue.ToString(CultureInfo.InvariantCulture),
            long longValue => longValue.ToString(CultureInfo.InvariantCulture),
            decimal decimalValue => decimalValue.ToString(CultureInfo.InvariantCulture),
            Enum enumValue => enumValue.ToString(),
            IEnumerable _ when value is not string => JsonSerializer.Serialize(value, PrettyJsonOptions),
            _ => JsonSerializer.Serialize(value, PrettyJsonOptions)
        };

    private static string FormatTypeName(Type type)
    {
        if (Nullable.GetUnderlyingType(type) is Type nullableType)
        {
            return $"{FormatTypeName(nullableType)}?";
        }

        if (type.IsGenericType)
        {
            var genericTypeName = type.Name[..type.Name.IndexOf('`')];
            var genericArguments = string.Join(", ", type.GetGenericArguments().Select(FormatTypeName));
            return $"{genericTypeName}<{genericArguments}>";
        }

        return type.Name;
    }
}
