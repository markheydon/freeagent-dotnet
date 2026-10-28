using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client;

namespace FreeAgent.Client.BlazorSample.Services;

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

    private static readonly JsonSerializerOptions MappingComparisonJsonOptions =
        FreeAgentJsonSerialization.ApiSerializerOptions;

    private const BindingFlags ModelPropertyFlags =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

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
    /// Builds diagnostics for a primitive string item from a named wire array.
    /// </summary>
    public static ModelProbeSnapshot BuildStringArrayItem(string value, JsonElement wireElement)
    {
        if (wireElement.ValueKind == JsonValueKind.Undefined)
        {
            return new ModelProbeSnapshot(
                JsonSerializer.Serialize(value, PrettyJsonOptions),
                [new ModelFieldRow("Value", "(array item)", "String", value)],
                [new ModelMappingRow(
                    "Value",
                    "(array item)",
                    "String",
                    value,
                    "Not returned",
                    "Not returned",
                    MappingCheckStatus.NotReturned)]);
        }

        var hasWireString = wireElement.ValueKind == JsonValueKind.String;
        var wireText = hasWireString
            ? wireElement.GetString() ?? string.Empty
            : FormatJsonValue(wireElement);

        var status = hasWireString && string.Equals(wireElement.GetString(), value, StringComparison.Ordinal)
            ? MappingCheckStatus.Match
            : MappingCheckStatus.Mismatch;

        return new ModelProbeSnapshot(
            JsonSerializer.Serialize(value, PrettyJsonOptions),
            [new ModelFieldRow("Value", "(array item)", "String", value)],
            [new ModelMappingRow(
                "Value",
                "(array item)",
                "String",
                value,
                wireText,
                wireElement.ValueKind.ToString(),
                status)]);
    }

    /// <summary>
    /// Builds string-array-item diagnostics from a raw list payload and row index.
    /// </summary>
    public static ModelProbeSnapshot BuildStringArrayItemFromWirePayload(
        string value,
        string wirePayload,
        string arrayPropertyName,
        int index)
    {
        if (TryGetArrayItem(wirePayload, arrayPropertyName, index, out var wireItem))
        {
            return BuildStringArrayItem(value, wireItem);
        }

        return BuildStringArrayItem(value, default);
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
        var included = new HashSet<string>(StringComparer.Ordinal);

        foreach (var property in modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
            {
                continue;
            }

            included.Add(property.Name);
            rows.Add(CreateFieldRow(model, property, property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name));
        }

        foreach (var (facade, jsonName) in DiscoverLinkFacadeProperties(modelType))
        {
            if (!included.Add(facade.Name))
            {
                continue;
            }

            rows.Add(CreateFieldRow(model, facade, jsonName));
        }

        rows.Sort(static (left, right) => string.Compare(left.PropertyName, right.PropertyName, StringComparison.Ordinal));
        return rows;
    }

    private static ModelFieldRow CreateFieldRow<T>(T model, PropertyInfo property, string jsonFieldName)
    {
        var rawValue = property.GetValue(model);
        var stringValue = rawValue switch
        {
            null => "Not returned",
            string text when string.IsNullOrWhiteSpace(text) => "Not returned",
            string text => text,
            DateOnly date => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            IEnumerable<string> stringCollection => string.Join(", ", stringCollection),
            _ => JsonSerializer.Serialize(rawValue, PrettyJsonOptions)
        };

        return new ModelFieldRow(
            property.Name,
            jsonFieldName,
            FormatTypeName(property.PropertyType),
            stringValue);
    }

    private static List<ModelMappingRow> BuildMappingRows<T>(T model, JsonElement wireElement, Type modelType)
    {
        var rows = new List<ModelMappingRow>();
        var mappedJsonFields = new HashSet<string>(StringComparer.Ordinal);

        foreach (var property in modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
            {
                continue;
            }

            var jsonName = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;
            mappedJsonFields.Add(jsonName);
            rows.Add(CreateMappingRow(model, wireElement, property, jsonName));
        }

        foreach (var linkProperty in DiscoverWireLinkProperties(modelType))
        {
            var jsonName = linkProperty.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name;
            if (!mappedJsonFields.Add(jsonName))
            {
                continue;
            }

            rows.Add(CreateMappingRow(model, wireElement, linkProperty, jsonName));
        }

        foreach (var (facade, jsonName) in DiscoverLinkFacadeProperties(modelType))
        {
            rows.Add(CreateLinkFacadeMappingRow(model, wireElement, facade, jsonName));
        }

        rows.Sort(static (left, right) => string.Compare(left.PropertyName, right.PropertyName, StringComparison.Ordinal));
        return rows;
    }

    private static ModelMappingRow CreateMappingRow<T>(
        T model,
        JsonElement wireElement,
        PropertyInfo property,
        string jsonName)
    {
        var modelRawValue = property.GetValue(model);
        JsonElement rawElement = default;
        var hasRawValue = wireElement.ValueKind == JsonValueKind.Object
            && wireElement.TryGetProperty(jsonName, out rawElement);

        var status = DetermineMappingStatus(
            hasRawValue,
            hasRawValue ? rawElement : default,
            modelRawValue,
            property.PropertyType);

        return new ModelMappingRow(
            property.Name,
            jsonName,
            FormatTypeName(property.PropertyType),
            FormatModelValue(modelRawValue),
            hasRawValue ? FormatJsonValue(rawElement) : "Not returned",
            hasRawValue ? rawElement.ValueKind.ToString() : "Not returned",
            status);
    }

    private static ModelMappingRow CreateLinkFacadeMappingRow<T>(
        T model,
        JsonElement wireElement,
        PropertyInfo facade,
        string jsonName)
    {
        var modelRawValue = facade.GetValue(model);
        JsonElement rawElement = default;
        var hasRawValue = wireElement.ValueKind == JsonValueKind.Object
            && wireElement.TryGetProperty(jsonName, out rawElement);

        var status = facade.PropertyType == typeof(long?) || facade.PropertyType == typeof(long)
            ? DetermineLinkIdMappingStatus(hasRawValue, hasRawValue ? rawElement : default, modelRawValue as long?)
            : DetermineMappingStatus(
                hasRawValue,
                hasRawValue ? rawElement : default,
                modelRawValue,
                facade.PropertyType);

        return new ModelMappingRow(
            facade.Name,
            jsonName,
            FormatTypeName(facade.PropertyType),
            FormatModelValue(modelRawValue),
            hasRawValue ? FormatJsonValue(rawElement) : "Not returned",
            hasRawValue ? rawElement.ValueKind.ToString() : "Not returned",
            status);
    }

    private static IEnumerable<PropertyInfo> DiscoverWireLinkProperties(Type modelType) =>
        modelType
            .GetProperties(ModelPropertyFlags)
            .Where(static property =>
                property.Name.EndsWith("Link", StringComparison.Ordinal)
                && property.GetCustomAttribute<JsonPropertyNameAttribute>() is not null
                && property.GetCustomAttribute<JsonIncludeAttribute>() is not null);

    private static IEnumerable<(PropertyInfo Facade, string JsonName)> DiscoverLinkFacadeProperties(Type modelType)
    {
        foreach (var linkProperty in DiscoverWireLinkProperties(modelType))
        {
            var jsonName = linkProperty.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name;
            var baseName = linkProperty.Name[..^4];

            var idProperty = modelType.GetProperty(baseName + "Id", ModelPropertyFlags);
            if (idProperty?.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
            {
                yield return (idProperty, jsonName);
            }

            var navigationProperty = modelType.GetProperty(baseName, ModelPropertyFlags);
            if (navigationProperty?.GetCustomAttribute<JsonIgnoreAttribute>() is not null
                && navigationProperty.Name != idProperty?.Name)
            {
                yield return (navigationProperty, jsonName);
            }
        }
    }

    private static MappingCheckStatus DetermineLinkIdMappingStatus(
        bool hasRawValue,
        JsonElement rawValue,
        long? modelId)
    {
        var hasModelValue = modelId is > 0;

        if (!hasRawValue)
        {
            return hasModelValue ? MappingCheckStatus.ModelOnly : MappingCheckStatus.NotReturned;
        }

        if (!hasModelValue)
        {
            return MappingCheckStatus.MissingInModel;
        }

        if (rawValue.ValueKind == JsonValueKind.String)
        {
            return FreeAgentResourceUri.TryParseResourceId(rawValue.GetString(), out var wireId) && wireId == modelId
                ? MappingCheckStatus.Match
                : MappingCheckStatus.Mismatch;
        }

        if (rawValue.ValueKind == JsonValueKind.Object
            && rawValue.TryGetProperty("url", out var urlElement)
            && urlElement.ValueKind == JsonValueKind.String)
        {
            return FreeAgentResourceUri.TryParseResourceId(urlElement.GetString(), out var wireId) && wireId == modelId
                ? MappingCheckStatus.Match
                : MappingCheckStatus.Mismatch;
        }

        return MappingCheckStatus.Mismatch;
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
