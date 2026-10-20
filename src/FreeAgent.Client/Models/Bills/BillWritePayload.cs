using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Bills;

/// <summary>
/// Writable bill attributes for create and update requests.
/// </summary>
internal sealed class BillWritePayload
{
    [JsonPropertyName("contact")]
    [JsonConverter(typeof(WriteLinkJsonConverter<ContactReference>))]
    public WriteLink<ContactReference>? Contact { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    [JsonPropertyName("due_on")]
    public DateOnly? DueOn { get; set; }

    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    [JsonPropertyName("input_total_values_inc_tax")]
    public bool? InputTotalValuesIncTax { get; set; }

    [JsonPropertyName("total_value")]
    public decimal? TotalValue { get; set; }

    [JsonPropertyName("is_paid_by_hire_purchase")]
    public bool? IsPaidByHirePurchase { get; set; }

    [JsonPropertyName("ec_status")]
    public BillEcStatus? EcStatus { get; set; }

    [JsonPropertyName("comments")]
    public string? Comments { get; set; }

    [JsonPropertyName("project")]
    [JsonConverter(typeof(WriteLinkJsonConverter<ProjectReference>))]
    public WriteLink<ProjectReference>? Project { get; set; }

    [JsonPropertyName("rebill_type")]
    public BillRebillType? RebillType { get; set; }

    [JsonPropertyName("rebill_factor")]
    public decimal? RebillFactor { get; set; }

    [JsonPropertyName("rebill_to_project")]
    [JsonConverter(typeof(WriteLinkJsonConverter<ProjectReference>))]
    public WriteLink<ProjectReference>? RebillToProject { get; set; }

    [JsonPropertyName("property")]
    public string? Property { get; set; }

    [JsonPropertyName("recurring")]
    public BillRecurringSchedule? Recurring { get; set; }

    [JsonPropertyName("recurring_end_date")]
    public DateOnly? RecurringEndDate { get; set; }

    [JsonPropertyName("attachment")]
    public BillAttachmentWritePayload? Attachment { get; set; }

    [JsonPropertyName("cis_deduction_band")]
    public CisDeductionRate? CisDeductionBand { get; set; }

    [JsonPropertyName("cis_deduction_rate")]
    public decimal? CisDeductionRate { get; set; }

    [JsonPropertyName("bill_items")]
    public List<BillItemWritePayload>? BillItems { get; set; }

    public static BillWritePayload FromBill(
        Bill bill,
        FreeAgentEnvironment environment,
        bool omitLineItems = false,
        LinkedResourceWriteOptions linkOptions = default,
        bool isUpdate = false)
    {
        ArgumentNullException.ThrowIfNull(bill);

        List<BillItemWritePayload>? items = null;
        if (!omitLineItems && bill.BillItems is not null)
        {
            items = bill.BillItems.ConvertAll(i => BillItemWritePayload.FromBillItem(i, environment, isUpdate));
        }

        return new BillWritePayload
        {
            Contact = LinkedResourceWriteMapper.ResolveContactReference(
                environment,
                bill.ContactIdBacking,
                bill.ContactLinkId,
                linkOptions.OmitContact),
            Reference = bill.Reference,
            DatedOn = bill.DatedOn,
            DueOn = bill.DueOn,
            Currency = bill.Currency,
            InputTotalValuesIncTax = bill.InputTotalValuesIncTax,
            TotalValue = bill.TotalValue,
            IsPaidByHirePurchase = bill.IsPaidByHirePurchase,
            EcStatus = bill.EcStatus,
            Comments = bill.Comments,
            Project = LinkedResourceWriteMapper.ResolveProjectReference(
                environment,
                bill.ProjectIdBacking,
                bill.ProjectLinkId,
                linkOptions.OmitProject),
            RebillType = bill.RebillType,
            RebillFactor = bill.RebillFactor,
            RebillToProject = LinkedResourceWriteMapper.ResolveProjectReference(
                environment,
                bill.RebillToProjectIdBacking,
                bill.RebillToProjectLinkId,
                linkOptions.OmitRebillToProject),
            Property = bill.PropertyUri,
            Recurring = bill.Recurring,
            RecurringEndDate = bill.RecurringEndDate,
            Attachment = bill.AttachmentWrite,
            CisDeductionBand = bill.CisDeductionBand,
            CisDeductionRate = bill.CisDeductionRate,
            BillItems = items
        };
    }
}
