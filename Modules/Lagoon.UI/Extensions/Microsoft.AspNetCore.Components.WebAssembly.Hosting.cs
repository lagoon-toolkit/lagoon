using Lagoon.UI.Application;


namespace Microsoft.AspNetCore.Components.WebAssembly.Hosting;

/// <summary>
/// Extensions methods
/// </summary>
public static class LagoonExtensions
{

    private const string DEFAULT_CULTURE = "en-US";

    /// <summary>
    /// Load the current application language to use from the cookie or from the default parameter.
    /// </summary>
    /// <param name="builder">A builder for configuring and creating a WebAssemblyHost.</param>
    /// <param name="app">Lagoon application</param>
    public static void SetCulture(this WebAssemblyHostBuilder builder, LgApplication app)
    {
        string appName = app.ApplicationInformation.RootName;
        string defaultCulture = app.GetDefaultCulture();
        string xLangCookieName = $".{appName}.Lang";
        bool shouldUpdateCookieLang = false;
        CultureInfo culture = null;
        // Get the JS runtime service
        ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
        IJSInProcessRuntime js = (IJSInProcessRuntime)serviceProvider.GetRequiredService<IJSRuntime>();
        // Restore selected language
        string lng = js.Invoke<string>("Lagoon.JsUtils.GetCookie", xLangCookieName);
        if (!string.IsNullOrEmpty(lng))
        {
            // Ensure that the value contained in the cookie is a language defined in the application
            if (!app.GetSupportedCultures().Contains(lng))
            {
                lng = defaultCulture;
                shouldUpdateCookieLang = true;
            }
            try
            {

                culture = new LgCultureInfo(lng);
            }
            catch (Exception)
            {
                //Ignore a wrong cookie value
            }
        }
        // Load the default language
        if (culture is null)
        {
            try
            {
                culture = new LgCultureInfo(defaultCulture);
                if (defaultCulture != DEFAULT_CULTURE)
                {
                    shouldUpdateCookieLang = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"The default culture \"{defaultCulture}\" can't be loaded.", ex);
            }
        }
        if (shouldUpdateCookieLang)
        {
            // Create and HTTP cookie to synchronize server language
            js.InvokeVoid("Lagoon.JsUtils.CreateCookie", xLangCookieName, culture.Name, 365 * 5);
        }
        // Change threads default language
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        // Change the HTML document attribute "lang"
        js.InvokeVoid("Lagoon.JsUtils.ChangeLang", xLangCookieName, culture.TwoLetterISOLanguageName, culture.Name);
    }

}

internal class LgHttpClientHandler : HttpClientHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            //If the task has been cancelled, we don't pack it in a new exception.
            throw ex;
        }
        catch (Exception ex)
        {
            throw new LgHttpFetchException(ex);
        }
    }
}
