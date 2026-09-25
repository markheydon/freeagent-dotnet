using FreeAgent.Client.Models.Estimates;

namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Normalises Turpinverse quote statuses to FreeAgent estimate transitions.
/// </summary>
internal static class TurpinverseEstimateStatusSupport
{
    public static bool StatusesEquivalent(EstimateStatus? current, EstimateStatus target) =>
        current == target
        || (target == EstimateStatus.Sent && current is EstimateStatus.Open or EstimateStatus.Sent);

    public static bool RequiresSentState(EstimateStatus target) =>
        target is EstimateStatus.Sent or EstimateStatus.Approved or EstimateStatus.Rejected;
}
