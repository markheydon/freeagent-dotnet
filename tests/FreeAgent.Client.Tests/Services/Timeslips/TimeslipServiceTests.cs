using System.Globalization;
using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Models.Timeslips;
using FreeAgent.Client.Services.Timeslips;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Timeslips;

public class TimeslipServiceTests
{
    [Fact]
    public async System.Threading.Tasks.Task ListAsync_ReturnsPaginatedResponse()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("page=1", request.RequestUri!.Query);
            Assert.Contains("per_page=2", request.RequestUri.Query);
            Assert.Contains("view=unbilled", request.RequestUri.Query);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "timeslips": [
                    {
                      "url": "https://api.freeagent.com/v2/timeslips/1",
                      "dated_on": "2011-08-15",
                      "hours": "12.0"
                    },
                    {
                      "url": "https://api.freeagent.com/v2/timeslips/2",
                      "dated_on": "2011-08-16",
                      "hours": "1.5"
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
        var service = new TimeslipService(client);

        var page = await service.ListAsync(page: 1, perPage: 2, view: TimeslipViews.Unbilled);

        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PerPage);
        Assert.Equal(5, page.Total);
        Assert.True(page.HasNextPage);
        Assert.Equal(new DateOnly(2011, 8, 15), page.Items[0].DatedOn);
        Assert.Equal(12.0m, page.Items[0].Hours);
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAsync_IncludesDateUpdatedSinceNestedAndFilters()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("from_date=2012-01-01", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("to_date=2012-03-31", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("updated_since=2017-05-22T09%3A00%3A00.000Z", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("nested=true", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("user=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fusers%2F2", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("task=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Ftasks%2F3", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("project=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fprojects%2F4", request.RequestUri.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "timeslips": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await service.ListAsync(
            fromDate: new DateOnly(2012, 1, 1),
            toDate: new DateOnly(2012, 3, 31),
            updatedSince: DateTimeOffset.Parse("2017-05-22T09:00:00.000Z", CultureInfo.InvariantCulture),
            nested: true,
            userId: 2,
            taskId: 3,
            projectId: 4);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTimeslipAsync_ReturnsTimeslip()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/timeslips/25", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "timeslip": {
                    "url": "https://api.freeagent.com/v2/timeslips/25",
                    "dated_on": "2011-08-15",
                    "hours": "12.0",
                    "billed_on_invoice": "https://api.freeagent.com/v2/invoices/7",
                    "timer": {
                      "running": true,
                      "start_from": "2011-08-16T01:32:00Z"
                    },
                    "created_at": "2011-08-16T13:32:00Z",
                    "updated_at": "2011-08-16T13:32:00Z"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        var timeslip = await service.GetTimeslipAsync(25);

        Assert.Equal(25, timeslip.ResourceId);
        Assert.Equal(12.0m, timeslip.Hours);
        Assert.Equal(7, timeslip.BilledOnInvoiceId);
        Assert.True(timeslip.Timer!.Running);
        Assert.Equal(DateTimeOffset.Parse("2011-08-16T01:32:00Z", CultureInfo.InvariantCulture), timeslip.Timer.StartFrom);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTimeslipAsync_PostsTimeslipEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/timeslips", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"timeslip\"", body, StringComparison.Ordinal);
            Assert.Contains("\"task\":\"https://api.freeagent.com/v2/tasks/1\"", body, StringComparison.Ordinal);
            Assert.Contains("\"user\":\"https://api.freeagent.com/v2/users/1\"", body, StringComparison.Ordinal);
            Assert.Contains("\"project\":\"https://api.freeagent.com/v2/projects/1\"", body, StringComparison.Ordinal);
            Assert.Contains("\"dated_on\":\"2011-08-15\"", body, StringComparison.Ordinal);
            Assert.Contains("\"hours\":1.5", body, StringComparison.Ordinal);
            Assert.DoesNotContain("billed_on_invoice", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "timeslip": {
                    "url": "https://api.freeagent.com/v2/timeslips/25",
                    "dated_on": "2011-08-15",
                    "hours": "1.5"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        var created = await service.CreateTimeslipAsync(new Timeslip
        {
            LinkedTask = TaskReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
            LinkedUser = UserReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
            LinkedProject = ProjectReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
            DatedOn = new DateOnly(2011, 8, 15),
            Hours = 1.5m
        });

        Assert.Equal(1.5m, created.Hours);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTimeslipsAsync_PostsBatchEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"timeslips\"", body, StringComparison.Ordinal);
            Assert.DoesNotContain("\"timeslip\"", body, StringComparison.Ordinal);
            Assert.Contains("\"dated_on\":\"2011-08-15\"", body, StringComparison.Ordinal);
            Assert.Contains("\"dated_on\":\"2011-08-14\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "timeslips": [
                    {
                      "url": "https://api.freeagent.com/v2/timeslips/25",
                      "dated_on": "2011-08-15",
                      "hours": "12.0"
                    },
                    {
                      "url": "https://api.freeagent.com/v2/timeslips/26",
                      "dated_on": "2011-08-14",
                      "hours": "12.0"
                    }
                  ]
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        var created = await service.CreateTimeslipsAsync([
            new Timeslip
            {
                LinkedTask = TaskReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
                LinkedUser = UserReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
                LinkedProject = ProjectReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
                DatedOn = new DateOnly(2011, 8, 15),
                Hours = 12.0m
            },
            new Timeslip
            {
                LinkedTask = TaskReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
                LinkedUser = UserReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
                LinkedProject = ProjectReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
                DatedOn = new DateOnly(2011, 8, 14),
                Hours = 12.0m
            }
        ]);

        Assert.Equal(2, created.Count);
        Assert.Equal(new DateOnly(2011, 8, 14), created[1].DatedOn);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTimeslipAsync_PutsTimeslipEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/timeslips/25", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"hours\":2.5", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "timeslip": {
                    "url": "https://api.freeagent.com/v2/timeslips/25",
                    "hours": "2.5"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        var updated = await service.UpdateTimeslipAsync(25, new Timeslip { Hours = 2.5m });

        Assert.Equal(2.5m, updated.Hours);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteTimeslipAsync_SendsDelete()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/timeslips/25", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await service.DeleteTimeslipAsync(25);
    }

    [Fact]
    public async System.Threading.Tasks.Task StartTimerAsync_PostsTimerEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/timeslips/25/timer", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "timeslip": {
                    "url": "https://api.freeagent.com/v2/timeslips/25",
                    "timer": { "running": true, "start_from": "2011-08-16T01:32:00Z" }
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        var timeslip = await service.StartTimerAsync(25);

        Assert.True(timeslip.Timer!.Running);
    }

    [Fact]
    public async System.Threading.Tasks.Task StopTimerAsync_DeletesTimerEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/timeslips/25/timer", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "timeslip": {
                    "url": "https://api.freeagent.com/v2/timeslips/25",
                    "hours": "12.5"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        var timeslip = await service.StopTimerAsync(25);

        Assert.Equal(12.5m, timeslip.Hours);
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAsync_MissingTimeslipsBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAutoPagingAsync_YieldsAllPagesUntilComplete()
    {
        var handler = new QueueHttpMessageHandler(
            request =>
            {
                Assert.Contains("page=1", request.RequestUri!.Query);
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "timeslips": [
                        { "url": "https://api.freeagent.com/v2/timeslips/1" },
                        { "url": "https://api.freeagent.com/v2/timeslips/2" }
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
                      "timeslips": [
                        { "url": "https://api.freeagent.com/v2/timeslips/3" }
                      ]
                    }
                    """)
                };
                response.Headers.TryAddWithoutValidation("X-Total-Count", "3");
                return response;
            });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        var ids = new List<long>();
        await foreach (var timeslip in service.ListAutoPagingAsync(perPage: 2))
        {
            ids.Add(timeslip.ResourceId);
        }

        Assert.Equal([1, 2, 3], ids);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTimeslipAsync_WithNested_IncludesQueryParam()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("nested=true", request.RequestUri!.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "timeslip": {
                    "url": "https://api.freeagent.com/v2/timeslips/25",
                    "hours": "1.0"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await service.GetTimeslipAsync(25, nested: true, options: null);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTimeslipAsync_WithHydrationOptions_FetchesLinkedResources()
    {
        var requestCount = 0;
        HttpResponseMessage RouteRequest(HttpRequestMessage request)
        {
            requestCount++;
            if (request.RequestUri!.AbsolutePath.EndsWith("/timeslips/25", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "timeslip": {
                        "url": "https://api.freeagent.com/v2/timeslips/25",
                        "task": "https://api.freeagent.com/v2/tasks/3",
                        "project": "https://api.freeagent.com/v2/projects/4",
                        "user": "https://api.freeagent.com/v2/users/2"
                      }
                    }
                    """)
                };
            }

            if (request.RequestUri.AbsolutePath.EndsWith("/tasks/3", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "task": {
                        "url": "https://api.freeagent.com/v2/tasks/3",
                        "name": "Linked Task"
                      }
                    }
                    """)
                };
            }

            if (request.RequestUri.AbsolutePath.EndsWith("/projects/4", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "project": {
                        "url": "https://api.freeagent.com/v2/projects/4",
                        "name": "Linked Project"
                      }
                    }
                    """)
                };
            }

            if (request.RequestUri.AbsolutePath.EndsWith("/users/2", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "user": {
                        "url": "https://api.freeagent.com/v2/users/2",
                        "first_name": "Ada",
                        "last_name": "Lovelace"
                      }
                    }
                    """)
                };
            }

            throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
        }

        var handler = new QueueHttpMessageHandler(
            RouteRequest,
            RouteRequest,
            RouteRequest,
            RouteRequest);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        var timeslip = await service.GetTimeslipAsync(
            25,
            new TimeslipGetOptions
            {
                IncludeTask = true,
                IncludeProject = true,
                IncludeUser = true
            });

        Assert.Equal(4, requestCount);
        Assert.Equal("Linked Task", timeslip.Task!.Name);
        Assert.Equal("Linked Project", timeslip.Project!.Name);
        Assert.Equal("Ada", timeslip.User!.FirstName);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTimeslipAsync_WithHydrationOptions_SkipsFetchWhenLinksAlreadyExpanded()
    {
        var requestCount = 0;
        var handler = new QueueHttpMessageHandler(request =>
        {
            requestCount++;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "timeslip": {
                    "url": "https://api.freeagent.com/v2/timeslips/25",
                    "task": {
                      "url": "https://api.freeagent.com/v2/tasks/3",
                      "name": "Already Nested Task"
                    },
                    "project": {
                      "url": "https://api.freeagent.com/v2/projects/4",
                      "name": "Already Nested Project"
                    },
                    "user": {
                      "url": "https://api.freeagent.com/v2/users/2",
                      "first_name": "Already",
                      "last_name": "Nested"
                    }
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        var timeslip = await service.GetTimeslipAsync(
            25,
            new TimeslipGetOptions
            {
                IncludeTask = true,
                IncludeProject = true,
                IncludeUser = true
            });

        Assert.Equal(1, requestCount);
        Assert.Equal("Already Nested Task", timeslip.Task!.Name);
        Assert.Equal("Already Nested Project", timeslip.Project!.Name);
        Assert.Equal("Already", timeslip.User!.FirstName);
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAsync_UserAndUserId_Throws()
    {
        using var httpClient = new HttpClient(new HttpClientHandler()) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.ListAsync(
            user: UserReference.Parse("https://api.freeagent.com/v2/users/1"),
            userId: 2));
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAsync_TaskAndTaskId_Throws()
    {
        using var httpClient = new HttpClient(new HttpClientHandler()) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.ListAsync(
            task: TaskReference.Parse("https://api.freeagent.com/v2/tasks/1"),
            taskId: 2));
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAsync_ProjectAndProjectId_Throws()
    {
        using var httpClient = new HttpClient(new HttpClientHandler()) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.ListAsync(
            project: ProjectReference.Parse("https://api.freeagent.com/v2/projects/1"),
            projectId: 2));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTimeslipAsync_MissingTimeslipBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetTimeslipAsync(25));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTimeslipAsync_MissingTimeslipBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.CreateTimeslipAsync(new Timeslip
        {
            LinkedTask = TaskReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
            LinkedUser = UserReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
            LinkedProject = ProjectReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
            DatedOn = new DateOnly(2011, 8, 15),
            Hours = 1.5m
        }));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTimeslipsAsync_EmptyList_Throws()
    {
        using var httpClient = new HttpClient(new HttpClientHandler()) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateTimeslipsAsync([]));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTimeslipsAsync_MissingTimeslipsBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.CreateTimeslipsAsync([
            new Timeslip
            {
                LinkedTask = TaskReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
                LinkedUser = UserReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
                LinkedProject = ProjectReference.ForEnvironment(FreeAgentEnvironment.Production, 1),
                DatedOn = new DateOnly(2011, 8, 15),
                Hours = 1.5m
            }
        ]));
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTimeslipAsync_MissingTimeslipBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.UpdateTimeslipAsync(25, new Timeslip { Hours = 2.5m }));
    }

    [Fact]
    public async System.Threading.Tasks.Task StartTimerAsync_MissingTimeslipBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.StartTimerAsync(25));
    }

    [Fact]
    public async System.Threading.Tasks.Task StopTimerAsync_MissingTimeslipBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.StopTimerAsync(25));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTimeslipAsync_WithHydrationOptions_ThrowsWhenLinkedTaskMissing()
    {
        var handler = new QueueHttpMessageHandler(
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "timeslip": {
                    "url": "https://api.freeagent.com/v2/timeslips/25",
                    "task": "https://api.freeagent.com/v2/tasks/3"
                  }
                }
                """)
            },
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetTimeslipAsync(
            25,
            new TimeslipGetOptions { IncludeTask = true }));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTimeslipAsync_WithHydrationOptions_ThrowsWhenLinkedProjectMissing()
    {
        var handler = new QueueHttpMessageHandler(
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "timeslip": {
                    "url": "https://api.freeagent.com/v2/timeslips/25",
                    "project": "https://api.freeagent.com/v2/projects/4"
                  }
                }
                """)
            },
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetTimeslipAsync(
            25,
            new TimeslipGetOptions { IncludeProject = true }));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTimeslipAsync_WithHydrationOptions_ThrowsWhenLinkedUserMissing()
    {
        var handler = new QueueHttpMessageHandler(
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "timeslip": {
                    "url": "https://api.freeagent.com/v2/timeslips/25",
                    "user": "https://api.freeagent.com/v2/users/2"
                  }
                }
                """)
            },
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new TimeslipService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetTimeslipAsync(
            25,
            new TimeslipGetOptions { IncludeUser = true }));
    }
}
