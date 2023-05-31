using Lagoon.Server.Application.Authentication;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;
using AuthenticationOptions = Lagoon.Server.Application.AuthenticationOptions;

namespace Lagoon.Server.Controllers;


/// <summary>
/// Controller used to handle authentication with OpenIddict.
/// Base on 'Balosar' OpenIddict sample :
/// https://github.com/openiddict/openiddict-samples/blob/dev/samples/Balosar/Balosar.Server/Controllers/AuthorizationController.cs
/// </summary>
public class LgAuthorizationController : ControllerBase
{

    #region private vars

    // Openiddict managers
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    // Lagoon helpers
    private readonly ILgAuthHelper _authHelper;
    private readonly ILgAuthApplication _app;
    private readonly AuthenticationOptions _authOptions;

    #endregion

    #region initialization

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="applicationManager">Openiddict application manager</param>
    /// <param name="authorizationManager">Openiddidct authorization manager</param>
    /// <param name="scopeManager">Openiddic scope manager</param>
    /// <param name="authHelper">User and SignIn Manager wrapper</param>
    /// <param name="authOptions">Lagoon authentication options</param>
    /// <param name="application">Current LgApplication</param>
    public LgAuthorizationController(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        AuthenticationOptions authOptions,
        ILgAuthHelper authHelper, 
        ILgApplication application)
    {
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
        _scopeManager = scopeManager;
        _authHelper = authHelper;
        _authOptions = authOptions;
        _app = (ILgAuthApplication)application;
    }

    #endregion

    #region authentication endpoints

    /// <summary>
    /// Authorization Endpoint
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="Exception"></exception>
    [HttpGet("connect/authorize")]
    [HttpPost("connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> AuthorizeAsync()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
        
        // If prompt=login was specified by the client application,
        // immediately return the user agent to the login page.
        if (request.HasPrompt(Prompts.Login))
        {
            // To avoid endless login -> authorization redirects, the prompt=login flag
            // is removed from the authorization request payload before redirecting the user.
            var prompt = string.Join(" ", request.GetPrompts().Remove(Prompts.Login));

            var parameters = Request.HasFormContentType ?
                Request.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList() :
                Request.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();

            parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));

            return Challenge(
                authenticationSchemes: IdentityConstants.ApplicationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(parameters)
                });
        }

        // Retrieve the user principal stored in the authentication cookie.
        // If a max_age parameter was provided, ensure that the cookie is not too old.
        // If the user principal can't be extracted or the cookie is too old, redirect the user to the login page.
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (result == null || !result.Succeeded || (request.MaxAge != null && result.Properties?.IssuedUtc != null &&
            DateTimeOffset.UtcNow - result.Properties.IssuedUtc > TimeSpan.FromSeconds(request.MaxAge.Value)))
        {
            // If the client application requested promptless authentication,
            // return an error indicating that the user is not logged in.
            if (request.HasPrompt(Prompts.None))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
                    }));
            }

            return Challenge(
                authenticationSchemes: IdentityConstants.ApplicationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                        Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                });
        }
        // Retrieve the profile of the logged in user.
        var userId = await _authHelper.GetUserIdAsync(result.Principal);      
        if (userId == null)
        {
            return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
                    }));
        }
        // Retrieve the application details from the database.
        var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
            throw new InvalidOperationException("Details concerning the calling client application cannot be found.");
        // Retrieve the permanent authorizations associated with the user and the calling client application.
        var authorizations = await _authorizationManager.FindAsync(
            subject: userId,
            client: await _applicationManager.GetIdAsync(application),
            status: Statuses.Valid,
            type: AuthorizationTypes.Permanent,
            scopes: request.GetScopes()).ToListAsync();
        // Consent type management
        switch (await _applicationManager.GetConsentTypeAsync(application))
        {
            // If the consent is external (e.g when authorizations are granted by a sysadmin),
            // immediately return an error if no authorization can be found in the database.
            case ConsentTypes.External when !authorizations.Any():
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The logged in user is not allowed to access this client application."
                    }));

            // If the consent is implicit or if an authorization was found,
            // return an authorization response without displaying the consent form.
            case ConsentTypes.Implicit:
            case ConsentTypes.External when authorizations.Any():
            case ConsentTypes.Explicit when authorizations.Any() && !request.HasPrompt(Prompts.Consent):
                var principal = await _authHelper.CreateUserPrincipalAsync(userId);

                // The granted scopes match the requested scope
                principal.SetScopes(request.GetScopes());
                principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());

                // Automatically create a permanent authorization to avoid requiring explicit consent
                // for future authorization or token requests containing the same scopes.
                var authorization = authorizations.LastOrDefault() 
                    ?? await _authorizationManager.CreateAsync(
                        principal: principal,
                        subject: userId,
                        client: await _applicationManager.GetIdAsync(application),
                        type: AuthorizationTypes.Permanent,
                        scopes: principal.GetScopes());

                principal.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));
                // Add claims to the principal
                principal = RemoveRoleGroupChoice(principal);
                foreach (var claim in principal.Claims)
                {
                    claim.SetDestinations(GetClaimDestinations(claim));
                }

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // At this point, no authorization was found in the database and an error must be returned
            // if the client application specified prompt=none in the authorization request.
            case ConsentTypes.Explicit when request.HasPrompt(Prompts.None):
            case ConsentTypes.Systematic when request.HasPrompt(Prompts.None):
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "Interactive user consent is required."
                    }));

            // In every other case, stop the authentication 
            default:
                throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Token endpoint
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [HttpPost("~/connect/token"), Produces("application/json")]
    public async Task<IActionResult> ExchangeAsync()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
        {
            // Retrieve the claims principal stored in the authorization code/device code/refresh token.
            var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

            // Retrieve the user profile corresponding to the authorization code/refresh token.
            // We want to automatically invalidate the authorization code/refresh token
            // when the user password/roles change
            bool validUser = await _authHelper.ValidateSecurityStampAsync(principal);
            if (!validUser)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
                    }));
            }

            // Ensure the user is still allowed to sign in.
            if (!await _authHelper.CanSignInAsync(principal))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                    }));
            }
            // Set claims for AccessToken and IdToken
            principal = RemoveRoleGroupChoice(principal);
            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(GetClaimDestinations(claim));
            }
            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        // M2M flow
        else if (request.IsClientCredentialsGrantType())
        {
            var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                throw new InvalidOperationException("The application cannot be found.");

            var authClient = _authOptions.AdditionnalClients.Where(client => client.ClientId == request.ClientId).FirstOrDefault() ??
                throw new InvalidOperationException("The authentication option cannot be found.");

            // Create a new ClaimsIdentity containing the claims that will be used to create an id_token, a token or a code.
            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

            // Use the client_id as the subject identifier.
            identity.AddClaim(Claims.Subject,
                await _applicationManager.GetClientIdAsync(application),
                Destinations.AccessToken, Destinations.IdentityToken);

            identity.AddClaim(Claims.Name,
                await _applicationManager.GetDisplayNameAsync(application),
                Destinations.AccessToken, Destinations.IdentityToken);

            // Add the corresponding role for this client
            foreach (string role in authClient.Roles)
            {
                identity.AddClaim(Claims.Role, role, Destinations.AccessToken, Destinations.IdentityToken);
            }

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new InvalidOperationException("The specified grant type is not supported.");
    }

    /// <summary>
    /// Logout endpoint
    /// </summary>
    /// <returns></returns>
    [HttpGet("~/connect/logout")]
    public Task<IActionResult> LogoutAsync()
    {
        return LogoutPostAsync();
    }

    /// <summary>
    /// Logout endpoint
    /// </summary>
    /// <returns></returns>
    [HttpPost("~/connect/logout"), ValidateAntiForgeryToken]
    public async Task<IActionResult> LogoutPostAsync()
    {
        // Ask ASP.NET Core Identity to delete the local and external cookies created
        // when the user agent is redirected from the external identity provider
        // after a successful authentication flow (e.g Google or Facebook).
        //zz await _signInManager.SignOutAsync();
        await _authHelper.SignOutAsync();

        // Returning a SignOutResult will ask OpenIddict to redirect the user agent
        // to the post_logout_redirect_uri specified by the client application or to
        // the RedirectUri specified in the authentication properties if none was set.
        return SignOut(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties
            {
                RedirectUri = "/"
            });
    }

    #endregion

    #region Utils

    /// <summary>
    /// By default claims are NOT automatically included in the access and identity tokens by Openiddict.
    /// Lagoon convention: All claims suffixed with 'id' should go to AccessToken AND IdentityToken
    /// </summary>
    /// <param name="claim">The claim for which we have to chose a destination</param>
    /// <returns></returns>
    private static IEnumerable<string> GetClaimDestinations(Claim claim)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // We have to specify where the claims should be added (id_token, access_token or bothà
        switch (claim.Type)
        {
            case Claims.Name:
            case Claims.FamilyName:
            case Claims.GivenName:
            case Claims.Email:
                // Theses claims can be easy retrieve from server-side if required.
                // Don't include theses claims in the AccessToken to avoid increasing bearer size
                yield return Destinations.IdentityToken;
                yield break;

            case Claims.Role:
                // We need role in the AccessToken for server-side authorization & validation
                yield return Destinations.AccessToken;
                // We need role  in IdentityToken because of Blazor "AuthorizeView" mechanism which is based on role
                yield return Destinations.IdentityToken;
                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp":
                yield break;

            default:
                // Lagoon convention: All claims suffixed with id should go to the access_token 
                if (claim.Type.ToLowerInvariant().EndsWith("id"))
                {
                    yield return Destinations.AccessToken;
                }
                yield return Destinations.IdentityToken;
                yield break;
        }
    }

    /// <summary>
    /// Remove roles from claimsPrincipal according to the content of the GroupChoiceCookie
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    private ClaimsPrincipal RemoveRoleGroupChoice(ClaimsPrincipal claimsPrincipal)
    {
        // Check if user has choose a group and if so remove roles from user according to group content
        string groupChoice = HttpContext.GetCookieValue(_app.GroupChoiceCookieName());
        if (!string.IsNullOrEmpty(groupChoice))
        {
            // We have to create a new identity to be able to modify claims from the claimsPrincipal
            var claimsIdentiy = (ClaimsIdentity)claimsPrincipal.Identity;
            var allUserRoles = claimsIdentiy.GetClaims(Claims.Role);
            // Decode & extract role asked by the groupchoice option
            string decodedGroupChoice = _app.Unprotect(groupChoice, ILgAuthApplication.GROUP_CHOICE_PROTECT_KEY);
            string[] roles = decodedGroupChoice.Split(",");
            // Remove all claims and add only role for the chosen profile
            claimsIdentiy.RemoveClaims(Claims.Role);
            // Add role(s) claims according to user roles (we could have roles in cookie that has been deleted from user) and the role in the GroupChoice cookie
            claimsIdentiy.AddClaims(roles.Where(roleName => allUserRoles.Contains(roleName)).Select(roleName => new Claim(Claims.Role, roleName)));
            return new ClaimsPrincipal(claimsIdentiy);
        }
        return claimsPrincipal;
    } 

    #endregion

}

internal static class AsyncEnumerableExtensions
{
    internal static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return ExecuteAsync();

        async Task<List<T>> ExecuteAsync()
        {
            var list = new List<T>();

            await foreach (var element in source)
            {
                list.Add(element);
            }

            return list;
        }
    }
}
