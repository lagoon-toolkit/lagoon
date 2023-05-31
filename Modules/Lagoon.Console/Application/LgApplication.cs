using Lagoon.Console.Commands;
using Lagoon.Core.Application;
using Lagoon.Core.Application.Logging;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace Lagoon.Console.Application;

/// <summary>
/// Console application.
/// </summary>
[HelpOption]
[VersionOptionFromMember(MemberName = nameof(GetVersion))]
[Subcommand(typeof(StatusCommand))]
public class LgApplication : LgApplicationBase, ILgApplication
{

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
    protected LgApplication(ILgApplicationBuilder builder) : base(builder)
    { }

    /// <summary>
    /// Extract the application root name.
    /// </summary>
    /// <returns>The application root name.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override string InitializeApplicationRootName()
    {
        // Get the application root name from the "ProductName", because the assembly name is lower case for console applications.
        return FileVersionInfo.GetVersionInfo(ApplicationAssembly.Location).ProductName.Split('.')[0];
    }

    #endregion

    #region initialization

    /// <summary>
    /// Configure the application arguments handler.
    /// </summary>
    /// <param name="commandLineApplication">The application arguments handler.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal virtual void ConfigureCommandLineApplication(CommandLineApplication commandLineApplication)
    {
        commandLineApplication.Conventions.AddConvention(new SpecialOptionsConvention());
        OnConfigureCommandLineApplication(commandLineApplication);
    }

    /// <summary>
    /// Configure the application arguments handler.
    /// </summary>
    /// <param name="commandLineApplication">The application arguments handler.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual void OnConfigureCommandLineApplication(CommandLineApplication commandLineApplication)
    { }

    ///<inheritdoc/>
    protected override void InternalConfigureLogging(ILoggingBuilder builder)
    {
        // Clear all previously registered providers
        builder.ClearProviders();
        // Activate the log for "batch" command or command with "--log" option
        bool batchCommand = false;
        bool noNoLogOption = false;
        bool logOption = false;
        foreach (string arg in Environment.GetCommandLineArgs())
        {
            switch (arg.ToLowerInvariant())
            {
                case SpecialOptionsConvention.BATCH_COMMAND:
                    batchCommand = true;
                    break;
                case SpecialOptionsConvention.LOG_OPTION:
                    logOption = true;
                    break;
                case SpecialOptionsConvention.NOLOG_OPTION:
                    noNoLogOption = true;
                    break;
            }
        }
        if (logOption == noNoLogOption)
        {
            logOption = batchCommand;
        }
        // Register the Lagoon file logger
        if (logOption)
        {
            base.InternalConfigureLogging(builder);
        }
    }

    /// <summary>
    /// Configure the logger.
    /// </summary>
    /// <param name="_">Host builder context.</param>
    /// <param name="logging">The logging builder.</param>
    internal void BuildLogging(HostBuilderContext _, ILoggingBuilder logging)
    {
        ConfigureLogging(logging);
    }

    ///<inheritdoc/>
    protected override LgFileLoggerOptions GetFileLoggerOptions()
    {
        // Try to get settings defined for the batch
        LgFileLoggerOptions options = TryGetFileLoggerOptions("Batch:FileLogger");
        if (options is null)
        {
            // We get the settings from the "Server" application, we just rename
            options = base.GetFileLoggerOptions();
            options.LogFilename = Path.GetFileNameWithoutExtension(options.LogFilename) + ".exe"
                + Path.GetExtension(options.LogFilename);
        }
        return options;
    }

    /// <summary>
    /// Configure dependency injection services.
    /// </summary>
    /// <param name="context">The Hast builder context.</param>
    /// <param name="services">The service collection.</param>
    internal void BuildHost(HostBuilderContext context, IServiceCollection services)
    {
        // Set the configuration and load the application information
        LoadApplicationInformation(context.Configuration, context.HostingEnvironment.EnvironmentName);
        // Register services
        services.AddSingleton<ILgApplication>(this);
        ConfigureServices(services);
    }

    /// <summary>
    /// Configure the built host.
    /// </summary>
    /// <param name="host">The built host.</param>
    internal void ConfigureHost(IHost host)
    {
        Configure(host.Services);
        OnConfigureHost(host);
    }

    /// <summary>
    /// Configure the application.
    /// </summary>
    /// <param name="host">The application host.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual void OnConfigureHost(IHost host) { }

    #endregion

    #region methods

    /// <summary>
    /// Method to call if there's no parameter.
    /// </summary>
    /// <remarks>When overriding method, use the "ServiceScopeFactory" property to access to a service.</remarks>
    public virtual Task<int> OnExecuteAsync()
    {
        // Show the hint message
        ConsoleEx.WriteLine($"Specify -? or --help for a list of available options and commands.");
        return Task.FromResult(0);
    }

    /// <summary>
    /// Show the status of the application's resources.
    /// </summary>
    /// <remarks>When overriding method, use the "ServiceScopeFactory" property to access to a service.</remarks>
    public virtual int ShowStatus()
    {
        ShowVersion();
        return 0;
    }

    /// <summary>
    /// Show the current application version.
    /// </summary>
    public void ShowVersion()
    {
        ConsoleEx.WriteLine(GetVersion());
    }

    /// <summary>
    /// Get the application and the version.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetVersion()
    {
        return $"{ApplicationInformation.Name} version {ApplicationInformation.Version}";
    }

    ///<inheritdoc/>
    public override void TraceCriticalException(Exception exception)
    {
        // Trace to default logger
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

    #region console method

    ///// <summary>
    ///// Formats and writes a debug log message for the application.
    ///// </summary>
    ///// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
    ///// <param name="args">An object array that contains zero or more objects to format.</param>
    //public void ShowSuccess(string message, params object[] args)
    //{
    //    TraceInfo(message, args);
    //}

    ///// <summary>
    ///// Formats and writes an informational log message for the application.
    ///// </summary>
    ///// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
    ///// <param name="args">An object array that contains zero or more objects to format.</param>
    //public void ShowInformation(string message, params object[] args)
    //{
    //    TraceInfo(message, args);
    //}

    ///// <summary>
    ///// Formats and writes a warning log message for the application.
    ///// </summary>
    ///// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
    ///// <param name="args">An object array that contains zero or more objects to format.</param>
    //public void ShowWarning(string message, params object[] args)
    //{
    //    TraceWarning(message, args);
    //}

    /// <summary>
    /// Formats and writes an error log message for the application.
    /// </summary>
    /// <param name="ex">The exception to trace.</param>
    public void ShowException(Exception ex)
    {
        ConsoleEx.WriteError($"Error: {ex.Message}");
        TraceException(ex);
    }

    #endregion

}
