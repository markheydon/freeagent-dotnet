using System.Net;
using System.Net.Http;
using System.Text;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.CreditNoteReconciliations;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Services.CreditNoteReconciliations;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.CreditNoteReconciliations;

public class CreditNoteReconciliationServiceTests
{
    [Fact]
    public async System.Threading.Tasks.Task ListAsync_ReturnsReconciliations()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/credit_note_reconciliations", request.RequestUri!.AbsolutePath);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_note_reconciliations": [
                    {
                      "url": "https://api.freeagent.com/v2/credit_note_reconciliations/1",
                      "gross_value": "100.0",
                      "invoice": "https://api.freeagent.com/v2/invoices/1",
                      "credit_note": "https://api.freeagent.com/v2/credit_notes/1"
                    }
                  ]
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        var reconciliations = await service.ListAsync();

        Assert.Single(reconciliations);
        Assert.Equal(100m, reconciliations[0].GrossValue);
        Assert.Equal(1, reconciliations[0].InvoiceId);
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAsync_IncludesDateFilters()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("updated_since=", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("from_date=2017-05-22", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("to_date=2017-05-22", request.RequestUri.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "credit_note_reconciliations": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        await service.ListAsync(
            updatedSince: new DateTimeOffset(2017, 5, 22, 9, 0, 0, TimeSpan.Zero),
            fromDate: new DateOnly(2017, 5, 22),
            toDate: new DateOnly(2017, 5, 22));
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "credit_note_reconciliations": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
    }

    [Fact]
    public async System.Threading.Tasks.Task GetCreditNoteReconciliationAsync_ReturnsReconciliation()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/credit_note_reconciliations/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_note_reconciliation": {
                    "url": "https://api.freeagent.com/v2/credit_note_reconciliations/42",
                    "gross_value": "3.0",
                    "invoice": "https://api.freeagent.com/v2/invoices/1",
                    "credit_note": "https://api.freeagent.com/v2/credit_notes/2"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        var reconciliation = await service.GetCreditNoteReconciliationAsync(42);

        Assert.Equal(3m, reconciliation.GrossValue);
        Assert.Equal(1, reconciliation.InvoiceId);
        Assert.Equal(2, reconciliation.CreditNoteId);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetCreditNoteReconciliationAsync_WithIncludeInvoice_FetchesInvoice()
    {
        var requestCount = 0;
        HttpResponseMessage RouteRequest(HttpRequestMessage request)
        {
            requestCount++;
            if (request.RequestUri!.AbsolutePath.EndsWith("/credit_note_reconciliations/42", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "credit_note_reconciliation": {
                        "url": "https://api.freeagent.com/v2/credit_note_reconciliations/42",
                        "invoice": "https://api.freeagent.com/v2/invoices/1",
                        "credit_note": "https://api.freeagent.com/v2/credit_notes/2"
                      }
                    }
                    """)
                };
            }

            if (request.RequestUri!.AbsolutePath.EndsWith("/invoices/1", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "invoice": {
                        "url": "https://api.freeagent.com/v2/invoices/1",
                        "reference": "001"
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
        var service = new CreditNoteReconciliationsService(client);

        var reconciliation = await service.GetCreditNoteReconciliationAsync(
            42,
            new CreditNoteReconciliationGetOptions { IncludeInvoice = true });

        Assert.Equal(2, requestCount);
        Assert.Equal("001", reconciliation.Invoice!.Reference);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetCreditNoteReconciliationAsync_WithIncludeCreditNote_FetchesCreditNote()
    {
        var requestCount = 0;
        HttpResponseMessage RouteRequest(HttpRequestMessage request)
        {
            requestCount++;
            if (request.RequestUri!.AbsolutePath.EndsWith("/credit_note_reconciliations/42", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "credit_note_reconciliation": {
                        "url": "https://api.freeagent.com/v2/credit_note_reconciliations/42",
                        "invoice": "https://api.freeagent.com/v2/invoices/1",
                        "credit_note": "https://api.freeagent.com/v2/credit_notes/2"
                      }
                    }
                    """)
                };
            }

            if (request.RequestUri!.AbsolutePath.EndsWith("/credit_notes/2", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "credit_note": {
                        "url": "https://api.freeagent.com/v2/credit_notes/2",
                        "reference": "CN-001"
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
        var service = new CreditNoteReconciliationsService(client);

        var reconciliation = await service.GetCreditNoteReconciliationAsync(
            42,
            new CreditNoteReconciliationGetOptions { IncludeCreditNote = true });

        Assert.Equal(2, requestCount);
        Assert.Equal("CN-001", reconciliation.CreditNote!.Reference);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetCreditNoteReconciliationAsync_WithAlternateEnvelopeKey_ReturnsReconciliation()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/credit_note_reconciliations/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_note_reconciliations": {
                    "url": "https://api.freeagent.com/v2/credit_note_reconciliations/42",
                    "gross_value": "7.0",
                    "invoice": "https://api.freeagent.com/v2/invoices/1",
                    "credit_note": "https://api.freeagent.com/v2/credit_notes/2"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        var reconciliation = await service.GetCreditNoteReconciliationAsync(42);

        Assert.Equal(7m, reconciliation.GrossValue);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetCreditNoteReconciliationAsync_WithIncludeInvoiceAndCreditNote_FetchesBoth()
    {
        var requestCount = 0;
        HttpResponseMessage RouteRequest(HttpRequestMessage request)
        {
            requestCount++;
            if (request.RequestUri!.AbsolutePath.EndsWith("/credit_note_reconciliations/42", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "credit_note_reconciliation": {
                        "url": "https://api.freeagent.com/v2/credit_note_reconciliations/42",
                        "invoice": "https://api.freeagent.com/v2/invoices/1",
                        "credit_note": "https://api.freeagent.com/v2/credit_notes/2"
                      }
                    }
                    """)
                };
            }

            if (request.RequestUri!.AbsolutePath.EndsWith("/invoices/1", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "invoice": {
                        "url": "https://api.freeagent.com/v2/invoices/1",
                        "reference": "001"
                      }
                    }
                    """)
                };
            }

            if (request.RequestUri!.AbsolutePath.EndsWith("/credit_notes/2", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "credit_note": {
                        "url": "https://api.freeagent.com/v2/credit_notes/2",
                        "reference": "CN-001"
                      }
                    }
                    """)
                };
            }

            throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
        }

        var handler = new QueueHttpMessageHandler(RouteRequest, RouteRequest, RouteRequest);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        var reconciliation = await service.GetCreditNoteReconciliationAsync(
            42,
            new CreditNoteReconciliationGetOptions
            {
                IncludeInvoice = true,
                IncludeCreditNote = true
            });

        Assert.Equal(3, requestCount);
        Assert.Equal("001", reconciliation.Invoice!.Reference);
        Assert.Equal("CN-001", reconciliation.CreditNote!.Reference);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetCreditNoteReconciliationAsync_InvalidId_Throws()
    {
        using var httpClient = new HttpClient(new HttpClientHandler()) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetCreditNoteReconciliationAsync(0));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetCreditNoteReconciliationAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "credit_note_reconciliation": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetCreditNoteReconciliationAsync(1));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateCreditNoteReconciliationAsync_PostsEnvelopeWithRequiredFields()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/credit_note_reconciliations", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"credit_note_reconciliation\"", body, StringComparison.Ordinal);
            Assert.Contains("\"gross_value\":100", body, StringComparison.Ordinal);
            Assert.Contains("\"invoice\":\"https://api.freeagent.com/v2/invoices/1\"", body, StringComparison.Ordinal);
            Assert.Contains("\"credit_note\":\"https://api.freeagent.com/v2/credit_notes/2\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_note_reconciliation": {
                    "url": "https://api.freeagent.com/v2/credit_note_reconciliations/10",
                    "gross_value": "100.0",
                    "invoice": "https://api.freeagent.com/v2/invoices/1",
                    "credit_note": "https://api.freeagent.com/v2/credit_notes/2"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        var created = await service.CreateCreditNoteReconciliationAsync(
            CreateCreditNoteReconciliationRequest.Create(100m, invoiceId: 1, creditNoteId: 2));

        Assert.Equal(10, created.ResourceId);
        Assert.Equal(100m, created.GrossValue);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateCreditNoteReconciliationAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "credit_note_reconciliation": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() =>
            service.CreateCreditNoteReconciliationAsync(
                CreateCreditNoteReconciliationRequest.Create(1m, invoiceId: 1, creditNoteId: 2)));
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateCreditNoteReconciliationAsync_PutsEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/credit_note_reconciliations/5", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"gross_value\":50", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_note_reconciliation": {
                    "url": "https://api.freeagent.com/v2/credit_note_reconciliations/5",
                    "gross_value": "50.0"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        var updated = await service.UpdateCreditNoteReconciliationAsync(
            5,
            UpdateCreditNoteReconciliationRequest.Create(grossValue: 50m));

        Assert.Equal(50m, updated.GrossValue);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateCreditNoteReconciliationAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "credit_note_reconciliation": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() =>
            service.UpdateCreditNoteReconciliationAsync(
                5,
                UpdateCreditNoteReconciliationRequest.Create(grossValue: 50m)));
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateCreditNoteReconciliationAsync_InvalidId_Throws()
    {
        using var httpClient = new HttpClient(new HttpClientHandler()) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            service.UpdateCreditNoteReconciliationAsync(
                -1,
                UpdateCreditNoteReconciliationRequest.Create(grossValue: 50m)));
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteCreditNoteReconciliationAsync_DeletesResource()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/credit_note_reconciliations/9", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        await service.DeleteCreditNoteReconciliationAsync(9);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteCreditNoteReconciliationAsync_InvalidId_Throws()
    {
        using var httpClient = new HttpClient(new HttpClientHandler()) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CreditNoteReconciliationsService(client);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.DeleteCreditNoteReconciliationAsync(0));
    }
}
