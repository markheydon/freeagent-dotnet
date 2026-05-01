using FreeAgent.Client.Infrastructure.Authentication;
using FreeAgent.Client.Sample.Components;
using FreeAgent.Client.Sample.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
});

// NOTE: TokenStore and OAuthService are registered as singletons.
// This sample is intended for local, single-user development use only.
// In a multi-user scenario, tokens would be shared across all circuits/sessions.
builder.Services.AddSingleton<TokenStore>();
builder.Services.AddSingleton<OAuthService>();
builder.Services.AddSingleton<ApiDiagnosticsService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();

// OAuth authorization callback — FreeAgent redirects here after the user approves or denies the app.
app.MapGet("/oauth/callback", async (
    string? code,
    string? state,
    string? error,
    TokenStore tokenStore,
    OAuthService oauthService) =>
{
    // FreeAgent returned an error (e.g. user denied access)
    if (!string.IsNullOrEmpty(error))
    {
        return Results.Redirect($"/?auth_error={Uri.EscapeDataString(error)}");
    }

    if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
    {
        return Results.Redirect("/?auth_error=missing_params");
    }

    // CSRF check
    if (!tokenStore.ValidateAndClearState(state, out var pendingEnvironment))
    {
        return Results.Redirect("/?auth_error=invalid_state");
    }

    try
    {
        var token = await oauthService.ExchangeCodeAsync(code);
        tokenStore.SetToken(token, pendingEnvironment);
    }
    catch (FreeAgentOAuthException ex)
    {
        Log.OAuthRejected(app.Logger, ex);
        return Results.Redirect("/?auth_error=token_exchange_failed");
    }
    catch (HttpRequestException ex)
    {
        Log.OAuthNetworkError(app.Logger, ex);
        return Results.Redirect("/?auth_error=network_error");
    }
    catch (Exception ex)
    {
        Log.OAuthUnexpectedError(app.Logger, ex);
        return Results.Redirect("/?auth_error=unexpected_error");
    }

    return Results.Redirect("/");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static partial class Log
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "OAuth token exchange rejected by FreeAgent.")]
    public static partial void OAuthRejected(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Error, Message = "Network error during OAuth token exchange.")]
    public static partial void OAuthNetworkError(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unexpected error during OAuth token exchange.")]
    public static partial void OAuthUnexpectedError(ILogger logger, Exception ex);
}
