using FreeAgent.Client;
using FreeAgent.Client.ConsoleSample;
using FreeAgent.Client.ConsoleSample.Samples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

ConsoleRunOptions runOptions;
try
{
    runOptions = ConsoleRunOptions.Parse(args);
}
catch (InvalidOperationException ex)
{
    Console.Error.WriteLine(ex.Message);
    return 1;
}

FreeAgentEnvironment environment;
try
{
    environment = SampleEnvironment.Resolve();
}
catch (InvalidOperationException ex)
{
    Console.Error.WriteLine(ex.Message);
    return 1;
}

var allowProductionWrites = SandboxWriteGuard.ResolveAllowProductionWrites(runOptions.AllowProductionWrites);

if (runOptions.RunAll)
{
    try
    {
        SandboxWriteGuard.EnsureRunAllAllowed(environment);
    }
    catch (InvalidOperationException ex)
    {
        Console.Error.WriteLine(ex.Message);
        return 1;
    }
}

var settings = AppSettings.Load();

using var oauthClient = new FreeAgentOAuthClient(
    settings.ClientId,
    settings.ClientSecret,
    settings.RedirectUri,
    environment);

Console.WriteLine("FreeAgent.Client console sample");
Console.WriteLine($"Environment: {environment}");
ConsoleSampleStartup.WriteEnvironmentNotice(environment, allowProductionWrites);
Console.WriteLine();

OAuthTokenResponse token;
try
{
    token = await AuthBootstrap.AuthenticateAsync(settings, oauthClient, runOptions.RunAll);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Authentication failed: {ex.Message}");
    return 1;
}

if (runOptions.BootstrapRefreshToken)
{
    try
    {
        RefreshTokenBootstrap.WriteInstructions(token);
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex.Message);
        return 1;
    }
}

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(token);
builder.Services.AddSingleton(_ =>
{
    var options = runOptions.RunAll ? RunAllHttpClientOptions.Create() : null;
    return new FreeAgentClient(oauthClient, token, environment, options);
});
builder.Services.AddSingleton(new SampleRuntimeContext(environment, allowProductionWrites));
builder.Services.AddSingleton<SampleContext>();
builder.Services.AddConsoleSamples();

using var host = builder.Build();

using var scope = host.Services.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<ConsoleSampleRunner>();

if (runOptions.RunAll)
{
    return await runner.RunAllAsync(runOptions);
}

_ = await runner.RunInteractiveAsync();
return 0;
