using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Online payment method flags for an invoice.
/// </summary>
public sealed class InvoicePaymentMethods
{
    /// <summary>Payable online using PayPal.</summary>
    [JsonPropertyName("paypal")]
    public bool? PayPal { get; set; }

    /// <summary>Payable using a previously authorised GoCardless Direct Debit Mandate.</summary>
    [JsonPropertyName("gocardless_preauth")]
    public bool? GoCardlessPreauth { get; set; }

    /// <summary>Payable online using GoCardless Instant Bank Pay.</summary>
    [JsonPropertyName("gocardless_instant_bank_pay")]
    public bool? GoCardlessInstantBankPay { get; set; }

    /// <summary>Payable online using Stripe.</summary>
    [JsonPropertyName("stripe")]
    public bool? Stripe { get; set; }

    /// <summary>Payable online using Tyl.</summary>
    [JsonPropertyName("tyl")]
    public bool? Tyl { get; set; }
}
