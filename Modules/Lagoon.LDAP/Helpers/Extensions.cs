using Lagoon.Services;

namespace Lagoon.Helpers;


/// <summary>
/// Extension method to add LDAP services
/// </summary>
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public static class Extensions
{

    #region IServiceCollection extensions

    /// <summary>
    /// Add Active Directory service
    /// </summary>
    /// <remarks>
    /// The activeDirectoryName key in ConnectionStrings section must be defined in appsettings.json
    /// </remarks>
    /// <param name="services">Extension method for IServiceCollection</param>
    /// <returns>IServiceCollection for chaining</returns>
    public static IServiceCollection AddLdap(this IServiceCollection services, IConfiguration config)
    {
        string activeDirectoryName = config.GetConnectionString("ActiveDirectoryName");
        if (string.IsNullOrWhiteSpace(activeDirectoryName)) throw new InvalidOperationException("The key 'activeDirectoryName' must be set in appsettings.json under 'ConnectionStrings' section. ");
        return services.AddSingleton(service => new ActiveDirectoryService(activeDirectoryName, service.GetService<ILogger<ActiveDirectoryService>>()));
    }

    #endregion

}
