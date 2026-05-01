using System.Net;
using System.Net.Http;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Company;
using FreeAgent.Client.Services.Company;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Company;

public class CompanyServiceTests
{
    [Fact]
    public async Task GetCompanyAsync_ReturnsCompanyData()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
            {
              "company": {
                "url": "https://api.freeagent.com/v2/company",
                "id": 12345,
                "name": "My Company",
                "subdomain": "my-company",
                "type": "UkLimitedCompany",
                "currency": "GBP",
                "mileage_units": "miles",
                "sales_tax_registration_status": "De-registered",
                "initial_vat_basis": "Cash",
                "business_category": "Software Development",
                "sales_tax_rates": ["20", 5, { "name": "Zero", "rate": "0" }]
              }
            }
            """)
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token");
        var service = new CompanyService(client);

        var company = await service.GetCompanyAsync();

        Assert.Equal(12345, company.Id);
        Assert.Equal("My Company", company.Name);
        Assert.Equal("Software Development", company.BusinessCategory);
        Assert.Equal(3, company.SalesTaxRates.Count);
        Assert.Equal(20m, company.SalesTaxRates[0].Rate);
        Assert.Equal(5m, company.SalesTaxRates[1].Rate);
        Assert.Equal("Zero", company.SalesTaxRates[2].Name);
        Assert.Equal(0m, company.SalesTaxRates[2].Rate);
        Assert.Equal(CompanyType.UkLimitedCompany, company.Type);
        Assert.Equal(CurrencyCode.GBP, company.Currency);
        Assert.Equal(MileageUnit.Miles, company.MileageUnits);
        Assert.Equal(SalesTaxRegistrationStatus.Deregistered, company.SalesTaxRegistrationStatus);
        Assert.Equal(InitialVatBasis.Cash, company.InitialVatBasis);
    }

    [Fact]
    public async Task GetCompanyAsync_DeserializesUnregisteredSalesTaxStatus()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
            {
              "company": {
                "url": "https://api.freeagent.com/v2/company",
                "id": 12345,
                "name": "My Company",
                "subdomain": "my-company",
                "type": "UkLimitedCompany",
                "currency": "GBP",
                "sales_tax_registration_status": "Unregistered"
              }
            }
            """)
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token");
        var service = new CompanyService(client);

        var company = await service.GetCompanyAsync();

        Assert.Equal(SalesTaxRegistrationStatus.Unregistered, company.SalesTaxRegistrationStatus);
    }

    [Fact]
    public async Task GetCompanyAsync_WhenSalesTaxStatusIsUnknown_MapsToUnknown()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
            {
              "company": {
                "url": "https://api.freeagent.com/v2/company",
                "id": 12345,
                "name": "My Company",
                "subdomain": "my-company",
                "type": "UkLimitedCompany",
                "currency": "GBP",
                "sales_tax_registration_status": "SomeUnexpectedStatus"
              }
            }
            """)
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token");
        var service = new CompanyService(client);

        var company = await service.GetCompanyAsync();

        Assert.Equal(SalesTaxRegistrationStatus.Unknown, company.SalesTaxRegistrationStatus);
    }

    [Fact]
    public async Task GetCompanyAsync_DeserializesKilometresMileageUnit()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
            {
              "company": {
                "url": "https://api.freeagent.com/v2/company",
                "id": 12345,
                "name": "My Company",
                "subdomain": "my-company",
                "type": "UkLimitedCompany",
                "currency": "GBP",
                "mileage_units": "kilometres"
              }
            }
            """)
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token");
        var service = new CompanyService(client);

        var company = await service.GetCompanyAsync();

        Assert.Equal(MileageUnit.Kilometers, company.MileageUnits);
    }

    [Fact]
    public async Task GetCompanyAsync_WhenPayloadMissing_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"company\": null}")
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token");
        var service = new CompanyService(client);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetCompanyAsync());

        Assert.Contains("Company data missing", exception.Message);
    }

    [Fact]
    public async Task GetBusinessCategoriesAsync_ReturnsCategories()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
            {
              "business_categories": ["Software Development", "Design", "IT Contractor / Consulting"]
            }
            """)
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token");
        var service = new CompanyService(client);

        var categories = await service.GetBusinessCategoriesAsync();

        Assert.Equal(3, categories.Count);
        Assert.Contains("Software Development", categories);
    }

    [Fact]
    public async Task GetTaxTimelineAsync_ReturnsTimelineItems()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
            {
              "timeline_items": [
                {
                  "description": "VAT Return 09 11",
                  "nature": "Electronic Submission and Payment Due",
                  "dated_on": "2011-11-07",
                  "amount_due": "-214.16",
                  "is_personal": false
                },
                {
                  "description": "Accounting Period Ending 31 May 11",
                  "nature": "Companies House First Accounts Due",
                  "dated_on": "2012-02-01",
                  "is_personal": false
                }
              ]
            }
            """)
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token");
        var service = new CompanyService(client);

        var timeline = await service.GetTaxTimelineAsync();

        Assert.Equal(2, timeline.Count);
        Assert.Equal("VAT Return 09 11", timeline[0].Description);
        Assert.Equal(-214.16m, timeline[0].AmountDue);
        Assert.False(timeline[0].IsPersonal);
        Assert.Equal(new DateOnly(2011, 11, 7), timeline[0].DatedOn);
    }

    [Fact]
    public async Task GetCompanyAsync_DeserializesDateOnlyFields()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
            {
              "company": {
                "url": "https://api.freeagent.com/v2/company",
                "id": 1,
                "name": "Test Co",
                "subdomain": "test",
                "type": "UkLimitedCompany",
                "currency": "GBP",
                "company_start_date": "2020-05-01",
                "freeagent_start_date": "2020-05-01",
                "first_accounting_year_end": "2021-04-30"
              }
            }
            """)
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token");
        var service = new CompanyService(client);

        var company = await service.GetCompanyAsync();

        Assert.Equal(new DateOnly(2020, 5, 1), company.CompanyStartDate);
        Assert.Equal(new DateOnly(2020, 5, 1), company.FreeAgentStartDate);
        Assert.Equal(new DateOnly(2021, 4, 30), company.FirstAccountingYearEnd);
    }

    [Fact]
    public async Task GetCompanyAsync_DeserializesAnnualAccountingPeriods()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
            {
              "company": {
                "url": "https://api.freeagent.com/v2/company",
                "id": 1,
                "name": "Test Co",
                "subdomain": "test",
                "type": "UkLimitedCompany",
                "currency": "GBP",
                "annual_accounting_periods": [
                  { "starts_on": "2020-05-01", "ends_on": "2021-04-30" }
                ]
              }
            }
            """)
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token");
        var service = new CompanyService(client);

        var company = await service.GetCompanyAsync();

        Assert.Single(company.AnnualAccountingPeriods);
        Assert.Equal(new DateOnly(2020, 5, 1), company.AnnualAccountingPeriods[0].StartsOn);
        Assert.Equal(new DateOnly(2021, 4, 30), company.AnnualAccountingPeriods[0].EndsOn);
    }
}
