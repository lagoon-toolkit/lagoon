using Lagoon.Core.Application;
using Lagoon.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace Lagoon.Console.Application;

/// <summary>
/// Console application with a main database.
/// </summary>
public class LgApplication<TDbContext> : LgApplication
    where TDbContext : DbContext
{

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public LgApplication() { }

    /// <summary>
    /// New instance with an additionnal configurator.
    /// </summary>
    /// <param name="builder"></param>
    protected LgApplication(ILgApplicationBuilder builder) : base(builder) { }

    #endregion

    #region register database service

    /// <summary>
    /// Register services for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    protected override void InternalConfigureServices(IServiceCollection services)
    {
        // Registration of the application database
        services.AddDbContext<TDbContext>(ConfigureDbContext, ServiceLifetime.Singleton, ServiceLifetime.Singleton);
        services.AddSingleton(sp => (ILgApplicationDbContext)sp.GetService<TDbContext>());
        // Register other services
        base.InternalConfigureServices(services);
    }

    /// <summary>
    /// Configure the <see cref="DbContextOptions" /> for the context.
    /// This provides an alternative to performing configuration of the context
    /// by overriding the<see cref="DbContext.OnConfiguring" /> method in your derived context.
    /// </summary>
    /// <param name="builder">Configuring <see cref="DbContextOptions" />. Databases</param>
    /// <remarks>This method should only be overridden by intermediate customization projects (Lagoon.Server.*)</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal virtual void ConfigureDbContext(DbContextOptionsBuilder builder)
    {
        OnConfigureDbContext(builder);
    }

    /// <summary>
    /// Configure the <see cref="DbContextOptions" /> for the context.
    /// This provides an alternative to performing configuration of the context
    /// by overriding the<see cref="DbContext.OnConfiguring" /> method in your derived context.
    /// </summary>
    /// <param name="builder">Configuring <see cref="DbContextOptions" />. Databases</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual void OnConfigureDbContext(DbContextOptionsBuilder builder) { }

    #endregion

    #region method

    /// <summary>
    /// Show the status of the application's resources.
    /// </summary>
    /// <remarks>When overriding method, use the "ServiceScopeFactory" property to access to a service.</remarks>
    public override int ShowStatus()
    {
        int errorCode = base.ShowStatus();
        using (IServiceScope scope = ServiceScopeFactory.CreateScope())
        {
            errorCode |= ShowDbStatus(scope.ServiceProvider.GetRequiredService<TDbContext>());
        }
        return errorCode;
    }

    /// <summary>
    /// Show the connection status and the migration status of the database.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <returns>0 if everything is ok.</returns>
    public virtual int ShowDbStatus(TDbContext db)
    {
        if (!ShowDbConnection(db))
        {
            return 7001;
        }
        return !ShowMigrations(db) ? 7002 : 0;
    }

    /// <summary>
    /// Show the connection status status of the database.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <returns><c>true</c> if everything is ok.</returns>
    public virtual bool ShowDbConnection(TDbContext db)
    {
        ConsoleEx.Write("Connecting to database...");
        bool canConnect = false;
        try
        {
            db.Database.OpenConnection();
            db.Database.CloseConnection();
            canConnect = true;
            ConsoleEx.WriteLine("OK.");
        }
        catch (Exception ex)
        {
            ConsoleEx.WriteLine("Failed!");
            ConsoleEx.WriteError($"error: {ex.Message}");
            TraceException(ex);
        }
        return canConnect;
    }

    /// <summary>
    /// Show the migration status of the database.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <returns><c>true</c> if everything is ok.</returns>
    public virtual bool ShowMigrations(TDbContext db)
    {
        ConsoleEx.WriteLine("Checking applied migrations...");
        List<string> knownMigrations = db.Database.GetMigrations().ToList();
        List<string> appliedMigrations = db.Database.GetAppliedMigrations().ToList();
        List<string> pendingMigrations = knownMigrations.Except(appliedMigrations).ToList();
        bool hasUnknownMigrations = appliedMigrations.Except(knownMigrations).Any();

        ConsoleEx.WriteLine($"Last migration applied : {knownMigrations.LastOrDefault() ?? "none"}");
        if (hasUnknownMigrations)
        {
            ConsoleEx.WriteWarning("Warning: There are unknown migrations applied to the database, check that it is the latest version of the application.");
        }
        if (pendingMigrations.Count == 0)
        {
            ConsoleEx.WriteSuccess("The database is up to date !");
            return !hasUnknownMigrations;
        }
        else
        {
            ConsoleEx.WriteWarning($"Pending migration{(pendingMigrations.Count > 1 ? "s" : "")} : ");
            foreach (string migration in pendingMigrations)
            {
                ConsoleEx.WriteLine($"    - {migration}");
            }
            ConsoleEx.WriteWarning($"The database is not up to date !");
            return false;
        }

    }

    #endregion

}
