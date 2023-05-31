using Lagoon.Core.Application;
using Lagoon.Internal;
using Lagoon.Shared.Model;
using Lagoon.UI.Application.Auth;
using Lagoon.UI.Application.Logging;
using Lagoon.UI.Helpers.Route;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop.WebAssembly;

namespace Lagoon.UI.Application;


/// <summary>
/// Application configuration endpoint
/// </summary>
public abstract class LgApplication : LgApplicationBase
{
    #region constants

    /// <summary>
    /// Uri Désirade logo
    /// </summary>
    public const string LOGO_URI_DZ = "_content/Lagoon.UI/images/Logo_Desirade.png";

    /// <summary>
    /// Uri Infotel logo
    /// </summary>
    public const string LOGO_URI_INFOTEL = "_content/Lagoon.UI/images/Logo_Infotel.png";

    #endregion

    #region fields

    /// <summary>
    /// Dictionary is intentionally used instead of ReadOnlyDictionary to reduce Blazor size.
    /// </summary>
    private static readonly IReadOnlyDictionary<string, object> _emptyParametersDictionary = new Dictionary<string, object>();

    /// <summary>
    /// The prefix to use to build key names for LocalStorage and IndexedDB.
    /// </summary>
    private string _clientStoragePrefix;

    /// <summary>
    /// The culture to apply after confirmation.
    /// </summary>
    private CultureInfo _pendingChangeCulture;

    /// <summary>
    /// The routing table.
    /// </summary>
    private RouteTable _routeTable;

    /// <summary>
    /// A finger print of the source of root table.
    /// </summary>
    private RouteKey _routeTableLastBuiltForRouteKey;

    private DotNetObjectReference<LgApplication> _dotNetRef;

    #endregion

    #region properties

    /// <summary>
    /// The authentication bearer token.
    /// </summary>
    public IAccessTokenProvider AccessTokenProvider { get; private set; }

    /// <summary>
    /// Lagoon configuration for the application.
    /// </summary>
    public ApplicationBehavior BehaviorConfiguration { get; }

    /// <summary>
    /// The HttpClient provider.
    /// </summary>
    public IHttpClientFactory HttpClientFactory { get; private set; }

    /// <summary>
    /// The JS Runtime instance.
    /// </summary>
    public IJSInProcessRuntime JS => WasmJS;

    /// <summary>
    /// The localstorage manager.
    /// </summary>
    public LocalStorage LocalStorage { get; private set; }

    /// <summary>
    /// The navigation manager.
    /// </summary>
    public NavigationManager NavigationManager { get; private set; }

    /// <summary>
    /// Remove browser data store for the user when he log out.
    /// </summary>
    public SignOutCleaner SignOutCleaner { get; } = new();

    /// <summary>
    /// The JS runtime to access to unmarshalled methods.
    /// </summary>
    internal WebAssemblyJSRuntime WasmJS { get; private set; }

    /// <summary>
    /// Gets the browser window information.
    /// </summary>
    public WindowInformation WindowInformation { get; private set; }

    #endregion

    #region events

    /// <summary>
    /// Event raise when the size of the application window change.
    /// </summary>
    public event Func<WindowInformation, Task> OnWindowResize;

    /// <summary>
    /// Event raise when the WebPush subscription popup should be display
    /// </summary>
    internal event Func<Task> OnDisplayWebPushSubscription;

    /// <summary>
    /// Event raised when a service-worker ask for navigation
    /// </summary>
    public event Action<string> OnNavigateFromServiceWorker;

    #endregion

    #region Initialization

    /// <summary>
    /// New instance.
    /// </summary>
    public LgApplication()
    {
        Attributes.ServiceWorkerEnabledAttribute attribute = ApplicationAssembly.GetCustomAttribute<Attributes.ServiceWorkerEnabledAttribute>();
        BehaviorConfiguration = new ApplicationBehavior((attribute != null) && attribute.Value);
        InternalConfigureBehavior();
    }

    /// <summary>
    /// Extract the version number from the application DLL.
    /// </summary>
    /// <returns>The version number from the application DLL.</returns>
    protected override string InitializeFullVersion()
    {
        return ApplicationAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }

    /// <summary>
    /// Try to load the "App" section from the server "appSettings.json".
    /// </summary>
    /// <param name="httpClient">An anonymous HTTP client.</param>
    /// <param name="configuration">The configuration builder.</param>
    internal async Task TryLoadRemoteConfigurationAsync(IConfigurationBuilder configuration, HttpClient httpClient)
    {
        // We register the "AppSettings" key from local storage to deletion when user is disconnected
        string appSettingsKey = GetLocalStorageKey("AppSettings");
        SignOutCleaner.AddLocalStorageKey(appSettingsKey);
        // Try to get the configuration from the server
        string json = null;
        // Check the behavios settings
        if (BehaviorConfiguration.UseAppSettings)
        {

            try
            {
                // Try to download appSettings from server
                json = await httpClient.TryGetAsync<string>(Routes.CONFIGURATION_GET_URI);
                // If we get a JSON content
                if (!string.IsNullOrEmpty(json) && json[0] != '{')
                {
                    throw new Exception("lblParseError".Translate());
                }
                // Save the current configuration in the LocalStorage
                LocalStorage.SetItemAsString(appSettingsKey, json);
            }
            catch (Exception ex)
            {
                TraceWarning($"Failed to load configuration from remote server ({ex.Message}). Try to load from cache...");
                // Try to get the configuration from the LocalStorage
                json = LocalStorage.GetItemAsString(appSettingsKey);
                if (string.IsNullOrEmpty(json))
                {
                    throw new Exception("The \"App\" section of the \"appsettings.json\" server file can't be loaded. Check the server logs (and if the \"Server\" project is the startup project of the solution).", ex);
                }
            }
        }
        // We add the configuration with an empty class, else the "Configuration.Add" will fail without provider
        if (string.IsNullOrEmpty(json))
        {
            json = "{}";
        }
        // Load the configuration
        using (MemoryStream ms = new(Encoding.UTF8.GetBytes(json)))
        {
            configuration.AddJsonStream(ms);
        }
    }

    /// <summary>
    /// Register additional services for the application.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder.</param>
    /// <remarks>This method should only be overridden by intermediate customization projects (Lagoon.UI.*)</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal virtual void Initialize(WebAssemblyHostBuilder builder)
    {
        OnInitialize(builder);
    }

    /// <summary>
    /// Register additional services for the application.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected internal virtual void OnInitialize(WebAssemblyHostBuilder builder) { }

    /// <summary>
    /// Get the default logger provider for the application (To use before the application's host starts).
    /// </summary>
    /// <returns>The default logger provider for the application (To use before the application's host starts).</returns>
    protected override ILoggerProvider GetDefaultLoggerProvider()
    {
        return new LgClientLoggerProvider(this);
    }

    /// <summary>
    /// Configure the WebAssemblyHost.
    /// </summary>
    /// <param name="builder">A builder for configuring and creating a WebAssemblyHost.</param>
    internal async Task BuildHostAsync(WebAssemblyHostBuilder builder)
    {
        string publicUrl = BuildBaseAddress(builder.HostEnvironment.BaseAddress);
        // Register the <IHttpClientFactory> and <HttpClient> services
        RegisterHttpClientFactory(builder.Services, publicUrl, BehaviorConfiguration.DefaultAcceptHeader);
        // Get a service provider to get pre-injected singletons
        ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
        // JS runtime
        WasmJS = serviceProvider.GetRequiredService<IJSRuntime>() is WebAssemblyJSRuntime wasmJS ? wasmJS : throw new Exception("The WebAssemblyJSRuntime can't be found !");
        // Navigation manager
        NavigationManager = serviceProvider.GetRequiredService<NavigationManager>();
        // Calculate the localstorage prefix (ApplicationInformation is not loaded)
        _clientStoragePrefix = $"{ApplicationRootName}{new Uri(NavigationManager.BaseUri).AbsolutePath}";
        // LocalStorage
        LocalStorage = new(JS);
        // Get an anonymous HttpClient
        HttpClient httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateAnonymousClient();
        // Load section "App" from 'appSettings.json' server-side (Require LoacalStorage and HttpClientFactory property to be initialized)
        await TryLoadRemoteConfigurationAsync(builder.Configuration, httpClient);
        // Load the ApplicationInformation property after loading the configuration
        builder.Configuration["Lagoon:PublicUrl"] = publicUrl;
        LoadApplicationInformation(builder.Configuration, builder.HostEnvironment.Environment);
        // Configure the default collation
        if (int.TryParse(builder.Configuration["Lagoon:Collation"], out int intcol))
        {
            BehaviorConfiguration.TextFilterCollation = (CollationType)intcol;
        }
        else
        {
            BehaviorConfiguration.TextFilterCollation ??= CollationType.IgnoreCase;
        }
        // Apply migration for localStorage & indexedDb key names
        await JS.InvokeVoidAsync("Lagoon.JsUtils.CleanupLocalStorage", GetLocalStorageKey("{0}"));
        // Register services
        ConfigureServices(builder.Services);
        // Call the final application initialisation
        Initialize(builder);
    }

    ///<inheritdoc/>
    protected override void InternalConfigureServices(IServiceCollection services)
    {
        // Add <IApplicationBase> and <IApplicationInformation> services
        base.InternalConfigureServices(services);
        // Add the <LgApplication> service
        services.AddSingleton(this);
        services.AddSingleton(BehaviorConfiguration);
        // Register the server act as an OIDC provider due to OpenIddict integration by default
        RegisterOpenIdConnectProvider(services, ConfigureAuthorization);
        // Add custom log provider
        services.AddLogging(ConfigureLogging);
        // Add modal popup manager
        services.AddSingleton<WindowManager>();
        // Add signout manager
        services.AddScoped<SignOutManager>();
        // Policy management (helper)
        services.AddScoped<PolicyService>();
        // Keep a reference to the logger factory
        services.AddSingleton<MenuService>();
        // Keep user tabs opened
        ClientTabService.RegisterToSignoutManager(this);
        services.AddScoped<ClientTabService>();
    }

    /// <summary>
    /// Invoked by the ServiceWorkerBroadcastChannel when a WebPush notification has beeen clicked
    /// resulting in openning a tab in the application
    /// </summary>
    /// <param name="action"></param>
    [JSInvokable]
    public void NavigateFromServiceWorker(string action)
    {
        OnNavigateFromServiceWorker?.Invoke(action);
    }

    /// <summary>
    /// Configure the application.
    /// </summary>
    /// <param name="host">The WebAssemblyHost instance.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task ConfigureHostAsync(WebAssemblyHost host)
    {
        // Configure the host for the framework
        Configure(host.Services);
        // Call the application overridable method
        OnConfigureHost(host);
        _dotNetRef = DotNetObjectReference.Create(this);
        // Indicate that the appLagoon is initialized
        SetWindowInformation(JS.Invoke<WindowInformationData>("Lagoon.onAppInitialized", _dotNetRef));
        // Launch service-worker initialisation
        if (BehaviorConfiguration.ServiceWorkerEnabled)
        {
            if (BehaviorConfiguration.ServiceWorkerAutoInstall)
            {
                // Install service-worker
                await InitLagoonServiceWorkerAsync();
            }
            else
            {
                // Register to ui update event without installing the service-worker
                await JS.RegisterUiUpdateServiceWorkerAsync(_dotNetRef, ApplicationRootName);
            }
        }
        if (BehaviorConfiguration.RequireEulaConsent && BehaviorConfiguration.EulaFromServer)
        {
            await LoadConsentFromServerAsync(host.Services, this);
        }
    }

    /// <summary>
    /// Install service-worker
    /// </summary>
    public Task InitLagoonServiceWorkerAsync()
    {
        return JS.InitLagoonServiceWorkerAsync(_dotNetRef, ApplicationRootName);
    }

    ///<inheritdoc/>
    protected override void InternalConfigure(IServiceProvider serviceProvider)
    {
        base.InternalConfigure(serviceProvider);
        AccessTokenProvider = serviceProvider.GetRequiredService<IAccessTokenProvider>();
        // Get the host HttpClientFactory (If it's get earlier, the CreateAuthenticatedClient don't work)
        HttpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    }

    /// <summary>
    /// Configure the application.
    /// </summary>
    /// <param name="host">The WebAssemblyHost instance.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual void OnConfigureHost(WebAssemblyHost host) { }

    /// <summary>
    /// Adds the System.Net.Http.IHttpClientFactory and related services to the Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// and configures two System.Net.Http.HttpClient (One with authentication and one without). 
    /// </summary>
    /// <param name="services">Client services collection.</param>
    /// <param name="baseAddress">The base address for the application. This is typically derived from the 'base href' value in the host page.</param>
    /// <param name="defaultAcceptHeader">The default content-type to use in accept header </param>
    /// <returns> An Microsoft.Extensions.DependencyInjection.IHttpClientBuilder that can be used to configure the client.</returns>
    private static void RegisterHttpClientFactory(IServiceCollection services, string baseAddress, DefaultAcceptHeader defaultAcceptHeader)
    {
        string accept = defaultAcceptHeader == DefaultAcceptHeader.MessagePack ? "application/x-msgpack" : "application/json";
        // Configure anonymous HTTP client
        services.AddHttpClient(System.Net.Http.LagoonExtensions.ANONYMOUS_HTTP_CLIENT_NAME, client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", accept);
        }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new LgHttpClientHandler();
        });
        // Configure authenticated HTTP client
        RegisterAuthenticatedHttpClient(services, baseAddress, System.Net.Http.LagoonExtensions.AUTHENTICATED_HTTP_CLIENT_NAME, accept);
        RegisterAuthenticatedHttpClient(services, baseAddress, System.Net.Http.LagoonExtensions.JSON_AUTHENTICATED_HTTP_CLIENT_NAME, "application/json");
        // Supply HttpClient instances that include access tokens to use when making requests to the server project
        services.AddScoped(sp =>
        {
            return sp.GetRequiredService<IHttpClientFactory>().CreateClient(System.Net.Http.LagoonExtensions.AUTHENTICATED_HTTP_CLIENT_NAME);
        });
    }

    /// <summary>
    /// Add client to services.
    /// </summary>
    /// <param name="services">Client services collection.</param>
    /// <param name="baseAddress">The base address for the application. This is typically derived from the 'base href' value in the host page.</param>
    /// <param name="httpClientName">Client name.</param>
    /// <param name="accept">Header accept parameter.</param>
    private static void RegisterAuthenticatedHttpClient(IServiceCollection services, string baseAddress, string httpClientName, string accept)
    {
        // Configure authenticated HTTP client
        services.AddHttpClient(httpClientName, client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", accept);
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new LgHttpClientHandler();
        })
        .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
    }

    /// <summary>
    /// Retrieve EULA dat from Server
    /// </summary>
    /// <param name="serviceProvider">To retrieve services</param>
    /// <param name="app">Application manager</param>
    private static async Task LoadConsentFromServerAsync(IServiceProvider serviceProvider, LgApplication app)
    {
        HttpClient http = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateAnonymousClient();
        IEnumerable<Eula> eulasToLoad;
        string eulaKey = app.GetLocalStorageKey("EulaDico");

        if (app.LocalStorage.ContainKey(eulaKey))
        {
            IEnumerable<Eula> eulas = app.LocalStorage.GetItem<IEnumerable<Eula>>(eulaKey);
            // Check for a new version
            Eula lastEula = eulas.Where(x => x.Id == Eula.VersionKey).FirstOrDefault();
            if (lastEula == null)
            {
                // If no local version, remove the cache and try to download new eula version
                app.LocalStorage.RemoveItem(eulaKey);
                await LoadConsentFromServerAsync(serviceProvider, app);
                return;
            }
            eulasToLoad = eulas;
            string lastVersion = lastEula.Value;
            IEnumerable<Eula> newEulas = await http.TryGetAsync<IEnumerable<Eula>>($"{Routes.EULA_GET_URI}?version={lastVersion}");
            string newVersion = newEulas.Where(x => x.Id == Eula.VersionKey).FirstOrDefault()?.Value;
            if (newVersion != lastVersion)
            {
                if (newVersion is null)
                {
                    // If no more remote version, remove the cache 
                    app.LocalStorage.RemoveItem(eulaKey);
                    return;
                }
                else
                {
                    // Save new eulas locally
                    app.LocalStorage.SetItem(eulaKey, newEulas);
                    eulasToLoad = newEulas;
                }
            }
        }
        else
        {
            IEnumerable<Eula> newEulas = await http.TryGetAsync<IEnumerable<Eula>>($"{Routes.EULA_GET_URI}?version={DateTime.Now.Ticks}");
            app.LocalStorage.SetItem(eulaKey, newEulas);
            eulasToLoad = newEulas;
        }
        // Load eula
        Dictionary<string, Dictionary<string, string>> languagesToLoad = new();
        // Add eula translation keys
        foreach (Eula eula in eulasToLoad.Where(x => x.Id != Eula.VersionKey))
        {
            languagesToLoad.Add(eula.Id, new Dictionary<string, string>() { { Eula.ContentKey, eula.Value } });
        }
        // Add eula version keys (must be set for each language)
        Eula eulaVersionKey = eulasToLoad.Where(x => x.Id == Eula.VersionKey).FirstOrDefault();
        if (eulaVersionKey != null)
        {
            string eulaVersion = eulaVersionKey.Value;
            foreach (string dicoLngKey in languagesToLoad.Keys)
            {
                languagesToLoad[dicoLngKey].Add(Eula.VersionKey, eulaVersion);
            }
            app.LoadLanguages(languagesToLoad);
        }
    }

    /// <summary>
    /// Gets the HTTP base address to use for initializing the application.
    /// This is typically derived from the &gt;base href&lt; value in the host page.
    /// eg: 
    /// </summary>
    /// <param name="hostBaseAddress">The base address defined in the host environment.</param>
    /// <returns></returns>
    protected internal virtual string BuildBaseAddress(string hostBaseAddress)
    {
        return hostBaseAddress;
    }

    #endregion

    #region Configuration of dependency injection services



    #endregion

    #region Authentification configuration

    /// <summary>
    /// Register the server act as an OIDC provider due to OpenIddict integration by default.       
    /// </summary>
    /// <param name="services">The collection to register services.</param>
    /// <param name="authConfigure">An action delegate to configure the provided <see cref="AuthorizationOptions"/>.</param>
    /// <remarks>The application cand configure a OIDC provider by overriding this method and don't call base.RegisterOpenIdConnectProvider.</remarks>
    protected internal virtual void RegisterOpenIdConnectProvider(IServiceCollection services, Action<AuthorizationOptions> authConfigure)
    {
        // Add authorization services
        services.AddAuthorizationCore(authConfigure);
        // Configure the OpenId client for a self-hosted Openiddict authentication
        IRemoteAuthenticationBuilder<RemoteAuthenticationState, RemoteUserAccount> remoteAuthenticationBuilder = services.AddOidcAuthentication(options =>
        {
            // SPA authentication oidc configuration (APIs coexist with the authorization server)
            options.ProviderOptions.ClientId = $"{ApplicationInformation.RootName}.Client";
            options.ProviderOptions.Authority = ApplicationInformation.PublicURL;
            options.ProviderOptions.ResponseType = "code";
            // Note: response_mode=fragment is the best option for a SPA. Unfortunately, the Blazor WASM
            // authentication stack is impacted by a bug that prevents it from correctly extracting
            // authorization error responses (e.g error=access_denied responses) from the URL fragment.
            // For more information about this bug, visit https://github.com/dotnet/aspnetcore/issues/28344.
            options.ProviderOptions.ResponseMode = "query";
            // Add the "roles" (OpenIddictConstants.Scopes.Roles) scope and the "role" (OpenIddictConstants.Claims.Role) claim
            // (the same ones used in the Startup class of the Server) in order for the roles to be validated.
            // See the Counter component for an example of how to use the Authorize attribute with roles
            options.ProviderOptions.DefaultScopes.Add("roles");
            options.UserOptions.RoleClaim = "role";
            // We do the "AuthenticationService.completeSignOut" on a server page
            options.ProviderOptions.PostLogoutRedirectUri = "identity/account/signout";
        });
        // Custom claim factory to handle claims with array value
        if (BehaviorConfiguration.ServiceWorkerEnabled)
        {
            remoteAuthenticationBuilder.AddAccountClaimsPrincipalFactory<OfflineAccountClaimsPrincipalFactory>();
            OfflineAccountClaimsPrincipalFactory.RegisterToSignoutManager(this);
        }
        else
        {
            remoteAuthenticationBuilder.AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();
        }
    }

    /// <summary>
    /// Define the authorization options.
    /// </summary>
    /// <param name="options">Authorization options.</param>
    protected internal virtual void ConfigureAuthorization(AuthorizationOptions options)
    {
        OnConfigureAuthorization(options);
    }

    /// <summary>
    /// Define the authorization options.
    /// </summary>
    /// <param name="options">Authorization options.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected internal virtual void OnConfigureAuthorization(AuthorizationOptions options) { }

    #endregion

    #region Load global application parameters        

    /// <summary>
    /// Load lagoon configuration.
    /// </summary>
    internal void InternalConfigureBehavior()
    {
        var behaviorConfiguration = BehaviorConfiguration;
        // Call the overridable methods for customisation
        InternalConfigureBehavior(behaviorConfiguration);
        OnConfigureBehavior(behaviorConfiguration);
        // Consolidate the configuration
        behaviorConfiguration.Select.ApplyInputDefault(behaviorConfiguration.Input);
    }

    /// <summary>
    /// Load the culture to use.
    /// </summary>
    /// <exception cref="Exception"></exception>
    internal void LoadCulture()
    {
        // Configuration de la culture
        if (CultureInfo.CurrentCulture is not LgCultureInfo culture)
        {
            throw new Exception("The \"SetCulture\" method wasn't called on \"builder\" in \"Program.cs\".");
        }
        OnInitCulture(culture);
    }

    /// <summary>
    /// Configure the default behaviors of the application.
    /// </summary>
    /// <param name="app">Default behaviors of the application.</param>
    /// <remarks>This method should only be overridden by intermediate customization projects (Lagoon.UI.*)</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void InternalConfigureBehavior(ApplicationBehavior app)
    { }

    /// <summary>
    /// Configure the default behaviors of the application.
    /// </summary>
    /// <param name="app">Default behaviors of the application.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual void OnConfigureBehavior(ApplicationBehavior app)
    { }

    #endregion

    #region Load routable component from linked Lagoon assemblies

    /// <summary>
    /// Get the routing data associated with an uri.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>The routing data associated with an uri.</returns>
    internal RouteData GetRouteData(string uri)
    {
        if (uri is not null)
        {
            // Remove the relative folder
            if (uri.StartsWith("./"))
            {
                uri = uri[1..];
            }
            // Trim query and hash
            int firstIndex = uri.AsSpan().IndexOfAny('?', '#');
            // Find the handler
            uri = firstIndex < 0 ? uri : uri[..firstIndex];
        }
        RouteContext context = new(uri);
        _routeTable.Route(context);
        return context.Handler is null ? null : new RouteData(context.Handler, context.Parameters ?? _emptyParametersDictionary);
    }

    /// <summary>
    /// Load the route table.
    /// </summary>
    internal void RefreshRouteTable()
    {
        RouteKey routeKey = new(GetType().Assembly, GetRoutableComponentsAssemblies().Reverse());
        if (!routeKey.Equals(_routeTableLastBuiltForRouteKey))
        {
            _routeTable = RouteTableFactory.Create(routeKey);
            _routeTableLastBuiltForRouteKey = routeKey;
        }
    }

    /// <summary>
    /// Return a list of assemblies (linked to the current assembly) which contain routable component
    /// </summary>
    /// <returns>Assemblies with routable components</returns>

    private IEnumerable<Assembly> GetRoutableComponentsAssemblies()
    {

        System.IO.Stream fileStream = GetType().Assembly.GetManifestResourceStream("Lagoon.RoutableComponents");
        if (fileStream is not null)
        {
            using (fileStream)
            {
                using (System.IO.StreamReader reader = new(fileStream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Assembly asm = null;
                        try
                        {
                            asm = Assembly.Load(line);
                        }
                        catch (Exception)
                        {
                            TraceWarning($"The routes from the \"{line}\" library cannot be loaded.");
                        }
                        if (asm is not null)
                        {
                            yield return asm;
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region Multilanguage management

    /// <summary>
    /// Event triggered when language has changed. cf. SetLanguage()
    /// Rq: Protected because this event should not be handled by client app
    /// </summary>
    internal event Action<LanguageChangedEventArgs> OnLanguageChanged;

    ///<inheritdoc/>
    public override string GetDefaultLanguage()
    {
        return JS?.Invoke<string>("Lagoon.getUserLanguage");
    }

    ///<inheritdoc/>
    public override void SetCulture(CultureInfo culture)
    {
        ShowConfirmAsync("#changeLanguageMessage".CheckTranslate(), ChangeLanguageAcceptedAsync, "#changeLanguageTitle".CheckTranslate()).GetAwaiter();
        _pendingChangeCulture = culture;
    }

    /// <summary>
    /// Add additionnal dictionnary or overload existing key at runtime and notify ui
    /// </summary>
    /// <param name="additionnalDico"></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override void LoadLanguages(Dictionary<string, Dictionary<string, string>> additionnalDico)
    {
        base.LoadLanguages(additionnalDico);
        ChangeLanguage();
    }

    private void ChangeLanguage()
    {
        OnLanguageChanged?.Invoke(new LanguageChangedEventArgs());
    }

    /// <summary>
    /// Change the culture after modal confirmation.
    /// </summary>
    private Task ChangeLanguageAcceptedAsync()
    {
        base.SetCulture(_pendingChangeCulture);
        ChangeLanguage();
        return Task.CompletedTask;
    }

    #endregion

    #region Media management (orientation, type)

    /// <summary>
    /// Keep window information data.
    /// </summary>
    /// <param name="data">window information data</param>
    internal void SetWindowInformation(WindowInformationData data)
    {
        WindowInformation = new WindowInformation(data);
    }

    /// <summary>
    /// Method called when the browser window resizing.
    /// </summary>
    /// <param name="data">The window information data.</param>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task WindowResizeAsync(WindowInformationData data)
    {
        SetWindowInformation(data);
        if (OnWindowResize is not null)
        {
            await OnWindowResize(WindowInformation);
        }
    }

    #endregion

    #region Show Messages

    /// <summary>
    /// Show and trace an exception message
    /// </summary>
    /// <param name="ex">Exception to trace and display</param>
    public void ShowException(Exception ex)
    {
        switch (ex)
        {
            case LgValidationException _:
                // LgValidationException are managed by LgEditForm
                return;
            case TaskCanceledException _:
                // Don't show the task canceled errors
                return;
            case AggregateException aex:
                // Show all errors contained in Aggregate exception
                foreach (Exception e in aex.InnerExceptions)
                {
                    ShowException(e);
                }
                return;
        }
        // Log exceptions
        TraceException(ex);
        // Show ui message (In release mode, show an generic exception message)
        ShowError(GetContactAdminMessage(ex, true));
    }

    /// <summary>
    /// Display an error message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title of the message.</param>
    public void ShowError(string message, string title = null)
    {
        ShowToastr(Level.Error, message, title, true);
    }

    /// <summary>
    /// Display a HTML error message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title of the message.</param>
    public void ShowError(MarkupString message, MarkupString? title = null)
    {
        ShowToastr(Level.Error, message.Value, title?.Value, false);
    }

    /// <summary>
    /// Display a warning message
    /// </summary>
    /// <param name="message">toastr message</param>
    /// <param name="title">toastr title</param>
    public void ShowWarning(string message, string title = null)
    {
        ShowToastr(Level.Warning, message, title, true);
    }

    /// <summary>
    /// Display a warning HTML message.
    /// </summary>
    /// <param name="message">toastr message</param>
    /// <param name="title">toastr title</param>
    public void ShowWarning(MarkupString message, MarkupString? title = null)
    {
        ShowToastr(Level.Warning, message.Value, title?.Value, false);
    }

    /// <summary>
    /// Display a success message
    /// </summary>
    /// <param name="message">toastr message</param>
    /// <param name="title">toastr title</param>
    public void ShowSuccess(string message, string title = null)
    {
        ShowToastr(Level.Success, message, title, true);
    }

    /// <summary>
    /// Display a success HTML message.
    /// </summary>
    /// <param name="message">toastr message</param>
    /// <param name="title">toastr title</param>
    public void ShowSuccess(MarkupString message, MarkupString? title = null)
    {
        ShowToastr(Level.Success, message.Value, title?.Value, false);
    }

    /// <summary>
    /// Display an information message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title of the message.</param>
    public void ShowInformation(string message, string title = null)
    {
        ShowToastr(Level.Info, message, title, true);
    }

    /// <summary>
    /// Display a HTML information message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title of the message.</param>
    public void ShowInformation(MarkupString message, MarkupString? title = null)
    {
        ShowToastr(Level.Info, message.Value, title?.Value, false);
    }

    #endregion

    #region Toastr

    /// <summary>
    /// Event triggered when a toastr is displayed
    /// </summary>
    internal event Action<string, string, Level, bool> OnShowToastr;

    /// <summary>
    /// Show the toastr
    /// </summary>
    /// <param name="level">Level</param>
    /// <param name="message">Message</param>
    /// <param name="title">Title</param>
    /// <param name="escapeHtml">Indicate if the message and the title are using raw text (true) or HTML format (false).</param>
    private void ShowToastr(Level level, string message, string title, bool escapeHtml)
    {
        OnShowToastr?.Invoke(message, title, level, escapeHtml);
    }

    #endregion

    #region Screen Reader information
    /// <summary>
    /// Event trigger when Sr information must be displayed
    /// </summary>
    internal event Action<string> OnShowScreenReaderInformation;

    /// <summary>
    /// Display information for screen reader only
    /// </summary>
    /// <param name="message">Message</param>
    internal void ShowScreenReaderInformation(string message)
    {
        OnShowScreenReaderInformation?.Invoke(message);
    }
    #endregion

    #region Confirmation message

    /// <summary>
    /// Event triggered when a confirmation modal is displayed
    /// </summary>
    internal event Func<string, Func<Task>, Func<Task>, string, Task> OnShowConfirm;

    /// <summary>
    /// Show the confirmation modal
    /// </summary>
    /// <param name="confirmationMessage">Confirmation message</param>
    /// <param name="confirmCallback">Confirmation callback</param>
    /// <param name="title">Title of the window.</param>
    internal Task ShowConfirmAsync(string confirmationMessage, Func<Task> confirmCallback, string title = "")
    {
        return OnShowConfirm?.Invoke(confirmationMessage, confirmCallback, null, title);
    }

    /// <summary>
    /// Show the confirmation modal
    /// </summary>
    /// <param name="confirmationMessage">Confirmation message</param>
    /// <param name="confirmCallback">confirmation callback</param>
    /// <param name="cancelCallback">cancel callback</param>
    /// <param name="title">Title of the window.</param>
    internal Task ShowConfirmAsync(string confirmationMessage, Func<Task> confirmCallback, Func<Task> cancelCallback, string title = "")
    {
        return OnShowConfirm?.Invoke(confirmationMessage, confirmCallback, cancelCallback, title);
    }

    /// <summary>
    /// Event triggered when a confirmation modal is displayed
    /// </summary>
    internal event Action<string> OnShowModal;

    /// <summary>
    /// Show the confirmation modal
    /// </summary>
    /// <param name="route">Page url</param>
    internal void ShowModal(string route)
    {
        OnShowModal?.Invoke(route);
    }
    #endregion

    #region Navigation

    /// <summary>
    /// Navigates to the specified URI.
    /// </summary>
    /// <param name="uri">Uri to reach</param>
    /// <param name="forceLoad">Full navigation</param>
    /// <remarks>It's a shortcut to <see cref="NavigationManager.NavigateTo(string, bool)"/>.</remarks>
    public void NavigateTo(string uri, bool forceLoad = false)
    {
        NavigationManager.NavigateTo(uri, forceLoad);
    }

    #endregion

    #region Remote open or download methods

    /// <summary>
    /// Ask the browser to start a download from an URL.
    /// </summary>
    /// <param name="url">The url to download from.</param>
    public Task DownloadAsync(string url)
    {
        return DownloadAsync(url, null);
    }

    /// <summary>
    /// Ask the browser to start a download from an URL.
    /// </summary>
    /// <param name="url">The url to download from.</param>
    /// <param name="newFileName">The new name of the file.</param>
    public Task DownloadAsync(string url, string newFileName)
    {
        return OpenUriAsync(url, newFileName, null);
    }

    /// <summary>
    /// Ask the browser to open an URL in a (new) window.
    /// </summary>
    /// <param name="uri">The uri to navigate to.</param>
    /// <param name="target">The name of the target window in the browser.</param>
    public Task OpenWindowAsync(string uri, string target = "_blank")
    {
        return OpenUriAsync(uri, null, target);
    }

    /// <summary>
    /// Ask the browser to download, or open in a (new) window, an URI .
    /// </summary>
    /// <param name="uri">The uri to open or the url to download from.</param>
    /// <param name="newFileName">The new name of the file.</param>
    /// <param name="target">The name of the target window in the browser.</param>
    private async Task OpenUriAsync(string uri, string newFileName, string target)
    {
        // Get the token if the request is to the "Server" application (else it will be ignore by the JS process)
        string token = await GetAccessTokenValueAsync();
        // Call the Javascript process
        JS.Invoke<object>("Lagoon.JsFileUtils.OpenURL", uri, newFileName, target, token);
    }

    #endregion

    #region WebPush notification

    /// <summary>
    /// Subscribe to WebPush notification event
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    public async Task<bool> SubscribeToPushNotificationAsync()
    {
        if (string.IsNullOrEmpty(Configuration["App:VapidPublicKey"]))
        {
            throw new InvalidOperationException("The 'App:VapidPublicKey' is not configured. RTFM");
        }
        // Subcribe for webpush notification and retrieve the subcription object
        Core.Models.WebPushSubscription reg = await JS.SubcribeToPushNotificationAsync(Configuration["App:VapidPublicKey"]);
        if (reg != null)
        {
            // Send the subcription to the server
            await HttpClientFactory.CreateAuthenticatedClient().TryPostAsync("LgWebPushNotification", reg);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Unsubscribe to WebPush notification event
    /// </summary>
    /// <param name="deleteServerSideRegistration">Flag to indicate if the subscription info should be removed from server-side</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    public async Task UnsubscribeToPushNotificationAsync(bool deleteServerSideRegistration = true)
    {
        string oldEndPoint = await JS.UnsubscribeToPushNotificationAsync();
        // rq: oldEndpoint can be null if no active subscription found
        if (!string.IsNullOrEmpty(oldEndPoint) && deleteServerSideRegistration)
        {
            // in case of an previous valid registration: delete subscription from server-side too
            // rq: could be done with an unauthenticated client and anonylous endpoing (to speed up the signout process since we unregister subscription when disconnecting)
            await HttpClientFactory.CreateAuthenticatedClient().TryDeleteAsync($"LgWebPushNotification?endpoint={Uri.EscapeDataString(oldEndPoint)}");
        }
    }

    /// <summary>
    /// Show the WebPush notification subscription/unsubscription popup
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    public async Task DisplayWebPushSubscriptionPopupAsync()
    {
        if (OnDisplayWebPushSubscription != null)
        {
            await OnDisplayWebPushSubscription.Invoke();
        }
    }

    #endregion

    #region Local open or download methods

    #region Save client memory contents

    /// <summary>
    /// Ask the browser to open the "SaveAs" dialog to save the text as file.
    /// </summary>
    /// <param name="filename">The name of the file.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <param name="encoding">The encoding to apply to the string.</param>
    public void SaveAsFile(string filename, string contents, Encoding encoding)
    {
        OpenFromText(filename, contents, encoding, Tools.ExtrapolateContentType(filename, "text/plain"));
    }

    /// <summary>
    /// Ask the browser to open the "SaveAs" dialog to save the text as file.
    /// </summary>
    /// <param name="filename">The name of the file.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <remarks>This method uses UTF-8 encoding without a Byte-Order Mark (BOM).
    /// If it is necessary to include a UTF-8 identifier, such as a byte order mark, at the beginning of a file,
    /// use the <see cref="SaveAsFile(string, string, Encoding, string)"/> method overload with <see cref="Encoding.UTF8"/> encoding.
    /// </remarks>
    public void SaveAsFile(string filename, string contents)
    {
        SaveAsFile(filename, contents, new UTF8Encoding(false));
    }

    /// <summary>
    /// Ask the browser to open the "SaveAs" dialog to save the text as file.
    /// </summary>
    /// <param name="filename">The name of the file.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <param name="encoding">The encoding to apply to the string.</param>
    /// <param name="contentType">The content type of the file.</param>
    public void SaveAsFile(string filename, string contents, Encoding encoding, string contentType)
    {
        OpenFromText(filename, contents, encoding, contentType);
    }

    /// <summary>
    /// Ask the browser to open the "SaveAs" dialog to save the text as file.
    /// </summary>
    /// <param name="filename">The name of the file.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <param name="contentType">The content type of the file.</param>
    /// <remarks>This method uses UTF-8 encoding without a Byte-Order Mark (BOM).
    /// If it is necessary to include a UTF-8 identifier, such as a byte order mark, at the beginning of a file,
    /// use the <see cref="SaveAsFile(string, string, Encoding, string)"/> method overload with <see cref="Encoding.UTF8"/> encoding.
    /// </remarks>
    public void SaveAsFile(string filename, string contents, string contentType)
    {
        SaveAsFile(filename, contents, new UTF8Encoding(false), contentType);
    }

    /// <summary>
    /// Ask the browser to open the "SaveAs" dialog to save the Blob as file.
    /// </summary>
    /// <param name="filename">Browser download file name</param>
    /// <param name="contents">File content</param>
    public void SaveAsFile(string filename, byte[] contents)
    {
        OpenFromBlob(filename, null, contents, Tools.ExtrapolateContentType(filename));
    }

    /// <summary>
    /// Ask the browser to open the "SaveAs" dialog to save the Blob as file.
    /// </summary>
    /// <param name="filename">Browser download file name</param>
    /// <param name="contents">File content</param>
    /// <param name="contentType">Content Type</param>
    public void SaveAsFile(string filename, byte[] contents, string contentType)
    {
        OpenFromBlob(filename, null, contents, contentType);
    }

    #endregion

    #region Open client memory contents

    /// <summary>
    /// Show the text in a (new) window of the browser.
    /// </summary>
    /// <param name="contents">The text to show.</param>
    /// <param name="fileNameOrContentType">The name of the file OR the content type of the file.</param>
    /// <param name="target">The name of the target window in the browser.</param>
    public void OpenWindow(string contents, string fileNameOrContentType = "text/plain", string target = "_blank")
    {
        OpenWindow(contents, Encoding.UTF8, fileNameOrContentType, target);
    }

    /// <summary>
    /// Show the text in a (new) window of the browser.
    /// </summary>
    /// <param name="contents">The text to show.</param>
    /// <param name="encoding">The encoding to apply to the string.</param>
    /// <param name="fileNameOrContentType">The name of the file OR the content type of the file.</param>
    /// <param name="target">The name of the target window in the browser.</param>
    public void OpenWindow(string contents, Encoding encoding, string fileNameOrContentType = "text/plain", string target = "_blank")
    {
        string filename = Tools.DetectFileNameOrContentType(ref fileNameOrContentType, "text/plain");
        if (string.IsNullOrEmpty(target))
        {
            target = "_blank";
        }
        OpenFromText(filename, contents, encoding, fileNameOrContentType, target);
    }

    /// <summary>
    /// Show the file content in a (new) window of the browser.
    /// </summary>
    /// <param name="contents">The file content.</param>
    /// <param name="fileNameOrContentType">The name of the file OR the content type of the file.</param>
    /// <param name="target">The name of the target window in the browser.</param>
    public void OpenWindow(byte[] contents, string fileNameOrContentType, string target = "_blank")
    {
        string filename = Tools.DetectFileNameOrContentType(ref fileNameOrContentType, null);
        if (fileNameOrContentType is null)
        {
            throw new ArgumentException($"The contentType can't be extrapolated from the filename \"{filename}\"", nameof(fileNameOrContentType));
        }
        if (string.IsNullOrEmpty(target))
        {
            target = "_blank";
        }
        OpenFromBlob(filename, null, contents, fileNameOrContentType, target);
    }

    #endregion

    #region SaveAs

    /// <summary>
    /// Show the "Save as" box or show the text in a (new) window of the browser, depending if a target is specified.
    /// </summary>
    /// <param name="filename">The file name.</param>
    /// <param name="contents">The string to encode.</param>
    /// <param name="encoding">The encoding to apply to the string.</param>
    /// <param name="contentType">The content-type of the data.</param>
    /// <param name="target"><c>null</c> if navigator should download the file, otherwise open the file in the target window name.</param>
    private void OpenFromText(string filename, string contents, Encoding encoding, string contentType, string target = null)
    {
        OpenFromBlob(filename, encoding.GetPreamble(), encoding.GetBytes(contents), contentType, target);
    }

    /// <summary>
    /// Show the "Save as" box or show the file content in a (new) window of the browser, depending if a target is specified.
    /// </summary>
    /// <param name="filename">The file name.</param>
    /// <param name="bom">The byte order mark for text files.</param>
    /// <param name="contents">The content of the file.</param>
    /// <param name="contentType">The content-type of the data.</param>
    /// <param name="target"><c>null</c> if navigator should download the file, otherwise open the file in the target window name.</param>
    private void OpenFromBlob(string filename, byte[] bom, byte[] contents, string contentType, string target = null)
    {
        static void CheckArgument(string argName, string argValue)
        {
            if (argValue is not null && argValue.Contains('|'))
            {
                throw new ArgumentException($"The pipe character ('|') can't be used in the {argName} value.");
            }
        }
        if (JS is WebAssemblyJSRuntime js)
        {
            // We use the pipe character because the InvokeUnmarshalled can have a maximum 3 parameters
            CheckArgument(nameof(filename), filename);
            CheckArgument(nameof(contentType), contentType);
            CheckArgument(nameof(target), target);
            // Call the JS method
            js.InvokeUnmarshalled<string, byte[], byte[], bool>("Lagoon.JsFileUtils.OpenBlobUnmarshalled",
                $"{filename}|{target}|{contentType}", contents, bom);
        }
        else
        {
            throw new Exception("The WebAssemblyJSRuntime can't be found !");
        }
    }

    #endregion

    #endregion

    #region Utilities

    /// <summary>
    /// Create a key to use in LocalStorage/IndexedDB. Format: AppName|BaseHref|Lg'Setting'
    /// </summary>
    /// <param name="setting">Key name (without 'Lg' suffix)</param>
    /// <returns>The key for the setting</returns>
    public string GetLocalStorageKey(string setting)
    {
        return _clientStoragePrefix + setting;
    }

    /// <summary>
    /// Gets the serialized representation of the token.
    /// </summary>
    /// <returns>The serialized representation of the token. If the token is not found, <c>null</c> is returned.</returns>
    public async Task<string> GetAccessTokenValueAsync()
    {
        if (AccessTokenProvider is not null)
        {
            AccessTokenResult result = await AccessTokenProvider.RequestAccessToken();
            if (result is not null && result.TryGetToken(out AccessToken token))
            {
                return token?.Value;
            }
        }
        return null;
    }

    #endregion

    #region Trace exceptions

    ///<inheritdoc/>
    protected override void Trace(LogLevel logLevel, Exception exception, string message, params object[] args)
    {
        if (Logger is null)
        {
            if (exception is not null)
            {
                message = exception.ToString();
            }
            if (JS is null)
            {
                string prefix = logLevel switch
                {
                    LogLevel.Debug => "dbug",
                    LogLevel.Information => "info",
                    LogLevel.Warning => "warn",
                    LogLevel.Error => "fail",
                    LogLevel.Critical => "crit",
                    _ => "trce",
                };
                Console.Error.WriteLine($"{prefix}: {message}");
            }
            else
            {
                string method = logLevel switch
                {
                    LogLevel.Debug => "debug",
                    LogLevel.Information => "info",
                    LogLevel.Warning => "warn",
                    LogLevel.Error => "error",
                    _ => "log",
                };
                JS.InvokeVoid($"console.{method}", message);
            }
        }
        else
        {
            base.Trace(logLevel, exception, message, args);
        }
    }

    ///<inheritdoc/>
    [System.Diagnostics.StackTraceHidden]
    public override void TraceCriticalException(Exception exception)
    {
        try
        {
            // Trace to default logger
            base.TraceCriticalException(exception);
            // Trace to console
            if (JS is null)
            {
                Console.Error.WriteLine($"crit: {exception}");
            }
            else
            {
                JS.InvokeVoid("console.error", $"crit: {exception}");
            }
        }
        catch
        {
            //Fail to trace, use the default exception handler
            throw new Exception("Critical exception !", exception);
        }
    }

    #endregion

}