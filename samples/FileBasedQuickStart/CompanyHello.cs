#:project ../../src/FreeAgent.Client/FreeAgent.Client.csproj
#:property PublishAot=false

using FreeAgent.Client;

var accessToken = Environment.GetEnvironmentVariable("FREEAGENT_ACCESS_TOKEN")
    ?? throw new InvalidOperationException("Set FREEAGENT_ACCESS_TOKEN (run the OAuth Quick start step first).");

var environment = FreeAgentEnvironment.Sandbox;

using var client = new FreeAgentClient(accessToken, environment);
var company = await client.Company.GetCompanyAsync();
Console.WriteLine($"{company.Name} ({company.Currency})");
