namespace TemplateLagoonWeb.Model.Context;

/// <summary>
/// Database context
/// </summary>
public class ApplicationDbContext : LgApplicationDbContextGuid<ApplicationUser>
{

    #region constants

    /// <summary>
    /// The name of the "ConnectionString" in the "appsettings.json" file.
    /// </summary>
    internal protected const string CONNECTION_STRING_KEY = "LocalDb";

    #endregion

    #region dbsets

/* Example

    /// <summary>
    /// The foos.
    /// </summary>
    public DbSet<Foo> Foos => Set<Foo>();

*/
    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="options">The options to be used by a Microsoft.EntityFrameworkCore.DbContext.</param>
    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// New instance.
    /// </summary>
    public ApplicationDbContext() { }

    #endregion

    #region methods        

    /// <summary>
    /// Configure the database (and other options) to be used for this context.
    /// </summary>
    /// <param name="builder">A builder used to create or modify options for this context. Databases (and other extensions)
    /// typically define extension methods on this object that allow you to configure the context.</param>
    /// <param name="configuration">The configuration manager.</param>
    /// <param name="enableDetailedLogging">Option to include error messages for migrations.</param>
    public static void Configure(DbContextOptionsBuilder builder, IConfiguration configuration, bool enableDetailedLogging = false)
    {
        // Configures the context to connect to a PostgreSQL database with Npgsql
        builder.UseSqlServer(
            configuration.GetConnectionString(CONNECTION_STRING_KEY),
            options => options.MigrationsAssembly(GetAssemblyName<ApplicationDbContext>()));
        // Registers the identity manager entity sets
        builder.UseOpenIddict<Guid>();
        // Activate the detailed logging
        if (enableDetailedLogging)
        {
            // Enable detailed errors when handling of data value exceptions
            builder.EnableDetailedErrors();
            // Enable application data to be included in exception messages, logging, etc
            builder.EnableSensitiveDataLogging();
        }
    }

    /// <summary>
    /// Configure the schema need for the identity framework.
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Ancestor configuration
        base.OnModelCreating(builder);
        /* #UserNameAsId: Remove the "SetUserNameRequired" line if the "UserName" property isn't used to identify an user. */
        // Sets the name field of the "AspNetUser" table as required.
        SetUserNameRequired(builder);
        // Sets the email field of the "AspNetUser" table as required.
        SetUserEmailRequired(builder);
        // Globally disable cascading deletion
        DisableCascadingDeletionBehavior(builder);
        // Register application roles
        HasIdentityRoles<Roles>(builder);
    }

    /// <summary>
    /// Override this method to set defaults and configure conventions before they run. This method is invoked before <see cref="OnModelCreating" />.
    /// </summary>
    /// <param name="configurationBuilder">The builder being used to set defaults and configure conventions that will be used to build the model for this context.</param>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // WARNING : Never change the convention when the application is already in production !!!
        base.ConfigureConventions(configurationBuilder);
        // Ensure that the date stored in the database is in UTC format and that the date read from the database is in "local" format (using the time zone of the web server)
        configurationBuilder.DateSaveAsUtcLoadAsLocal();
    }

    #endregion

}
