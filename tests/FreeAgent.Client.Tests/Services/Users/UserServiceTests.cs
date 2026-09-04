using System.Globalization;
using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Users;
using FreeAgent.Client.Services.Users;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Users;

public class UserServiceTests
{
    [Fact]
    public async Task GetUsersAsync_ReturnsUsers()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("view=all", request.RequestUri!.Query, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "users": [
                    {
                      "url": "https://api.freeagent.com/v2/users/1",
                      "first_name": "Development",
                      "last_name": "Team",
                      "email": "dev@example.com",
                      "role": "Director",
                      "permission_level": 8
                    }
                  ]
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        var users = await service.GetUsersAsync(view: UserViews.All);

        Assert.Single(users);
        Assert.Equal("Development Team", users[0].DisplayName);
        Assert.Equal(UserRole.Director, users[0].Role);
        Assert.Equal(UserPermissionLevel.Full, users[0].PermissionLevel);
    }

    [Fact]
    public async Task GetUsersAsync_DefaultView_IsAll()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("view=all", request.RequestUri!.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "users": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        await service.GetUsersAsync();
    }

    [Fact]
    public async Task GetUserAsync_ReturnsUser()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/users/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "user": {
                    "url": "https://api.freeagent.com/v2/users/42",
                    "first_name": "Jane",
                    "last_name": "Doe",
                    "email": "jane@example.com",
                    "role": "Employee"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        var user = await service.GetUserAsync(42);

        Assert.Equal("Jane Doe", user.DisplayName);
        Assert.Equal(UserRole.Employee, user.Role);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ReturnsCurrentUser()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/users/me", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "user": {
                    "url": "https://api.freeagent.com/v2/users/1",
                    "first_name": "Current",
                    "last_name": "User",
                    "email": "me@example.com",
                    "role": "Owner"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        var user = await service.GetCurrentUserAsync();

        Assert.Equal("Current User", user.DisplayName);
        Assert.Equal(UserRole.Owner, user.Role);
    }

    [Fact]
    public async Task CreateUserAsync_PostsUserEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/users", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"user\"", body, StringComparison.Ordinal);
            Assert.Contains("\"email\":\"new@example.com\"", body, StringComparison.Ordinal);
            Assert.Contains("\"role\":\"Employee\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "user": {
                    "url": "https://api.freeagent.com/v2/users/70",
                    "first_name": "New",
                    "last_name": "User",
                    "email": "new@example.com",
                    "role": "Employee"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        var created = await service.CreateUserAsync(new User
        {
            FirstName = "New",
            LastName = "User",
            Email = "new@example.com",
            Role = UserRole.Employee,
            OpeningMileage = 0
        });

        Assert.Equal("New User", created.DisplayName);
        Assert.Equal(UserRole.Employee, created.Role);
    }

    [Fact]
    public async Task UpdateUserAsync_PutsUserEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/users/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"last_name\":\"Renamed\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "user": {
                    "url": "https://api.freeagent.com/v2/users/42",
                    "first_name": "Jane",
                    "last_name": "Renamed"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        var updated = await service.UpdateUserAsync(42, new User { LastName = "Renamed" });

        Assert.Equal("Jane Renamed", updated.DisplayName);
    }

    [Fact]
    public async Task UpdateCurrentUserAsync_PutsUserMeEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/users/me", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"opening_mileage\":120", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "user": {
                    "url": "https://api.freeagent.com/v2/users/1",
                    "first_name": "Current",
                    "last_name": "User",
                    "opening_mileage": 120
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        var updated = await service.UpdateCurrentUserAsync(new User { OpeningMileage = 120 });

        Assert.Equal(120m, updated.OpeningMileage);
    }

    [Fact]
    public async Task DeleteUserAsync_SendsDelete()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/users/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        await service.DeleteUserAsync(42);
    }

    [Fact]
    public async Task GetUsersAsync_WhenUsersMissing_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "users": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetUsersAsync());
    }

    [Fact]
    public async Task GetUserAsync_WhenPayloadMissing_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "user": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetUserAsync(1));
    }

    [Fact]
    public async Task CreateUserAsync_DoesNotSerializeReadOnlyFields()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.DoesNotContain("\"url\"", body, StringComparison.Ordinal);
            Assert.DoesNotContain("created_at", body, StringComparison.Ordinal);
            Assert.DoesNotContain("updated_at", body, StringComparison.Ordinal);
            Assert.DoesNotContain("current_payroll_profile", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "user": {
                    "url": "https://api.freeagent.com/v2/users/70",
                    "first_name": "New",
                    "last_name": "User",
                    "email": "new@example.com",
                    "role": "Employee"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        var user = new User
        {
            Url = "https://api.freeagent.com/v2/users/70",
            FirstName = "New",
            LastName = "User",
            Email = "new@example.com",
            Role = UserRole.Employee,
            CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z", CultureInfo.InvariantCulture),
            CurrentPayrollProfile = new CurrentPayrollProfile { TotalPayInPreviousEmployment = 100m }
        };

        await service.CreateUserAsync(user);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WhenPayloadMissing_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "user": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetCurrentUserAsync());
    }

    [Fact]
    public async Task CreateUserAsync_WhenPayloadMissing_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent("""{ "user": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.CreateUserAsync(new User
        {
            FirstName = "New",
            LastName = "User",
            Email = "new@example.com",
            Role = UserRole.Employee,
            OpeningMileage = 0
        }));
    }

    [Fact]
    public async Task UpdateUserAsync_WhenPayloadMissing_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "user": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.UpdateUserAsync(42, new User { LastName = "Renamed" }));
    }

    [Fact]
    public async Task UpdateCurrentUserAsync_WhenPayloadMissing_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "user": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new UserService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.UpdateCurrentUserAsync(new User { OpeningMileage = 120 }));
    }
}
