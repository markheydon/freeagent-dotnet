using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Contacts;

/// <summary>
/// Represents a FreeAgent contact.
/// </summary>
public class Contact
{
    /// <summary>
    /// Contact resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Contact display name.
    /// </summary>
    [JsonPropertyName("contact_name")]
    public string? ContactName { get; set; }

    /// <summary>
    /// Contact first name.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Contact last name.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    /// <summary>
    /// Contact organisation name.
    /// </summary>
    [JsonPropertyName("organisation_name")]
    public string? OrganisationName { get; set; }

    /// <summary>
    /// Contact email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Billing email address.
    /// </summary>
    [JsonPropertyName("billing_email")]
    public string? BillingEmail { get; set; }

    /// <summary>
    /// Telephone number.
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Mobile number.
    /// </summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>
    /// First address line.
    /// </summary>
    [JsonPropertyName("address1")]
    public string? Address1 { get; set; }

    /// <summary>
    /// Second address line.
    /// </summary>
    [JsonPropertyName("address2")]
    public string? Address2 { get; set; }

    /// <summary>
    /// Third address line.
    /// </summary>
    [JsonPropertyName("address3")]
    public string? Address3 { get; set; }

    /// <summary>
    /// Town.
    /// </summary>
    [JsonPropertyName("town")]
    public string? Town { get; set; }

    /// <summary>
    /// Region or state.
    /// </summary>
    [JsonPropertyName("region")]
    public string? Region { get; set; }

    /// <summary>
    /// Postcode or ZIP code.
    /// </summary>
    [JsonPropertyName("postcode")]
    public string? Postcode { get; set; }

    /// <summary>
    /// Country.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    /// Whether invoices use a contact-level sequence.
    /// </summary>
    [JsonPropertyName("uses_contact_invoice_sequence")]
    public bool? UsesContactInvoiceSequence { get; set; }

    /// <summary>
    /// Whether invoices show the contact name alongside the organisation name.
    /// </summary>
    [JsonPropertyName("contact_name_on_invoices")]
    public bool? ContactNameOnInvoices { get; set; }

    /// <summary>
    /// Sales tax charging behaviour.
    /// </summary>
    [JsonPropertyName("charge_sales_tax")]
    public ChargeSalesTax? ChargeSalesTax { get; set; }

    /// <summary>
    /// Sales tax registration number displayed on invoices when applicable.
    /// </summary>
    [JsonPropertyName("sales_tax_registration_number")]
    public string? SalesTaxRegistrationNumber { get; set; }

    /// <summary>
    /// Contact status.
    /// </summary>
    [JsonPropertyName("status")]
    public ContactStatus? Status { get; set; }

    /// <summary>
    /// Default payment terms in days.
    /// </summary>
    [JsonPropertyName("default_payment_terms_in_days")]
    public int? DefaultPaymentTermsInDays { get; set; }

    /// <summary>
    /// Invoice and estimate language locale.
    /// </summary>
    [JsonPropertyName("locale")]
    public ContactLocale? Locale { get; set; }

    /// <summary>
    /// Account balance for the contact.
    /// </summary>
    [JsonPropertyName("account_balance")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? AccountBalance { get; set; }

    /// <summary>
    /// Number of active projects for the contact.
    /// </summary>
    [JsonPropertyName("active_projects_count")]
    public int? ActiveProjectsCount { get; set; }

    /// <summary>
    /// GoCardless direct debit mandate state when present.
    /// </summary>
    [JsonPropertyName("direct_debit_mandate_state")]
    public DirectDebitMandateState? DirectDebitMandateState { get; set; }

    /// <summary>
    /// GoCardless direct debit mandate details when present.
    /// </summary>
    [JsonPropertyName("direct_debit_mandate")]
    public DirectDebitMandate? DirectDebitMandate { get; set; }

    /// <summary>
    /// Whether the contact is a CIS subcontractor.
    /// </summary>
    [JsonPropertyName("is_cis_subcontractor")]
    public bool? IsCisSubcontractor { get; set; }

    /// <summary>
    /// CIS deduction rate band.
    /// </summary>
    [JsonPropertyName("cis_deduction_rate")]
    public CisDeductionRate? CisDeductionRate { get; set; }

    /// <summary>
    /// Unique Tax Reference (10 digits).
    /// </summary>
    [JsonPropertyName("unique_tax_reference")]
    public string? UniqueTaxReference { get; set; }

    /// <summary>
    /// Subcontractor Verification Number from HMRC.
    /// </summary>
    [JsonPropertyName("subcontractor_verification_number")]
    public string? SubcontractorVerificationNumber { get; set; }

    /// <summary>
    /// Creation timestamp in UTC.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp in UTC.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Human-friendly contact name derived from available FreeAgent fields.
    /// </summary>
    [JsonIgnore]
    public string DisplayName
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(ContactName))
            {
                return ContactName;
            }

            if (!string.IsNullOrWhiteSpace(OrganisationName))
            {
                return OrganisationName;
            }

            var fullName = string.Join(" ", new[] { FirstName, LastName }.Where(static x => !string.IsNullOrWhiteSpace(x)));
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                return fullName;
            }

            return Url;
        }
    }
}
