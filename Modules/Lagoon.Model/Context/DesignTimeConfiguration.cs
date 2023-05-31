namespace Lagoon.Model.Context;


/// <summary>
/// The EF Core tools configuration manager.
/// </summary>
public static class DesignTimeConfiguration
{

    #region methods

    /// <summary>
    /// Get the configuration from "app.Settings", "appsettings.Development.json".
    /// </summary>
    /// <returns>The loaded configuration.</returns>
    public static IConfiguration GetDevelopmentConfiguration()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        // Load the appSettings files
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(currentDirectory)
            .AddJsonFile($"{currentDirectory}/../Server/appsettings.json")
            .AddJsonFile($"{currentDirectory}/../Server/appsettings.Development.json", true)
            .Build();
        // Return the loaded configuration
        return configuration;
    }

    #endregion

}