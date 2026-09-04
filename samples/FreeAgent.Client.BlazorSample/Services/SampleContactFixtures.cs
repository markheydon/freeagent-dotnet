using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.BlazorSample.Services;

/// <summary>
/// Sample contact payloads for SDK mapping probes.
/// </summary>
public static class SampleContactFixtures
{
    public const string FullDetailProbeEmail = "sdk.full-detail.probe@turpinverse.uk";

    /// <summary>
    /// Builds a contact with every writable attribute populated for wire-to-model diagnostics.
    /// CIS fields are omitted because they require company-level CIS to be enabled.
    /// </summary>
    public static Contact CreateFullDetailProbeContact() =>
        new()
        {
            FirstName = "Patricia",
            LastName = "Probe",
            OrganisationName = "Turpinverse Mapping Services Ltd",
            Email = FullDetailProbeEmail,
            BillingEmail = "billing.probe@turpinverse.uk",
            PhoneNumber = "020 7946 0958",
            Mobile = "07700 900123",
            Address1 = "11 George Street",
            Address2 = "South Court",
            Address3 = "Flat 6",
            Town = "London",
            Region = "Greater London",
            Postcode = "EC1A 1BB",
            Country = "United Kingdom",
            UsesContactInvoiceSequence = true,
            ContactNameOnInvoices = true,
            ChargeSalesTax = ChargeSalesTax.Auto,
            SalesTaxRegistrationNumber = "GB123456789",
            Status = ContactStatus.Active,
            DefaultPaymentTermsInDays = 30,
            Locale = ContactLocale.English
        };
}
