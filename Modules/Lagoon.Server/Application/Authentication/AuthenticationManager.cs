using Lagoon.Model.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Lagoon.Server.Application;

/// <summary>
/// The Suser sign-in service.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
public class AuthenticationManager<TUser> : IAuthenticationManager
    where TUser : class, ILgIdentityUser
{

    #region fields

    /// <summary>
    /// The main application.
    /// </summary>
    private ILgApplication _app;

    #endregion

    #region properties

    /// <summary>
    /// The sign-in manager.
    /// </summary>
    public SignInManager<TUser> SignInManager { get; }

    /// <summary>
    /// The user manager.
    /// </summary>
    public UserManager<TUser> UserManager { get; }

    #endregion

    #region constructor

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="app">The main application.</param>
    /// <param name="signInManager">The sign-in manager.</param>
    public AuthenticationManager(ILgApplication app, SignInManager<TUser> signInManager)
    {
        _app = app;
        SignInManager = signInManager;
        UserManager = SignInManager.UserManager;
    }

    #endregion

    #region methods

    /// <summary>
    /// Generate a (OTP) link for an authenticator app.
    /// </summary>
    /// <param name="user">User for which we want to create an OTP uri.</param>
    /// <param name="urlEncoder">Encoder user to generate the link.</param>
    /// <returns>A (OTP) link for an authenticator app.</returns>
    public async Task<string> GenerateMfaAuthenticationUriAsync(ClaimsPrincipal user, UrlEncoder urlEncoder)
    {
        TUser appUser = (user.Identity.IsAuthenticated
            ? await UserManager.GetUserAsync(user)
            : await SignInManager.GetTwoFactorAuthenticationUserAsync()) ?? throw new AuthenticationException("Unable to retrieve the user.");
        string unformattedKey = await UserManager.GetAuthenticatorKeyAsync(appUser);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await UserManager.ResetAuthenticatorKeyAsync(appUser);
            unformattedKey = await UserManager.GetAuthenticatorKeyAsync(appUser);
        }

        return string.Format("otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
                                urlEncoder.Encode(_app.ApplicationInformation.RootName),
                                urlEncoder.Encode(appUser.UserName),
                                unformattedKey);
    }

    /// <summary>
    /// Enable MFA for the specified user.
    /// </summary>
    /// <param name="user">User for which we want to validate the MFA.</param>
    /// <param name="code">Authenticator code.</param>
    /// <returns>A list of codes (to be able to login without the authenticator app).</returns>
    public async Task<IEnumerable<string>> EnableMfaAsync(ClaimsPrincipal user, string code)
    {
        TUser appUser = (user != null && user.Identity.IsAuthenticated
            ? await UserManager.GetUserAsync(user)
            : await SignInManager.GetTwoFactorAuthenticationUserAsync()) ?? throw new AuthenticationException("Unable to retrieve user");
        bool is2faTokenValid = await UserManager.VerifyTwoFactorTokenAsync(appUser, UserManager.Options.Tokens.AuthenticatorTokenProvider, code);
        if (!is2faTokenValid)
        {
            throw new AuthenticationException("Verification code is invalid.");
        }
        await UserManager.SetTwoFactorEnabledAsync(appUser, true);
        return await UserManager.CountRecoveryCodesAsync(appUser) == 0
            ? await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(appUser, 10)
            : null;
    }

    /// <summary>
    /// Log an user with MFA.
    /// </summary>
    /// <param name="authenticatorCode">An authenticator code.</param>
    /// <param name="persistent">true is the authentication cookie must be persisted, false otherwise.</param>
    /// <param name="rememberMachine">true is we want to allow the user to remember the browser to bypass MFA check.</param>
    public async Task TwoFactorAuthenticatorSignInAsync(string authenticatorCode, bool persistent, bool rememberMachine)
    {
        SignInResult result = await SignInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, persistent, rememberMachine);
        if (!result.Succeeded)
        {
            throw new AuthenticationException("Invalid authenticator code.");
        }
    }

    /// <summary>
    /// Returns a flag indicating if the current client browser has been remembered by
    /// two factor authentication for the user attempting to login, as an asynchronous
    /// operation.
    /// </summary>
    /// <param name="user">The user attempting to login.</param>
    /// <returns>
    /// The task object representing the asynchronous operation containing true if the
    /// browser has been remembered for the current user.
    /// </returns>
    public Task<bool> IsTwoFactorClientRememberedAsync(TUser user)
    {
        return SignInManager.IsTwoFactorClientRememberedAsync(user);
    }

    /// <summary>
    /// Signs in the specified user.
    /// </summary>
    /// <param name="user">The user to sign-in.</param>
    /// <param name="isPersistent">
    /// Flag indicating whether the sign-in cookie should persist after the browser is
    /// closed.
    /// </param>
    /// <param name="authenticationMethod">Name of the method used to authenticate the user.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public Task SignInAsync(TUser user, bool isPersistent, string authenticationMethod = null)
    {
        return SignInManager.SignInAsync(user, isPersistent, authenticationMethod);
    }

    /// <summary>
    /// Signs in the specified user.
    /// </summary>
    /// <param name="user">The user to sign-in.</param>
    /// <param name="authenticationProperties">Properties applied to the login and authentication cookie.</param>
    /// <param name="authenticationMethod">Name of the method used to authenticate the user.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public Task SignInAsync(TUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
    {
        return SignInManager.SignInAsync(user, authenticationProperties, authenticationMethod);
    }

    /// <summary>
    /// Sign an user with MFA.
    /// </summary>
    /// <param name="user">USer to log.</param>
    /// <param name="loginProvider">Login provider name.</param>
    public async Task SignInTwoFactorAsync(TUser user, string loginProvider)
    {
        // Store the userId for use after two factor check
        string userId = await UserManager.GetUserIdAsync(user);
        await SignInManager.Context.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, StoreTwoFactorInfo(userId, loginProvider));
    }

    /// <summary>
    /// Creates a claims principal for the specified 2fa information.
    /// </summary>
    /// <param name="userId">The user who is logging in via 2fa.</param>
    /// <param name="loginProvider">The 2fa provider.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> containing the user 2fa information.</returns>
    private static ClaimsPrincipal StoreTwoFactorInfo(string userId, string loginProvider)
    {
        ClaimsIdentity identity = new(IdentityConstants.TwoFactorUserIdScheme);
        identity.AddClaim(new Claim(ClaimTypes.Name, userId));
        if (loginProvider != null)
        {
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, loginProvider));
        }
        return new ClaimsPrincipal(identity);
    }


    /// <summary>
    /// Signout the current user.
    /// </summary>
    public Task SignOutAsync()
    {
        return SignInManager.SignOutAsync();
    }

    /// <summary>
    ///  Return the list of authentication method configured in the application.
    /// </summary>
    public async Task<List<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
    {
        return (await SignInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    /// <summary>
    /// Return the authentication properties for a specific external provider
    /// </summary>
    /// <param name="providerName">External provider name</param>
    /// <param name="redirectUrl">SSO page redirection when successfully authentified</param>
    public AuthenticationProperties ConfigureExternalAuthenticationProperties(string providerName, string redirectUrl)
    {
        return SignInManager.ConfigureExternalAuthenticationProperties(providerName, redirectUrl);
    }

    /// <summary>
    /// Retrieve the data (claims) provided by an external authentication provider (depending on SSO provider)
    /// </summary>
    public Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
    {
        return SignInManager.GetExternalLoginInfoAsync();
    }

    /// <summary>
    /// Stores any authentication tokens found in the external authentication cookie into the associated user.
    /// </summary>
    /// <param name="externalLoginInfo">The information from the external login provider.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the operation.</returns>
    public Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo externalLoginInfo)
    {
        return SignInManager.UpdateExternalAuthenticationTokensAsync(externalLoginInfo);
    }

    #endregion

}