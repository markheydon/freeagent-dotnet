using System.Globalization;
using System.Text.Json;
using FreeAgent.Client.Models.Users;

namespace FreeAgent.Client.Tests.Models.Users;

public class UserModelSerializationTests
{
    [Theory]
    [InlineData("\"Owner\"", UserRole.Owner)]
    [InlineData("\"Director\"", UserRole.Director)]
    [InlineData("\"Partner\"", UserRole.Partner)]
    [InlineData("\"Company Secretary\"", UserRole.CompanySecretary)]
    [InlineData("\"Employee\"", UserRole.Employee)]
    [InlineData("\"Shareholder\"", UserRole.Shareholder)]
    [InlineData("\"Accountant\"", UserRole.Accountant)]
    public void UserRole_DeserializesWireValues(string wireValue, UserRole expected)
    {
        var user = JsonSerializer.Deserialize<User>($$"""{ "role": {{wireValue}} }""");

        Assert.Equal(expected, user!.Role);
    }

    [Theory]
    [InlineData(UserRole.CompanySecretary, "Company Secretary")]
    [InlineData(UserRole.Owner, "Owner")]
    public void UserRole_SerializesWireValues(UserRole role, string expectedWireValue)
    {
        var payload = UserWritePayload.FromUser(new User { Role = role });
        var json = JsonSerializer.Serialize(new UserRequest { User = payload });

        Assert.Contains($"\"role\":\"{expectedWireValue}\"", json, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(0, UserPermissionLevel.NoAccess)]
    [InlineData(1, UserPermissionLevel.Time)]
    [InlineData(8, UserPermissionLevel.Full)]
    public void UserPermissionLevel_DeserializesIntegerValues(int wireValue, UserPermissionLevel expected)
    {
        var user = JsonSerializer.Deserialize<User>($$"""{ "permission_level": {{wireValue}} }""");

        Assert.Equal(expected, user!.PermissionLevel);
    }

    [Fact]
    public void CurrentPayrollProfile_DeserializesNestedObject()
    {
        const string json = """
        {
          "current_payroll_profile": {
            "total_pay_in_previous_employment": "1000.5",
            "total_tax_in_previous_employment": "200.25"
          }
        }
        """;

        var user = JsonSerializer.Deserialize<User>(json);

        Assert.NotNull(user!.CurrentPayrollProfile);
        Assert.Equal(1000.5m, user.CurrentPayrollProfile.TotalPayInPreviousEmployment);
        Assert.Equal(200.25m, user.CurrentPayrollProfile.TotalTaxInPreviousEmployment);
    }

    [Fact]
    public void Serialize_OmitsReadOnlyFields()
    {
        var payload = UserWritePayload.FromUser(new User
        {
            Url = "https://api.freeagent.com/v2/users/42",
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            Role = UserRole.Employee,
            PermissionLevel = UserPermissionLevel.Time,
            CurrentPayrollProfile = new CurrentPayrollProfile { TotalPayInPreviousEmployment = 100m },
            CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z", CultureInfo.InvariantCulture),
            UpdatedAt = DateTimeOffset.Parse("2026-01-02T00:00:00Z", CultureInfo.InvariantCulture)
        });

        var json = JsonSerializer.Serialize(new UserRequest { User = payload });

        Assert.DoesNotContain("\"url\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("current_payroll_profile", json, StringComparison.Ordinal);
        Assert.DoesNotContain("created_at", json, StringComparison.Ordinal);
        Assert.DoesNotContain("updated_at", json, StringComparison.Ordinal);
        Assert.Contains("\"permission_level\":1", json, StringComparison.Ordinal);
    }
}
