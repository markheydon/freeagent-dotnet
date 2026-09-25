using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.CreditNotes;

/// <summary>
/// Credit note status values returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<CreditNoteStatus>))]
public enum CreditNoteStatus
{
    /// <summary>Draft credit note.</summary>
    [JsonStringEnumMemberName("Draft")]
    Draft,

    /// <summary>Open credit note.</summary>
    [JsonStringEnumMemberName("Open")]
    Open,

    /// <summary>Overdue credit note.</summary>
    [JsonStringEnumMemberName("Overdue")]
    Overdue,

    /// <summary>Refunded credit note.</summary>
    [JsonStringEnumMemberName("Refunded")]
    Refunded,

    /// <summary>Written-off credit note.</summary>
    [JsonStringEnumMemberName("Written-off")]
    WrittenOff
}
