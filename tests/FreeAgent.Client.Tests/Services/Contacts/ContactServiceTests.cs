using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Contacts;
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

        var page = await service.GetContactsPageAsync(page: 1, perPage: 2, view: ContactViews.All);

        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PerPage);
        Assert.Equal(5, page.Total);
        Assert.True(page.HasNextPage);
        Assert.Equal("Acme Ltd", page.Items[0].DisplayName);
        Assert.Equal("Jane Globex", page.Items[1].DisplayName);
    }

    [Fact]
    public async Task GetContactsPageAsync_DefaultView_IsActive()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("view=active", request.RequestUri!.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "contacts": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ContactService(client);

        await service.GetContactsPageAsync();
    }

    [Fact]
    public async Task GetContactsPageAsync_IncludesSortAndUpdatedSince()
    {
        var updatedSince = new DateTimeOffset(2025, 3, 15, 9, 0, 0, TimeSpan.Zero);

        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("sort=-updated_at", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("updated_since=", request.RequestUri.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "contacts": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ContactService(client);

        await service.GetContactsPageAsync(sort: "-updated_at", updatedSince: updatedSince);
    }

    [Fact]
    public async Task GetContactAsync_ReturnsContact()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/contacts/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "contact": {
                    "url": "https://api.freeagent.com/v2/contacts/42",
                    "organisation_name": "Acme Ltd",
                    "status": "Active"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ContactService(client);

        var contact = await service.GetContactAsync(42);

        Assert.Equal("Acme Ltd", contact.DisplayName);
        Assert.Equal(ContactStatus.Active, contact.Status);
    }

    [Fact]
    public async Task CreateContactAsync_PostsContactEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/contacts", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"contact\"", body, StringComparison.Ordinal);
            Assert.Contains("\"organisation_name\":\"New Co\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "contact": {
                    "url": "https://api.freeagent.com/v2/contacts/70",
                    "organisation_name": "New Co",
                    "status": "Active"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ContactService(client);

        var created = await service.CreateContactAsync(new Contact { OrganisationName = "New Co" });

        Assert.Equal("New Co", created.OrganisationName);
        Assert.Equal(ContactStatus.Active, created.Status);
    }

    [Fact]
    public async Task UpdateContactAsync_PutsContactEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/contacts/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"organisation_name\":\"Renamed\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "contact": {
                    "url": "https://api.freeagent.com/v2/contacts/42",
                    "organisation_name": "Renamed"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ContactService(client);

        var updated = await service.UpdateContactAsync(42, new Contact { OrganisationName = "Renamed" });

        Assert.Equal("Renamed", updated.OrganisationName);
    }

    [Fact]
    public async Task DeleteContactAsync_SendsDelete()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/contacts/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ContactService(client);

        await service.DeleteContactAsync(42);
    }

    [Fact]
    public async Task GetContactsPageAsync_WhenViewContainsSpaces_EscapesViewQueryParameter()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("view=clients%20only", request.RequestUri!.Query, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "contacts": [
                    { "url": "https://api.freeagent.com/v2/contacts/1", "organisation_name": "Acme Ltd" }
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

        var page = await service.GetContactsPageAsync(page: 1, perPage: 25, view: "clients only");

        Assert.Equal(1, page.Total);
        Assert.False(page.HasNextPage);
    }

    [Fact]
    public async Task GetContactsPageAsync_WhenTotalHeaderIsInvalid_FallsBackToEstimatedTotal()
    {
        var handler = new QueueHttpMessageHandler(request =>
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

            response.Headers.TryAddWithoutValidation("X-Total-Count", "invalid");
            return response;
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };

        var options = new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", options);
        var service = new ContactService(client);

        var page = await service.GetContactsPageAsync(page: 1, perPage: 2, view: ContactViews.All);

        Assert.Equal(3, page.Total);
        Assert.True(page.HasNextPage);
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
        await foreach (var contact in service.GetAllContactsAsync(perPage: 2, view: ContactViews.All))
        {
            contacts.Add(contact.DisplayName);
        }

        Assert.Equal(3, contacts.Count);
        Assert.Equal(["Acme Ltd", "Globex Corp", "Soylent Co"], contacts);
    }

    [Fact]
    public async Task GetContactAsync_WhenPayloadMissing_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "contact": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ContactService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetContactAsync(1));
    }
}
