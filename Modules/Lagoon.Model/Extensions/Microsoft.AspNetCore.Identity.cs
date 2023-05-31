namespace Microsoft.AspNetCore.Identity;

/// <summary>
/// Lagoon extensions for "Identity".
/// </summary>
public static class LagoonExtensions
{

    /// <summary>
    /// Add the specified <paramref name="user"/> to the named role.
    /// </summary>
    /// <param name="userManager">The user management service.</param>
    /// <param name="user">The user to add to the named role.</param>
    /// <param name="role">The name of the role to add the user to.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public static Task<IdentityResult> AddToRoleAsync<TUser>(this UserManager<TUser> userManager, TUser user, Enum role) where TUser : class
    {
        return userManager.AddToRoleAsync(user, role.ToString());
    }

}
