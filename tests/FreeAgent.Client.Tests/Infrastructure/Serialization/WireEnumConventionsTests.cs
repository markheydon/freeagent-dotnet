using System.Reflection;
using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Company;

namespace FreeAgent.Client.Tests.Infrastructure.Serialization;

public class WireEnumConventionsTests
{
    private static readonly HashSet<Type> ExcludedEnumTypes =
    [
        typeof(SalesTaxRegistrationStatus)
    ];

    [Fact]
    public void WireEnums_UsingCompatibleConverter_DefineUnknownMember()
    {
        var assembly = typeof(FreeAgentClient).Assembly;
        var compatibleConverterDefinition = typeof(JsonStringEnumMemberNameCompatibleConverter<>);

        var enumTypes = assembly
            .GetTypes()
            .Where(t => t.IsEnum)
            .Where(t => t.GetCustomAttribute<JsonConverterAttribute>() is { ConverterType: { IsGenericType: true } converterType }
                        && converterType.GetGenericTypeDefinition() == compatibleConverterDefinition)
            .Where(t => !ExcludedEnumTypes.Contains(t))
            .ToList();

        Assert.NotEmpty(enumTypes);

        var missing = enumTypes
            .Where(t => !Enum.IsDefined(t, "Unknown"))
            .Select(t => t.FullName)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();

        Assert.True(missing.Count == 0, "Enums missing Unknown member:\n" + string.Join('\n', missing));
    }
}
