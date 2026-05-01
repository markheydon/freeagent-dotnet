using System.Net;
using System.Net.Http;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.Contacts;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Contacts;

public class ContactServiceTests
{
    [Fact]
    public async Task GetContactsPageAsync_ReturnsPaginatedResponse()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("page=1", request.RequestUri!.Query);
            Assert.Contains("per_page=2", request.RequestUri.Query);
            Assert.Contains("view=all", request.RequestUri.Query);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "contacts": [
                                        { "url": "https://api.freeagent.com/v2/contacts/1", "organisation_name": "Acme Ltd" },
                                        { "url": "https://api.freeagent.com/v2/contacts/2", "first_name": "Jane", "last_name": "Globex" }
                  ]
                }
                """)
            };
            response.Headers.TryAddWithoutValidation("X-Total-Count", "5");
            return response;
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };

        var options = new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", options);
        var service = new ContactService(client);

        var page = await service.GetContactsPageAsync(page: 1, perPage: 2);

        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PerPage);
        Assert.Equal(5, page.Total);
        Assert.True(page.HasNextPage);
        Assert.Equal("Acme Ltd", page.Items[0].DisplayName);
        Assert.Equal("Jane Globex", page.Items[1].DisplayName);
    }

    [Fact]
    public async Task GetAllContactsAsync_IteratesAcrossPages()
    {
        var handler = new QueueHttpMessageHandler(
            request =>
            {
                Assert.Contains("page=1", request.RequestUri!.Query);
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "contacts": [
                                                { "url": "https://api.freeagent.com/v2/contacts/1", "organisation_name": "Acme Ltd" },
                                                { "url": "https://api.freeagent.com/v2/contacts/2", "organisation_name": "Globex Corp" }
                      ]
                    }
                    """)
                };
                response.Headers.TryAddWithoutValidation("X-Total-Count", "3");
                return response;
            },
            request =>
            {
                Assert.Contains("page=2", request.RequestUri!.Query);
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "contacts": [
                                                { "url": "https://api.freeagent.com/v2/contacts/3", "organisation_name": "Soylent Co" }
                      ]
                    }
                    """)
                };
                response.Headers.TryAddWithoutValidation("X-Total-Count", "3");
                return response;
            });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };

        var options = new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", options);
        var service = new ContactService(client);

        var contacts = new List<string>();
        await foreach (var contact in service.GetAllContactsAsync(perPage: 2))
        {
            contacts.Add(contact.DisplayName);
        }

        Assert.Equal(3, contacts.Count);
        Assert.Equal(["Acme Ltd", "Globex Corp", "Soylent Co"], contacts);
    }

    [Fact]
    public async Task GetAllContactsAsync_ContinuesWhenTotalHeaderMissing()
    {
        var handler = new QueueHttpMessageHandler(
            request =>
            {
                Assert.Contains("page=1", request.RequestUri!.Query);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "contacts": [
                        { "url": "https://api.freeagent.com/v2/contacts/1", "organisation_name": "Acme Ltd" },
                        { "url": "https://api.freeagent.com/v2/contacts/2", "organisation_name": "Globex Corp" }
                      ]
                    }
                    """)
                };
            },
            request =>
            {
                Assert.Contains("page=2", request.RequestUri!.Query);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "contacts": [
                        { "url": "https://api.freeagent.com/v2/contacts/3", "organisation_name": "Soylent Co" }
                      ]
                    }
                    """)
                };
            });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };

        var options = new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", options);
        var service = new ContactService(client);

        var contacts = new List<string>();
        await foreach (var contact in service.GetAllContactsAsync(perPage: 2))
        {
            contacts.Add(contact.DisplayName);
        }

        Assert.Equal(["Acme Ltd", "Globex Corp", "Soylent Co"], contacts);
    }
}
