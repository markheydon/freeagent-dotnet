using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models;

/// <summary>
/// Wrapper for Company API responses.
/// </summary>
public class CompanyResponse
{
    /// <summary>
    /// Company data.
    /// </summary>
    [JsonPropertyName("company")]
    public Company? Company { get; set; }
}
