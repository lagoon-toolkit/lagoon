using Microsoft.EntityFrameworkCore.Design;

namespace TemplateLagoonWeb.Model.Context;

/// <summary>
/// The design time DbContext for EF Core Tools commands.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Create a new database context.
    /// </summary>
    /// <param name="args">Additional argument to the command.
    /// example : FirstAppArg "This is all the second application argument" ThirdAppArg
    /// </param>
    /// <returns>The new database context.</returns>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Get the configuration from "app.Settings", "appsettings.Development.json" of "Server" project
        IConfiguration configuration = DesignTimeConfiguration.GetDevelopmentConfiguration();
        // Configure and instanciate de the DbContext
        DbContextOptionsBuilder<ApplicationDbContext> builder = new();
        ApplicationDbContext.Configure(builder, configuration, true);
        return new ApplicationDbContext(builder.Options);
    }

}
