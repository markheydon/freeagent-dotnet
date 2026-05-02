using FreeAgent.Client.Infrastructure.Serialization;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Company;

/// <summary>
/// Represents ISO 4217 currency codes supported by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<CurrencyCode>))]
public enum CurrencyCode
{
    /// <summary>British pound sterling.</summary>
    [JsonStringEnumMemberName("GBP")]
    GBP,
    /// <summary>US dollar.</summary>
    [JsonStringEnumMemberName("USD")]
    USD,
    /// <summary>Euro.</summary>
    [JsonStringEnumMemberName("EUR")]
    EUR,
    /// <summary>Australian dollar.</summary>
    [JsonStringEnumMemberName("AUD")]
    AUD,
    /// <summary>Canadian dollar.</summary>
    [JsonStringEnumMemberName("CAD")]
    CAD,
    /// <summary>New Zealand dollar.</summary>
    [JsonStringEnumMemberName("NZD")]
    NZD,
    /// <summary>Singapore dollar.</summary>
    [JsonStringEnumMemberName("SGD")]
    SGD,
    /// <summary>Hong Kong dollar.</summary>
    [JsonStringEnumMemberName("HKD")]
    HKD,
    /// <summary>Japanese yen.</summary>
    [JsonStringEnumMemberName("JPY")]
    JPY,
    /// <summary>Swiss franc.</summary>
    [JsonStringEnumMemberName("CHF")]
    CHF,
    /// <summary>Swedish krona.</summary>
    [JsonStringEnumMemberName("SEK")]
    SEK,
    /// <summary>Danish krone.</summary>
    [JsonStringEnumMemberName("DKK")]
    DKK,
    /// <summary>Norwegian krone.</summary>
    [JsonStringEnumMemberName("NOK")]
    NOK,
    /// <summary>Polish złoty.</summary>
    [JsonStringEnumMemberName("PLN")]
    PLN,
    /// <summary>Czech koruna.</summary>
    [JsonStringEnumMemberName("CZK")]
    CZK,
    /// <summary>Hungarian forint.</summary>
    [JsonStringEnumMemberName("HUF")]
    HUF,
    /// <summary>Mexican peso.</summary>
    [JsonStringEnumMemberName("MXN")]
    MXN,
    /// <summary>Brazilian real.</summary>
    [JsonStringEnumMemberName("BRL")]
    BRL,
    /// <summary>Indian rupee.</summary>
    [JsonStringEnumMemberName("INR")]
    INR,
    /// <summary>South African rand.</summary>
    [JsonStringEnumMemberName("ZAR")]
    ZAR
}
