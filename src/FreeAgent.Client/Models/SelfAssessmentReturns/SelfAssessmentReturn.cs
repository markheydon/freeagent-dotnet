using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.SelfAssessmentReturns;

/// <summary>
/// A user self assessment return period.
/// </summary>
public sealed class SelfAssessmentReturn : IFreeAgentResource
{
    /// <summary>
    /// Self assessment return resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Period start date.
    /// </summary>
    [JsonPropertyName("period_starts_on")]
    public DateOnly? PeriodStartsOn { get; set; }

    /// <summary>
    /// Period end date (used as the resource identifier in API paths).
    /// </summary>
    [JsonPropertyName("period_ends_on")]
    public DateOnly? PeriodEndsOn { get; set; }

    /// <summary>
    /// Scheduled payments for the return.
    /// </summary>
    [JsonPropertyName("payments")]
    public List<SelfAssessmentReturnPayment>? Payments { get; set; }

    /// <summary>
    /// Filing due date.
    /// </summary>
    [JsonPropertyName("filing_due_on")]
    public DateOnly? FilingDueOn { get; set; }

    /// <summary>
    /// Filing status.
    /// </summary>
    [JsonPropertyName("filing_status")]
    public SelfAssessmentReturnFilingStatus? FilingStatus { get; set; }

    /// <summary>
    /// When the return was filed online.
    /// </summary>
    [JsonPropertyName("filed_at")]
    public DateTimeOffset? FiledAt { get; set; }

    /// <summary>
    /// Online filing reference when provided.
    /// </summary>
    [JsonPropertyName("filed_reference")]
    public string? FiledReference { get; set; }
}
