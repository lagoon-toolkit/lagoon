using Lagoon.Server.Application.Authentication;
using Lagoon.Server.Application.IdentitySources;
using System.Security.Claims;

namespace Lagoon.Server;


/// <summary>
/// Interface for an authenticated application.
/// </summary>
public interface ILgAuthApplication : ILgApplication
{

    #region constants

    internal const string GROUP_CHOICE_PROTECT_KEY = "LgGroupChoice";

    #endregion

    #region properties

    /// <summary>
    /// List all browser items that need to be removed when a user log out.
    /// </summary>
    SignOutCleaner SignOutCleaner { get; }

    #endregion

    #region methods

    /// <summary>
    /// Initialize the OpenIddict application manager for Blazor hosted application (eg. the authorization server co-exist with the APIs)
    /// </summary>
    /// <param name="serviceProvider">A scoped service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal Task InitializeOpenIddictAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);

    /// <summary>
    /// Method to use for impersonate a user during development (login page bypass).
    /// </summary>
    /// <remarks>The base implementation return <c>null</c>.</remarks>
    public IdentitySource GetImpersonatedIdentity();

    /// <summary>
    /// SignIn user from login / password screen
    /// </summary>
    /// <param name="login">User login</param>
    /// <param name="password">User password</param>
    /// <param name="rememberMe">If true, store authentication claims, else the user have to login every time</param>
    public Task<LgAuthenticationState> SignInAsync(string login, string password, bool rememberMe);

    /// <summary>
    /// SignIn an user
    /// </summary>
    /// <param name="identitySource">User identity</param>
    public Task<LgAuthenticationState> SignInAsync(IdentitySource identitySource);

    /// <summary>
    /// Signout the current user.
    /// </summary>
    public Task SignOutAsync();

    /// <summary>
    /// Generate a reset password link for From authentication
    /// </summary>
    /// <param name="userIdentifier">The user identifier for wich we want a reset password link</param>
    public Task<string> GenerateResetPasswordLinkAsync(string userIdentifier);

    /// <summary>
    /// Update a password from a password reset token
    /// </summary>
    /// <param name="userId">User unique identifier</param>
    /// <param name="code">Password reset token</param>
    /// <param name="password">New password</param>
    /// <returns>The <see cref="IdentityResult" /></returns>
    public Task<IdentityResult> ResetPasswordAsync(string userId, string code, string password);

    /// <summary>
    /// Update a password for a ClaimsPrincipal with it's actual password.
    /// <see cref="ResetPasswordAsync"/> to change password with an authorization token instead of actual password
    /// </summary>
    /// <param name="userPrincipal">ClaimsPrincipal</param>
    /// <param name="actualPassword">Current user password</param>
    /// <param name="newPassword">New user password</param>
    /// <returns>The <see cref="IdentityResult" /></returns>
    public Task<IdentityResult> ChangePasswordAsync(ClaimsPrincipal userPrincipal, string actualPassword, string newPassword);

    /// <summary>
    /// Get the name of the cookie containing the group choice per application.
    /// </summary>
    /// <returns>The name of the cookie.</returns>
    /// <remarks>In developpment the cookies are shared between the applications,
    /// even if they are on different ports : <see href="https://stackoverflow.com/a/16328399/3568845"/>
    /// </remarks>
    internal string GroupChoiceCookieName()
    {
        return $".{ApplicationInformation.RootName}.Profile";
    }

    /// <summary>
    /// Encrypt a value
    /// </summary>
    /// <param name="value">Value to encrypt</param>
    /// <param name="purpose">The purpose to be assigned to the newly-created Microsoft.AspNetCore.DataProtection.IDataProtector.</param>
    /// <remarks>
    ///  The purpose parameter must be unique for the intended use case; two different
    ///  Microsoft.AspNetCore.DataProtection.IDataProtector instances created with two
    ///  different purpose values will not be able to decipher each other's payloads.
    ///  The purpose parameter value is not intended to be kept secret.
    /// </remarks>
    public string Protect(string value, string purpose);


    /// <summary>
    /// Decrypt a value
    /// </summary>
    /// <param name="value">Value to decrypt</param>
    /// <param name="purpose">The purpose to be assigned to the newly-created Microsoft.AspNetCore.DataProtection.IDataProtector.</param>
    public string Unprotect(string value, string purpose);

    #endregion

}