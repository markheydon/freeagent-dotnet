using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.SalesTaxPeriods;

namespace FreeAgent.Client.Tests.Models.SalesTaxPeriods;

public class SalesTaxPeriodModelSerializationTests
{
    [Fact]
    public void DeserializeSalesTaxPeriod_MapsDocumentedFields()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/sales_tax_periods/1",
              "sales_tax_name": "First Tax",
              "sales_tax_rate_1": "2.0",
              "sales_tax_rate_2": "3.0",
              "sales_tax_rate_3": "4.0",
              "sales_tax_is_value_added": true,
              "sales_tax_registration_status": "Registered",
              "sales_tax_registration_number": "12345678",
              "effective_date": "2016-09-21",
              "is_locked": false,
              "second_sales_tax_name": "Second Tax",
              "second_sales_tax_rate_1": "10.0",
              "second_sales_tax_rate_2": "20.0",
              "second_sales_tax_rate_3": "50.0",
              "second_sales_tax_is_compound": true
            }
            """;

        var period = JsonSerializer.Deserialize<SalesTaxPeriod>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(period);
        Assert.Equal(1, period!.ResourceId);
        Assert.Equal("First Tax", period.SalesTaxName);
        Assert.Equal(SalesTaxPeriodRegistrationStatus.Registered, period.SalesTaxRegistrationStatus);
        Assert.Equal(2.0m, period.SalesTaxRate1);
        Assert.Equal(3.0m, period.SalesTaxRate2);
        Assert.Equal(4.0m, period.SalesTaxRate3);
        Assert.True(period.SalesTaxIsValueAdded);
        Assert.Equal("12345678", period.SalesTaxRegistrationNumber);
        Assert.Equal(new DateOnly(2016, 9, 21), period.EffectiveDate);
        Assert.False(period.IsLocked);
        Assert.Equal("Second Tax", period.SecondSalesTaxName);
        Assert.Equal(10.0m, period.SecondSalesTaxRate1);
        Assert.True(period.SecondSalesTaxIsCompound);
    }

    [Fact]
    public void DeserializeSalesTaxPeriod_MapsNotRegisteredStatus()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/sales_tax_periods/2",
              "sales_tax_registration_status": "Not Registered"
            }
            """;

        var period = JsonSerializer.Deserialize<SalesTaxPeriod>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(period);
        Assert.Equal(SalesTaxPeriodRegistrationStatus.NotRegistered, period!.SalesTaxRegistrationStatus);
    }
}
