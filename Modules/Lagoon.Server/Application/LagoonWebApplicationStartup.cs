using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Lagoon.Server.Application;


/// <summary>
/// A startup manager to configure a IWebHostBuilder.
/// </summary>
/// <typeparam name="TApplication">The main application.</typeparam>
internal class LagoonWebApplicationStartup<TApplication>
    where TApplication : ILgApplication, new()
{

    #region fields

    /// <summary>
    /// The main application.
    /// </summary>
    private TApplication _app;

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="env">The environment.</param>
    public LagoonWebApplicationStartup(IConfiguration configuration, IWebHostEnvironment env)
    {
        // New instance of the application
        _app = new();
        // Load the configuration
        _app.LoadApplicationInformation(configuration, env.EnvironmentName);
    }

    #endregion

    #region methods

    /// <summary>
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services">The service collection to complete.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        // Register services
        _app.ConfigureServices(services);
        // Add the Lagoon logging management
        services.AddLogging(_app.ConfigureLogging);
    }

    /// <summary>
    /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    public void Configure(IApplicationBuilder app)
    {
        _app.Configure(app);
    }

    #endregion

}