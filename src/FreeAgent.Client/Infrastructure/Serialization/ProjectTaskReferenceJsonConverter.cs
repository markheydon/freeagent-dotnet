using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="ProjectTaskReference"/> as a URI string.
/// </summary>
internal sealed class ProjectTaskReferenceJsonConverter : JsonConverter<ProjectTaskReference>
{
    public override ProjectTaskReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Task reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing task reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Task reference URI cannot be null or empty.");
        }

        return ProjectTaskReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, ProjectTaskReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
