using Lagoon.Server;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Lagoon backend Web application builder.
/// </summary>
public static class LagoonWebApplication
{

    /// <summary>
    /// Initialise a Web application.
    /// </summary>
    /// <param name="args">The main arguments.</param>
    /// <returns>The initialized application.</returns>
    public static void Run<TApplication>(string[] args = null)
    where TApplication : ILgApplication, new()
    {
        Run<TApplication>(new WebApplicationOptions() { Args = args });
    }

    /// <summary>
    /// Initialise a Web application.
    /// </summary>
    /// <param name="options">The web application options.</param>
    /// <returns>The initialized application.</returns>
    public static void Run<TApplication>(WebApplicationOptions options)
    where TApplication : ILgApplication, new()
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        // Build the application
        // Create The application manager instance
        using (TApplication main = new())
        {
            try
            {
                // Initialize the builder
                WebApplicationBuilder builder = WebApplication.CreateBuilder(options);
                // Load the configuration
                main.LoadApplicationInformation(builder.Configuration, builder.Environment.EnvironmentName);
                // Add the application services to the container
                main.ConfigureServices(builder.Services);
                // Add the Lagoon logging management
                main.ConfigureLogging(builder.Logging);
#if false //TOCLEAN
                Lagoon.Helpers.Trace.ToConsole($"Registred Services :");
                foreach (var service in builder.Services)
                {
                    Lagoon.Helpers.Trace.ToConsole($"\t* {service.ServiceType} ({service.ImplementationType}) {service.Lifetime}");
                }
#endif
                main.ConfigureWebHost(builder.WebHost);
                // Build the application
                WebApplication app = builder.Build();
                // Configure the application
                main.Configure(app);
                // Run the Web application configured by the "Main" class.
                app.Run();
            }
            catch (Exception ex)
            {
                // Trace unhandled exceptions
                main.TraceCriticalException(ex);
            }
        }
    }
}