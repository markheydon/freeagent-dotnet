using System.Text.Json;
using FreeAgent.Client.Models.Company;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class CurrencyCodeTests
{
    /// <summary>
    /// ISO 4217 codes listed at https://dev.freeagent.com/docs/currencies (frozen contract).
    /// </summary>
    private static readonly string[] DocumentedCodes =
    [
        "AED", "AMD", "AOA", "ARS", "AUD", "AWG", "AZN", "BBD", "BDT", "BGN",
        "BRL", "BWP", "CAD", "CHF", "CLP", "CNY", "COP", "CRC", "CUC", "CUP",
        "CZK", "DKK", "DOP", "EGP", "EUR", "FJD", "GBP", "GEL", "GHS", "GTQ",
        "GYD", "HKD", "HNL", "HRK", "HUF", "IDR", "ILS", "INR", "ISK", "JMD",
        "JPY", "KES", "KRW", "KWD", "KYD", "KZT", "LAK", "LBP", "LKR", "LTL",
        "LVL", "MAD", "MDL", "MGA", "MUR", "MVR", "MWK", "MXN", "MYR", "MZN",
        "NAD", "NGN", "NOK", "NPR", "NZD", "OMR", "PEN", "PHP", "PKR", "PLN",
        "QAR", "RON", "RSD", "RUB", "RWF", "SAR", "SCR", "SEK", "SGD", "THB",
        "TND", "TRY", "TTD", "TWD", "TZS", "UAH", "UGX", "USD", "UYU", "VEF",
        "VND", "VUV", "XAF", "XCD", "XOF", "ZAR", "ZMK"
    ];

    [Fact]
    public void EnumMembers_MatchDocumentedCurrencyList()
    {
        var actual = Enum.GetValues<CurrencyCode>()
            .Select(code => code.ToString())
            .OrderBy(code => code, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(DocumentedCodes.Length, actual.Length);
        Assert.Equal(DocumentedCodes, actual);
    }

    [Theory]
    [MemberData(nameof(AllCurrencyCodes))]
    public void RoundTrip_SerialisesAndDeserialisesWireValue(CurrencyCode code, string wireValue)
    {
        var json = JsonSerializer.Serialize(code);
        Assert.Equal($"\"{wireValue}\"", json);

        var parsed = JsonSerializer.Deserialize<CurrencyCode>($"\"{wireValue}\"");
        Assert.Equal(code, parsed);
    }

    [Fact]
    public void Company_DeserializesPreviouslyUnsupportedCurrency()
    {
        const string json = """
        {
          "currency": "AED"
        }
        """;

        var company = JsonSerializer.Deserialize<Company>(json);

        Assert.Equal(CurrencyCode.AED, company!.Currency);
    }

    [Fact]
    public void Company_DeserializesGbp()
    {
        const string json = """
        {
          "currency": "GBP"
        }
        """;

        var company = JsonSerializer.Deserialize<Company>(json);

        Assert.Equal(CurrencyCode.GBP, company!.Currency);
    }

    [Fact]
    public void Deserialize_UnknownCode_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CurrencyCode>("\"XXX\""));
    }

    public static IEnumerable<object[]> AllCurrencyCodes()
    {
        foreach (var code in Enum.GetValues<CurrencyCode>())
        {
            yield return [code, code.ToString()];
        }
    }
}
