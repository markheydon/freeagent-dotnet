using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Categories;

namespace FreeAgent.Client.Tests.Infrastructure.Serialization;

public class NullableBooleanJsonConverterTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new NullableBooleanJsonConverter() }
    };

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("True", true)]
    [InlineData("False", false)]
    public void Read_StringBooleanValues_RoundTrip(string jsonValue, bool expected)
    {
        var json = $$"""{"allowable_for_tax":"{{jsonValue}}"}""";

        var category = JsonSerializer.Deserialize<Category>(json, Options);

        Assert.Equal(expected, category!.AllowableForTax);
    }

    [Theory]
    [InlineData("0", false)]
    [InlineData("1", true)]
    public void Read_NumericBooleanValues_ReturnsExpected(string jsonValue, bool expected)
    {
        var json = $$"""{"allowable_for_tax":{{jsonValue}}}""";

        var category = JsonSerializer.Deserialize<Category>(json, Options);

        Assert.Equal(expected, category!.AllowableForTax);
    }

    [Fact]
    public void Read_Null_ReturnsNull()
    {
        const string json = """{"allowable_for_tax":null}""";

        var category = JsonSerializer.Deserialize<Category>(json, Options);

        Assert.Null(category!.AllowableForTax);
    }

    [Fact]
    public void Read_InvalidString_ThrowsJsonException()
    {
        const string json = """{"allowable_for_tax":"not-a-bool"}""";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Category>(json, Options));
    }

    [Fact]
    public void Read_InvalidNumber_ThrowsJsonException()
    {
        const string json = """{"allowable_for_tax":2}""";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Category>(json, Options));
    }
}
