#:project ../../src/FreeAgent.Client/FreeAgent.Client.csproj
#:property PublishAot=false

using System.Text.Json;
using FreeAgent.Client;

var fixtureJson = """
    {
      "company": {
        "url": "https://api.freeagent.com/v2/company",
        "name": "Example Ltd",
        "type": "UkLimitedCompany",
        "currency": "GBP"
      }
    }
    """;

var response = JsonSerializer.Deserialize<CompanyResponse>(fixtureJson, FreeAgentJsonSerialization.ApiSerializerOptions)
    ?? throw new InvalidOperationException("Company fixture deserialised to null.");

if (string.IsNullOrWhiteSpace(response.Company?.Name))
{
    throw new InvalidOperationException("Expected company name from fixture.");
}

Console.WriteLine($"Deserialised company: {response.Company.Name}");

var accessToken = Environment.GetEnvironmentVariable("FREEAGENT_ACCESS_TOKEN");
if (!string.IsNullOrWhiteSpace(accessToken))
{
    var environment = string.Equals(
        Environment.GetEnvironmentVariable("FREEAGENT_ENVIRONMENT"),
        "Production",
        StringComparison.OrdinalIgnoreCase)
        ? FreeAgentEnvironment.Production
        : FreeAgentEnvironment.Sandbox;

    using var client = new FreeAgentClient(accessToken, environment);
    var live = await client.Company.GetCompanyAsync();
    Console.WriteLine($"Live company: {live.Name}");
}
