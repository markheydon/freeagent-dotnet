using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Services.PayrollProfiles;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.PayrollProfiles;

public class PayrollProfilesServiceTests
{
    private const int TaxYearEnd = 2021;
    private const string UserUri = "https://api.freeagent.com/v2/users/107";

    [Fact]
    public async Task ListAllProfilesForTaxYearAsync_CallsDocumentedEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal($"/v2/payroll_profiles/{TaxYearEnd}", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "profiles": [
                    {
                      "user": "https://api.freeagent.com/v2/users/107",
                      "payroll_reference": "EMP001",
                      "date_of_birth": "1990-08-17",
                      "total_pay_in_previous_employment": "1000.0",
                      "created_at": "2024-08-07T14:27:32.000Z"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var profiles = await service.ListAllProfilesForTaxYearAsync(TaxYearEnd);

        Assert.Single(profiles);
        Assert.Equal(107, profiles[0].UserId);
        Assert.Equal("EMP001", profiles[0].PayrollReference);
        Assert.Equal(new DateOnly(1990, 8, 17), profiles[0].DateOfBirth);
        Assert.Equal(1000m, profiles[0].TotalPayInPreviousEmployment);
    }

    [Fact]
    public async Task ListProfilesForUserAsync_IncludesUserQueryParameter()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal($"/v2/payroll_profiles/{TaxYearEnd}", request.RequestUri!.AbsolutePath);
            Assert.Contains("user=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fusers%2F107", request.RequestUri!.Query);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "profiles": [
                    {
                      "user": "https://api.freeagent.com/v2/users/107",
                      "payroll_reference": "EMP001"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var profiles = await service.ListProfilesForUserAsync(
            TaxYearEnd,
            UserReference.Parse(UserUri));

        Assert.Single(profiles);
        Assert.Equal(107, profiles[0].UserId);
    }

    [Fact]
    public async Task ListAllProfilesForTaxYearAsync_ThrowsWhenProfilesMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAllProfilesForTaxYearAsync(TaxYearEnd));
    }

    private static PayrollProfilesService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new PayrollProfilesService(client);
    }
}
