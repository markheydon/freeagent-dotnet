using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Services.Projects;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Projects;

public class ProjectServiceTests
{
    [Fact]
    public async Task GetProjectsPageAsync_ReturnsPaginatedResponse()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("page=1", request.RequestUri!.Query);
            Assert.Contains("per_page=2", request.RequestUri.Query);
            Assert.Contains("view=active", request.RequestUri.Query);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "projects": [
                    {
                      "url": "https://api.freeagent.com/v2/projects/1",
                      "name": "Alpha",
                      "status": "Active"
                    },
                    {
                      "url": "https://api.freeagent.com/v2/projects/2",
                      "name": "Beta",
                      "status": "Completed"
                    }
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

        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        var page = await service.GetProjectsPageAsync(page: 1, perPage: 2, view: ProjectViews.Active);

        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PerPage);
        Assert.Equal(5, page.Total);
        Assert.True(page.HasNextPage);
        Assert.Equal("Alpha", page.Items[0].Name);
        Assert.Equal(ProjectStatus.Completed, page.Items[1].Status);
    }

    [Fact]
    public async Task GetProjectsPageAsync_IncludesSortContactAndNestedFilters()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("sort=-updated_at", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("contact=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fcontacts%2F2", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("nested=true", request.RequestUri.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "projects": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        await service.GetProjectsPageAsync(
            sort: "-updated_at",
            contactId: 2,
            nested: true);
    }

    [Fact]
    public async Task GetProjectsPageAsync_ContactReferenceFilter_UsesUri()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains(
                "contact=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fcontacts%2F9",
                request.RequestUri!.Query,
                StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "projects": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        await service.GetProjectsPageAsync(
            contact: ContactReference.Parse("https://api.freeagent.com/v2/contacts/9"));
    }

    [Fact]
    public async Task GetProjectAsync_ReturnsProject()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/projects/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "project": {
                    "url": "https://api.freeagent.com/v2/projects/42",
                    "name": "Probe Project",
                    "status": "Active",
                    "is_deletable": true
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        var project = await service.GetProjectAsync(42);

        Assert.Equal("Probe Project", project.Name);
        Assert.Equal(ProjectStatus.Active, project.Status);
        Assert.True(project.IsDeletable);
    }

    [Fact]
    public async Task GetProjectAsync_WithIncludeBillingContact_FetchesContact()
    {
        var requestCount = 0;
        HttpResponseMessage RouteRequest(HttpRequestMessage request)
        {
            requestCount++;
            if (request.RequestUri!.AbsolutePath.EndsWith("/projects/42", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "project": {
                        "url": "https://api.freeagent.com/v2/projects/42",
                        "contact": "https://api.freeagent.com/v2/contacts/7",
                        "contact_name": "Acme Trading",
                        "name": "Probe Project"
                      }
                    }
                    """)
                };
            }

            if (request.RequestUri!.AbsolutePath.EndsWith("/contacts/7", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "contact": {
                        "url": "https://api.freeagent.com/v2/contacts/7",
                        "organisation_name": "Acme Trading Ltd"
                      }
                    }
                    """)
                };
            }

            throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
        }

        var handler = new QueueHttpMessageHandler(RouteRequest, RouteRequest);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        var project = await service.GetProjectAsync(
            42,
            new ProjectGetOptions { IncludeBillingContact = true });

        Assert.Equal(2, requestCount);
        Assert.Equal("Acme Trading Ltd", project.Contact!.OrganisationName);
        Assert.Equal("Acme Trading", project.ContactName);
    }

    [Fact]
    public async Task GetProjectAsync_WithIncludeBillingContact_SkipsFetchWhenAlreadyNested()
    {
        var requestCount = 0;
        var handler = new QueueHttpMessageHandler(request =>
        {
            requestCount++;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "project": {
                    "url": "https://api.freeagent.com/v2/projects/42",
                    "contact": {
                      "url": "https://api.freeagent.com/v2/contacts/7",
                      "organisation_name": "Already Nested"
                    },
                    "name": "Probe Project"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        var project = await service.GetProjectAsync(
            42,
            new ProjectGetOptions { IncludeBillingContact = true });

        Assert.Equal(1, requestCount);
        Assert.Equal("Already Nested", project.Contact!.OrganisationName);
    }

    [Fact]
    public async Task CreateProjectAsync_PostsProjectEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/projects", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"project\"", body, StringComparison.Ordinal);
            Assert.Contains("\"name\":\"New Project\"", body, StringComparison.Ordinal);
            Assert.Contains("\"contact\":\"https://api.freeagent.com/v2/contacts/1\"", body, StringComparison.Ordinal);
            Assert.DoesNotContain("\"url\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "project": {
                    "url": "https://api.freeagent.com/v2/projects/70",
                    "name": "New Project",
                    "status": "Active"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        var created = await service.CreateProjectAsync(new Project
        {
            Name = "New Project",
            BillingContact = ContactReference.Parse("https://api.freeagent.com/v2/contacts/1"),
            Status = ProjectStatus.Active,
            Currency = CurrencyCode.GBP,
            BudgetUnits = ProjectBudgetUnits.Hours
        });

        Assert.Equal("New Project", created.Name);
        Assert.Equal(ProjectStatus.Active, created.Status);
    }

    [Fact]
    public async Task UpdateProjectAsync_PutsProjectEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/projects/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"name\":\"Renamed\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "project": {
                    "url": "https://api.freeagent.com/v2/projects/42",
                    "name": "Renamed"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        var updated = await service.UpdateProjectAsync(42, new Project { Name = "Renamed" });

        Assert.Equal("Renamed", updated.Name);
    }

    [Fact]
    public async Task DeleteProjectAsync_SendsDelete()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/projects/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        await service.DeleteProjectAsync(42);
    }

    [Fact]
    public async Task GetProjectsPageAsync_MissingProjectsBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetProjectsPageAsync());
    }

    [Fact]
    public async Task GetProjectsPageAsync_ContactAndContactId_Throws()
    {
        using var httpClient = new HttpClient(new HttpClientHandler()) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.GetProjectsPageAsync(
            contact: ContactReference.Parse("https://api.freeagent.com/v2/contacts/1"),
            contactId: 2));
    }

    [Fact]
    public async Task GetProjectAsync_MissingProjectBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetProjectAsync(42));
    }

    [Fact]
    public async Task CreateProjectAsync_MissingProjectBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.CreateProjectAsync(new Project { Name = "Example" }));
    }

    [Fact]
    public async Task UpdateProjectAsync_MissingProjectBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.UpdateProjectAsync(42, new Project { Name = "Example" }));
    }

    [Fact]
    public async Task GetAllProjectsAsync_IteratesPagesUntilComplete()
    {
        var page = 0;
        var handler = new QueueHttpMessageHandler(request =>
        {
            page++;
            var projects = page == 1
                ? """{ "projects": [ { "url": "https://api.freeagent.com/v2/projects/1", "name": "One" } ] }"""
                : """{ "projects": [] }""";

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(projects)
            };

            if (page == 1)
            {
                response.Headers.TryAddWithoutValidation("X-Total-Count", "1");
            }

            return response;
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectService(client);

        var names = new List<string>();
        await foreach (var project in service.GetAllProjectsAsync(perPage: 1))
        {
            names.Add(project.Name!);
        }

        Assert.Equal(["One"], names);
    }
}
