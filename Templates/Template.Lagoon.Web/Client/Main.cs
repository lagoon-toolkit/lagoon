using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace TemplateLagoonWeb.Client;

/// <summary>
/// The web assembly application's configuration and event manager.
/// </summary>
public class Main : LgCustomApplication
{

    /// <summary>
    /// Configure the default behaviors of the application.
    /// </summary>
    /// <param name="app">Default behaviors of the application.</param>
    protected override void OnConfigureBehavior(ApplicationBehavior app)
    {
//-:cnd:noEmit
#if RELEASE
        // EULA configuration
        app.RequireEulaConsent = true;
        app.EulaFromServer = true;
#endif
//+:cnd:noEmit
        // Page "About" Logos
        app.DevelopedByLogoUri = new List<string> { LOGO_URI_DZ, LOGO_URI_INFOTEL };
        // Gridview global configuration
        app.GridView(options =>
        {
            options.ShowSummaryFilters = true;
        });
    }

    /// <summary>
    /// Register additional services for the application.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder.</param>
    protected override void OnInitialize(WebAssemblyHostBuilder builder)
    {
        // Set the target DOM element for root component 
        builder.RootComponents.Add<Shared.App>("#app");
        // Register services defined in this application
        Services.Registry.RegisterServices(builder.Services);
    }

    /// <summary>
    /// Define the authorization options.
    /// </summary>
    /// <param name="options">Authorization options.</param>
    protected override void OnConfigureAuthorization(AuthorizationOptions options)
    {
        Policies.ConfigureAuthorization(options);
    }

}
