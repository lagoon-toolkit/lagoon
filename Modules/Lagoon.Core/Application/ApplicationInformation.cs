using System.Text.RegularExpressions;

namespace Lagoon.Core.Application;

/// <summary>
/// Information about the current application.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class ApplicationInformation : IApplicationInformation
{

    #region properties

    /// <summary>
    /// Application name.
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Environment context of the current application. (Ex: Production, Validation, Development, ...)
    /// <see href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments" />
    /// </summary>
    public string EnvironmentName { get; protected set; }

    /// <summary>
    /// The "EnvironmentDisplayName" value, from "app" section, defined in the "appsettings.{env}.json".
    /// </summary>
    public string EnvironmentDisplayName { get; protected set; }

    /// <summary>
    /// The "EnvironmentColor" color value, from "app" section, defined in the "appsettings.{env}.json".
    /// </summary>
    public string EnvironmentColor { get; protected set; }

    /// <summary>
    /// Version of the current application. (eg: 1.2.3-beta4)
    /// </summary>
    public string FullVersion { get; protected set; }

    /// <summary>
    /// Indicate if the application was built with DEBUG configuration.
    /// </summary>
    public bool IsDebug { get; protected set; }

    /// <summary>
    /// Checks if the current host environment name is "Development".
    /// </summary>
    public bool IsDevelopment { get; protected set; }

    /// <summary>
    /// The public URL of the application (to be used in an e-mail message, for example...).
    /// e.g. https://my-application.com/ or https://my-server.com/my-application/.
    /// The value is defined in "Lagoon:PublicUrl" from the "appSettings.json"
    /// </summary>
    /// <remarks>The value always ends with a slash but can be null.</remarks>
    public string PublicURL { get; protected set; }

    /// <summary>
    /// The application root name (based on the assembly name). Example : 'AppRootName.[Client|Shared|Server|...]'.
    /// </summary>
    public string RootName { get; protected set; }

    /// <summary>
    /// The email address of the administrator.
    /// </summary>
    /// <remarks>The value is defined in "App:SupportEmail" from the "appSettings.json".</remarks>
    public string SupportEmail { get; protected set; }

    /// <summary>
    /// Version of the current application with 3-number format (Major.Minor.Build).
    /// </summary>
    public Version Version { get; protected set; }

    #endregion

    #region constructor

    /// <summary>
    /// Load application informations.
    /// </summary>
    /// <param name="app">Application manager.</param>
    /// <param name="configuration">Configuration.</param>
    /// <param name="environmentName">The name of the environment in which the application is running.</param>
    public ApplicationInformation(LgApplicationBase app, IConfiguration configuration, string environmentName)
    {
        RootName = app.ApplicationRootName;
        EnvironmentName = environmentName;
        IsDebug = app.IsDebugEnabled;
        IsDevelopment = "Development".Equals(EnvironmentName, StringComparison.OrdinalIgnoreCase);
        FullVersion = app.InitializeFullVersion();
        Version = ExtractVersion(FullVersion);
        if (configuration is not null)
        {
            Name = configuration["App:Name"] ?? RootName;
            EnvironmentDisplayName = configuration["App:EnvironmentDisplayName"] ?? configuration["App:EnvironmentName"];
            EnvironmentColor = configuration["App:EnvironmentColor"];
            SupportEmail = configuration["App:SupportEmail"];
            PublicURL = configuration["Lagoon:PublicUrl"];
            if (string.IsNullOrEmpty(PublicURL))
            {
                PublicURL = DetectPublicUrl();
            }
            if (!string.IsNullOrEmpty(PublicURL) && !PublicURL.EndsWith('/'))
            {
                PublicURL += '/';
            }
        }
        if (string.IsNullOrEmpty(PublicURL))
        {
            throw new Exception("The PublicURL is undefined, add the complete application URL in \"Lagoon:PublicUrl\" key of the \"appsettings.json\" or in the \"ASPNETCORE_URLS\" environment variable.");
        }
    }

    /// <summary>
    /// Extract a 3-number version (Major.Minor.Build) from the full version.
    /// </summary>
    /// <param name="fullVersion">The full version (eg: "1.25.3.4-alpha6")</param>
    /// <returns>The 3-number version.</returns>
    private static Version ExtractVersion(string fullVersion)
    {
        static int ne(string s) => string.IsNullOrEmpty(s) ? 0 : int.Parse(s);
        Match m = Regex.Match(fullVersion, "(\\d*)\\.?(\\d*)\\.?(\\d*)");
        return new Version(ne(m.Groups[1].Value), ne(m.Groups[2].Value), ne(m.Groups[3].Value));
    }

    /// <summary>
    /// Detect the public url if not defined in the configuration.
    /// </summary>
    /// <returns>The public URL.</returns>
    protected virtual string DetectPublicUrl()
    {
        //See: https://andrewlock.net/5-ways-to-set-the-urls-for-an-aspnetcore-app/
        string urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
            ?? Environment.GetEnvironmentVariable("DOTNET_URLS");
        if (!string.IsNullOrEmpty(urls))
        {
            foreach (string url in urls.Split(';'))
            {
                if (url.StartsWith("https://"))
                {
                    return url;
                }
            }
        }
        return null;
    }

    #endregion

}
