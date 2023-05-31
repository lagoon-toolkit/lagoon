namespace Lagoon.Core.Application;


/// <summary>
/// Interface used to configure applications.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface ILgApplicationBuilder
{

    /// <summary>
    /// Load the configuration settings and initialize the application information property.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="environmentName">The name of the environment in which the application is running.</param>
    void LoadConfiguration(IConfiguration configuration, string environmentName);

    /// <summary>
    /// Register services for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    void ConfigureServices(IServiceCollection services);

}
