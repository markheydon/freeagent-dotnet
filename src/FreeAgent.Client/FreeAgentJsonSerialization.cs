using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// JSON serialisation settings used by the FreeAgent API client for request and response bodies.
/// </summary>
public static class FreeAgentJsonSerialization
{
    /// <summary>
    /// Serializer options aligned with <see cref="FreeAgentClient"/> HTTP traffic (including source-generated metadata).
    /// </summary>
    public static JsonSerializerOptions ApiSerializerOptions => FreeAgentJsonSerializerOptions.Default;
}
