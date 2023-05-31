using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Lagoon.UI.Application;

/// <summary>
/// Lagoon frontend Web application builder.
/// </summary>
public static class LagoonWebAssemblyApplication
{
    /// <summary>
    /// Run the Web application configured by the "Main" class.
    /// </summary>
    /// <param name="args">The main arguments.</param>
    public static async Task RunAsync<TApp>(string[] args) where TApp : LgApplication, new()
    {
        using (TApp main = new())
        {
            try
            {
                // Create and configure WebAssembly host
                WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
                // Add Lagoon required services
                await main.BuildHostAsync(builder);
                // Language used if the user has not explicitly chosen a language (rq: ApplicationInformation only available after BuildHostAsync)
                builder.SetCulture(main);
                main.LoadCulture();
                // Build the application
                WebAssemblyHost app = builder.Build();
                // Configure the build app
                await main.ConfigureHostAsync(app);
                // Launch the WebAssembly host
                await app.RunAsync();
            }
            catch (Exception ex)
            {
                // Hide the "app" element when critical error occurs
                main.TraceCriticalException(ex);
            }
        }
    }
}


