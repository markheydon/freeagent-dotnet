using System.Globalization;
using System.Text.Json;
using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Contacts;

public class ContactModelSerializationTests
{
    [Theory]
    [InlineData("\"Auto\"", ChargeSalesTax.Auto)]
    [InlineData("\"Always\"", ChargeSalesTax.Always)]
    [InlineData("\"Never\"", ChargeSalesTax.Never)]
    public void ChargeSalesTax_DeserializesWireValues(string wireValue, ChargeSalesTax expected)
    {
        var contact = JsonSerializer.Deserialize<Contact>($$"""{ "charge_sales_tax": {{wireValue}} }""");

        Assert.Equal(expected, contact!.ChargeSalesTax);
    }

    [Theory]
    [InlineData("\"cis_gross\"", CisDeductionRate.Gross)]
    [InlineData("\"cis_standard\"", CisDeductionRate.Standard)]
    [InlineData("\"cis_higher\"", CisDeductionRate.Higher)]
    public void CisDeductionRate_DeserializesWireValues(string wireValue, CisDeductionRate expected)
    {
        var contact = JsonSerializer.Deserialize<Contact>($$"""{ "cis_deduction_rate": {{wireValue}} }""");

        Assert.Equal(expected, contact!.CisDeductionRate);
    }

    [Theory]
    [InlineData("\"en\"", ContactLocale.English)]
    [InlineData("\"en-US\"", ContactLocale.EnglishUnitedStates)]
    [InlineData("\"fr-BE\"", ContactLocale.FrenchBelgium)]
    public void ContactLocale_DeserializesWireValues(string wireValue, ContactLocale expected)
    {
        var contact = JsonSerializer.Deserialize<Contact>($$"""{ "locale": {{wireValue}} }""");

        Assert.Equal(expected, contact!.Locale);
    }

    [Theory]
    [InlineData("\"setup\"", DirectDebitMandateState.Setup)]
    [InlineData("\"pending\"", DirectDebitMandateState.Pending)]
    [InlineData("\"inactive\"", DirectDebitMandateState.Inactive)]
    [InlineData("\"active\"", DirectDebitMandateState.Active)]
    [InlineData("\"failed\"", DirectDebitMandateState.Failed)]
    public void DirectDebitMandateState_DeserializesWireValues(string wireValue, DirectDebitMandateState expected)
    {
        var contact = JsonSerializer.Deserialize<Contact>($$"""{ "direct_debit_mandate_state": {{wireValue}} }""");

        Assert.Equal(expected, contact!.DirectDebitMandateState);
    }

    [Fact]
    public void DirectDebitMandate_DeserializesNestedObject()
    {
        const string json = """
        {
          "direct_debit_mandate": {
            "currency": "GBP",
            "max_amount": "100.0",
            "remaining_amount": "25.5",
            "next_interval_starts_on": "2026-04-01"
          }
        }
        """;

        var contact = JsonSerializer.Deserialize<Contact>(json);

        Assert.NotNull(contact!.DirectDebitMandate);
        Assert.Equal(CurrencyCode.GBP, contact.DirectDebitMandate.Currency);
        Assert.Equal(100m, contact.DirectDebitMandate.MaxAmount);
        Assert.Equal(25.5m, contact.DirectDebitMandate.RemainingAmount);
        Assert.Equal(new DateOnly(2026, 4, 1), contact.DirectDebitMandate.NextIntervalStartsOn);
    }

    [Fact]
    public void Serialize_OmitsReadOnlyFields()
    {
        var payload = ContactWritePayload.FromContact(new Contact
        {
            Url = "https://api.freeagent.com/v2/contacts/42",
            OrganisationName = "Acme Ltd",
            AccountBalance = 12.5m,
            ActiveProjectsCount = 3,
            DirectDebitMandateState = DirectDebitMandateState.Active,
            DirectDebitMandate = new DirectDebitMandate { Currency = CurrencyCode.GBP },
            CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z", CultureInfo.InvariantCulture),
            UpdatedAt = DateTimeOffset.Parse("2026-01-02T00:00:00Z", CultureInfo.InvariantCulture)
        });

        var json = JsonSerializer.Serialize(new ContactRequest { Contact = payload });

        Assert.DoesNotContain("\"url\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("account_balance", json, StringComparison.Ordinal);
        Assert.DoesNotContain("active_projects_count", json, StringComparison.Ordinal);
        Assert.DoesNotContain("direct_debit_mandate", json, StringComparison.Ordinal);
        Assert.DoesNotContain("created_at", json, StringComparison.Ordinal);
        Assert.DoesNotContain("updated_at", json, StringComparison.Ordinal);
        Assert.Contains("\"organisation_name\":\"Acme Ltd\"", json, StringComparison.Ordinal);
    }
}
