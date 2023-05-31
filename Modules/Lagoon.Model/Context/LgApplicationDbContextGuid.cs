using Lagoon.Helpers;
using Lagoon.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Lagoon.Model.Context;


/// <summary>
/// Context of the application database with string keys for users and roles.
/// </summary>
/// <typeparam name="TUser">Application user.</typeparam>
public abstract class LgApplicationDbContextGuid<TUser> : LgApplicationDbContextBase<TUser, IdentityRole<Guid>, Guid>
    where TUser : LgIdentityUserGuid
{

    #region constructors

    /// <summary>
    /// Database context initialisation.
    /// </summary>
    /// <param name="options">The options to be used by a Microsoft.EntityFrameworkCore.DbContext.</param>
    /// <param name="collationType">The collation type.</param>
    public LgApplicationDbContextGuid(DbContextOptions options, CollationType? collationType = null) : base(options, collationType)
    { }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="collationType">The collation type.</param>
    protected LgApplicationDbContextGuid(CollationType? collationType = null) : base(collationType)
    { }

    #endregion

    #region methods

    /// <summary>
    /// Get a constant role Id from a role name.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <returns>The role id.</returns>
    protected override Guid GetRoleId(string roleName)
    {
        int roleNameLength = roleName.Length;
        if (roleNameLength > 16)
        {
            throw new ArgumentException("The max length for a role name is 16 characters", nameof(roleName));
        }
        byte[] buffer = new byte[16];
        MemoryStream stream = new(buffer);
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(roleName);
        // Add the role name
        stream.Write(inputBytes);
        if (roleNameLength < 16)
        {
            stream.WriteByte(13);
            if (roleNameLength < 15)
            {
                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    stream.Write(md5.ComputeHash(inputBytes), 0, 15 - roleNameLength);
                }
            }
        }
        return new Guid(buffer);
    }

    #endregion

}
