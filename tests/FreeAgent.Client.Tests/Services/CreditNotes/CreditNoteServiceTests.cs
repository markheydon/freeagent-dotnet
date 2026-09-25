using System.Net;
using System.Net.Http;
using System.Text;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.CreditNotes;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Services.CreditNotes;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.CreditNotes;

public class CreditNoteServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsPaginatedResponse()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("page=1", request.RequestUri!.Query);
            Assert.Contains("per_page=2", request.RequestUri.Query);
            Assert.Contains("view=open", request.RequestUri.Query);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_notes": [
                    {
                      "url": "https://api.freeagent.com/v2/credit_notes/1",
                      "reference": "001",
                      "status": "Open"
                    },
                    {
                      "url": "https://api.freeagent.com/v2/credit_notes/2",
                      "reference": "002",
                      "status": "Draft"
                    }
                  ]
                }
                """)
            };
            response.Headers.TryAddWithoutValidation("X-Total-Count", "5");
            return response;
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        var page = await service.ListAsync(page: 1, perPage: 2, view: CreditNoteViews.Open);

        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PerPage);
        Assert.Equal(5, page.Total);
        Assert.True(page.HasNextPage);
        Assert.Equal("001", page.Items[0].Reference);
        Assert.Equal(CreditNoteStatus.Draft, page.Items[1].Status);
    }

    [Fact]
    public async Task ListAsync_IncludesFiltersAndNestedItems()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("sort=-updated_at", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("updated_since=", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("contact=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fcontacts%2F2", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("project=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fprojects%2F3", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("nested_credit_note_items=true", request.RequestUri.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "credit_notes": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        await service.ListAsync(
            sort: $"-{CreditNoteSortOptions.UpdatedAt}",
            updatedSince: new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            contactId: 2,
            projectId: 3,
            nestedCreditNoteItems: true);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public async Task ListAsync_InvalidPerPage_Throws(int perPage)
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.ListAsync(perPage: perPage));
    }

    [Fact]
    public async Task ListAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "credit_notes": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
    }

    [Fact]
    public async Task ListAutoPagingAsync_YieldsAllPages()
    {
        var page = 0;
        Func<HttpRequestMessage, HttpResponseMessage> respond = request =>
        {
            page++;
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(page == 1
                    ? """
                      {
                        "credit_notes": [
                          { "url": "https://api.freeagent.com/v2/credit_notes/1" },
                          { "url": "https://api.freeagent.com/v2/credit_notes/2" }
                        ]
                      }
                      """
                    : """
                      {
                        "credit_notes": [
                          { "url": "https://api.freeagent.com/v2/credit_notes/3" }
                        ]
                      }
                      """)
            };
            if (page == 1)
            {
                response.Headers.TryAddWithoutValidation("X-Total-Count", "3");
            }

            return response;
        };

        var handler = new QueueHttpMessageHandler(respond, respond);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        var ids = new List<long>();
        await foreach (var creditNote in service.ListAutoPagingAsync(perPage: 2))
        {
            ids.Add(creditNote.ResourceId);
        }

        Assert.Equal([1, 2, 3], ids);
    }

    [Fact]
    public async Task GetCreditNoteAsync_ReturnsCreditNote()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/credit_notes/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_note": {
                    "url": "https://api.freeagent.com/v2/credit_notes/42",
                    "reference": "007",
                    "status": "Draft"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        var creditNote = await service.GetCreditNoteAsync(42);

        Assert.Equal("007", creditNote.Reference);
        Assert.Equal(CreditNoteStatus.Draft, creditNote.Status);
    }

    [Fact]
    public async Task GetCreditNoteAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "credit_note": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetCreditNoteAsync(1));
    }

    [Fact]
    public async Task GetCreditNotePdfAsync_InvalidBase64_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
            {
              "pdf": {
                "content": "not-valid-base64!!!"
              }
            }
            """)
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetCreditNotePdfAsync(7));

        Assert.Contains("base64", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetCreditNotePdfAsync_DecodesBase64Content()
    {
        var expected = new byte[] { 0x25, 0x50, 0x44, 0x46 };
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/credit_notes/7/pdf", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($$"""
                {
                  "pdf": {
                    "content": "{{Convert.ToBase64String(expected)}}"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        var pdf = await service.GetCreditNotePdfAsync(7);

        Assert.Equal(expected, pdf);
    }

    [Fact]
    public async Task CreateCreditNoteAsync_PostsCreditNotePayload()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/credit_notes", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "credit_note": {
                    "url": "https://api.freeagent.com/v2/credit_notes/3",
                    "status": "Draft"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        var created = await service.CreateCreditNoteAsync(new CreditNote
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 0,
            CreditNoteItems =
            [
                new CreditNoteItem
                {
                    Description = "Refund",
                    ItemType = InvoiceItemType.Hours,
                    Quantity = 1,
                    Price = -100
                }
            ]
        });

        Assert.NotNull(postedJson);
        Assert.Contains("\"contact\":\"https://api.freeagent.com/v2/contacts/2\"", postedJson, StringComparison.Ordinal);
        Assert.Contains("\"credit_note_items\"", postedJson, StringComparison.Ordinal);
        Assert.Contains("\"description\":\"Refund\"", postedJson, StringComparison.Ordinal);
        Assert.Equal(CreditNoteStatus.Draft, created.Status);
    }

    [Fact]
    public async Task CreateCreditNoteAsync_IncludesShowProjectNameWhenSet()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "credit_note": {
                    "url": "https://api.freeagent.com/v2/credit_notes/3",
                    "status": "Draft"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        await service.CreateCreditNoteAsync(new CreditNote
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 0,
            ShowProjectName = true,
            CreditNoteItems =
            [
                new CreditNoteItem
                {
                    Description = "Refund",
                    ItemType = InvoiceItemType.Hours,
                    Quantity = 1,
                    Price = -100
                }
            ]
        });

        Assert.NotNull(postedJson);
        Assert.Contains("\"show_project_name\":true", postedJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task UpdateCreditNoteAsync_NonDraft_DoesNotIncludeShowProjectName()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_note": {
                    "url": "https://api.freeagent.com/v2/credit_notes/5",
                    "status": "Open"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        await service.UpdateCreditNoteAsync(5, new CreditNote
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 0,
            Status = CreditNoteStatus.Open,
            ShowProjectName = true,
            Comments = "Updated"
        }, new CreditNoteUpdateOptions { OmitLineItems = true });

        Assert.NotNull(postedJson);
        Assert.DoesNotContain("\"show_project_name\"", postedJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task UpdateCreditNoteAsync_ShowProjectNameWithoutStatus_Throws()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateCreditNoteAsync(5, new CreditNote
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 0,
            ShowProjectName = true,
            Comments = "Updated"
        }, new CreditNoteUpdateOptions { OmitLineItems = true }));

        Assert.Equal("creditNote", exception.ParamName);
    }

    [Fact]
    public async Task UpdateCreditNoteAsync_Draft_IncludesShowProjectNameWhenSet()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_note": {
                    "url": "https://api.freeagent.com/v2/credit_notes/5",
                    "status": "Draft"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        await service.UpdateCreditNoteAsync(5, new CreditNote
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 0,
            Status = CreditNoteStatus.Draft,
            ShowProjectName = true,
            Comments = "Updated"
        }, new CreditNoteUpdateOptions { OmitLineItems = true });

        Assert.NotNull(postedJson);
        Assert.Contains("\"show_project_name\":true", postedJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task UpdateCreditNoteAsync_Draft_IncludesShowProjectNameFalseWhenSet()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_note": {
                    "url": "https://api.freeagent.com/v2/credit_notes/5",
                    "status": "Draft"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        await service.UpdateCreditNoteAsync(5, new CreditNote
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 0,
            Status = CreditNoteStatus.Draft,
            ShowProjectName = false,
            Comments = "Updated"
        }, new CreditNoteUpdateOptions { OmitLineItems = true });

        Assert.NotNull(postedJson);
        Assert.Contains("\"show_project_name\":false", postedJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CreateCreditNoteAsync_IncludesShowProjectNameFalseWhenSet()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "credit_note": {
                    "url": "https://api.freeagent.com/v2/credit_notes/3",
                    "status": "Draft"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        await service.CreateCreditNoteAsync(new CreditNote
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 0,
            ShowProjectName = false,
            CreditNoteItems =
            [
                new CreditNoteItem
                {
                    Description = "Refund",
                    ItemType = InvoiceItemType.Hours,
                    Quantity = 1,
                    Price = -100
                }
            ]
        });

        Assert.NotNull(postedJson);
        Assert.Contains("\"show_project_name\":false", postedJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task UpdateCreditNoteAsync_PostsItemUpdateAndDestroy()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/credit_notes/5", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_note": {
                    "url": "https://api.freeagent.com/v2/credit_notes/5",
                    "status": "Draft"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        await service.UpdateCreditNoteAsync(5, new CreditNote
        {
            CreditNoteItems =
            [
                new CreditNoteItem
                {
                    ItemId = 10,
                    Description = "Updated refund",
                    Price = -50
                },
                new CreditNoteItem
                {
                    ItemId = 11,
                    Destroy = 1
                }
            ]
        });

        Assert.NotNull(postedJson);
        Assert.Contains("\"id\":10", postedJson, StringComparison.Ordinal);
        Assert.Contains("\"description\":\"Updated refund\"", postedJson, StringComparison.Ordinal);
        Assert.Contains("\"_destroy\":1", postedJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeleteCreditNoteAsync_SendsDelete()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/credit_notes/8", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        await service.DeleteCreditNoteAsync(8);
    }

    [Fact]
    public async Task SendCreditNoteEmailAsync_PostsEmailPayload()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/credit_notes/4/send_email", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        await service.SendCreditNoteEmailAsync(4, new SendCreditNoteEmailRequest
        {
            Email = new CreditNoteEmailDetails
            {
                To = "customer@example.com",
                From = "accounts@example.com",
                Subject = "Credit note",
                Body = "Please find attached.",
                EmailToSender = false
            }
        });

        Assert.NotNull(postedJson);
        Assert.Contains("\"to\":\"customer@example.com\"", postedJson, StringComparison.Ordinal);
        Assert.Contains("\"credit_note\"", postedJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task MarkCreditNoteAsSentAsync_CallsTransitionThenGet()
    {
        var requests = new List<HttpRequestMessage>();
        Func<HttpRequestMessage, HttpResponseMessage> respond = request =>
        {
            requests.Add(request);
            if (request.Method == HttpMethod.Put)
            {
                Assert.EndsWith("/credit_notes/6/transitions/mark_as_sent", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_note": {
                    "url": "https://api.freeagent.com/v2/credit_notes/6",
                    "status": "Open"
                  }
                }
                """)
            };
        };

        var handler = new QueueHttpMessageHandler(respond, respond);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        var creditNote = await service.MarkCreditNoteAsSentAsync(6);

        Assert.Equal(2, requests.Count);
        Assert.Equal(CreditNoteStatus.Open, creditNote.Status);
    }

    [Fact]
    public async Task MarkCreditNoteAsDraftAsync_CallsTransitionThenGet()
    {
        Func<HttpRequestMessage, HttpResponseMessage> respond = request =>
        {
            if (request.Method == HttpMethod.Put)
            {
                Assert.EndsWith("/credit_notes/6/transitions/mark_as_draft", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_note": {
                    "url": "https://api.freeagent.com/v2/credit_notes/6",
                    "status": "Draft"
                  }
                }
                """)
            };
        };

        var handler = new QueueHttpMessageHandler(respond, respond);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        var creditNote = await service.MarkCreditNoteAsDraftAsync(6);

        Assert.Equal(CreditNoteStatus.Draft, creditNote.Status);
    }

    [Fact]
    public async Task ListAsync_ContactAndContactIdBothSpecified_Throws()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.ListAsync(
            contact: ContactReference.Parse("https://api.freeagent.com/v2/contacts/1"),
            contactId: 1));
    }

    [Fact]
    public async Task ListAsync_ProjectAndProjectIdBothSpecified_Throws()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.ListAsync(
            project: ProjectReference.Parse("https://api.freeagent.com/v2/projects/1"),
            projectId: 1));
    }

    [Fact]
    public async Task GetCreditNoteAsync_WithHydration_FetchesLinkedResources()
    {
        var calls = new List<string>();
        Func<HttpRequestMessage, HttpResponseMessage> respond = request =>
        {
            calls.Add(request.RequestUri!.AbsolutePath);
            if (request.RequestUri.AbsolutePath.EndsWith("/credit_notes/42", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "credit_note": {
                        "url": "https://api.freeagent.com/v2/credit_notes/42",
                        "contact": "https://api.freeagent.com/v2/contacts/2",
                        "project": "https://api.freeagent.com/v2/projects/3"
                      }
                    }
                    """)
                };
            }

            if (request.RequestUri.AbsolutePath.EndsWith("/contacts/2", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "contact": {
                        "url": "https://api.freeagent.com/v2/contacts/2",
                        "organisation_name": "Example Ltd"
                      }
                    }
                    """)
                };
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "project": {
                    "url": "https://api.freeagent.com/v2/projects/3",
                    "name": "Example project"
                  }
                }
                """)
            };
        };

        var handler = new QueueHttpMessageHandler(respond, respond, respond);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNotesService(client);

        var creditNote = await service.GetCreditNoteAsync(
            42,
            new CreditNoteGetOptions { IncludeContact = true, IncludeProject = true });

        Assert.Equal("Example Ltd", creditNote.Contact?.OrganisationName);
        Assert.Equal("Example project", creditNote.Project?.Name);
        Assert.Contains(calls, path => path.Contains("/contacts/2", StringComparison.Ordinal));
        Assert.Contains(calls, path => path.Contains("/projects/3", StringComparison.Ordinal));
    }
}
