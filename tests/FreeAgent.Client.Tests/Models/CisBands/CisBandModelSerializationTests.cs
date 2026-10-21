using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.Tests.Models.CisBands;

public class CisBandModelSerializationTests
{
    [Fact]
    public void DeserializeCisBand_MapsDocumentedFields()
    {
        const string json = """
            {
              "name": "cis_higher",
              "deduction_rate": "0.3",
              "income_description": "CIS Income (30%)",
              "deduction_description": "CIS Deduction (30%)",
              "nominal_code": "063"
            }
            """;

        var band = JsonSerializer.Deserialize<CisBand>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(band);
        Assert.Equal(CisDeductionRate.Higher, band!.Name);
        Assert.Equal(0.3m, band.DeductionRate);
        Assert.Equal("CIS Income (30%)", band.IncomeDescription);
        Assert.Equal("063", band.NominalCode);
    }
}
