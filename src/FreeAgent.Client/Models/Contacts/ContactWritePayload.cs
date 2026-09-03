using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Contacts;

/// <summary>
/// Writable contact attributes for create and update requests.
/// </summary>
internal sealed class ContactWritePayload
{
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("organisation_name")]
    public string? OrganisationName { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("billing_email")]
    public string? BillingEmail { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    [JsonPropertyName("address1")]
    public string? Address1 { get; set; }

    [JsonPropertyName("address2")]
    public string? Address2 { get; set; }

    [JsonPropertyName("address3")]
    public string? Address3 { get; set; }

    [JsonPropertyName("town")]
    public string? Town { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("postcode")]
    public string? Postcode { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("uses_contact_invoice_sequence")]
    public bool? UsesContactInvoiceSequence { get; set; }

    [JsonPropertyName("contact_name_on_invoices")]
    public bool? ContactNameOnInvoices { get; set; }

    [JsonPropertyName("charge_sales_tax")]
    public ChargeSalesTax? ChargeSalesTax { get; set; }

    [JsonPropertyName("sales_tax_registration_number")]
    public string? SalesTaxRegistrationNumber { get; set; }

    [JsonPropertyName("status")]
    public ContactStatus? Status { get; set; }

    [JsonPropertyName("default_payment_terms_in_days")]
    public int? DefaultPaymentTermsInDays { get; set; }

    [JsonPropertyName("locale")]
    public ContactLocale? Locale { get; set; }

    [JsonPropertyName("is_cis_subcontractor")]
    public bool? IsCisSubcontractor { get; set; }

    [JsonPropertyName("cis_deduction_rate")]
    public CisDeductionRate? CisDeductionRate { get; set; }

    [JsonPropertyName("unique_tax_reference")]
    public string? UniqueTaxReference { get; set; }

    [JsonPropertyName("subcontractor_verification_number")]
    public string? SubcontractorVerificationNumber { get; set; }

    public static ContactWritePayload FromContact(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);

        return new ContactWritePayload
        {
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            OrganisationName = contact.OrganisationName,
            Email = contact.Email,
            BillingEmail = contact.BillingEmail,
            PhoneNumber = contact.PhoneNumber,
            Mobile = contact.Mobile,
            Address1 = contact.Address1,
            Address2 = contact.Address2,
            Address3 = contact.Address3,
            Town = contact.Town,
            Region = contact.Region,
            Postcode = contact.Postcode,
            Country = contact.Country,
            UsesContactInvoiceSequence = contact.UsesContactInvoiceSequence,
            ContactNameOnInvoices = contact.ContactNameOnInvoices,
            ChargeSalesTax = contact.ChargeSalesTax,
            SalesTaxRegistrationNumber = contact.SalesTaxRegistrationNumber,
            Status = contact.Status,
            DefaultPaymentTermsInDays = contact.DefaultPaymentTermsInDays,
            Locale = contact.Locale,
            IsCisSubcontractor = contact.IsCisSubcontractor,
            CisDeductionRate = contact.CisDeductionRate,
            UniqueTaxReference = contact.UniqueTaxReference,
            SubcontractorVerificationNumber = contact.SubcontractorVerificationNumber
        };
    }
}
