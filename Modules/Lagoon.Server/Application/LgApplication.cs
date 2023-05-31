using Lagoon.Core;
using Lagoon.Core.Application;
using Lagoon.Core.Application.Logging;
using Lagoon.Internal;
using Lagoon.Model.Context;
using Lagoon.Server.Application.Logging;
using Lagoon.Server.Controllers;
using Lagoon.Server.Middlewares;
using MessagePack;
using MessagePack.Resolvers;
using Lagoon.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Lagoon.Server.Application;

/// <summary>
/// Application core on server side
/// </summary>
public class LgApplication<TDbContext> : LgApplicationBase, ILgApplication
    where TDbContext : DbContext
{

    #region fields

    /// <summary>
    /// Provides access to the current <see cref="HttpContext"/>, if one is available.
    /// </summary>
    private IHttpContextAccessor _httpContextAccessor;


    /// <summary>
    /// The list of virtual path with their targets.
    /// </summary>
    private IReadOnlyDictionary<string, string> _virtualPaths;

    #endregion

    #region properties

    /// <summary>
    /// Provides access to the current <see cref="HttpContext"/>, if one is available.
    /// </summary>
    IHttpContextAccessor ILgApplication.HttpContextAccessor => _httpContextAccessor;

    /// <summary>
    /// The list of virtual path with their targets.
    /// </summary>
    IReadOnlyDictionary<string, string> ILgApplication.VirtualPaths => _virtualPaths;

    /// <summary>
    /// The content of the "wwwroot" folder.
    /// </summary>
    public IFileProvider WebRootFileProvider { get; private set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public LgApplication()
    { }

    /// <summary>
    /// New instance with an additionnal configurator.
    /// </summary>
    /// <param name="builder"></param>
    protected LgApplication(ILgApplicationBuilder builder)
        : base(builder)
    { }

    #endregion

    #region database configuration and update

    /// <summary>
    /// Configure the <see cref="DbContextOptions" /> for the context.
    /// This provides an alternative to performing configuration of the context
    /// by overriding the<see cref="DbContext.OnConfiguring" /> method in your derived context.
    /// </summary>
    /// <param name="builder">Configuring <see cref="DbContextOptions" />. Databases</param>
    /// <remarks>This method should only be overridden by intermediate customization projects (Lagoon.Server.*)</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal virtual void ConfigureDbContext(DbContextOptionsBuilder builder)
    {
        OnConfigureDbContext(builder);
    }

    /// <summary>
    /// Configure the <see cref="DbContextOptions" /> for the context.
    /// This provides an alternative to performing configuration of the context
    /// by overriding the<see cref="DbContext.OnConfiguring" /> method in your derived context.
    /// </summary>
    /// <param name="builder">Configuring <see cref="DbContextOptions" />. Databases</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual void OnConfigureDbContext(DbContextOptionsBuilder builder) { }

    /// <summary>
    /// Apply pending migrations and call the <see cref="OnUpdateDatabaseAsync(IServiceProvider, TDbContext, CancellationToken)"/> method.
    /// </summary>
    /// <param name="scopedServiceProvider">A scoped service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task UpdateDatabaseAsync(IServiceProvider scopedServiceProvider, CancellationToken cancellationToken = default)
    {
        TDbContext db = scopedServiceProvider.GetRequiredService<TDbContext>();
        await InternalUpdateDatabaseAsync(scopedServiceProvider, db, cancellationToken);
        await OnUpdateDatabaseAsync(scopedServiceProvider, db, cancellationToken);
    }

    /// <summary>
    /// Apply pending migrations.
    /// </summary>
    /// <param name="scopedServiceProvider">A scoped service provider.</param>
    /// <param name="db">The DbContext of the application.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected virtual Task InternalUpdateDatabaseAsync(IServiceProvider scopedServiceProvider, TDbContext db, CancellationToken cancellationToken)
    {
        return db.Database.MigrateAsync(cancellationToken);
    }

    /// <summary>
    /// Method called when the migrations have been applied.
    /// </summary>
    /// <param name="scopedServiceProvider">A scoped service provider.</param>
    /// <param name="db">The database.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected virtual Task OnUpdateDatabaseAsync(IServiceProvider scopedServiceProvider, TDbContext db, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    #endregion

    #region dependency injection

    /// <summary>
    /// Gets the complete application information.
    /// </summary>
    /// <param name="environmentName">The name of the environment in which the application is running.</param>
    /// <returns>The complete application information.</returns>
    protected override IApplicationInformation GetApplicationInformation(string environmentName)
    {
        return new ApplicationInformation(this, environmentName);
    }

    ///<inheritdoc/>
    protected override void InternalConfigureServices(IServiceCollection services)
    {
        base.InternalConfigureServices(services);
        // Add access to lagoon application
        services.AddSingleton<ILgApplication>(this);
        // Add the OpenIddict application intialisation
        services.AddHostedService<ApplicationInitializator>();
        // Push notification services
        services.AddScoped<IWebPushNotificationManager, WebPushNotificationManager>();
        services.AddScoped<IWebPushSubscriptionManager, WebPushSubscriptionManager>();
        // Add blazor pages
        ConfigureMvc(services.AddControllersWithViews());
        services.AddRazorPages();
        // Registration of the application database
        services.AddDbContext<TDbContext>(ConfigureDbContext);
        services.AddScoped(sp => (ILgApplicationDbContext)sp.GetRequiredService<TDbContext>());
        // Register authentication services
        RegisterAuthentication(services);
        // Register the ResponseFactory for the QueryableAttribute
        services.AddScoped(typeof(ResponseFactory<>));
    }

    ///<inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ConfigureWebHost(ConfigureWebHostBuilder webHost)
    {
        InternalConfigureWebHost(webHost);
        OnConfigureWebHost(webHost);
    }

    /// <summary>
    /// Configure the application web host.
    /// </summary>
    /// <param name="webHost">The web host.</param>
    /// <remarks>This method should only be overridden by intermediate customization projects (Lagoon.Server.*)</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void InternalConfigureWebHost(ConfigureWebHostBuilder webHost)
    { }

    /// <summary>
    /// Configure the application web host.
    /// </summary>
    /// <param name="webHost">The web host.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual void OnConfigureWebHost(ConfigureWebHostBuilder webHost)
    { }

    /// <summary>
    /// Configure the application.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Configure(IApplicationBuilder app)
    {
        // Call the final application configuration
        InternalConfigureRequestPipeline(app);
        // Call the final application configuration
        OnConfigure(app);
    }

    /// <summary>
    /// Configure the application.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual void OnConfigure(IApplicationBuilder app) { }

    ///<inheritdoc/>
    protected override void InternalConfigureLogging(ILoggingBuilder builder)
    {
        // Clear existing logger
        if (!"true".Equals(Configuration["Lagoon:DefaultLoggers:Enabled"], StringComparison.OrdinalIgnoreCase))
        {
            builder.ClearProviders();
            // Show, in the console, debug information for the current application and all the warnings and all errors
            string console = Configuration["Lagoon:LightConsoleLogger:Enabled"];
            if ("true".Equals(console, StringComparison.OrdinalIgnoreCase) || ((!"false".Equals(console, StringComparison.OrdinalIgnoreCase)) && ApplicationInformation.IsDevelopment))
            {
                builder.AddProvider(new LgConsoleLoggerProvider(this));
            }
        }
        // Add the file logger
        base.InternalConfigureLogging(builder);
    }

    ///<inheritdoc/>
    protected override LgFileLoggerOptions GetFileLoggerOptions()
    {
        LgFileLoggerOptions options = base.GetFileLoggerOptions();
        if (options is not null && options.Enabled)
        {
            LgConsoleLogger.Log(LogLevel.Information, $"The complete log is available here: {options.GetFilePath()}");
        }
        return options;
    }

    /// <summary>
    /// Create a logger that handle the HTTP request informations.
    /// </summary>
    /// <param name="options">The file logger options.</param>
    /// <returns>The new provider.</returns>
    protected override LgFileLoggerProvider CreateFileLogger(LgFileLoggerOptions options)
    {
        return new LgServerFileLoggerProvider(this, options);
    }

    /// <summary>
    /// Configure the application with the registred services.
    /// </summary>
    /// <param name="serviceProvider">The registred services provider.</param>
    /// <remarks>This method should only be overridden by intermediate customization projects (Lagoon.Server.*)</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void InternalConfigure(IServiceProvider serviceProvider)
    {
        base.InternalConfigure(serviceProvider);
        // Keep references to needed singletons
        _httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
        WebRootFileProvider = serviceProvider.GetRequiredService<IWebHostEnvironment>().WebRootFileProvider;
        _virtualPaths = LoadVirtualPaths(WebRootFileProvider);
    }

    /// <summary>
    /// Release the references to the singletons.
    /// </summary>
    protected override void FreeSingletonReferences()
    {
        base.FreeSingletonReferences();
        _httpContextAccessor = null;
        WebRootFileProvider = null;
    }

    /// <summary>
    /// Configure the application's request pipeline.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <remarks>This method should only be overridden by intermediate customization projects (Lagoon.Server.*)</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void InternalConfigureRequestPipeline(IApplicationBuilder app)
    {
        // Enable headers forwarding to work behing a reverse proxy
        if (Configuration.GetValue("Lagoon:UseForwardedHeaders", false))
        {
            ForwardedHeadersOptions fordwardedHeaderOptions = new()
            {
                ForwardedHeaders = ForwardedHeaders.All
            };
            fordwardedHeaderOptions.KnownNetworks.Clear();
            fordwardedHeaderOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(fordwardedHeaderOptions);
        }
        // Force HTTP query to be redirected to HTTPS
        if (ApplicationInformation.PublicURL.StartsWith("https://") && Configuration.GetValue("Lagoon:UseHttpsRedirection", true))
        {
            if (!ApplicationInformation.IsDevelopment && Configuration.GetValue("Lagoon:UseHsts", true))
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
        }
        // Catch unhandled exception
        if (!ApplicationInformation.IsDevelopment)
        {
            app.UseExceptionHandler("/Error");
        }
        // If a controller is called, we return unhandled errors as JSON
        app.UseWhen(x => x.Request.Path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase), builder =>
        {
            builder.UseExceptionHandler("/ApiError");
        });
        // Handle the special uris
        app.UseMiddleware<LagoonResourceMiddleware>();
        app.UseMiddleware<DataQueryRequestMiddleware>();
    }

    /// <summary>
    /// Called when the services for controllers are added by "AddControllersWithViews" .
    /// </summary>
    /// <param name="builder"></param>
    /// <remarks>This method should only be overridden by intermediate customization projects (Lagoon.Server.*)</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal virtual void ConfigureMvc(IMvcBuilder builder)
    {
        // Register Lagoon custom library (Lagoon.Server)
        Assembly lgApplicationAsm = typeof(ILgAuthApplication).Assembly;
        builder.AddApplicationPart(lgApplicationAsm);
        // Register Lagoon client custom library (Lagoon.Server.Client)
        Assembly lgClientAsm = GetType().BaseType.Assembly;
        if (lgClientAsm != lgApplicationAsm)
        {
            builder.AddApplicationPart(lgClientAsm);
        }
        // Register client customized routes ("Lagoon.Server.XXX") before "Lagoon.Server" routes to allow overriding
        builder.ConfigureApplicationPartManager(o =>
        {
            SortApplicationParts(o.ApplicationParts);
#if false //TOCLEAN
            Lagoon.Helpers.Trace.ToConsole(this, "Application parts :");
            foreach (ApplicationPart apart in o.ApplicationParts)
            {
                Lagoon.Helpers.Trace.ToConsole(this, $"\t> {apart.Name} ({apart.GetType().Name})");
            }
#endif
        });
        // Don't serialize the null value (For .NET 6, don't serialize the default values)
        builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
        });
        // Add MessagePack formatter for api response
        builder.AddMvcOptions(option =>
        {
            option.OutputFormatters.Add(new LagoonMessagePackOutputFormatter(new MessagePackSerializerOptions(TypelessContractlessStandardResolver.Instance)
                                            .WithCompression(MessagePackCompression.Lz4BlockArray)
                                            .WithAllowAssemblyVersionMismatch(true)
                                            .WithOmitAssemblyVersion(true)));
        });
        // Call application customization
        OnConfigureMvc(builder);
    }

    /// <summary>
    /// Change the "ApplicationPart" order to allow overriding routes defined in "Lagoon.Server", by routes defined in "Lagoon.Server.xxx".
    /// </summary>
    /// <param name="applicationParts">The list of applications parts.</param>
    private static void SortApplicationParts(IList<ApplicationPart> applicationParts)
    {
        /* Output sample :
        MyApp.Server (AssemblyPart)
        MyApp.Server (CompiledRazorAssemblyPart)
        MyApp.Model (AssemblyPart)
        Lagoon.Server.XXX (AssemblyPart)
        Lagoon.Server.XXX (CompiledRazorAssemblyPart)
        Lagoon.Server (CompiledRazorAssemblyPart)
        Lagoon.Server (AssemblyPart)
        Swashbuckle.AspNetCore.SwaggerGen (AssemblyPart)
        Microsoft.AspNetCore.Mvc.TagHelpers (FrameworkAssemblyPart)
        Microsoft.AspNetCore.Mvc.Razor (FrameworkAssemblyPart)
        */
        ApplicationPart part;
        int last = -1;
        for (int i = applicationParts.Count - 1; i >= 0; i--)
        {
            part = applicationParts[i];
            if (part.Name.StartsWith("Lagoon.", StringComparison.OrdinalIgnoreCase))
            {
                if (part.Name.Equals("Lagoon.Server", StringComparison.OrdinalIgnoreCase))
                {
                    if (last != -1)
                    {
                        applicationParts.Insert(last, part);
                        applicationParts.RemoveAt(i);
                    }
                }
                else
                {
                    if (last == -1)
                    {
                        last = i + 1;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Called when the services for controllers are added by "AddControllersWithViews" .
    /// </summary>
    /// <param name="builder"></param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual void OnConfigureMvc(IMvcBuilder builder) { }

    /// <summary>
    /// Register services for the authentication.
    /// </summary>
    /// <param name="services">The service collection.</param>
    internal virtual void RegisterAuthentication(IServiceCollection services)
    {
    }

    #endregion

    #region Start & Stop events

    /// <summary>
    /// Method called when the application starts.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    async Task ILgApplication.StartAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        //// Register the singleton
        Configure(serviceProvider);
        // Initialize the application
        using (IServiceScope scope = ServiceScopeFactory.CreateScope())
        {
            await InternalStartAsync(scope.ServiceProvider, cancellationToken);
        }
    }

    /// <summary>
    /// Method called when the application is stoping.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    async Task ILgApplication.StopAsync(CancellationToken cancellationToken)
    {
        using (IServiceScope scope = ServiceScopeFactory.CreateScope())
        {
            await StopAsync(scope.ServiceProvider, cancellationToken);
        }
    }

    /// <summary>
    /// Method called when the application starts.
    /// </summary>
    /// <param name="scopedServiceProvider">A scoped service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>This method should only be overridden by intermediate customization projects (Lagoon.Server.*)</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual async Task InternalStartAsync(IServiceProvider scopedServiceProvider, CancellationToken cancellationToken)
    {
        if (Configuration.GetValue("AutoUpdateDatabase", true))
        {
            // Apply pending migrations and call the "OnUpdateDatabaseAsync" method
            await UpdateDatabaseAsync(scopedServiceProvider, cancellationToken);
        }
        await OnStartAsync(scopedServiceProvider, cancellationToken);
        // Keep this call for compatibility
        await OnStartAsync(scopedServiceProvider, ApplicationInformation.IsDevelopment, cancellationToken);
    }

    /// <summary>
    /// Method called when the application starts.
    /// </summary>
    /// <param name="scopedServiceProvider">A scoped service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual Task OnStartAsync(IServiceProvider scopedServiceProvider, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Method called when the application starts.
    /// </summary>
    /// <param name="serviceProvider">A scoped service provider.</param>
    /// <param name="isDevelopment">Indicate if the current host environment name is "Development".</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual Task OnStartAsync(IServiceProvider serviceProvider, bool isDevelopment, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Method called when the application is stoping.
    /// </summary>
    /// <param name="scopedServiceProvider">A scoped service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>This method should only be overridden by intermediate customization projects (Lagoon.Server.*)</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual Task StopAsync(IServiceProvider scopedServiceProvider, CancellationToken cancellationToken)
    {
        return OnStopAsync(scopedServiceProvider, cancellationToken);
    }

    /// <summary>
    /// Method called when the application is stoping.
    /// </summary>
    /// <param name="serviceProvider">A scoped service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual Task OnStopAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        FreeSingletonReferences();
        return Task.CompletedTask;
    }

    #endregion

    #region DataProtectionProvider

    /// <summary>
    /// Encrypt a value
    /// </summary>
    /// <param name="value">Value to encrypt</param>
    /// <param name="purpose">The purpose to be assigned to the newly-created Microsoft.AspNetCore.DataProtection.IDataProtector.</param>
    /// <remarks>
    ///  The purpose parameter must be unique for the intended use case; two different
    ///  Microsoft.AspNetCore.DataProtection.IDataProtector instances created with two
    ///  different purpose values will not be able to decipher each other's payloads.
    ///  The purpose parameter value is not intended to be kept secret.
    /// </remarks>
    public string Protect(string value, string purpose)
    {
        return GetDataProtectionProvider(purpose).Protect(value);
    }

    /// <summary>
    /// Decrypt a value
    /// </summary>
    /// <param name="value">Value to decrypt</param>
    /// <param name="purpose">The purpose to be assigned to the newly-created Microsoft.AspNetCore.DataProtection.IDataProtector.</param>
    public string Unprotect(string value, string purpose)
    {
        return GetDataProtectionProvider(purpose).Unprotect(value);
    }

    /// <summary>
    /// Return the path to the directory wich contains generated keys
    /// </summary>
    /// <returns>Path to the directory</returns>
    private string GetKeyStoreDirectoryPath()
    {
        return Configuration.GetValue<string>("Lagoon:Authentication:KeyStoreDirectoryPath");
    }

    /// <summary>
    /// Return the application name configured in appSettings.json or project Assembly name if not defined
    /// </summary>
    /// <returns>Application name from appsettings.json or assemblt name</returns>
    private string GetApplicationIdentifier()
    {
        return Configuration.GetValue("Lagoon:Authentication:ApplicationIdentifier", ApplicationInformation.RootName);
    }

    /// <summary>
    /// Create a DataProtectionProvider for the desired purpose
    /// </summary>
    /// <param name="purpose">The purpose to be assigned to the newly-created Microsoft.AspNetCore.DataProtection.IDataProtector.</param>
    /// <returns>The IDataProtector initialized</returns>
    private IDataProtector GetDataProtectionProvider(string purpose)
    {
        // https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-6.0
        string keyFolder = GetKeyStoreDirectoryPath();
        if (!string.IsNullOrWhiteSpace(keyFolder))
        {
            // Use the specified directory to store keys
            return DataProtectionProvider.Create(new System.IO.DirectoryInfo(keyFolder)).CreateProtector(purpose);
        }
        // Use application name as unique identifier
        return DataProtectionProvider.Create(GetApplicationIdentifier()).CreateProtector(purpose);
    }

    #endregion

    #region Virtual paths

    /// <summary>
    /// Load the list of virtual paths with their target paths.
    /// </summary>
    /// <param name="webRootProvider">The wwrot file provider.</param>
    /// <returns>The list of virtual paths with their target paths.</returns>
    private IReadOnlyDictionary<string, string> LoadVirtualPaths(IFileProvider webRootProvider)
    {
        // Get the file to use as "/index.html"
        string indexPath = $"{Routes.VIRTUAL_ROOT_PATH}index.{ApplicationInformation.EnvironmentName}.html";
        if (!webRootProvider.GetFileInfo(indexPath).Exists)
        {
            indexPath = $"{Routes.VIRTUAL_ROOT_PATH}index.html";
        }
        Dictionary<string, string> dico = new()
        {
            // Redirect the index.html request to the generated ~/index.ENVIRONMENT.html if exists else ~/index.html
            { Routes.INDEX_PATH, indexPath },
            // Redirect to generated files
            { "/styles.css" , $"{Routes.VIRTUAL_ROOT_PATH}styles.css" },
            { "/icons.svg" , $"{Routes.VIRTUAL_ROOT_PATH}icons.svg" },
            { "/main.min.js", $"{Routes.VIRTUAL_ROOT_PATH}main.min.js" },
            // Redirect the service-worker-assets.js to the dynamic generation
            { LgServiceWorkerAssetsController.SW_ASSETS_FILE, $"/{Routes.SW_ASSETS_ROUTE}"}
        };
        return dico.ToImmutableSortedDictionary();
    }

    /// <summary>
    /// Get the application html physical file path.
    /// </summary>
    /// <returns>The application html phisical file path.</returns>
    string ILgApplication.GetIndexPath()
    {
        return _virtualPaths[Routes.INDEX_PATH];
    }

    /// <summary>
    /// Update the list of the file to save in the browser cache for an offline use.
    /// </summary>
    /// <param name="assets">The asset list.</param>
    void ILgApplication.LoadOfflineAssets(OfflineAssetManager assets)
    {
        InternalLoadOfflineAssets(assets);
        OnLoadOfflineAssets(assets);
    }

    /// <summary>
    /// Method called when the list of offline asset is created.
    /// </summary>
    /// <param name="assets">Tle list of offline assets.</param>
    /// <remarks>The base method does nothing.</remarks>
    protected virtual void InternalLoadOfflineAssets(OfflineAssetManager assets)
    {
        assets.IncludeOnlyExtensions(".blat", ".css", ".dat", ".dll", ".gif", ".html", ".ico", ".jpeg", ".jpg", ".js", ".json", ".png", ".svg", ".ttf", ".wasm", ".woff");
        if (ApplicationInformation.IsDevelopment)
        {
            assets.IncludeExtensions(".pdb");
        }
        assets.Exclude("service-worker.js");
        // Replace virtual paths
        IReadOnlyDictionary<string, string> rVPaths = _virtualPaths.GroupBy(p => p.Value, p => p.Key).ToImmutableDictionary(g => g.Key, g => g.First());
        string indexPath = _virtualPaths[Routes.INDEX_PATH].ToLowerInvariant();
        foreach (OfflineAsset asset in assets.Enumerate())
        {
            string path = $"/{asset.Url.ToLowerInvariant()}";
            if (path.StartsWith("/index.", StringComparison.Ordinal) && path.EndsWith(".html", StringComparison.Ordinal))
            {
                // Remove all the index*.html from the root folder
                asset.Exclude = true;
            }
            else if (path.StartsWith(Routes.VIRTUAL_ROOT_PATH))
            {
                if (path.StartsWith($"{Routes.VIRTUAL_ROOT_PATH}index.", StringComparison.Ordinal) && path.EndsWith(".html", StringComparison.Ordinal))
                {
                    // We keep only the used index.html
                    asset.Exclude = !string.Equals(path, indexPath, StringComparison.Ordinal);
                    if (!asset.Exclude)
                    {
                        asset.Url = Routes.INDEX_PATH[1..];
                    }
                }
                else
                {
                    // Change URL from "_vroot/filename.ext" to "filename.ext"
                    if (rVPaths.TryGetValue(path, out string vPath))
                    {
                        asset.Url = rVPaths[path][1..];
                    }
                    else
                    {
                        TraceWarning($"There no virtual path associated to the {path} file. (The file will not be available offline)");
                        asset.Exclude = true;
                    }
                }
            }
        }
        // Add some controllers contents
        assets.Add(Routes.CONFIGURATION_GET_URI);
        assets.Add(Routes.EULA_GET_URI);
    }

    /// <summary>
    /// Method called when the list of offline asset is created.
    /// </summary>
    /// <param name="assets">Tle list of offline assets.</param>
    /// <remarks>The base method does nothing.</remarks>
    protected virtual void OnLoadOfflineAssets(OfflineAssetManager assets)
    { }

    #endregion

    #region Trace exceptions

    ///<inheritdoc/>
    public override void TraceCriticalException(Exception exception)
    {
        // Trace to console
        LgConsoleLogger.Log(LogLevel.Critical, exception.Message, exception.ToString(), ApplicationRootName);
        // Trace to the default logger (File Logger)
        base.TraceCriticalException(exception);
        // Trace to Window Event Viewer
        bool isWindows =
                OperatingSystem.IsWindows();
        if (isWindows)
        {
            // ".NET Runtime" and 1000 values are mandatory : https://stackoverflow.com/a/46834838/56621
            EventLog.WriteEntry(".NET Runtime", GetEventViewerDescription(exception), EventLogEntryType.Error, 1000);
        }
    }

    #endregion

}
