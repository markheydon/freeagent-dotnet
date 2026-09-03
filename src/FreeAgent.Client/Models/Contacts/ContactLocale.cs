using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Contacts;

/// <summary>
/// Invoice and estimate language locale for a contact.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ContactLocale>))]
public enum ContactLocale
{
    /// <summary>Bulgarian (wire value: "bg").</summary>
    [JsonStringEnumMemberName("bg")] Bulgarian,
    /// <summary>Catalan (wire value: "ca").</summary>
    [JsonStringEnumMemberName("ca")] Catalan,
    /// <summary>Welsh (wire value: "cy").</summary>
    [JsonStringEnumMemberName("cy")] Welsh,
    /// <summary>Czech (wire value: "cz").</summary>
    [JsonStringEnumMemberName("cz")] Czech,
    /// <summary>German (wire value: "de").</summary>
    [JsonStringEnumMemberName("de")] German,
    /// <summary>Danish (wire value: "dk").</summary>
    [JsonStringEnumMemberName("dk")] Danish,
    /// <summary>English (wire value: "en").</summary>
    [JsonStringEnumMemberName("en")] English,
    /// <summary>English United States (wire value: "en-US").</summary>
    [JsonStringEnumMemberName("en-US")] EnglishUnitedStates,
    /// <summary>Spanish (wire value: "es").</summary>
    [JsonStringEnumMemberName("es")] Spanish,
    /// <summary>Estonian (wire value: "et").</summary>
    [JsonStringEnumMemberName("et")] Estonian,
    /// <summary>Finnish (wire value: "fi").</summary>
    [JsonStringEnumMemberName("fi")] Finnish,
    /// <summary>French (wire value: "fr").</summary>
    [JsonStringEnumMemberName("fr")] French,
    /// <summary>French Belgium (wire value: "fr-BE").</summary>
    [JsonStringEnumMemberName("fr-BE")] FrenchBelgium,
    /// <summary>French Canada (wire value: "fr-CA").</summary>
    [JsonStringEnumMemberName("fr-CA")] FrenchCanada,
    /// <summary>Icelandic (wire value: "is").</summary>
    [JsonStringEnumMemberName("is")] Icelandic,
    /// <summary>Italian (wire value: "it").</summary>
    [JsonStringEnumMemberName("it")] Italian,
    /// <summary>Latvian (wire value: "lv-LV").</summary>
    [JsonStringEnumMemberName("lv-LV")] Latvian,
    /// <summary>Dutch (wire value: "nl").</summary>
    [JsonStringEnumMemberName("nl")] Dutch,
    /// <summary>Dutch Belgium (wire value: "nl-BE").</summary>
    [JsonStringEnumMemberName("nl-BE")] DutchBelgium,
    /// <summary>Norwegian (wire value: "nk").</summary>
    [JsonStringEnumMemberName("nk")] Norwegian,
    /// <summary>Polish (wire value: "pl-PL").</summary>
    [JsonStringEnumMemberName("pl-PL")] Polish,
    /// <summary>Brazilian Portuguese (wire value: "pt-BR").</summary>
    [JsonStringEnumMemberName("pt-BR")] BrazilianPortuguese,
    /// <summary>Romanian (wire value: "ro").</summary>
    [JsonStringEnumMemberName("ro")] Romanian,
    /// <summary>Serbian (wire value: "rs").</summary>
    [JsonStringEnumMemberName("rs")] Serbian,
    /// <summary>Russian (wire value: "ru").</summary>
    [JsonStringEnumMemberName("ru")] Russian,
    /// <summary>Swedish (wire value: "se").</summary>
    [JsonStringEnumMemberName("se")] Swedish,
    /// <summary>Slovak (wire value: "sk").</summary>
    [JsonStringEnumMemberName("sk")] Slovak,
    /// <summary>Turkish (wire value: "tr").</summary>
    [JsonStringEnumMemberName("tr")] Turkish
}
