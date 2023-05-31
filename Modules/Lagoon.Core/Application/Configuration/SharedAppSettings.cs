namespace Lagoon.Core.Application.Configuration;

internal static class SharedAppSettings
{
    /// <summary>
    /// Adds parameters that do not exist from another parameter file defined in "Lagoon:SharedAppSettings".
    /// </summary>
    /// <param name="configuration">The current configuration.</param>
    /// <param name="environmentName">The name of the environment in which the application is running.</param>
    internal static void LoadTo(IConfiguration configuration, string environmentName)
    {
        // Check if the file exists
        string filename = configuration["Lagoon:SharedAppSettings"];
        if (string.IsNullOrEmpty(filename))
        {
            return;
        }
        // Complete relative file path
        filename = Path.Combine(Directory.GetCurrentDirectory(), filename);
        if (!File.Exists(filename))
        {
            return;
        }
        // Load the appSettings files
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(filename))
            .AddJsonFile(filename);
        if (!string.IsNullOrEmpty(environmentName))
        {
            string extension = Path.GetExtension(filename);
            builder.AddJsonFile($"{filename[..^extension.Length]}.{environmentName}{extension}", true);
        }
        var sharedSettings = builder.Build();
        foreach (var entry in sharedSettings.AsEnumerable())
        {
            if (entry.Value is not null && string.IsNullOrEmpty(configuration[entry.Key]))
            {
                configuration[entry.Key] = entry.Value;
            }
        }
    }
}
