using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="CisSettingsUpdate"/> with omit / null / object semantics from the CIS settings API.
/// </summary>
internal static class CisSettingsUpdateSerializer
{
    private static readonly JsonSerializerOptions SectionOptions = new(FreeAgentJsonSerializer.Options);

    internal static StringContent CreateContent(CisSettingsUpdate update)
    {
        ArgumentNullException.ThrowIfNull(update);

        var cisSettings = new JsonObject();

        if (update.ContractorDetails.IsSpecified)
        {
            cisSettings["contractor_details"] = update.ContractorDetails.Value is null
                ? null
                : JsonSerializer.SerializeToNode(update.ContractorDetails.Value, SectionOptions);
        }

        if (update.SubcontractorDetails.IsSpecified)
        {
            cisSettings["subcontractor_details"] = update.SubcontractorDetails.Value is null
                ? null
                : SerializeSubcontractorUpdate(update.SubcontractorDetails.Value);
        }

        var envelope = new JsonObject
        {
            ["cis_settings"] = cisSettings
        };

        var json = envelope.ToJsonString();
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static JsonObject SerializeSubcontractorUpdate(CisSubcontractorDetailsUpdate update)
    {
        var node = new JsonObject();

        if (update.CisDeductionRates is not null)
        {
            node["cis_deduction_rates"] = JsonSerializer.SerializeToNode(update.CisDeductionRates, SectionOptions);
        }

        if (update.PriorDeductions.IsSpecified)
        {
            node["prior_deductions"] = update.PriorDeductions.Value is null
                ? null
                : JsonSerializer.SerializeToNode(update.PriorDeductions.Value, SectionOptions);
        }

        return node;
    }
}
