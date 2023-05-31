using Lagoon.Helpers;
using Lagoon.Model.Collation;
using Lagoon.Model.EntityConfigurations;
using Lagoon.Model.Models;
using Lagoon.Shared.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace Lagoon.Model.Context;

/// <summary>
/// Base class for the application database context used for identity.
/// </summary>
/// <typeparam name="TUser">The type of user entities.</typeparam>
/// <typeparam name="TRole">The type of role entities.</typeparam>
/// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
public abstract class LgApplicationDbContextBase<TUser, TRole, TKey>
    : IdentityDbContext<TUser, TRole, TKey>, ILgApplicationDbContext, IWithCollation
    where TUser : IdentityUser<TKey>, ILgIdentityUser
    where TRole : IdentityRole<TKey>, new()
    where TKey : IEquatable<TKey>
{

    #region static fields

    private static Func<string, TKey> _convertUserIdFromString;

    #endregion

    #region fields


    /// <summary>
    /// The collation type to use for the database.
    /// </summary>
    private CollationType? _collationType;

    /// <summary>
    /// DB provider name.
    /// </summary>
    private string _providerName;

    #endregion

    #region properties

    ///<inheritdoc/>
    CollationType? IWithCollation.CollationType => _collationType;


    /// <summary>
    /// Get or set the Eula table
    /// </summary>
    public DbSet<Eula> Eulas { get; set; }

    /// <summary>
    /// Get or set the girdview profiles table
    /// </summary>
    public DbSet<GridViewProfile> GridViewProfiles { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// Database context initialisation.
    /// </summary>
    /// <param name="options">The options to be used by a Microsoft.EntityFrameworkCore.DbContext.</param>
    /// <param name="collationType">The collation type.</param>
    public LgApplicationDbContextBase(DbContextOptions options, CollationType? collationType = null) : base(options)
    {
        _collationType = collationType;
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="collationType">The collation type.</param>
    protected LgApplicationDbContextBase(CollationType? collationType = null)
    {
        _collationType = collationType;
    }

    private static Func<string, TKey> InitializeUserIdStringConverter()
    {
        Delegate func = null;
        if (typeof(TKey) == typeof(Guid))
        {
            func = (Func<string, Guid>)Guid.Parse;
        }
        else
        {
            func = typeof(TKey) == typeof(int)
                ? (Delegate)(Func<string, int>)int.Parse
                : throw new NotImplementedException($"The type {typeof(TKey)} is not supported as Id of the User table.");
        }
        return (Func<string, TKey>)func;
    }

    #endregion

    #region methods

    /// <summary>
    /// Return the full name of the current assembly.
    /// </summary>
    /// <typeparam name="T">The type to extract assembly name from.</typeparam>
    /// <returns>The full name of the current assembly.</returns>
    protected static string GetAssemblyName<T>()
    {
        return typeof(T).Assembly.FullName;
    }

    ///<inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        _providerName ??= optionsBuilder.GetProviderName();
        if (_collationType.HasValue)
        {
            // Case and accent sensitive support
            switch (_providerName)
            {
                case CollationProvider.PostgreSQL:
                    optionsBuilder.ConfigurePosgreSQLCollation(_collationType.Value);
                    break;
                case CollationProvider.Oracle:
                    optionsBuilder.ConfigureOracleCollation(GetFrenchCollationName(_collationType.Value));
                    break;
            }
        }
        // Contains transformed into LIKE to add sensitives support
        if (_providerName == CollationProvider.MySQL)
        {
            optionsBuilder.ConfigureMySQLCollation();
        }
    }

    ///<inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        if (typeof(TKey) == typeof(Guid))
        {
            // Limit the size of the role name to 16 character to encode it as ASCI in a Guid.
            EntityTypeBuilder<TRole> role = builder.Entity<TRole>();
            role.Property(r => r.Name).HasMaxLength(16);
            role.Property(r => r.NormalizedName).HasMaxLength(16);
        }
        // LagoonSettings entity with string or Guid user id foreign key
        EntityTypeBuilder lagoonSettings = typeof(TKey) == typeof(string)
            ? builder.Entity<LagoonSettings<TUser>>() // string UserId                                                         
            : builder.Entity<LagoonSettingsGuid<TUser>>(); // Guid UserId
        lagoonSettings.ToTable("LgSettings");
        // Add collation support
        if (_collationType.HasValue)
        {
            ConfigureFrenchCollation(builder, _collationType.Value);
        }
        // Apply configuration for the GridView
        builder.ApplyConfiguration(new GridViewProfileConfiguration());
    }

    /// <summary>
    /// Define database collation based on the french language
    /// </summary>        
    /// <param name="builder">Model builder.</param>    
    /// <param name="collationType">Sensitive case and accent.</param>
    /// <returns></returns>
    private void ConfigureFrenchCollation(ModelBuilder builder, CollationType collationType)
    {
        string collation = GetFrenchCollationName(collationType);
        // Oracle base configuration not supporting collation
        if (!string.IsNullOrEmpty(collation) && _providerName != CollationProvider.Oracle)
        {
            builder.UseCollation(collation);
        }
        if (_providerName == CollationProvider.PostgreSQL)
        {
            // Active unaccent method
            HasPostgresExtension(builder, "unaccent");
        }
    }

    /// <summary>
    /// Return collation name for the french language
    /// </summary>
    /// <param name="collationType"></param>
    /// <returns></returns>
    public string GetFrenchCollationName(CollationType collationType)
    {
        Dictionary<string, string> localesFr = new()
        {
            {CollationProvider.Oracle,      "FRENCH_M"},
            {CollationProvider.SQLServer,   "French"},
            {CollationProvider.MySQL,       "utf8mb4_0900"},
            {CollationProvider.MariaDB,     "utf8mb4_uca1400"}
        };
        return !localesFr.TryGetValue(_providerName, out string collationName) ? null : GetCollationName(collationType, collationName);
    }

    /// <summary>
    /// Return collation name following current RDBMS and locale
    /// </summary>        
    /// <param name="collationType"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    /// <remarks>Provider list : https://learn.microsoft.com/en-us/ef/core/providers/?tabs=dotnet-core-cli</remarks>
    public string GetCollationName(CollationType collationType, string locale)
    {
        CollationProvider providerCollation = _providerName switch
        {
            CollationProvider.SQLServer => new SqlServerCollationProvider(collationType, locale),
            CollationProvider.Oracle => new OracleCollationProvider(collationType, locale),
            CollationProvider.MySQL => new MySQLCollationProvider(collationType, locale),
            CollationProvider.MariaDB => new MariaDBCollationProvider(collationType, locale),
            _ => throw new NotSupportedException("Collation not implemented for this relationnel data management system."),
        };
        return providerCollation.GetCollation();
    }

    /// <summary>
    /// Registers a PostgreSQL extension in the model.
    /// </summary>
    /// <param name="modelBuilder">The model builder in which to define the extension.</param>
    /// <param name="name">The name of the extension to create.</param>
    /// <returns>
    /// The updated <see cref="ModelBuilder"/>.
    /// </returns>
    /// <remarks>
    /// See: https://www.postgresql.org/docs/current/external-extensions.html
    /// </remarks>
    private static ModelBuilder HasPostgresExtension(ModelBuilder modelBuilder, string name)
    {
        Type type = Type.GetType("Microsoft.EntityFrameworkCore.NpgsqlModelBuilderExtensions, Npgsql.EntityFrameworkCore.PostgreSQL", true, false);
        MethodInfo info = type.GetMethod("HasPostgresExtension", BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(ModelBuilder), typeof(string)});
        return (ModelBuilder)info.Invoke(null, new object[] { modelBuilder, name });
    }

    /// <summary>
    /// Sets the email field of the AspNetUser table as required.
    /// </summary>
    /// <param name="builder"></param>
    protected void SetUserEmailRequired(ModelBuilder builder)
    {
        EntityTypeBuilder<TUser> user = builder.Entity<TUser>();
        user.Property(u => u.Email).IsRequired();
        user.Property(u => u.NormalizedEmail).IsRequired();
    }

    /// <summary>
    /// Sets the name field of the AspNetUser table as required.
    /// </summary>
    /// <param name="builder"></param>
    protected void SetUserNameRequired(ModelBuilder builder)
    {
        EntityTypeBuilder<TUser> user = builder.Entity<TUser>();
        user.Property(u => u.UserName).IsRequired();
        user.Property(u => u.NormalizedUserName).IsRequired();
    }

    /// <summary>
    /// Append IdentityRole values to database from <typeparamref name="TAppRole"/>.
    /// </summary>
    /// <typeparam name="TAppRole">The enumeration containing roles.</typeparam>
    /// <param name="builder">The model builder.</param>
    protected void HasIdentityRoles<TAppRole>(ModelBuilder builder) where TAppRole : Enum
    {
        // Create all application roles
        foreach (TAppRole role in Enum.GetValues(typeof(TAppRole)))
        {
            string roleName = role.ToString();
            string roleUpperCaseName = roleName.ToUpperInvariant();
            builder.Entity<TRole>().HasData(new TRole()
            {
                Id = GetRoleId(roleUpperCaseName),
                ConcurrencyStamp = Guid.Empty.ToString(),
                Name = roleName,
                NormalizedName = roleUpperCaseName
            });
        }
    }

    /// <summary>
    /// Get a constant role Id from a role name.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <returns>The role id.</returns>
    protected abstract TKey GetRoleId(string roleName);

    /// <summary>
    /// Add the notification entities.
    /// </summary>
    /// <typeparam name="TNotification">The type for a notification.</typeparam>
    /// <param name="builder">The model builder.</param>
    /// <param name="notificationTableName">The name of the table containing the notification details.</param>
    /// <param name="notificationUsersTableName">The name of the table to associate a notification to an user.</param>
    protected void UseNotifications<TNotification>(ModelBuilder builder,
        string notificationTableName = "Notifications", string notificationUsersTableName = "NotificationUsers")
        where TNotification : NotificationBase
    {
        builder.Entity<TNotification>().ToTable(notificationTableName);
        EntityTypeBuilder entity = typeof(TKey) == typeof(string)
            ? builder.Entity<NotificationUserWithUser<TNotification, TUser>>()
            : builder.Entity<NotificationUserWithUserGuid<TNotification, TUser>>();
        entity.ToTable(notificationUsersTableName);
    }

    /// <summary>
    /// Disable the default "Cascade" deletion or "ClientSetNull" behaviors.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected static void DisableCascadingDeletionBehavior(ModelBuilder modelBuilder)
    {
        foreach (IMutableForeignKey rel in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            rel.DeleteBehavior = DeleteBehavior.NoAction;
        }
    }

    /// <summary>
    /// Convert a string representation of an user's id to the right type.
    /// </summary>
    /// <param name="userId">The string user id.</param>
    /// <returns>The typed user id.</returns>
    public TKey ConvertUserUriFromString(string userId)
    {
        _convertUserIdFromString ??= InitializeUserIdStringConverter();
        return _convertUserIdFromString(userId);
    }

    #endregion

}
