using System.Net;
using System.Net.Http;
using System.Text;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.CisSettings;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.CisSettings;

public class CisSettingsServiceTests
{
    [Fact]
    public async Task GetCisSettingsAsync_ReturnsSettings()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/cis_settings", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """
                    {
                      "cis_settings": {
                        "contractor_details": {
                          "reporting_starts_on": "2024-05-06",
                          "paye_ni_period": "Monthly"
                        },
                        "subcontractor_details": null
                      }
                    }
                    """,
                    Encoding.UTF8,
                    "application/json")
            };
        });

        var service = CreateService(handler);
        var settings = await service.GetCisSettingsAsync();

        Assert.NotNull(settings.ContractorDetails);
        Assert.Equal(new DateOnly(2024, 5, 6), settings.ContractorDetails!.ReportingStartsOn);
        Assert.Equal(CisSettingsPayeNiPeriod.Monthly, settings.ContractorDetails.PayeNiPeriod);
        Assert.Null(settings.SubcontractorDetails);
    }

    [Fact]
    public async Task UpdateCisSettingsAsync_SendsPartialPayloadAndReturnsSettings()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/cis_settings", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            using var document = System.Text.Json.JsonDocument.Parse(body);
            var contractor = document.RootElement.GetProperty("cis_settings").GetProperty("contractor_details");
            Assert.Equal(System.Text.Json.JsonValueKind.Null, contractor.ValueKind);
            Assert.False(document.RootElement.GetProperty("cis_settings").TryGetProperty("subcontractor_details", out _));

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """
                    {
                      "cis_settings": {
                        "contractor_details": null,
                        "subcontractor_details": {
                          "cis_deduction_rates": ["cis_standard"],
                          "prior_deductions": null
                        }
                      }
                    }
                    """,
                    Encoding.UTF8,
                    "application/json")
            };
        });

        var service = CreateService(handler);
        var settings = await service.UpdateCisSettingsAsync(new CisSettingsUpdate
        {
            ContractorDetails = PatchValue.Set<CisContractorDetails>(null)
        });

        Assert.Null(settings.ContractorDetails);
        Assert.NotNull(settings.SubcontractorDetails);
        Assert.Single(settings.SubcontractorDetails!.CisDeductionRates!);
        Assert.Equal(CisDeductionRate.Standard, settings.SubcontractorDetails!.CisDeductionRates![0]);
    }

    [Fact]
    public async Task GetCisSettingsAsync_ThrowsWhenPayloadMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        });

        var service = CreateService(handler);
        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetCisSettingsAsync());
    }

    private static CisSettingsService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new CisSettingsService(client);
    }
}
