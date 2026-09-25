using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// Writable estimate attributes for create and update requests.
/// </summary>
internal sealed class EstimateWritePayload
{
    [JsonPropertyName("contact")]
    [JsonConverter(typeof(WriteLinkJsonConverter<ContactReference>))]
    public WriteLink<ContactReference>? Contact { get; set; }

    [JsonPropertyName("project")]
    [JsonConverter(typeof(WriteLinkJsonConverter<ProjectReference>))]
    public WriteLink<ProjectReference>? Project { get; set; }

    [JsonPropertyName("status")]
    public EstimateStatus? Status { get; set; }

    [JsonPropertyName("estimate_type")]
    public EstimateType? EstimateType { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("discount_percent")]
    public decimal? DiscountPercent { get; set; }

    [JsonPropertyName("client_contact_name")]
    public string? ClientContactName { get; set; }

    [JsonPropertyName("ec_status")]
    public EstimateEcStatus? EcStatus { get; set; }

    [JsonPropertyName("place_of_supply")]
    public string? PlaceOfSupply { get; set; }

    [JsonPropertyName("include_sales_tax_on_total_value")]
    public bool? IncludeSalesTaxOnTotalValue { get; set; }

    [JsonPropertyName("estimate_items")]
    public List<EstimateItemWritePayload>? EstimateItems { get; set; }

    public static EstimateWritePayload FromEstimate(
        Estimate estimate,
        FreeAgentEnvironment environment,
        bool omitLineItems = false,
        LinkedResourceWriteOptions linkOptions = default,
        bool includeStatus = false)
    {
        ArgumentNullException.ThrowIfNull(estimate);

        List<EstimateItemWritePayload>? items = null;
        if (!omitLineItems && estimate.EstimateItems is not null)
        {
            items = estimate.EstimateItems.ConvertAll(i => EstimateItemWritePayload.FromEstimateItem(i, environment));
        }

        return new EstimateWritePayload
        {
            Contact = LinkedResourceWriteMapper.ResolveContactReference(
                environment,
                estimate.ContactIdBacking,
                estimate.ContactLinkId,
                linkOptions.OmitContact),
            Project = LinkedResourceWriteMapper.ResolveProjectReference(
                environment,
                estimate.ProjectIdBacking,
                estimate.ProjectLinkId,
                linkOptions.OmitProject),
            Status = includeStatus ? EstimateStatus.Draft : null,
            EstimateType = estimate.EstimateType,
            Reference = estimate.Reference,
            DatedOn = estimate.DatedOn,
            Currency = estimate.Currency,
            Notes = estimate.Notes,
            DiscountPercent = estimate.DiscountPercent,
            ClientContactName = estimate.ClientContactName,
            EcStatus = estimate.EcStatus,
            PlaceOfSupply = estimate.PlaceOfSupply,
            IncludeSalesTaxOnTotalValue = estimate.IncludeSalesTaxOnTotalValue,
            EstimateItems = items
        };
    }
}
