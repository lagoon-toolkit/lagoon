using Lagoon.Core.Application;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Builder;

namespace Lagoon.Server;


/// <summary>
/// Interface for an authenticated application.
/// </summary>
public interface ILgApplication : ILgApplicationBase, IDisposable
{

    #region properties

    /// <summary>
    /// Provides access to the current <see cref="HttpContext"/>, if one is available.
    /// </summary>
    internal IHttpContextAccessor HttpContextAccessor { get; }

    /// <summary>
    /// The wwwroot file provider.
    /// </summary>
    IFileProvider WebRootFileProvider { get; }

    /// <summary>
    /// The list of virtual path with their targets.
    /// </summary>
    internal IReadOnlyDictionary<string, string> VirtualPaths { get; }

    #endregion

    #region methods

    /// <summary>
    /// Configure the application.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    internal void Configure(IApplicationBuilder app);

    /// <summary>
    /// Configure Logger provider for the application.
    /// </summary>
    /// <param name="builder">The provide logging builder.</param>
    internal void ConfigureLogging(ILoggingBuilder builder);

    /// <summary>
    /// Configure the application web host.
    /// </summary>
    /// <param name="webHost">The web host.</param>
    internal void ConfigureWebHost(ConfigureWebHostBuilder webHost);

    /// <summary>
    /// Register services for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    internal void ConfigureServices(IServiceCollection services);

    /// <summary>
    /// The path of "index.html" file to use (eg: "/~/index.html" or "/~/index.development.html").
    /// </summary>
    /// <returns></returns>
    internal string GetIndexPath();

    /// <summary>
    /// Load the configuration settings and initialize the application information property.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    /// <param name="environmentName">The name of the environment in which the application is running.</param>
    internal void LoadApplicationInformation(IConfiguration configuration, string environmentName);

    /// <summary>
    /// Update the list of the file to save in the browser cache for an offline use.
    /// </summary>
    /// <param name="assets">The asset list.</param>
    internal void LoadOfflineAssets(OfflineAssetManager assets);

    /// <summary>
    /// Method called when the application starts.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal Task StartAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);

    /// <summary>
    /// Method called when the application is stoping.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal Task StopAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Trace a critical exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    internal void TraceCriticalException(Exception exception);

    /// <summary>
    /// Apply pending migrations and call the "OnUpdateDatabaseAsync()" method.
    /// </summary>
    /// <param name="scopedServiceProvider">A scoped service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task UpdateDatabaseAsync(IServiceProvider scopedServiceProvider, CancellationToken cancellationToken = default);

    #endregion
}
