using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Construction Industry Scheme settings for the company.
/// </summary>
public sealed class CisSettings
{
    /// <summary>
    /// Contractor registration, or null when not registered as a contractor.
    /// </summary>
    [JsonPropertyName("contractor_details")]
    public CisContractorDetails? ContractorDetails { get; set; }

    /// <summary>
    /// Subcontractor registration, or null when not registered as a subcontractor.
    /// </summary>
    [JsonPropertyName("subcontractor_details")]
    public CisSubcontractorDetails? SubcontractorDetails { get; set; }
}
