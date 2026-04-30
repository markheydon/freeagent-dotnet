using FreeAgent.Client.Authentication;
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

builder.Services.AddSingleton<TokenStore>();
builder.Services.AddSingleton<OAuthService>();

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
    if (!tokenStore.ValidateAndClearState(state))
    {
        return Results.Redirect("/?auth_error=invalid_state");
    }

    try
    {
        var token = await oauthService.ExchangeCodeAsync(code);
        tokenStore.SetToken(token);
    }
    catch (FreeAgentOAuthException ex)
    {
        return Results.Redirect($"/?auth_error={Uri.EscapeDataString(ex.Message)}");
    }

    return Results.Redirect("/");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
