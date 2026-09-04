using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>
/// Request envelope for creating or updating a category.
/// </summary>
internal sealed class CategoryRequest
{
    /// <summary>
    /// Category attributes.
    /// </summary>
    [JsonPropertyName("category")]
    public CategoryWritePayload Category { get; set; } = new();
}
