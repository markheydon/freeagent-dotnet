using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using CisSettingsModel = global::FreeAgent.Client.CisSettings;

namespace FreeAgent.Client.Tests.Models.CisSettings;

public class CisSettingsModelSerializationTests
{
    [Fact]
    public void DeserializeCisSettings_MapsDocumentedFields()
    {
        const string json = """
            {
              "contractor_details": null,
              "subcontractor_details": {
                "cis_deduction_rates": ["cis_gross", "cis_higher"],
                "prior_deductions": {
                  "start_date": "2019-04-06",
                  "initial_balance": "300.00"
                }
              }
            }
            """;

        var settings = JsonSerializer.Deserialize<CisSettingsModel>(json, FreeAgentJsonSerializer.Options);

        Assert.Null(settings!.ContractorDetails);
        Assert.NotNull(settings.SubcontractorDetails);
        Assert.Equal(2, settings.SubcontractorDetails!.CisDeductionRates!.Count);
        Assert.Equal(CisDeductionRate.Gross, settings.SubcontractorDetails.CisDeductionRates[0]);
        Assert.Equal(new DateOnly(2019, 4, 6), settings.SubcontractorDetails.PriorDeductions!.StartDate);
        Assert.Equal(300.00m, settings.SubcontractorDetails.PriorDeductions.InitialBalance);
    }
}
