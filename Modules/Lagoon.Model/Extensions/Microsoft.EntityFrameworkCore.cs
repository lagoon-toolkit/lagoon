using Lagoon.Model.Collation;
using Lagoon.Model.Context;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Extension for EntityFrameworkCore
/// </summary>
public static class LagoonExtensions
{

    /// <summary>
    /// Ensure that the date stored in the database is in UTC format and that the date read from the database is in "local" format (using the time zone of the web server).
    /// </summary>
    /// <param name="configurationBuilder">Setting defaults and configuring conventions before they run.</param>
    public static void DateSaveAsUtcLoadAsLocal(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcValueConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<UtcValueConverter>();
    }

    /// <summary>
    /// Return RDMS provider name
    /// </summary>        
    /// <param name="optionsBuilder"></param>
    /// <returns></returns>
    public static string GetProviderName(this DbContextOptionsBuilder optionsBuilder)
    {
        var extensionDb = optionsBuilder.Options.Extensions.Where(e => e.Info.IsDatabaseProvider).FirstOrDefault();
        string extensionName = extensionDb?.ToString();
        if(extensionName == null)
        {
            return null;
        }
        if (extensionName.Contains(CollationProvider.Oracle))
        {
            return CollationProvider.Oracle;
        }
        if (extensionName.Contains(CollationProvider.PostgreSQL))
        {
            return CollationProvider.PostgreSQL;
        }
        if (extensionName.Contains(CollationProvider.SQLServer))
        {
            return CollationProvider.SQLServer;
        }
        if (extensionName.Contains(CollationProvider.MySQL))
        {
            // Identify MariaDB RDMS
            var serverInfoProperty = extensionDb.GetType().GetProperty("ServerVersion");
            if (serverInfoProperty is not null && serverInfoProperty.GetValue(extensionDb).ToString().Contains("mariadb"))
            {
                return CollationProvider.MariaDB;
            }
            else
            {
                return CollationProvider.MySQL;
            }
        }
        return null;
    }
    
}
