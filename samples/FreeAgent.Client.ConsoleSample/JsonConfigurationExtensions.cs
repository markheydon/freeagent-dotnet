using System.Text.Json;

namespace FreeAgent.Client.ConsoleSample;

internal static class JsonConfigurationExtensions
{
    public static string GetPropertyOrDefault(this JsonElement element, string propertyName, string fallback = "") =>
        element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? fallback
            : fallback;
}
