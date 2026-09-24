using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Notes;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Services.Notes;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Notes;

public class NoteServiceTests
{
    [Fact]
    public async System.Threading.Tasks.Task ListContactNotesAsync_ReturnsNotes()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains(
                "contact=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fcontacts%2F5",
                request.RequestUri!.Query,
                StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "notes": [
                    {
                      "url": "https://api.freeagent.com/v2/notes/1",
                      "note": "Contact note",
                      "parent_url": "https://api.freeagent.com/v2/contacts/5"
                    }
                  ]
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new NoteService(client);

        var notes = await service.ListContactNotesAsync(contactId: 5);

        Assert.Single(notes);
        Assert.Equal("Contact note", notes[0].Content);
        Assert.Equal(5, notes[0].ContactId);
    }

    [Fact]
    public async System.Threading.Tasks.Task ListProjectNotesAsync_ProjectReferenceFilter_UsesUri()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains(
                "project=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fprojects%2F9",
                request.RequestUri!.Query,
                StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "notes": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new NoteService(client);

        await service.ListProjectNotesAsync(
            project: ProjectReference.Parse("https://api.freeagent.com/v2/projects/9"));
    }

    [Fact]
    public async System.Threading.Tasks.Task ListContactNotesAsync_MissingParentFilter_ThrowsArgumentException()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new NoteService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.ListContactNotesAsync());
    }

    [Fact]
    public async System.Threading.Tasks.Task ListContactNotesAsync_BothContactFilters_ThrowsArgumentException()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new NoteService(client);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ListContactNotesAsync(
                contact: ContactReference.Parse("https://api.freeagent.com/v2/contacts/1"),
                contactId: 1));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetNoteAsync_ReturnsNote()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/notes/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "note": {
                    "url": "https://api.freeagent.com/v2/notes/42",
                    "note": "Probe note",
                    "parent_url": "https://api.freeagent.com/v2/projects/7",
                    "author": "Development Team"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new NoteService(client);

        var note = await service.GetNoteAsync(42);

        Assert.Equal("Probe note", note.Content);
        Assert.Equal(7, note.ProjectId);
        Assert.Equal("Development Team", note.Author);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetNoteAsync_WithIncludeParentProject_FetchesProject()
    {
        var requestCount = 0;
        HttpResponseMessage RouteRequest(HttpRequestMessage request)
        {
            requestCount++;
            if (request.RequestUri!.AbsolutePath.EndsWith("/notes/42", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "note": {
                        "url": "https://api.freeagent.com/v2/notes/42",
                        "note": "Probe note",
                        "parent_url": "https://api.freeagent.com/v2/projects/7"
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
        var service = new NoteService(client);

        var note = await service.GetNoteAsync(
            42,
            new NoteGetOptions { IncludeParentProject = true });

        Assert.Equal(2, requestCount);
        Assert.Equal("Parent Project", note.Project!.Name);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateContactNoteAsync_PostsNoteEnvelopeWithContactQuery()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/notes", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            Assert.Contains(
                "contact=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fcontacts%2F3",
                request.RequestUri.Query,
                StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"note\"", body, StringComparison.Ordinal);
            Assert.Contains("\"note\":\"Contact note text\"", body, StringComparison.Ordinal);
            Assert.DoesNotContain("\"parent_url\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "note": {
                    "url": "https://api.freeagent.com/v2/notes/10",
                    "note": "Contact note text",
                    "parent_url": "https://api.freeagent.com/v2/contacts/3"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new NoteService(client);

        var created = await service.CreateContactNoteAsync(
            3,
            CreateContactNoteRequest.Create("Contact note text"));

        Assert.Equal("Contact note text", created.Content);
        Assert.Equal(3, created.ContactId);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateProjectNoteAsync_PostsNoteEnvelopeWithProjectQuery()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Contains(
                "project=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fprojects%2F8",
                request.RequestUri!.Query,
                StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"note\":\"Project note text\"", body, StringComparison.Ordinal);
            Assert.DoesNotContain("\"parent_url\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "note": {
                    "url": "https://api.freeagent.com/v2/notes/11",
                    "note": "Project note text",
                    "parent_url": "https://api.freeagent.com/v2/project/8"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new NoteService(client);

        var created = await service.CreateProjectNoteAsync(
            ProjectReference.Parse("https://api.freeagent.com/v2/projects/8"),
            CreateProjectNoteRequest.Create("Project note text"));

        Assert.Equal("Project note text", created.Content);
        Assert.Equal(8, created.ProjectId);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateNoteAsync_PutsNoteEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/notes/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"note\":\"Updated text\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "note": {
                    "url": "https://api.freeagent.com/v2/notes/42",
                    "note": "Updated text"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new NoteService(client);

        var updated = await service.UpdateNoteAsync(42, UpdateNoteRequest.Create("Updated text"));

        Assert.Equal("Updated text", updated.Content);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteNoteAsync_DeletesNote()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/notes/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new NoteService(client);

        await service.DeleteNoteAsync(42);
    }

    [Fact]
    public async System.Threading.Tasks.Task ListContactNotesAsync_MissingNotesBranch_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new NoteService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListContactNotesAsync(contactId: 1));
    }
}
