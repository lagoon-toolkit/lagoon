using Lagoon.Helpers;
using Lagoon.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Lagoon.Model.Context;

/// <summary>
/// Context of the application database with string keys for users and roles.
/// </summary>
/// <typeparam name="TUser">Application user.</typeparam>
public abstract class LgApplicationDbContext<TUser> : LgApplicationDbContextBase<TUser, IdentityRole, string>
    where TUser : LgIdentityUser
{

    #region constructors

    /// <summary>
    /// Database context initialisation.
    /// </summary>
    /// <param name="options">The options to be used by a Microsoft.EntityFrameworkCore.DbContext.</param>
    /// <param name="collationType">The collation type.</param>
    public LgApplicationDbContext(DbContextOptions options, CollationType? collationType = null) : base(options, collationType)
    { }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="collationType">The collation type.</param>
    protected LgApplicationDbContext(CollationType? collationType = null) : base(collationType)
    { }

    #endregion

    #region methods

    /// <summary>
    /// Get a constant role Id from a role name.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <returns>The role id.</returns>
    protected override string GetRoleId(string roleName)
    {
        ArgumentNullException.ThrowIfNull(roleName);
        return roleName;
    }

    #endregion

}
