using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// Writable estimate attributes for create and update requests.
/// </summary>
internal sealed class EstimateWritePayload
{
    [JsonPropertyName("contact")]
    public ContactReference? Contact { get; set; }

    [JsonPropertyName("project")]
    public ProjectReference? Project { get; set; }

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

    public static EstimateWritePayload FromEstimate(Estimate estimate)
    {
        ArgumentNullException.ThrowIfNull(estimate);

        ContactReference? contact = null;
        if (!estimate.OmitBillingContactFromWrite)
        {
            contact = estimate.BillingContact
                ?? (estimate.ContactLink?.Uri is string contactUri ? ContactReference.Parse(contactUri) : null);
        }
        else
        {
            contact = estimate.BillingContact;
        }

        ProjectReference? project = null;
        if (!estimate.OmitProjectFromWrite)
        {
            project = estimate.LinkedProject
                ?? (estimate.ProjectLink?.Uri is string projectUri ? ProjectReference.Parse(projectUri) : null);
        }
        else
        {
            project = estimate.LinkedProject;
        }

        List<EstimateItemWritePayload>? items = null;
        if (!estimate.OmitEstimateItemsFromWrite && estimate.EstimateItems is not null)
        {
            items = estimate.EstimateItems.ConvertAll(EstimateItemWritePayload.FromEstimateItem);
        }

        return new EstimateWritePayload
        {
            Contact = contact,
            Project = project,
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
