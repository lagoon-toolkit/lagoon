using Lagoon.Console.Application;
using Microsoft.EntityFrameworkCore;

namespace TemplateLagoonWeb.Batch;


/// <summary>
/// The console application's configuration and event manager.
/// </summary>
[Subcommand(typeof(UpdateDbCommand))]
//    [Subcommand(typeof(BatchCommand))]  <-- Uncomment to activate the "batch" command
public class Main : LgApplication<ApplicationDbContext>
{

    #region Load shared configuration

    /// <summary>
    /// New instance handeling the Pierre-Fabre shared configuration.
    /// </summary>
    public Main() : base() { }

    #endregion

    #region Main Database and services registration

    /// <summary>
    /// Called when a main application DbContext need to be configured.
    /// </summary>
    /// <param name="builder">A builder used to create or modify options for the main database context.</param>
    protected override void OnConfigureDbContext(DbContextOptionsBuilder builder)
    {
        ApplicationDbContext.Configure(builder, Configuration, ApplicationInformation.IsDevelopment);
    }

    /// <summary>
    /// Register the services for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected override void OnConfigureServices(IServiceCollection services)
    {
        // Register services defined in this application
        Services.Registry.RegisterServices(services);
    }

    #endregion

    #region Show informations

    /// <summary>
    /// Show the status of the application's resources.
    /// </summary>
    /// <remarks>When overriding method, use the "ServiceScopeFactory" property to access to a service.</remarks>
    public override int ShowStatus()
    {
        // Show the default application status (Version, database connection state, migrations, ...)
        int errorCode = base.ShowStatus();
        // Add, if necessary, your custom status here
        // ...           
        // Return the status error code (0 if everything is ok)
        return errorCode;
    }

    #endregion

}
