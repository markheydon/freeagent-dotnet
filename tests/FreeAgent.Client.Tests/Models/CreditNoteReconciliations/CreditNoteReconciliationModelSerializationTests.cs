using System.Text.Json;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.CreditNoteReconciliations;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.CreditNoteReconciliations;

public class CreditNoteReconciliationModelSerializationTests
{
    [Fact]
    public void Deserialize_MapsAttributesDatesAndLinks()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/credit_note_reconciliations/1",
              "gross_value": "100.0",
              "dated_on": "2020-06-29",
              "currency": "GBP",
              "exchange_rate": "1.0",
              "invoice": "https://api.freeagent.com/v2/invoices/1",
              "credit_note": "https://api.freeagent.com/v2/credit_notes/1",
              "created_at": "2020-08-10T15:06:28.225Z",
              "updated_at": "2020-08-10T15:06:28.225Z"
            }
            """;

        var reconciliation = JsonSerializer.Deserialize<CreditNoteReconciliation>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(reconciliation);
        Assert.Equal(1, reconciliation!.ResourceId);
        Assert.Equal(100m, reconciliation.GrossValue);
        Assert.Equal(new DateOnly(2020, 6, 29), reconciliation.DatedOn);
        Assert.Equal(CurrencyCode.GBP, reconciliation.Currency);
        Assert.Equal(1m, reconciliation.ExchangeRate);
        Assert.Equal(1, reconciliation.InvoiceId);
        Assert.Equal(1, reconciliation.CreditNoteId);
        Assert.Equal(new DateTimeOffset(2020, 8, 10, 15, 6, 28, 225, TimeSpan.Zero), reconciliation.CreatedAt);
    }

    [Fact]
    public void DeserializeListEnvelope_ReadsCreditNoteReconciliationsArray()
    {
        const string json = """
            {
              "credit_note_reconciliations": [
                {
                  "url": "https://api.freeagent.com/v2/credit_note_reconciliations/2",
                  "gross_value": "3.0"
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<CreditNoteReconciliationsResponse>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(response?.CreditNoteReconciliations);
        Assert.Single(response!.CreditNoteReconciliations!);
        Assert.Equal(2, response.CreditNoteReconciliations![0].ResourceId);
    }

    [Fact]
    public void DeserializeSingleEnvelope_ReadsCreditNoteReconciliationObject()
    {
        const string json = """
            {
              "credit_note_reconciliation": {
                "url": "https://api.freeagent.com/v2/credit_note_reconciliations/3",
                "gross_value": "50.0"
              }
            }
            """;

        var response = JsonSerializer.Deserialize<CreditNoteReconciliationResponse>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(response?.ResolvePayload());
        Assert.Equal(3, response!.ResolvePayload()!.ResourceId);
        Assert.Equal(50m, response.ResolvePayload()!.GrossValue);
    }

    [Fact]
    public void DeserializeSingleEnvelope_ReadsAlternateDocsEnvelopeKey()
    {
        const string json = """
            {
              "credit_note_reconciliations": {
                "url": "https://api.freeagent.com/v2/credit_note_reconciliations/4",
                "gross_value": "25.0"
              }
            }
            """;

        var response = JsonSerializer.Deserialize<CreditNoteReconciliationResponse>(json, FreeAgentJsonSerializer.Options);

        Assert.Null(response?.CreditNoteReconciliation);
        Assert.NotNull(response?.ResolvePayload());
        Assert.Equal(4, response!.ResolvePayload()!.ResourceId);
        Assert.Equal(25m, response.ResolvePayload()!.GrossValue);
    }

    [Fact]
    public void WritePayload_FromCreate_SerialisesRequiredFields()
    {
        var request = CreateCreditNoteReconciliationRequest.Create(
            100m,
            invoiceId: 1,
            creditNoteId: 2,
            datedOn: new DateOnly(2020, 6, 29),
            exchangeRate: 1m,
            currency: CurrencyCode.GBP);

        var json = JsonSerializer.Serialize(
            CreditNoteReconciliationWritePayload.FromCreate(request, FreeAgentEnvironment.Production),
            FreeAgentJsonSerializer.Options);

        Assert.Contains("\"gross_value\":100", json, StringComparison.Ordinal);
        Assert.Contains("\"invoice\":\"https://api.freeagent.com/v2/invoices/1\"", json, StringComparison.Ordinal);
        Assert.Contains("\"credit_note\":\"https://api.freeagent.com/v2/credit_notes/2\"", json, StringComparison.Ordinal);
        Assert.Contains("\"dated_on\":\"2020-06-29\"", json, StringComparison.Ordinal);
        Assert.Contains("\"exchange_rate\":1", json, StringComparison.Ordinal);
        Assert.Contains("\"currency\":\"GBP\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"url\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void WritePayload_FromUpdate_SerialisesOnlySuppliedFields()
    {
        var request = UpdateCreditNoteReconciliationRequest.Create(grossValue: 25m);

        var json = JsonSerializer.Serialize(
            CreditNoteReconciliationWritePayload.FromUpdate(request, FreeAgentEnvironment.Production),
            FreeAgentJsonSerializer.Options);

        Assert.Contains("\"gross_value\":25", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"invoice\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"credit_note\"", json, StringComparison.Ordinal);
    }
}
