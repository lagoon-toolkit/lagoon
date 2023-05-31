using Lagoon.Server.Application.IdentitySources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Security.Principal;

namespace Lagoon.Server.Areas.Identity.Account;


/// <summary>
/// Page used as endpoint for SSO login response
/// </summary>
public class ExternalProvidersLoginModel : PageModel
{

    #region Fields

    /// <summary>
    /// Access to application manager for signin user.
    /// </summary>
    private readonly ILgAuthApplication _app;

    /// <summary>
    /// The sign-in service.
    /// </summary>
    private readonly IAuthenticationManager _authenticationManager;

    /// <summary>
    /// The authentication options.
    /// </summary>
    private Application.AuthenticationOptions _authOptions;

    #endregion

    #region Constructors

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="authenticationOptions">The authentication options.</param>
    /// <param name="application">The application.</param>
    /// <param name="AuthenticationManager">The sign-in service.</param>
    public ExternalProvidersLoginModel(Application.AuthenticationOptions authenticationOptions, ILgApplication application, IAuthenticationManager AuthenticationManager)
    {
        _authOptions = authenticationOptions;
        _app = (ILgAuthApplication)application;
        _authenticationManager = AuthenticationManager;
    }

    #endregion

    #region External authentication provider endpoint

    /// <summary>
    /// Called when an external authentication provider log an user for the application
    /// </summary>
    public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null)
    {
        // Get the SSO login informations
        ExternalLoginInfo info = await _authenticationManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (result.Succeeded)
            {
                string provider = result.Properties.Items.Where(x => x.Key == "LoginProvider").FirstOrDefault().Value;
                string subject = result.Principal.FindFirst(_authOptions.SSOUniqueIdentifier).Value;
                info = new ExternalLoginInfo(result.Principal, provider, subject, "");
            }
            else
            {
                // Send a user exception
                throw new AuthenticationException("Authentication failed.");
            }
        }
        if (info is not null)
        {
            // Try to extract user identifier
            // Remarks : SignInManager.GetExternalLoginInfoAsync return null if the claim "NameIdentifier" isn't present
            string uniqueIdentifier = string.IsNullOrEmpty(_authOptions.SSOUniqueIdentifier)
                ? info.Principal.Identity?.Name
                : info.Principal.FindFirst(_authOptions.SSOUniqueIdentifier)?.Value;
            if (string.IsNullOrEmpty(uniqueIdentifier))
            {
                StringBuilder sb = new();
                sb.Append("[ExternalProviderLogin] Valid SSO response but unable to retrieve the claim");
                sb.Append(_authOptions.SSOUniqueIdentifier);
                sb.AppendLine(".");
                sb.Append("Identity.Name: ");
                sb.AppendLine(info.Principal.Identity?.Name);
                sb.AppendLine("Claims: ");
                foreach (Claim claim in info.Principal.Claims)
                {
                    sb.Append("  ");
                    sb.Append(claim.Type);
                    sb.Append(": ");
                    sb.AppendLine(claim.Value);
                }
                throw new Exception(sb.ToString());
            }
            // Try to Signin the user
            IdentitySource identity = new(AuthenticationMode.SSO, uniqueIdentifier, info.Principal.Claims, info.LoginProvider, true);
            identity.ExternalLoginInfo = info;
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await _app.SignInAsync(identity);
            // If user must choose a group before access to the application
            if (identity.GroupChoices.Count > 1)
            {
                string groups = JsonSerializer.Serialize(identity.GroupChoices);
                // Add a cookie with possible groups selection & redirect the user to the group choice page
                HttpContext.AddCookie("LgGroupSelector", _app.Protect(groups, "LgGroupSelector"), true, Url.Content("~/"));
                // User must choose a group. Redirect to the group selection page
                return RedirectToPage("./GroupChoice", new { returnUrl });
            }
            else
            {
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
        }
        else
        {
            throw new Exception("[ExternalProviderLogin] No valid login information found.");
        }
    }

    #endregion
}
