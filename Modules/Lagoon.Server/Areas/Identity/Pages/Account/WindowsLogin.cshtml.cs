using Lagoon.Server.Application.IdentitySources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Lagoon.Server.Areas.Identity.Account;


/// <summary>
/// Windows login page model.
/// </summary>
public class WindowsLoginModel : PageModel
{

    #region Fields

    /// <summary>
    /// Application authentication configuration
    /// </summary>
    private readonly Application.AuthenticationOptions _authenticationOptions;

    /// <summary>
    /// Access to application manager for signin user.
    /// </summary>
    private readonly ILgAuthApplication _app;

    #endregion

    #region Public properties

    /// <summary>
    /// Potential error message to display
    /// </summary>
    [BindProperty]
    public string ErrorMessage { get; set; }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize instance.
    /// </summary>
    /// <param name="authenticationOptions">Options.</param>
    /// <param name="application">Application manager.</param>
    public WindowsLoginModel(Application.AuthenticationOptions authenticationOptions, ILgApplication application)
    {
        _authenticationOptions = authenticationOptions;
        _app = (ILgAuthApplication)application;
    }

    #endregion

    /// <summary>
    /// Windows authentication endpoint
    /// </summary>
    /// <param name="returnUrl">Application returnUrl</param>
    public async Task<IActionResult> OnGetAsync(string returnUrl)
    {
        returnUrl ??= Url.Content("~/");
        // Check if WindowsAuth is authorized, otherwise redirect to the login page
        if (!_authenticationOptions.AllowWindowsAuthentication)
        {
            string loginUrl = _authenticationOptions.LoginUrl.StartsWith("~") ? _authenticationOptions.LoginUrl : $"~{_authenticationOptions.LoginUrl}";
            return LocalRedirect(Url.Content(loginUrl));
        }
        // Retrieve user to authenticate from the http context
        AuthenticateResult result = await HttpContext.AuthenticateAsync("Windows");
        if (result.Succeeded)
        {
            // Retrieve application context & signin user
            bool removeDomain = _authenticationOptions.SSOUniqueIdentifier != ClaimTypes.Name;
            WindowsIdentitySource identity = new(result.Principal.Identity.Name, removeDomain);
            await _app.SignInAsync(identity);
            // If user must choose a group
            if (identity.GroupChoices.Count > 1)
            {
                // Redirect to group choice page
                string groups = JsonSerializer.Serialize(identity.GroupChoices);
                // Add a cookie with possible groups selection & redirect the user to the group choice page
                HttpContext.AddCookie("LgGroupSelector", _app.Protect(groups, "LgGroupSelector"), true, Url.Content("~/"));
                return RedirectToPage("./GroupChoice", new { returnUrl });
            }
            else
            {
                // No group, return to application
                return LocalRedirect(returnUrl);
            }
        }
        else
        {
            // trigger windows auth
            // since windows auth don't support the redirect uri,
            // this URL is re-triggered when we call challenge
            return Challenge("Windows");
        }
    }

}
