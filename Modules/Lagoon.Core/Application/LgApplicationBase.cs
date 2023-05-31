using Lagoon.Core.Application.Configuration;
using Lagoon.Core.Application.Logging;
using Lagoon.Core.Language;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lagoon.Core.Application;

/// <summary>
/// Application manager.
/// </summary>
public abstract class LgApplicationBase : ILgApplicationBase, IDisposable
{

    #region static properties

    /// <summary>
    /// Current LgApplication instance
    /// </summary>
    public static LgApplicationBase Current { get; private set; }

    #endregion

    #region fields

    /// <summary>
    /// The default logger provider associated to the application. (used if a log occurs before the application host has started)
    /// </summary>
    private ILoggerProvider _defaultLoggerProvider;

    /// <summary>
    /// Application dictionnary
    /// </summary>
    private readonly Dico _appDico;

    /// <summary>
    /// Additionnal configurator.
    /// </summary>
    private ILgApplicationBuilder _builder;

    #endregion

    #region properties

    /// <summary>
    /// Application assembly
    /// </summary>
    protected Assembly ApplicationAssembly { get; }

    /// <summary>
    /// Get the current application informations.
    /// </summary>
    public IApplicationInformation ApplicationInformation { get; private set; }

    /// <summary>
    /// Gets the application's root name (based on the assembly name). Example : 'AppRootName.[Client|Shared|Server|...]'.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal string ApplicationRootName { get; }

    /// <summary>
    /// Configuration from appSettings.json.
    /// </summary>
    public IConfiguration Configuration { get; private set; }

    /// <summary>
    /// Check application mode (debug / release)
    /// </summary>
    /// <returns>true if the application is enabled, false otherwise</returns>
    public bool IsDebugEnabled { get; }

    /// <summary>
    /// Gets the current logger.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected ILogger Logger { get; private set; }

    /// <summary>
    /// Service provider
    /// </summary>
    public IServiceScopeFactory ServiceScopeFactory { get; private set; }

    #endregion

    #region constructors

    /// <summary>
    /// Application manager initialization
    /// </summary>
    public LgApplicationBase() : this(null) { }

    /// <summary>
    /// New instance with an additionnal configurator.
    /// </summary>
    /// <param name="builder"></param>
    protected LgApplicationBase(ILgApplicationBuilder builder)
    {
        _builder = builder;
        // Track all unhandled application error
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        // Get the executing assembly informations
        ApplicationAssembly = GetType().Assembly;
        // Check if the application is compiled in DEBUG or RELEASE
        DebuggableAttribute attrDebug = ApplicationAssembly.GetCustomAttribute<DebuggableAttribute>();
        IsDebugEnabled = attrDebug != null && attrDebug.IsJITOptimizerDisabled;
        // Extract the root name of the application
        try
        {
            ApplicationRootName = InitializeApplicationRootName();
        }
        catch (Exception ex)
        {
            throw new Exception($"Unexpected application assembly name {ApplicationAssembly.FullName}. Must be in the format 'AppRootName.[Client|Shared|Server|...]'", ex);
        }
        // Initialize languages (look for a Lagoon.Dico file built from linked assemblies by lgn)
        _appDico = new Dico(ApplicationAssembly);
        // Keep a variable on the current instance
        Current = this;
    }

    /// <summary>
    /// Extract the application root name.
    /// </summary>
    /// <returns>The application root name.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual string InitializeApplicationRootName()
    {
        return ApplicationAssembly.GetName().Name.Split('.')[0];
    }

    #endregion

    #region dispose resources

    /// <summary>
    /// Free resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Free resources.
    /// </summary>
    /// <param name="disposing">Indicate if the resource should be released.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Stop watching unhandled exception
            AppDomain.CurrentDomain.UnhandledException -= UnhandledExceptionTrapper;
            // Free the default logger
            if (_defaultLoggerProvider is IDisposable provider)
            {
                _defaultLoggerProvider = null;
                provider.Dispose();
            }
        }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Register services for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ConfigureServices(IServiceCollection services)
    {
        InternalConfigureServices(services);
        OnConfigureServices(services);
    }

    /// <summary>
    /// Register services for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <remarks>This method should only be overridden by intermediate customization projects (Lagoon.Server.*)</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void InternalConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(GetType(), this);
        services.AddSingleton<ILgApplicationBase>(this);
        services.AddSingleton(ApplicationInformation);
        _builder?.ConfigureServices(services);
    }

    /// <summary>
    /// Register the services for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual void OnConfigureServices(IServiceCollection services) { }

    /// <summary>
    /// Configure the HTTP request pipeline for the application.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Configure(IServiceProvider serviceProvider)
    {
        // Call the method from the framework
        InternalConfigure(serviceProvider);
        // Configure the application with the registred services.
        OnConfigure(serviceProvider);
    }

    /// <summary>
    /// Configure the application with the registred services.
    /// </summary>
    /// <param name="serviceProvider">The registred services provider.</param>
    /// <remarks>This method should only be overridden by intermediate customization projects (Lagoon.Server.*)</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void InternalConfigure(IServiceProvider serviceProvider)
    {
        ServiceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        // Get a ILogger<Main> instance
        Logger = (ILogger)serviceProvider.GetRequiredService(typeof(ILogger<>).MakeGenericType(GetType()));
    }

    /// <summary>
    /// Release the references to the singletons.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void FreeSingletonReferences()
    {
        ServiceScopeFactory = null;
        Logger = null;
    }

    /// <summary>
    /// Configure the application with the registred services.
    /// </summary>
    /// <param name="serviceProvider">The registred services provider.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual void OnConfigure(IServiceProvider serviceProvider) { }

    /// <summary>
    /// Load the configuration settings and initialize the application information property.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    /// <param name="environmentName">The name of the environment in which the application is running.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void LoadApplicationInformation(IConfiguration configuration, string environmentName)
    {
        if (ApplicationInformation is not null)
        {
            throw new Exception("The application information are already loaded.");
        }
        // Define configuration property
        ArgumentNullException.ThrowIfNull(configuration);
        Configuration = configuration;
        // Ensure environment name is defined
        environmentName ??= Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
        // Update the configuration if needed
        LoadConfiguration(configuration, environmentName);
        _builder?.LoadConfiguration(configuration, environmentName);
        OnLoadConfiguration(configuration, environmentName);
        // Load the property
        ApplicationInformation = GetApplicationInformation(environmentName);
    }

    /// <summary>
    /// Method called when the <see cref="Configuration"/> property has been initialized.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="environmentName">The name of the environment in which the application is running.</param>
    /// <remarks>This method should only be overridden by intermediate customization projects (Lagoon.Server.*)</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void LoadConfiguration(IConfiguration configuration, string environmentName)
    {
        // Adds parameters that do not exist from another parameter file defined in "Lagoon:SharedAppSettings"
        SharedAppSettings.LoadTo(configuration, environmentName);
    }

    /// <summary>
    /// Method called when the <see cref="Configuration"/> property has been initialized.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="environmentName">The name of the environment in which the application is running.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual void OnLoadConfiguration(IConfiguration configuration, string environmentName) { }

    /// <summary>
    /// Gets the complete application information.
    /// </summary>
    /// <param name="environmentName">The name of the environment in which the application is running.</param>
    /// <returns>The complete application information.</returns>
    protected virtual IApplicationInformation GetApplicationInformation(string environmentName)
    {
        return new ApplicationInformation(this, Configuration, environmentName);
    }

    /// <summary>
    /// Configure Logger provider for the application.
    /// </summary>
    /// <param name="builder">The provide logging builder.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ConfigureLogging(ILoggingBuilder builder)
    {
        InternalConfigureLogging(builder);
        OnConfigureLogging(builder);
    }

    /// <summary>
    /// Configure Logger provider for the application.
    /// </summary>
    /// <param name="builder">The provided logging builder.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void InternalConfigureLogging(ILoggingBuilder builder)
    {
        // Initialise a default logger for the application
        ILoggerProvider provider = GetDefaultLoggerProvider();
        // Add a provider to the default logger
        if (provider is not null)
        {
            Logger = provider.CreateLogger(null);
            _defaultLoggerProvider = provider;
            // We encapsulate the provider in an LgApplicationDefaultLoggerProvider
            // to allow log after the application host is closed for critical errors
            builder.AddProvider(new LgApplicationDefaultLoggerProvider(provider));
        }
    }

    /// <summary>
    /// Get the default logger for the application (To use before the application's host starts).
    /// </summary>
    /// <returns>The default logger for the application (To use before the application's host starts).</returns>
    protected virtual ILoggerProvider GetDefaultLoggerProvider()
    {
        // Check if the configuration contains informations about logging to a file
        LgFileLoggerOptions options = GetFileLoggerOptions();
        // Initialize a logging to a file
        return options is not null && options.Enabled ? CreateFileLogger(options) : null;
    }

    /// <summary>
    /// The method called when a new file logger provider is needed.
    /// </summary>
    /// <param name="options">The file logger options.</param>
    /// <returns>The new provider.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual LgFileLoggerProvider CreateFileLogger(LgFileLoggerOptions options)
    {
        return new LgFileLoggerProvider(this, options);
    }

    /// <summary>
    /// Get the log file options.
    /// </summary>
    /// <returns>The log file options.</returns>
    protected virtual LgFileLoggerOptions GetFileLoggerOptions()
    {
        return TryGetFileLoggerOptions("Lagoon:FileLogger")
                                    ?? TryGetFileLoggerOptions("Lagoon:LagoonFileLogger");
    }

    /// <summary>
    /// Get the file logger options from a config section.
    /// </summary>
    /// <param name="sectionName">The section name.</param>
    /// <returns>The logger options.</returns>
    protected LgFileLoggerOptions TryGetFileLoggerOptions(string sectionName)
    {
        IConfigurationSection section = Configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            return null;
        }
        // Initialize default options
        LgFileLoggerOptions options = new();
        // Initialise properties from the configuration
        section.Bind(options);
        // Initialise the flush behavior
        if (options.AutoFlushLevel == LogLevel.None)
        {
            options.AutoFlushLevel = IsDebugEnabled ? LogLevel.Debug : LogLevel.Error;
        }
        // Initialize folder
        if (string.IsNullOrEmpty(options.FolderPath))
        {
            // We use the temp path if not defined
            options.FolderPath = Path.GetTempPath();
        }
        else
        {
            // Replace environment variables names
            options.FolderPath = Environment.ExpandEnvironmentVariables(options.FolderPath);
        }
        // Ensure filename is defined
        if (string.IsNullOrEmpty(options.LogFilename))
        {
            options.LogFilename = $"{ApplicationInformation.RootName}.log";
        }
        else if (string.IsNullOrEmpty(Path.GetExtension(options.LogFilename)))
        {
            options.LogFilename = $"{options.LogFilename}.log";
        }
        return options;
    }

    /// <summary>
    /// Configure Logger provider for the application.
    /// </summary>
    /// <param name="loggingBuilder">The provide logging builder.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual void OnConfigureLogging(ILoggingBuilder loggingBuilder) { }

    #endregion

    #region Embedded resources management    

    /// <summary>
    /// Gets the application assembly name. Example : 'AppRootName.Server'.
    /// </summary>
    /// <returns>The application assembly name. Example : 'AppRootName.Server'.</returns>
    internal string GetAssemblyName()
    {
        return ApplicationAssembly.GetName().Name;
    }

    /// <summary>
    /// Extract the version number from the "Client" application DLL if found; else the version of the current application.
    /// </summary>
    /// <returns>The full version.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual internal string InitializeFullVersion()
    {
        string appLocation = ApplicationAssembly.Location;
        string clientLocation = Path.Combine(Path.GetDirectoryName(appLocation), $"{ApplicationRootName}.Client.dll");
        return FileVersionInfo.GetVersionInfo(File.Exists(clientLocation) ? clientLocation : appLocation).ProductVersion;
    }

    #endregion

    #region Multilanguage management

    /// <summary>
    /// Gets the default culture to use when application starts.
    /// </summary>
    /// <returns>The default culture to use when application starts.</returns>
    /// <remarks>Method called by <see cref="GetDefaultCulture"/>.</remarks>
    public virtual string GetDefaultLanguage()
    {
        return "en";
    }

    /// <summary>
    /// Gets the default culture to use when application starts.
    /// </summary>
    /// <returns>The default culture to use when application starts.</returns>
    public virtual string GetDefaultCulture()
    {
        string culture = _appDico.TryGetCulture(GetDefaultLanguage());
        if (string.IsNullOrEmpty(culture))
        {
            return _appDico.GetDefaultCulture();
        }
        return culture;
    }

    /// <summary>
    /// Add additionnal dictionnary or overload existing key at runtime. 
    /// </summary>
    /// <param name="additionnalDico"></param>
    public virtual void LoadLanguages(Dictionary<string, Dictionary<string, string>> additionnalDico)
    {
        _appDico?.LoadAdditionnalDico(additionnalDico);
    }

    /// <summary>
    /// Public accessor to the dictionnary
    /// </summary>
    /// <param name="key">Dico key for which we want the translation</param>
    /// <param name="args">Optionnal argument used by String.Format() on a dico value</param>
    /// <returns>The corresponding value if the key is found (or the key if not found)</returns>
    public string Dico(string key, params object[] args)
    {
        return DicoFromLanguage(CultureInfo.CurrentCulture.TwoLetterISOLanguageName, key, args);
    }

    /// <summary>
    /// Public accessor to the dictionnary
    /// </summary>
    /// <param name="language">The target language to use</param>
    /// <param name="key">Dico key for which we want the translation</param>
    /// <param name="args">Optionnal argument used by String.Format() on a dico value</param>
    /// <returns>The corresponding value if the key is found (or the key if not found)</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string DicoFromLanguage(string language, string key, params object[] args)
    {   
        // Use the current language if not defined
        language ??= CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        // If the dico manager is not initialized, return key
        if (string.IsNullOrEmpty(key) || _appDico is null)
        {
            return key;
        }
        // Try to find the translation of 'key' for the specified language
        string dicoVal = _appDico.GetTranslation(language, key);
        return args != null && args.Length > 0 ? string.Format(dicoVal, args) : dicoVal;
    }

    /// <summary>
    /// Return the list of language (fr, en, ...) supported by the application.
    /// </summary>
    public IEnumerable<string> GetSupportedLanguages()
    {
        return _appDico.GetLanguages();
    }

    /// <summary>
    /// Return the list of culture (fr-FR, en-US, ...) supported by the application.
    /// </summary>
    public IEnumerable<string> GetSupportedCultures()
    {
        return _appDico.GetCultures();
    }

    /// <summary>
    /// Return the list of string's key of the application multi-langual dictionnary.
    /// </summary>
    public IEnumerable<string> GetDicoKeys()
    {
        return _appDico.GetAllKeys();
    }

    /// <summary>
    /// Export all dictionnary keys into a string formatted as CSV
    /// </summary>
    /// <returns></returns>
    public string DicoToCsv(string side, bool exportHeader = true, IEnumerable<string> knownKeys = null, bool importKnownKeys = true)
    {
        StringBuilder stb = new();
        IEnumerable<string> dicoCultures = GetSupportedLanguages();
        IEnumerable<string> dicoKeys = GetDicoKeys();
        if (exportHeader)
        {
            // Header row
            stb.Append("\"side\";\"key\"");
            foreach (string culture in dicoCultures)
            {
                stb.Append($";\"{culture}\"");
            }
            stb.AppendLine();
        }
        // Dico data
        foreach (string key in dicoKeys)
        {
            if (importKnownKeys || (knownKeys != null && !knownKeys.Contains(key)))
            {
                stb.Append($"\"{side}\";\"{key}\"");
                foreach (string culture in dicoCultures)
                {
                    stb.Append($";{CsvDocument<object>.EscapeString(_appDico.GetTranslation(culture, key)).Replace("\n", "\\n")}");
                }
                stb.AppendLine();
            }
        }
        return stb.ToString();
    }

    /// <summary>
    /// Set the current application culture.
    /// </summary>
    /// <param name="culture">The new culture</param>
    public virtual void SetCulture(CultureInfo culture)
    {
        culture ??= new CultureInfo(GetDefaultCulture());
        OnInitCulture(culture);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    /// <summary>
    /// Set the current application culture.
    /// </summary>
    /// <param name="cultureName">The name of the culture (eg: "fr-FR"). </param>
    public void SetLanguage(string cultureName)
    {
        CultureInfo culture = new(cultureName);
        SetCulture(culture);
    }

    /// <summary>
    /// Set culture formats
    /// </summary>
    /// <param name="culture">culture</param>
    protected virtual void OnInitCulture(CultureInfo culture)
    {
        culture.NumberFormat.NumberDecimalSeparator = ".";
        culture.NumberFormat.NumberGroupSeparator = " ";
    }

#endregion

#region Trace management

    /// <summary>
    /// Get the Lagoon's version.
    /// </summary>
    /// <returns>The Lagoon's version.</returns>
    protected static string GetLagoonVersion()
    {
        return typeof(LgApplicationBase).Assembly.GetProductVersion();
    }

    /// <summary>
    /// Unhandled errors from the ThreadPool. Catch errors from async void without try...catch.
    /// </summary>
    private void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
    {
        if (e?.ExceptionObject is not Exception ex)
        {
            ex = new Exception(e?.ExceptionObject?.ToString() ?? "UNKNOWN ERROR !");
        }
        TraceUnhandledException(ex);
    }

    /// <summary>
    /// Trace the exceptions that have not been caught up.
    /// </summary>
    /// <param name="ex">The exception.</param>
    protected virtual void TraceUnhandledException(Exception ex)
    {
        Exception uex = new("UNHANDLED EXCEPTION", ex);
        if (IsDebugEnabled)
        {
            TraceCriticalException(uex);
        }
        else
        {
            TraceException(uex);
        }
    }

    /// <summary>
    /// Get a generic message exception if we aren't in debug mode and it's not an exception for the end user.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <param name="includeOriginalMessage">Indicate if the original message must be included in DEBUG.</param>
    /// <returns>The generic message.</returns>
    public string GetContactAdminMessage(Exception ex, bool includeOriginalMessage)
    {
        if (ex is UserException)
        {
            return ex.Message;
        }
        else if (includeOriginalMessage && IsDebugEnabled)
        {
            return $"{GetContactAdminMessage()}\n---+ {ex.GetType().FullName}\n{ex.Message} +---";
        }
        else
        {
            return GetContactAdminMessage();
        }
    }

    /// <summary>
    /// Get the message to show to contact the administrator.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetContactAdminMessage()
    {
        return "ContactAdminException".Translate();
    }

    /// <summary>
    /// Formats and writes a debug log message for the application.
    /// </summary>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <remarks>Surround the TraceDebug with a #if DEBUG...#endif</remarks>
    public void TraceDebug(string message, params object[] args)
    {
        if (IsDebugEnabled)
        {
            Trace(LogLevel.Debug, null, message, args);
        }
    }

    /// <summary>
    /// Formats and writes an informational log message for the application.
    /// </summary>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void TraceInfo(string message, params object[] args)
    {
        TraceInformation(message, args);
    }

    /// <summary>
    /// Formats and writes an informational log message for the application.
    /// </summary>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public void TraceInformation(string message, params object[] args)
    {
        Trace(LogLevel.Information, null, message, args);
    }

    /// <summary>
    /// Formats and writes a warning log message for the application.
    /// </summary>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public void TraceWarning(string message, params object[] args)
    {
        Trace(LogLevel.Warning, null, message, args);
    }

    /// <summary>
    /// Formats and writes an error log message for the application.
    /// </summary>
    /// <param name="ex">The exception to trace.</param>
    public void TraceException(Exception ex)
    {
        if (ex is not UserException)
        {
            Trace(LogLevel.Error, ex, ex.Message, null);
        }
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    protected virtual void Trace(LogLevel logLevel, Exception exception, string message, params object[] args)
    {
#pragma warning disable CA2254 // (message) Template should be a static expression.
        Logger?.Log(logLevel, 0, exception, message, args);
#pragma warning restore CA2254 // (message) Template should be a static expression.
    }

    /// <summary>
    /// Trace a critical exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void TraceCriticalException(Exception exception)
    {
        // Trace to default logger
        if (_defaultLoggerProvider is not null)
        {
            ILogger logger = _defaultLoggerProvider.CreateLogger(null);
            if (logger.IsEnabled(LogLevel.Critical))
            {
#pragma warning disable CA2254 // (message) Template should be a static expression.
                logger.LogCritical(exception, exception.Message);
#pragma warning restore CA2254 // (message) Template should be a static expression.
            }
        }
    }

    /// <summary>
    /// Get the description to write in the event viewer for a critical error.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <returns>The description to write in the event viewer for a critical error.</returns>
    protected virtual string GetEventViewerDescription(Exception ex)
    {
        string detail = ex.ToString();
        StringBuilder sb = new(detail.Length + 100);
        sb.Append(AppDomain.CurrentDomain.FriendlyName);
        sb.Append(" version: ");
        sb.AppendLine(ApplicationAssembly.GetProductVersion());
        sb.Append("LAGOON version: ");
        sb.AppendLine(GetLagoonVersion());
        sb.Append("CoreCLR version: ");
        sb.Append(Environment.Version.ToString());
        sb.Append(" (");
        sb.Append(System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant());
        sb.AppendLine(")");
        sb.Append(".NET version: ");
        sb.AppendLine(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
        sb.Append("OS: ");
        sb.AppendLine(System.Runtime.InteropServices.RuntimeInformation.OSDescription);
        sb.AppendLine("Exception:");
        sb.Append(detail);
        return sb.ToString();
    }

#endregion


}
