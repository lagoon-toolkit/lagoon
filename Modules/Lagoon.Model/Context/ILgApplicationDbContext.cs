using Lagoon.Core;
using Lagoon.Core.Application;
using Lagoon.Shared.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Lagoon.Model.Context;

/// <summary>
/// Interface used by dependency injection.
/// </summary>
public interface ILgApplicationDbContext : IDisposable
{

    #region properties

    /// <summary>
    /// Provides access to database related information and operations for this context.
    /// </summary>
    DatabaseFacade Database { get; }

    /// <summary>
    /// Get or set the Eula table
    /// </summary>
    DbSet<Eula> Eulas { get; set; }

    /// <summary>
    /// Get or set the girdview profiles table
    /// </summary>
    DbSet<GridViewProfile> GridViewProfiles { get; set; }

    #endregion

    #region method

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    int SaveChanges();

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get and entity db set.
    /// </summary>
    /// <returns>The entity db set.</returns>
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    #endregion

}
