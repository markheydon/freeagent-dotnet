using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Estimates;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Estimates;

public class EstimateModelSerializationTests
{
    [Fact]
    public void DeserializeEstimate_MapsStatusDatesAndEnums()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/estimates/3",
              "status": "Approved",
              "estimate_type": "Quote",
              "contact": "https://api.freeagent.com/v2/contacts/2",
              "dated_on": "2011-08-29",
              "ec_status": "EC Goods",
              "created_at": "2011-08-29T00:00:00Z",
              "updated_at": "2011-08-29T00:00:00Z",
              "estimate_items": [
                {
                  "description": "Development",
                  "item_type": "Hours",
                  "price": "100.0",
                  "quantity": "2.0",
                  "sales_tax_status": "TAXABLE",
                  "category": "https://api.freeagent.com/v2/categories/001"
                }
              ]
            }
            """;

        var estimate = JsonSerializer.Deserialize<Estimate>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(estimate);
        Assert.Equal(3, estimate!.ResourceId);
        Assert.Equal(EstimateStatus.Approved, estimate.Status);
        Assert.Equal(EstimateType.Quote, estimate.EstimateType);
        Assert.Equal(new DateOnly(2011, 8, 29), estimate.DatedOn);
        Assert.Equal(EstimateEcStatus.EcGoods, estimate.EcStatus);
        Assert.Equal(2, estimate.ContactId);
        Assert.Single(estimate.EstimateItems!);
        Assert.Equal(EstimateItemType.Hours, estimate.EstimateItems![0].ItemType);
        Assert.Equal(InvoiceSalesTaxStatus.Taxable, estimate.EstimateItems[0].SalesTaxStatus);
        Assert.Equal("001", estimate.EstimateItems[0].CategoryNominalCode);
    }

    [Fact]
    public void DeserializeEstimateItem_MapsNoUnitItemType()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/estimate_items/5",
              "description": "Flat fee",
              "item_type": "-no unit-",
              "price": "500.0"
            }
            """;

        var item = JsonSerializer.Deserialize<EstimateItem>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(item);
        Assert.Equal(EstimateItemType.NoUnit, item!.ItemType);
    }

    [Fact]
    public void DeserializeEstimateItem_MapsCommentsItemType()
    {
        const string json = """
            {
              "description": "Note line",
              "item_type": "Comments"
            }
            """;

        var item = JsonSerializer.Deserialize<EstimateItem>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(item);
        Assert.Equal(EstimateItemType.Comments, item!.ItemType);
    }

    [Fact]
    public void WritePayload_SetContactId_OverridesRoundTrippedContact()
    {
        var estimate = JsonSerializer.Deserialize<Estimate>("""
            {
              "url": "https://api.freeagent.com/v2/estimates/1",
              "contact": "https://api.freeagent.com/v2/contacts/8",
              "dated_on": "2024-03-18",
              "reference": "EST-001"
            }
            """)!;
        estimate.ContactId = 3;

        var payload = EstimateWritePayload.FromEstimate(estimate, FreeAgentEnvironment.Production);

        Assert.Equal("https://api.freeagent.com/v2/contacts/3", payload.Contact!.Value.Uri);
    }

    [Fact]
    public void WritePayload_IncludeStatus_SerialisesStatus()
    {
        var estimate = new Estimate
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            EstimateType = EstimateType.Estimate
        };

        var payload = EstimateWritePayload.FromEstimate(
            estimate,
            FreeAgentEnvironment.Production,
            includeStatus: true);

        Assert.Equal(EstimateStatus.Draft, payload.Status);

        var json = JsonSerializer.Serialize(
            new EstimateRequest { Estimate = payload },
            FreeAgentJsonSerializer.Options);

        Assert.Contains("\"status\":\"Draft\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void WritePayload_IncludeStatus_IgnoresCallerStatus()
    {
        var estimate = new Estimate
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            Status = EstimateStatus.Sent,
            EstimateType = EstimateType.Estimate
        };

        var payload = EstimateWritePayload.FromEstimate(
            estimate,
            FreeAgentEnvironment.Production,
            includeStatus: true);

        Assert.Equal(EstimateStatus.Draft, payload.Status);
    }

    [Fact]
    public void WritePayload_ExcludeStatus_OmitsStatus()
    {
        var estimate = new Estimate
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            Status = EstimateStatus.Sent,
            EstimateType = EstimateType.Estimate
        };

        var payload = EstimateWritePayload.FromEstimate(estimate, FreeAgentEnvironment.Production);

        Assert.Null(payload.Status);

        var json = JsonSerializer.Serialize(
            new EstimateRequest { Estimate = payload },
            FreeAgentJsonSerializer.Options);

        Assert.DoesNotContain("\"status\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void WritePayload_OmitLineItems_ExcludesLineItems()
    {
        var estimate = new Estimate
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            EstimateItems =
            [
                new EstimateItem
                {
                    ItemId = 42,
                    Description = "Development"
                }
            ]
        };

        var payload = EstimateWritePayload.FromEstimate(estimate, FreeAgentEnvironment.Production, omitLineItems: true);

        Assert.Null(payload.EstimateItems);
    }

    [Fact]
    public void DeserializeEstimate_MapsEstimateLevelSalesTaxStatus()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/estimates/3",
              "sales_tax_status": "TAXABLE",
              "sales_tax_value": "5.04"
            }
            """;

        var estimate = JsonSerializer.Deserialize<Estimate>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(estimate);
        Assert.Equal(InvoiceSalesTaxStatus.Taxable, estimate!.SalesTaxStatus);
        Assert.Equal(5.04m, estimate.SalesTaxValue);
    }

    [Fact]
    public void EstimateReference_Parse_ReturnsTypedReference()
    {
        var reference = EstimateReference.Parse("https://api.freeagent.com/v2/estimates/99");

        Assert.Equal(99, reference.Id);
        Assert.Equal("https://api.freeagent.com/v2/estimates/99", reference.Uri);
    }
}
