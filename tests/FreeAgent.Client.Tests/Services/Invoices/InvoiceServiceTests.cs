using System.Net;
using System.Net.Http;
using System.Text;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Services.Invoices;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Invoices;

public class InvoiceServiceTests
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
                  "invoices": [
                    {
                      "url": "https://api.freeagent.com/v2/invoices/1",
                      "reference": "001",
                      "status": "Open"
                    },
                    {
                      "url": "https://api.freeagent.com/v2/invoices/2",
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
        var service = new InvoiceService(client);

        var page = await service.ListAsync(page: 1, perPage: 2, view: InvoiceViews.Open);

        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PerPage);
        Assert.Equal(5, page.Total);
        Assert.True(page.HasNextPage);
        Assert.Equal("001", page.Items[0].Reference);
        Assert.Equal(InvoiceStatus.Draft, page.Items[1].Status);
    }

    [Fact]
    public async Task ListAsync_IncludesFiltersAndNestedItems()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("sort=-updated_at", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("contact=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fcontacts%2F2", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("project=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fprojects%2F3", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("nested_invoice_items=true", request.RequestUri.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "invoices": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        await service.ListAsync(
            sort: "-updated_at",
            contactId: 2,
            projectId: 3,
            nestedInvoiceItems: true);
    }

    [Fact]
    public async Task GetInvoiceAsync_ReturnsInvoice()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/invoices/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "invoice": {
                    "url": "https://api.freeagent.com/v2/invoices/42",
                    "reference": "007",
                    "status": "Draft"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var invoice = await service.GetInvoiceAsync(42);

        Assert.Equal("007", invoice.Reference);
        Assert.Equal(InvoiceStatus.Draft, invoice.Status);
    }

    [Fact]
    public async Task GetInvoiceAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "invoice": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetInvoiceAsync(1));
    }

    [Fact]
    public async Task GetInvoicePdfAsync_DecodesBase64Content()
    {
        var expected = new byte[] { 0x25, 0x50, 0x44, 0x46 };
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/invoices/7/pdf", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
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
        var service = new InvoiceService(client);

        var pdf = await service.GetInvoicePdfAsync(7);

        Assert.Equal(expected, pdf);
    }

    [Fact]
    public async Task CreateInvoiceAsync_PostsInvoicePayload()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/invoices", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "invoice": {
                    "url": "https://api.freeagent.com/v2/invoices/3",
                    "status": "Draft"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var created = await service.CreateInvoiceAsync(new Invoice
        {
            BillingContact = ContactReference.Parse("https://api.freeagent.com/v2/contacts/2"),
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 14,
            InvoiceItems =
            [
                new InvoiceItem
                {
                    Description = "Consulting",
                    ItemType = InvoiceItemType.Hours,
                    Quantity = 2,
                    Price = 100
                }
            ]
        });

        Assert.NotNull(postedJson);
        Assert.Contains("\"contact\":\"https://api.freeagent.com/v2/contacts/2\"", postedJson, StringComparison.Ordinal);
        Assert.Contains("\"payment_terms_in_days\":14", postedJson, StringComparison.Ordinal);
        Assert.Equal(InvoiceStatus.Draft, created.Status);
    }

    [Fact]
    public async Task MarkInvoiceAsSentAsync_PutsTransitionEndpoint()
    {
        Func<HttpRequestMessage, HttpResponseMessage> respond = request =>
        {
            if (request.Method == HttpMethod.Put)
            {
                Assert.EndsWith("/invoices/5/transitions/mark_as_sent", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            Assert.EndsWith("/invoices/5", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "invoice": {
                    "url": "https://api.freeagent.com/v2/invoices/5",
                    "status": "Open"
                  }
                }
                """)
            };
        };

        var handler = new QueueHttpMessageHandler(respond, respond);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var invoice = await service.MarkInvoiceAsSentAsync(5);

        Assert.Equal(InvoiceStatus.Open, invoice.Status);
    }

    [Fact]
    public async Task SendInvoiceEmailAsync_PostsTemplateRequest()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/invoices/8/send_email", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        await service.SendInvoiceEmailAsync(8, new SendInvoiceEmailRequest
        {
            Email = new InvoiceEmailDetails { UseTemplate = true }
        });

        Assert.Contains("\"use_template\":true", postedJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ConvertToCreditNoteAsync_ReturnsCreditNote()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/invoices/9/transitions/convert_to_credit_note", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_note": {
                    "url": "https://api.freeagent.com/v2/credit_notes/11"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var creditNote = await service.ConvertToCreditNoteAsync(9);

        Assert.Equal(11, creditNote.ResourceId);
    }

    [Fact]
    public async Task ListTimelineAsync_ReturnsItems()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/invoices/timeline", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "invoice_timeline_items": [
                    {
                      "reference": "007",
                      "summary": "Payment received",
                      "dated_on": "2011-09-02",
                      "amount": "14.4"
                    }
                  ]
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var items = await service.ListTimelineAsync();

        Assert.Single(items);
        Assert.Equal("007", items[0].Reference);
    }

    [Fact]
    public async Task DefaultAdditionalText_GetUpdateDelete_UseExpectedRoutes()
    {
        var calls = new List<string>();
        HttpResponseMessage Respond(HttpRequestMessage request)
        {
            calls.Add($"{request.Method} {request.RequestUri!.AbsolutePath}");
            if (request.Method == HttpMethod.Get)
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{ "default_additional_text": "Pay within 21 days" }""")
                };
            }

            if (request.Method == HttpMethod.Put)
            {
                var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
                Assert.Contains("Pay within 7 days", body, StringComparison.Ordinal);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{ "default_additional_text": "Pay within 7 days" }""")
                };
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            };
        }

        var handler = new QueueHttpMessageHandler(Respond, Respond, Respond, Respond);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var current = await service.GetDefaultAdditionalTextAsync();
        var updated = await service.UpdateDefaultAdditionalTextAsync("Pay within 7 days");
        await service.DeleteDefaultAdditionalTextAsync();

        Assert.Equal("Pay within 21 days", current);
        Assert.Equal("Pay within 7 days", updated);
        Assert.Contains("GET /v2/invoices/default_additional_text", calls);
        Assert.Contains("PUT /v2/invoices/default_additional_text", calls);
        Assert.Contains("DELETE /v2/invoices/default_additional_text", calls);
    }

    [Fact]
    public async Task ListAsync_ContactAndContactId_Throws()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.ListAsync(
            contact: ContactReference.Parse("https://api.freeagent.com/v2/contacts/1"),
            contactId: 2));
    }

    [Fact]
    public async Task GetInvoiceAsync_WithHydration_FetchesLinkedResources()
    {
        var calls = new List<string>();
        Func<HttpRequestMessage, HttpResponseMessage> respond = request =>
        {
            calls.Add(request.RequestUri!.AbsolutePath);
            if (request.RequestUri.AbsolutePath.EndsWith("/invoices/42", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "invoice": {
                        "url": "https://api.freeagent.com/v2/invoices/42",
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
        var service = new InvoiceService(client);

        var invoice = await service.GetInvoiceAsync(
            42,
            new InvoiceGetOptions { IncludeContact = true, IncludeProject = true });

        Assert.Equal("Example Ltd", invoice.Contact?.OrganisationName);
        Assert.Equal("Example project", invoice.Project?.Name);
        Assert.Contains(calls, path => path.Contains("/contacts/2", StringComparison.Ordinal));
        Assert.Contains(calls, path => path.Contains("/projects/3", StringComparison.Ordinal));
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
                        "invoices": [
                          { "url": "https://api.freeagent.com/v2/invoices/1", "reference": "001" },
                          { "url": "https://api.freeagent.com/v2/invoices/2", "reference": "002" }
                        ]
                      }
                      """
                    : """
                      {
                        "invoices": [
                          { "url": "https://api.freeagent.com/v2/invoices/3", "reference": "003" }
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
        var service = new InvoiceService(client);

        var references = new List<string>();
        await foreach (var invoice in service.ListAutoPagingAsync(perPage: 2))
        {
            references.Add(invoice.Reference!);
        }

        Assert.Equal(["001", "002", "003"], references);
    }

    [Fact]
    public async Task DuplicateInvoiceAsync_PostsDuplicateEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/invoices/5/duplicate", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "invoice": {
                    "url": "https://api.freeagent.com/v2/invoices/6",
                    "status": "Draft"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var invoice = await service.DuplicateInvoiceAsync(5);

        Assert.Equal(6, invoice.ResourceId);
    }

    [Fact]
    public async Task UpdateInvoiceAsync_PutsInvoicePayload()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/invoices/4", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "invoice": {
                    "url": "https://api.freeagent.com/v2/invoices/4",
                    "comments": "Updated"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var invoice = await service.UpdateInvoiceAsync(4, new Invoice
        {
            BillingContact = ContactReference.Parse("https://api.freeagent.com/v2/contacts/2"),
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 14,
            OmitInvoiceItemsFromWrite = true,
            Comments = "Updated"
        });

        Assert.Equal("Updated", invoice.Comments);
    }

    [Fact]
    public async Task DeleteInvoiceAsync_DeletesEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/invoices/9", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        await service.DeleteInvoiceAsync(9);
    }

    [Fact]
    public async Task MarkInvoiceAsScheduledAsync_PutsTransitionEndpoint()
    {
        Func<HttpRequestMessage, HttpResponseMessage> respond = request =>
        {
            if (request.Method == HttpMethod.Put)
            {
                Assert.EndsWith("/invoices/5/transitions/mark_as_scheduled", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "invoice": {
                    "url": "https://api.freeagent.com/v2/invoices/5",
                    "status": "Scheduled To Email"
                  }
                }
                """)
            };
        };

        var handler = new QueueHttpMessageHandler(respond, respond);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var invoice = await service.MarkInvoiceAsScheduledAsync(5);

        Assert.Equal(InvoiceStatus.ScheduledToEmail, invoice.Status);
    }

    [Fact]
    public async Task ListAsync_UpdatedSince_IncludesFilter()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("updated_since=2024-03-18T09%3A00%3A00.0000000%2B00%3A00", request.RequestUri!.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "invoices": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        await service.ListAsync(updatedSince: new DateTimeOffset(2024, 3, 18, 9, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public async Task ListAsync_ProjectAndProjectId_Throws()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.ListAsync(
            project: ProjectReference.Parse("https://api.freeagent.com/v2/projects/1"),
            projectId: 2));
    }

    [Fact]
    public async Task MarkInvoiceAsDraftAsync_PutsTransitionEndpoint()
    {
        Func<HttpRequestMessage, HttpResponseMessage> respond = request =>
        {
            if (request.Method == HttpMethod.Put)
            {
                Assert.EndsWith("/invoices/5/transitions/mark_as_draft", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "invoice": {
                    "url": "https://api.freeagent.com/v2/invoices/5",
                    "status": "Draft"
                  }
                }
                """)
            };
        };

        var handler = new QueueHttpMessageHandler(respond, respond);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var invoice = await service.MarkInvoiceAsDraftAsync(5);

        Assert.Equal(InvoiceStatus.Draft, invoice.Status);
    }

    [Fact]
    public async Task MarkInvoiceAsCancelledAsync_PutsTransitionEndpoint()
    {
        Func<HttpRequestMessage, HttpResponseMessage> respond = request =>
        {
            if (request.Method == HttpMethod.Put)
            {
                Assert.EndsWith("/invoices/5/transitions/mark_as_cancelled", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "invoice": {
                    "url": "https://api.freeagent.com/v2/invoices/5",
                    "status": "Written-off"
                  }
                }
                """)
            };
        };

        var handler = new QueueHttpMessageHandler(respond, respond);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var invoice = await service.MarkInvoiceAsCancelledAsync(5);

        Assert.Equal(InvoiceStatus.WrittenOff, invoice.Status);
    }

    [Fact]
    public async Task TakeDirectDebitPaymentAsync_PostsDirectDebitEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/invoices/12/direct_debit", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        await service.TakeDirectDebitPaymentAsync(12);
    }

    [Fact]
    public async Task GetInvoicePdfAsync_InvalidBase64_ThrowsFreeAgentApiException()
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
        var service = new InvoiceService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetInvoicePdfAsync(1));
    }
}
