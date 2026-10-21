using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Tests.Infrastructure.Serialization;

internal static class FreeAgentJsonTestSupport
{
    internal static JsonSerializerOptions Options => FreeAgentJsonSerializerOptions.Default;
}
