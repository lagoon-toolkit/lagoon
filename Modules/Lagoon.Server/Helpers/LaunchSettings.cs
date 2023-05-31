using System.Text.RegularExpressions;

namespace Lagoon.Server.Helpers;

internal static class LaunchSettings
{
    #region constants

    private const string LAUNCH_SETTING_PATH = "Properties/launchSettings.json";

    #endregion

    #region methods

    /// <summary>
    /// Return the hosts addresses defined in the launchSettings.json file.
    /// </summary>
    /// <param name="secure">if <c>true</c> we use HTTPS else we use HTTP.</param>
    /// <returns>The hosts addresses defined in the launchSettings.json file.</returns>
    internal static IEnumerable<string> GetHttpHosts(bool secure)
    {
        List<string> hosts = new();
        if (File.Exists(LAUNCH_SETTING_PATH))
        {
            string content = File.ReadAllText(LAUNCH_SETTING_PATH);
            // Extract all applicationUrl from launchSettings.json file
            foreach (Match match in Regex.Matches(content, @"""applicationUrl"":\s*""([^""]*)""", RegexOptions.IgnoreCase).Cast<Match>())
            {
                foreach (string host in match.Groups[1].Value.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (host.StartsWith($"http{(secure ? 's' : "")}://") && !hosts.Contains(host))
                    {
                        hosts.Add(host);
                    }
                }
            }
            // Compute IISExpress url
            string iisHost = GetIISHttpAddress(content, secure)?.TrimEnd('/');
            if (iisHost is not null && !hosts.Contains(iisHost))
            {
                hosts.Add(iisHost);
            }
        }
        if (hosts.Count == 0)
        {
            // Add kestrel default URL
            hosts.Add(secure ? "https://localhost:5001" : "http://localhost:5000");
        }
        return hosts;
    }

    /// <summary>
    /// Get the HTTPS address defined for IIS in the launchSettings file.
    /// </summary>
    /// <param name="secure">if <c>true</c> we use HTTPS else we use HTTP.</param>
    /// <returns>The HTTPS address defined for IIS in the launchSettings file.</returns>
    public static string GetIISHttpAddress(bool secure)
    {
        return File.Exists(LAUNCH_SETTING_PATH) ? GetIISHttpAddress(File.ReadAllText(LAUNCH_SETTING_PATH), secure) : null;

    }

    /// <summary>
    /// Get the HTTPS address defined for IIS in the launchSettings file.
    /// </summary>
    /// <param name="content">The content of the LauchSettings file.</param>
    /// <param name="secure">if <c>true</c> we use HTTPS else we use HTTP.</param>
    /// <returns>The HTTPS address defined for IIS in the launchSettings file.</returns>
    private static string GetIISHttpAddress(string content, bool secure)
    {
        foreach (Match match in Regex.Matches(content, @"""iisExpress""[^}]*""applicationUrl"":\s*""([^""]*)""[^}]*""sslPort"":\s*(\d*)", RegexOptions.IgnoreCase).Cast<Match>())
        {
            if (secure)
            {
                UriBuilder uri = new(match.Groups[1].Value)
                {
                    Port = int.Parse(match.Groups[2].Value),
                    Scheme = "https"
                };
                return uri.ToString().TrimEnd('/');
            }
            else
            {
                return match.Groups[1].Value;
            }
        }
        return null;
    }

    #endregion

}
