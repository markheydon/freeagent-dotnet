using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Contacts;

/// <summary>
/// GoCardless direct debit mandate state for a contact.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<DirectDebitMandateState>))]
public enum DirectDebitMandateState
{
    /// <summary>Mandate created in UI; email pending (wire value: "setup").</summary>
    [JsonStringEnumMemberName("setup")]
    Setup,

    /// <summary>Customer emailed (wire value: "pending").</summary>
    [JsonStringEnumMemberName("pending")]
    Pending,

    /// <summary>Awaiting GoCardless confirmation (wire value: "inactive").</summary>
    [JsonStringEnumMemberName("inactive")]
    Inactive,

    /// <summary>Mandate active (wire value: "active").</summary>
    [JsonStringEnumMemberName("active")]
    Active,

    /// <summary>Mandate failed (wire value: "failed").</summary>
    [JsonStringEnumMemberName("failed")]
    Failed
}
