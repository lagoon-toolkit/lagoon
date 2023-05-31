using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Lagoon.Server.Application;

/// <summary>
/// The sig-in sign-out service.
/// </summary>
public interface IAuthenticationManager
{
    /// <summary>
    /// Return the authentication properties for a specific external provider
    /// </summary>
    /// <param name="providerName">External provider name</param>
    /// <param name="redirectUrl">SSO page redirection when successfully authentified</param>
    AuthenticationProperties ConfigureExternalAuthenticationProperties(string providerName, string redirectUrl);

    /// <summary>
    /// Enable MFA for the specified user.
    /// </summary>
    /// <param name="user">User for which we want to validate the MFA.</param>
    /// <param name="code">Authenticator code.</param>
    /// <returns>A list of codes (to be able to login without the authenticator app).</returns>
    Task<IEnumerable<string>> EnableMfaAsync(ClaimsPrincipal user, string code);

    /// <summary>
    /// Generate a (OTP) link for an authenticator app.
    /// </summary>
    /// <param name="user">User for which we want to create an OTP uri.</param>
    /// <param name="urlEncoder">Encoder user to generate the link.</param>
    /// <returns>A (OTP) link for an authenticator app.</returns>
    Task<string> GenerateMfaAuthenticationUriAsync(ClaimsPrincipal user, UrlEncoder urlEncoder);

    /// <summary>
    ///  Return the list of authentication method configured in the application.
    /// </summary>
    Task<List<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();

    /// <summary>
    /// Retrieve the data (claims) provided by an external authentication provider (depending on SSO provider)
    /// </summary>
    Task<ExternalLoginInfo> GetExternalLoginInfoAsync();

    /// <summary>
    /// Signout the current user.
    /// </summary>
    Task SignOutAsync();

    /// <summary>
    /// Log an user with MFA.
    /// </summary>
    /// <param name="authenticatorCode">An authenticator code.</param>
    /// <param name="persistent">true is the authentication cookie must be persisted, false otherwise.</param>
    /// <param name="rememberMachine">true is we want to allow the user to remember the browser to bypass MFA check.</param>
    Task TwoFactorAuthenticatorSignInAsync(string authenticatorCode, bool persistent, bool rememberMachine);

    /// <summary>
    /// Stores any authentication tokens found in the external authentication cookie into the associated user.
    /// </summary>
    /// <param name="externalLoginInfo">The information from the external login provider.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the operation.</returns>
    Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo externalLoginInfo);

}