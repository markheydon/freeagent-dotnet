using System.Text.Json.Serialization;

namespace FreeAgent.Client.Infrastructure.Serialization;

[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class FreeAgentJsonContext : JsonSerializerContext;
