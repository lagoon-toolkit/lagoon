using System.Security.Claims;

namespace Lagoon.Server.Application.Authentication;

/// <summary>
/// Interface used to wrap call to UserManager and SignInManager
/// without the need to know the type
/// </summary>
public interface ILgAuthHelper
{

    /// <summary>
    /// Return an the user identifier from the ClaimsPrincipal
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>The userId if found, null otherwise</returns>
    Task<string> GetUserIdAsync(ClaimsPrincipal user);

    /// <summary>
    /// Validates the security stamp for the specified principal against the persisted
    /// stamp for the current user, as an asynchronous operation.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns><c>true</c> if valid, <c>false</c> otherwise</returns>
    Task<bool> ValidateSecurityStampAsync(ClaimsPrincipal principal);

    /// <summary>
    /// Returns a flag indicating whether the specified user can sign in.
    /// </summary>
    /// <param name="user">The user whose sign-in status should be returned.</param>
    /// <returns>The task object representing the asynchronous operation, containing a flag that is true if the specified user can sign-in, otherwise false.</returns>
    Task<bool> CanSignInAsync(ClaimsPrincipal user);

    /// <summary>
    /// Creates a System.Security.Claims.ClaimsPrincipal for the specified user, as an asynchronous operation.
    /// </summary>
    /// <param name="userId">The userId to create a System.Security.Claims.ClaimsPrincipal for.</param>
    /// <returns>The task object representing the asynchronous operation, containing the ClaimsPrincipal for the specified user.</returns>
    Task<ClaimsPrincipal> CreateUserPrincipalAsync(string userId);

    /// <summary>
    /// Signs the current user out of the application.
    /// </summary>
    /// <returns>The task object representing the asynchronous operation</returns>
    Task SignOutAsync();

}
