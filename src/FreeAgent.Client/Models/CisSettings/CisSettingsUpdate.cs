namespace FreeAgent.Client;

/// <summary>
/// Partial update for CIS settings. Unspecified sections are left unchanged; specified null clears registration.
/// </summary>
public sealed class CisSettingsUpdate
{
    /// <summary>
    /// Contractor registration patch.
    /// </summary>
    public PatchValue<CisContractorDetails> ContractorDetails { get; set; }

    /// <summary>
    /// Subcontractor registration patch.
    /// </summary>
    public PatchValue<CisSubcontractorDetailsUpdate> SubcontractorDetails { get; set; }
}
