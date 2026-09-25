using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Models.Tasks;
using FreeAgent.Client.Services.Tasks;
using FreeAgent.Client.Tests.TestSupport;
using FreeAgentProjectTaskStatus = FreeAgent.Client.Models.Tasks.ProjectTaskStatus;

namespace FreeAgent.Client.Tests.Services.Tasks;

public class ProjectTaskServiceTests
{
    [Fact]
    public async System.Threading.Tasks.Task ListAsync_ReturnsPaginatedResponse()
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
                  "tasks": [
                    {
                      "url": "https://api.freeagent.com/v2/tasks/1",
                      "name": "Alpha",
                      "status": "Active"
                    },
                    {
                      "url": "https://api.freeagent.com/v2/tasks/2",
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
        var service = new ProjectTaskService(client);

        var page = await service.ListAsync(page: 1, perPage: 2, view: ProjectTaskViews.Active);

        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PerPage);
        Assert.Equal(5, page.Total);
        Assert.True(page.HasNextPage);
        Assert.Equal("Alpha", page.Items[0].Name);
        Assert.Equal(FreeAgentProjectTaskStatus.Completed, page.Items[1].Status);
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAsync_IncludesSortUpdatedSinceAndProjectFilters()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("sort=-updated_at", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("updated_since=2017-04-06", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("project=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fprojects%2F2", request.RequestUri.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "tasks": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        await service.ListAsync(
            sort: "-updated_at",
            updatedSince: new DateOnly(2017, 4, 6),
            projectId: 2);
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAsync_ProjectReferenceFilter_UsesUri()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains(
                "project=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fprojects%2F9",
                request.RequestUri!.Query,
                StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "tasks": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        await service.ListAsync(
            project: ProjectReference.Parse("https://api.freeagent.com/v2/projects/9"));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetProjectTaskAsync_ReturnsTask()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/tasks/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "task": {
                    "url": "https://api.freeagent.com/v2/tasks/42",
                    "name": "Probe Task",
                    "status": "Active",
                    "is_deletable": true
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        var task = await service.GetProjectTaskAsync(42);

        Assert.Equal("Probe Task", task.Name);
        Assert.Equal(FreeAgentProjectTaskStatus.Active, task.Status);
        Assert.True(task.IsDeletable);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetProjectTaskAsync_WithIncludeProject_FetchesProject()
    {
        var requestCount = 0;
        HttpResponseMessage RouteRequest(HttpRequestMessage request)
        {
            requestCount++;
            if (request.RequestUri!.AbsolutePath.EndsWith("/tasks/42", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "task": {
                        "url": "https://api.freeagent.com/v2/tasks/42",
                        "project": "https://api.freeagent.com/v2/projects/7",
                        "name": "Probe Task"
                      }
                    }
                    """)
                };
            }

            if (request.RequestUri!.AbsolutePath.EndsWith("/projects/7", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "project": {
                        "url": "https://api.freeagent.com/v2/projects/7",
                        "name": "Parent Project"
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
        var service = new ProjectTaskService(client);

        var task = await service.GetProjectTaskAsync(
            42,
            new ProjectTaskGetOptions { IncludeProject = true });

        Assert.Equal(2, requestCount);
        Assert.Equal("Parent Project", task.Project!.Name);
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAsync_EmptyTasksArray_ReturnsEmptyPage()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "tasks": [] }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        var page = await service.ListAsync();

        Assert.Empty(page.Items);
        Assert.False(page.HasNextPage);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetProjectTaskAsync_WithIncludeProject_SkipsFetchWhenProjectAlreadyExpanded()
    {
        var requestCount = 0;
        var handler = new QueueHttpMessageHandler(request =>
        {
            requestCount++;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "task": {
                    "url": "https://api.freeagent.com/v2/tasks/42",
                    "project": {
                      "url": "https://api.freeagent.com/v2/projects/7",
                      "name": "Already Nested"
                    },
                    "name": "Probe Task"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        var task = await service.GetProjectTaskAsync(
            42,
            new ProjectTaskGetOptions { IncludeProject = true });

        Assert.Equal(1, requestCount);
        Assert.Equal("Already Nested", task.Project!.Name);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateProjectTaskAsync_ProjectReference_PostsTaskEnvelopeWithProjectQuery()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains(
                "project=https%3A%2F%2Fapi.sandbox.freeagent.com%2Fv2%2Fprojects%2F3",
                request.RequestUri!.Query,
                StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "task": {
                    "url": "https://api.sandbox.freeagent.com/v2/tasks/5",
                    "name": "Sandbox Task"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.sandbox.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        var created = await service.CreateProjectTaskAsync(
            ProjectReference.Parse("https://api.sandbox.freeagent.com/v2/projects/3"),
            new ProjectTask { Name = "Sandbox Task" });

        Assert.Equal("Sandbox Task", created.Name);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateProjectTaskAsync_PostsTaskEnvelopeWithProjectQuery()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/tasks", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            Assert.Contains("project=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fprojects%2F1", request.RequestUri.Query, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"task\"", body, StringComparison.Ordinal);
            Assert.Contains("\"name\":\"New Task\"", body, StringComparison.Ordinal);
            Assert.Contains("\"is_billable\":true", body, StringComparison.Ordinal);
            Assert.DoesNotContain("\"url\"", body, StringComparison.Ordinal);
            Assert.DoesNotContain("\"project\"", body, StringComparison.Ordinal);
            Assert.DoesNotContain("\"currency\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "task": {
                    "url": "https://api.freeagent.com/v2/tasks/70",
                    "name": "New Task",
                    "status": "Active"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        var created = await service.CreateProjectTaskAsync(1, new ProjectTask
        {
            Name = "New Task",
            IsBillable = true,
            Status = FreeAgentProjectTaskStatus.Active
        });

        Assert.Equal("New Task", created.Name);
        Assert.Equal(FreeAgentProjectTaskStatus.Active, created.Status);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateProjectTaskAsync_PutsTaskEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/tasks/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"name\":\"Renamed\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "task": {
                    "url": "https://api.freeagent.com/v2/tasks/42",
                    "name": "Renamed"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        var updated = await service.UpdateProjectTaskAsync(42, new ProjectTask { Name = "Renamed" });

        Assert.Equal("Renamed", updated.Name);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteProjectTaskAsync_SendsDelete()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/tasks/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        await service.DeleteProjectTaskAsync(42);
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAsync_MissingTasksBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAsync_ProjectAndProjectId_Throws()
    {
        using var httpClient = new HttpClient(new HttpClientHandler()) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.ListAsync(
            project: ProjectReference.Parse("https://api.freeagent.com/v2/projects/1"),
            projectId: 2));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetProjectTaskAsync_MissingTaskBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetProjectTaskAsync(42));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateProjectTaskAsync_MissingTaskBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.CreateProjectTaskAsync(1, new ProjectTask { Name = "Example" }));
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateProjectTaskAsync_MissingTaskBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.UpdateProjectTaskAsync(42, new ProjectTask { Name = "Example" }));
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAutoPagingAsync_IteratesPagesUntilComplete()
    {
        var page = 0;
        var handler = new QueueHttpMessageHandler(request =>
        {
            page++;
            var tasks = page == 1
                ? """{ "tasks": [ { "url": "https://api.freeagent.com/v2/tasks/1", "name": "One" } ] }"""
                : """{ "tasks": [] }""";

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(tasks)
            };

            if (page == 1)
            {
                response.Headers.TryAddWithoutValidation("X-Total-Count", "1");
            }

            return response;
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ProjectTaskService(client);

        var names = new List<string>();
        await foreach (var task in service.ListAutoPagingAsync(perPage: 1))
        {
            names.Add(task.Name!);
        }

        Assert.Equal(["One"], names);
    }
}
