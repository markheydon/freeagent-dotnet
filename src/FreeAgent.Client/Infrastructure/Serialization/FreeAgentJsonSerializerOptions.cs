using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Shared <see cref="JsonSerializerOptions"/> for API and OAuth JSON traffic.
/// </summary>
internal static class FreeAgentJsonSerializerOptions
{
    internal static JsonSerializerOptions Default { get; } = CreateDefault();

    private static JsonSerializerOptions CreateDefault()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = JsonTypeInfoResolver.Combine(
                FreeAgentJsonContext.Default,
                new DefaultJsonTypeInfoResolver()),
        };

        options.MakeReadOnly();
        return options;
    }
}
