using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Represents ISO 4217 currency codes supported by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<CurrencyCode>))]
public enum CurrencyCode
{
    /// <summary>United Arab Emirates dirham.</summary>
    [JsonStringEnumMemberName("AED")]
    AED,
    /// <summary>Armenian dram.</summary>
    [JsonStringEnumMemberName("AMD")]
    AMD,
    /// <summary>Angolan kwanza.</summary>
    [JsonStringEnumMemberName("AOA")]
    AOA,
    /// <summary>Argentine peso.</summary>
    [JsonStringEnumMemberName("ARS")]
    ARS,
    /// <summary>Australian dollar.</summary>
    [JsonStringEnumMemberName("AUD")]
    AUD,
    /// <summary>Aruban florin.</summary>
    [JsonStringEnumMemberName("AWG")]
    AWG,
    /// <summary>Azerbaijani manat.</summary>
    [JsonStringEnumMemberName("AZN")]
    AZN,
    /// <summary>Barbadian dollar.</summary>
    [JsonStringEnumMemberName("BBD")]
    BBD,
    /// <summary>Bangladeshi taka.</summary>
    [JsonStringEnumMemberName("BDT")]
    BDT,
    /// <summary>Bulgarian lev.</summary>
    [JsonStringEnumMemberName("BGN")]
    BGN,
    /// <summary>Brazilian real.</summary>
    [JsonStringEnumMemberName("BRL")]
    BRL,
    /// <summary>Botswana pula.</summary>
    [JsonStringEnumMemberName("BWP")]
    BWP,
    /// <summary>Canadian dollar.</summary>
    [JsonStringEnumMemberName("CAD")]
    CAD,
    /// <summary>Swiss franc.</summary>
    [JsonStringEnumMemberName("CHF")]
    CHF,
    /// <summary>Chilean peso.</summary>
    [JsonStringEnumMemberName("CLP")]
    CLP,
    /// <summary>Chinese yuan.</summary>
    [JsonStringEnumMemberName("CNY")]
    CNY,
    /// <summary>Colombian peso.</summary>
    [JsonStringEnumMemberName("COP")]
    COP,
    /// <summary>Costa Rican colón.</summary>
    [JsonStringEnumMemberName("CRC")]
    CRC,
    /// <summary>Cuban convertible peso.</summary>
    [JsonStringEnumMemberName("CUC")]
    CUC,
    /// <summary>Cuban peso.</summary>
    [JsonStringEnumMemberName("CUP")]
    CUP,
    /// <summary>Czech koruna.</summary>
    [JsonStringEnumMemberName("CZK")]
    CZK,
    /// <summary>Danish krone.</summary>
    [JsonStringEnumMemberName("DKK")]
    DKK,
    /// <summary>Dominican peso.</summary>
    [JsonStringEnumMemberName("DOP")]
    DOP,
    /// <summary>Egyptian pound.</summary>
    [JsonStringEnumMemberName("EGP")]
    EGP,
    /// <summary>Euro.</summary>
    [JsonStringEnumMemberName("EUR")]
    EUR,
    /// <summary>Fijian dollar.</summary>
    [JsonStringEnumMemberName("FJD")]
    FJD,
    /// <summary>Pounds sterling.</summary>
    [JsonStringEnumMemberName("GBP")]
    GBP,
    /// <summary>Georgian lari.</summary>
    [JsonStringEnumMemberName("GEL")]
    GEL,
    /// <summary>Ghanaian cedi.</summary>
    [JsonStringEnumMemberName("GHS")]
    GHS,
    /// <summary>Guatemalan quetzal.</summary>
    [JsonStringEnumMemberName("GTQ")]
    GTQ,
    /// <summary>Guyanese dollar.</summary>
    [JsonStringEnumMemberName("GYD")]
    GYD,
    /// <summary>Hong Kong dollar.</summary>
    [JsonStringEnumMemberName("HKD")]
    HKD,
    /// <summary>Honduran lempira.</summary>
    [JsonStringEnumMemberName("HNL")]
    HNL,
    /// <summary>Croatian kuna.</summary>
    [JsonStringEnumMemberName("HRK")]
    HRK,
    /// <summary>Hungarian forint.</summary>
    [JsonStringEnumMemberName("HUF")]
    HUF,
    /// <summary>Indonesian rupiah.</summary>
    [JsonStringEnumMemberName("IDR")]
    IDR,
    /// <summary>Israeli shekel.</summary>
    [JsonStringEnumMemberName("ILS")]
    ILS,
    /// <summary>Indian rupee.</summary>
    [JsonStringEnumMemberName("INR")]
    INR,
    /// <summary>Icelandic króna.</summary>
    [JsonStringEnumMemberName("ISK")]
    ISK,
    /// <summary>Jamaican dollar.</summary>
    [JsonStringEnumMemberName("JMD")]
    JMD,
    /// <summary>Japanese yen.</summary>
    [JsonStringEnumMemberName("JPY")]
    JPY,
    /// <summary>Kenyan shilling.</summary>
    [JsonStringEnumMemberName("KES")]
    KES,
    /// <summary>South Korean won.</summary>
    [JsonStringEnumMemberName("KRW")]
    KRW,
    /// <summary>Kuwaiti dinar.</summary>
    [JsonStringEnumMemberName("KWD")]
    KWD,
    /// <summary>Cayman Islands dollar.</summary>
    [JsonStringEnumMemberName("KYD")]
    KYD,
    /// <summary>Kazakhstani tenge.</summary>
    [JsonStringEnumMemberName("KZT")]
    KZT,
    /// <summary>Lao kip.</summary>
    [JsonStringEnumMemberName("LAK")]
    LAK,
    /// <summary>Lebanese pound.</summary>
    [JsonStringEnumMemberName("LBP")]
    LBP,
    /// <summary>Sri Lankan rupee.</summary>
    [JsonStringEnumMemberName("LKR")]
    LKR,
    /// <summary>Lithuanian litas.</summary>
    [JsonStringEnumMemberName("LTL")]
    LTL,
    /// <summary>Latvian lats.</summary>
    [JsonStringEnumMemberName("LVL")]
    LVL,
    /// <summary>Moroccan dirham.</summary>
    [JsonStringEnumMemberName("MAD")]
    MAD,
    /// <summary>Moldovan leu.</summary>
    [JsonStringEnumMemberName("MDL")]
    MDL,
    /// <summary>Malagasy ariary.</summary>
    [JsonStringEnumMemberName("MGA")]
    MGA,
    /// <summary>Mauritian rupee.</summary>
    [JsonStringEnumMemberName("MUR")]
    MUR,
    /// <summary>Maldivian rufiyaa.</summary>
    [JsonStringEnumMemberName("MVR")]
    MVR,
    /// <summary>Malawian kwacha.</summary>
    [JsonStringEnumMemberName("MWK")]
    MWK,
    /// <summary>Mexican peso.</summary>
    [JsonStringEnumMemberName("MXN")]
    MXN,
    /// <summary>Malaysian ringgit.</summary>
    [JsonStringEnumMemberName("MYR")]
    MYR,
    /// <summary>Mozambican metical.</summary>
    [JsonStringEnumMemberName("MZN")]
    MZN,
    /// <summary>Namibian dollar.</summary>
    [JsonStringEnumMemberName("NAD")]
    NAD,
    /// <summary>Nigerian naira.</summary>
    [JsonStringEnumMemberName("NGN")]
    NGN,
    /// <summary>Norwegian krone.</summary>
    [JsonStringEnumMemberName("NOK")]
    NOK,
    /// <summary>Nepalese rupee.</summary>
    [JsonStringEnumMemberName("NPR")]
    NPR,
    /// <summary>New Zealand dollar.</summary>
    [JsonStringEnumMemberName("NZD")]
    NZD,
    /// <summary>Omani rial.</summary>
    [JsonStringEnumMemberName("OMR")]
    OMR,
    /// <summary>Peruvian nuevo sol.</summary>
    [JsonStringEnumMemberName("PEN")]
    PEN,
    /// <summary>Philippine peso.</summary>
    [JsonStringEnumMemberName("PHP")]
    PHP,
    /// <summary>Pakistani rupee.</summary>
    [JsonStringEnumMemberName("PKR")]
    PKR,
    /// <summary>Polish złoty.</summary>
    [JsonStringEnumMemberName("PLN")]
    PLN,
    /// <summary>Qatari riyal.</summary>
    [JsonStringEnumMemberName("QAR")]
    QAR,
    /// <summary>Romanian new leu.</summary>
    [JsonStringEnumMemberName("RON")]
    RON,
    /// <summary>Serbian dinar.</summary>
    [JsonStringEnumMemberName("RSD")]
    RSD,
    /// <summary>Russian rouble.</summary>
    [JsonStringEnumMemberName("RUB")]
    RUB,
    /// <summary>Rwandan franc.</summary>
    [JsonStringEnumMemberName("RWF")]
    RWF,
    /// <summary>Saudi riyal.</summary>
    [JsonStringEnumMemberName("SAR")]
    SAR,
    /// <summary>Seychelles rupee.</summary>
    [JsonStringEnumMemberName("SCR")]
    SCR,
    /// <summary>Swedish krona.</summary>
    [JsonStringEnumMemberName("SEK")]
    SEK,
    /// <summary>Singapore dollar.</summary>
    [JsonStringEnumMemberName("SGD")]
    SGD,
    /// <summary>Thai baht.</summary>
    [JsonStringEnumMemberName("THB")]
    THB,
    /// <summary>Tunisian dinar.</summary>
    [JsonStringEnumMemberName("TND")]
    TND,
    /// <summary>Turkish lira.</summary>
    [JsonStringEnumMemberName("TRY")]
    TRY,
    /// <summary>Trinidad and Tobago dollar.</summary>
    [JsonStringEnumMemberName("TTD")]
    TTD,
    /// <summary>New Taiwan dollar.</summary>
    [JsonStringEnumMemberName("TWD")]
    TWD,
    /// <summary>Tanzanian shilling.</summary>
    [JsonStringEnumMemberName("TZS")]
    TZS,
    /// <summary>Ukrainian hryvnia.</summary>
    [JsonStringEnumMemberName("UAH")]
    UAH,
    /// <summary>Ugandan shilling.</summary>
    [JsonStringEnumMemberName("UGX")]
    UGX,
    /// <summary>US dollar.</summary>
    [JsonStringEnumMemberName("USD")]
    USD,
    /// <summary>Uruguayan peso.</summary>
    [JsonStringEnumMemberName("UYU")]
    UYU,
    /// <summary>Venezuelan bolívar.</summary>
    [JsonStringEnumMemberName("VEF")]
    VEF,
    /// <summary>Vietnamese đồng.</summary>
    [JsonStringEnumMemberName("VND")]
    VND,
    /// <summary>Vanuatu vatu.</summary>
    [JsonStringEnumMemberName("VUV")]
    VUV,
    /// <summary>Central African CFA franc.</summary>
    [JsonStringEnumMemberName("XAF")]
    XAF,
    /// <summary>East Caribbean dollar.</summary>
    [JsonStringEnumMemberName("XCD")]
    XCD,
    /// <summary>West African CFA franc.</summary>
    [JsonStringEnumMemberName("XOF")]
    XOF,
    /// <summary>South African rand.</summary>
    [JsonStringEnumMemberName("ZAR")]
    ZAR,
    /// <summary>Zambian kwacha.</summary>
    [JsonStringEnumMemberName("ZMK")]
    ZMK
}
