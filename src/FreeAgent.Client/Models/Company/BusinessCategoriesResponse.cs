using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Company;

/// <summary>
/// Wrapper for company business categories API responses.
/// </summary>
public class BusinessCategoriesResponse
{
    /// <summary>
    /// Supported company business categories.
    /// </summary>
    [JsonPropertyName("business_categories")]
    public List<string>? BusinessCategories { get; set; }
}
