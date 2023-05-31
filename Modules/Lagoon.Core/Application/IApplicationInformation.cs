namespace Lagoon.Core;

/// <summary>
/// Information about the current application.
/// </summary>
public interface IApplicationInformation
{
    #region properties

    /// <summary>
    /// Application name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Environment context of the current application. (Ex: Production, Validation, Development, ...)
    /// <see href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments" />
    /// </summary>
    string EnvironmentName { get; }

    /// <summary>
    /// The "EnvironmentDisplayName" value, from "app" section, defined in the "appsettings.{env}.json".
    /// </summary>
    string EnvironmentDisplayName { get; }

    /// <summary>
    /// The "EnvironmentColor" color value, from "app" section, defined in the "appsettings.{env}.json".
    /// </summary>
    string EnvironmentColor { get; }

    /// <summary>
    /// Indicate if the application was built with DEBUG configuration.
    /// </summary>
    bool IsDebug { get; }

    /// <summary>
    /// Indicate if the current host environment name is "Development".
    /// </summary>
    bool IsDevelopment { get; }

    /// <summary>
    /// The public URL of the application (to be used in an e-mail message, for example...).
    /// </summary>
    /// <remarks>The value is defined in "Lagoon:PublicUrl" from the "appSettings.json".</remarks>
    public string PublicURL { get; }

    /// <summary>
    /// The application root name (based on the assembly name). Example : 'AppRootName.[Client|Shared|Server|...]'.
    /// </summary>
    string RootName { get; }

    /// <summary>
    /// The email address of the administrator.
    /// </summary>
    /// <remarks>The value is defined in "App:SupportEmail" from the "appSettings.json".</remarks>
    string SupportEmail {get; }

    /// <summary>
    /// Version of the current application.
    /// </summary>
    Version Version { get; }

    #endregion
    
}
