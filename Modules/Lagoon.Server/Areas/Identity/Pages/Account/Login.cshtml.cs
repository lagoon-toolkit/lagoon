using Lagoon.Server.Application.Authentication;
using Lagoon.Server.Application.IdentitySources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Lagoon.Server.Areas.Identity.Account;


/// <summary>
/// Default connection page
/// </summary>
[AllowAnonymous]
public class LoginModel : PageModel
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
    /// List of all external authentication provider registered
    /// </summary>
    private List<AuthenticationScheme> _externalAuth;

    /// <summary>
    /// Windows authentication activted ?
    /// </summary>
    private bool _isWindowsAuth;

    /// <summary>
    /// Form authentication activated ?
    /// </summary>
    private bool _isFormAuth;

    /// <summary>
    /// Only one external authentication provider registered ?
    /// </summary>
    private bool _isOnlyOneSso;

    /// <summary>
    /// No external authentication provider registered ?
    /// </summary>
    private bool _isNoSSO;

    /// <summary>
    /// Only explicit authentication provider registered ?
    /// (explicit = we must not check username to retrieve a potential external provider
    /// </summary>
    private bool _isOnlyExplicitSso;

    #endregion

    #region Public BindProperty

    /// <summary>
    /// Show login input ?
    /// </summary>
    [BindProperty]
    public bool ShowLogin { get; set; }

    /// <summary>
    /// Show password input ?
    /// </summary>
    [BindProperty]
    public bool ShowPassword { get; set; }

    /// <summary>
    /// User login
    /// </summary>
    [Display(Name = "Login")]
    [BindProperty]
    public string UserLogin { get; set; } = null;

    /// <summary>
    /// User password
    /// </summary>
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [BindProperty]
    public string UserPassword { get; set; } = null;

    /// <summary>
    /// Should remember user ?
    /// </summary>
    [Display(Name = "Remember me")]
    [BindProperty]
    public bool RememberMe { get; set; }

    /// <summary>
    /// If windows authentication is configured, show access 
    /// </summary>
    [BindProperty]
    public bool ShowWindowsAuth { get; set; }

    /// <summary>
    /// If "Remember me" option should be display
    /// </summary>
    [BindProperty]
    public bool ShowRememberMe { get; set; }

    /// <summary>
    /// Is form authentication allowed ?
    /// </summary>
    [BindProperty]
    public bool AllowFormAuthentication { get; set; }

    /// <summary>
    /// List of external provider with no direct pattern
    /// </summary>
    [BindProperty]
    public List<AuthenticationScheme> DirectExternalSSOProvider { get; set; }

    /// <summary>
    /// Application return url
    /// </summary>
    public string ReturnUrl { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    [BindProperty]
    public string ErrorMessage { get; set; }

    /// <summary>
    /// To show/hide register button
    /// </summary>
    [BindProperty]
    public string RegisterUrl { get; set; }

    /// <summary>
    /// To show/hide forgot password button
    /// </summary>
    [BindProperty]
    public string ForgotPasswordUrl { get; set; }

    #endregion

    #region Private properties

    /// <summary>
    /// Retrieve configured authentication options
    /// </summary>
    private readonly Application.AuthenticationOptions _authenticationOptions;

    /// <summary>
    /// Application name
    /// </summary>
    public string ApplicationName;

    /// <summary>
    /// Application version
    /// </summary>
    public string ApplicationVersion;

    #endregion

    #region Initialization

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="authenticationOptions">Application authentication parameters</param>
    /// <param name="application">Lagoon application manager.</param>
    /// <param name="AuthenticationManager">The sign-in service.</param>
    public LoginModel(Application.AuthenticationOptions authenticationOptions, ILgApplication application, IAuthenticationManager AuthenticationManager)
    {
        _authenticationOptions = authenticationOptions;
        _app = (ILgAuthApplication)application;
        _authenticationManager = AuthenticationManager;
        ShowWindowsAuth = _authenticationOptions.AllowWindowsAuthentication;
        ShowRememberMe = _authenticationOptions.AllowRememberMe;
        AllowFormAuthentication = _authenticationOptions.AllowFormAuthentication;
        ApplicationName = _app.ApplicationInformation.Name;
        ApplicationVersion = _app.ApplicationInformation.Version.ToString();
    }

    #endregion

    #region Login page configuration

    /// <summary>
    /// Launch OAuth authentication flow directly if sso provider is supplied in url
    /// Remark: SSO Provider must be know by the application (.AddGoogle, .AddFededation, .AddSaml2, ...)
    /// </summary>
    public async Task<IActionResult> OnGetAsync(string returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        // Try to use the impersonate account
        IdentitySource impersonatedIdentity = _app.GetImpersonatedIdentity();
        if (impersonatedIdentity is not null)
        {
            return await TrySignInAsync(impersonatedIdentity, ReturnUrl);
        }
        // Show the login page
        ForgotPasswordUrl = Url.Content(_authenticationOptions.ForgotPasswordUrl);
        RegisterUrl = Url.Content(_authenticationOptions.RegisterUrl);
        // Get application configuration helpers
        await InitConfigAsync();
        if (_isWindowsAuth && !_isFormAuth && _isNoSSO)
        {
            // If Windows authentication is the only authentication method, redirect to the WindowLogin page
            return RedirectToPage("./WindowsLogin", new { returnUrl = ReturnUrl });
        }
        else if (!_isWindowsAuth && _isOnlyOneSso && !_isFormAuth)
        {
            // If there is only one SSO configured, redirect to the single sso
            return LaunchExternalAuthentication(_externalAuth.First().Name, ReturnUrl, RememberMe);
        }
        else if (_isFormAuth && (_isNoSSO || _isOnlyExplicitSso))
        {
            // If Forms authentication and no sso domain config: the form area will only be used by forms authentication (there is no analyse for domain in login)
            ShowLogin = true;
            ShowPassword = true;
        }
        else if (_isFormAuth || !_isOnlyExplicitSso)
        {
            // If SSO + Forms only show login area (password area should be show only if we can't determinate an sso provider for authentication)
            ShowLogin = true;
        }
        else
        {
            throw new Exception("No valid authentication configuration found");
        }
        // Show login page if no direct login config available)
        return Page();
    }

    /// <summary>
    /// Launch authentication process with the selected provider
    /// </summary>
    /// <param name="providerName">Provider name</param>
    /// <param name="returnUrl">Application returnUrl</param>
    /// <param name="rememberMe">Should store cookie when the browser is closed</param>
    private IActionResult LaunchExternalAuthentication(string providerName, string returnUrl, bool rememberMe)
    {
        // Launch OAuth external provider authentication flow
        rememberMe = rememberMe && ShowRememberMe;
        string redirectUrl = Url.Page("./ExternalProvidersLogin", pageHandler: "Callback", values: new { returnUrl, rememberMe });
        AuthenticationProperties properties = _authenticationManager.ConfigureExternalAuthenticationProperties(providerName, redirectUrl);
        return new ChallengeResult(providerName, properties);
    }

    #endregion

    #region Forms submit (Forms + Direct Windows / SSO)

    /// <summary>
    /// On form submission
    /// </summary>
    /// <param name="returnUrl">Applicatioin return url</param>
    /// <param name="userLogin">User login</param>
    public async Task<IActionResult> OnPostAsync(string returnUrl = null, string userLogin = null)
    {
        try
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            await InitConfigAsync();
            if (!string.IsNullOrWhiteSpace(UserLogin) && UserLogin.Contains('@'))
            {
                // Extract domain from email and look for a configured SSO corresponding to this domain
                AuthenticationScheme externalDomainAuth = _externalAuth.Where(e => e.DisplayName.ToLower() != "explicit" && new Regex(e.DisplayName).IsMatch(userLogin)).FirstOrDefault();
                // If SSO provider found
                if (externalDomainAuth != null)
                {
                    // Launch OAuth external provider authentication flow
                    return LaunchExternalAuthentication(externalDomainAuth.Name, ReturnUrl, RememberMe);
                }
                else if (!_authenticationOptions.AllowFormAuthentication)
                {
                    ShowLogin = true;
                    throw new AuthenticationException("InvalidLoginAttempt".Translate());
                }
            }
            if (_authenticationOptions.AllowFormAuthentication)
            {
                ShowLogin = _isFormAuth && (_isNoSSO || _isOnlyExplicitSso);
                // Login / Password authentication
                if (string.IsNullOrWhiteSpace(userLogin))
                {
                    ShowLogin = true;
                }
                else if (!ShowPassword)
                {
                    ShowPassword = true;
                }
                else if (!string.IsNullOrWhiteSpace(UserLogin) && !string.IsNullOrWhiteSpace(UserPassword))
                {
                    // Build the identity source
                    FormsIdentitySource signinIdentity = new(userLogin, UserPassword, RememberMe && ShowRememberMe);
                    return await TrySignInAsync(signinIdentity, ReturnUrl);
                }
                else
                {
                    throw new AuthenticationException("InvalidLoginAttempt".Translate());
                }
            }
            else
            {
                ShowLogin = true;
                throw new AuthenticationException("InvalidLoginAttempt".Translate());
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, _app.GetContactAdminMessage(ex, true));
            // Trace exception
            _app.TraceException(ex);
        }

        return Page();
    }

    private async Task<IActionResult> TrySignInAsync(IdentitySource identity, string returnUrl)
    {
        LgAuthenticationState authState;
        try
        {
            // Try to sign the user
            authState = await _app.SignInAsync(identity);
            if (identity.GroupChoices.Count > 1)
            {
                // User must choose a group. Redirect to the group selection page
                string groups = JsonSerializer.Serialize(identity.GroupChoices);
                // Add a cookie with possible groups selection & redirect the user to the group choice page
                HttpContext.AddCookie("LgGroupSelector", _app.Protect(groups, "LgGroupSelector"), true, Url.Content("~/"));
            }
        }
        catch (System.Security.Cryptography.CryptographicException ex)
        {
            // Probably : The payload was invalid...
            foreach (string cookieName in HttpContext.Request.Cookies.Keys)
            {
                HttpContext.RemoveCookie(cookieName, Url.Content("~/"));
            }
            throw new Exception("You have probably reused a port previously used by another application: Authentication cookies have been reset.  The site cookies have been clean up, please TRY TO RELOAD APPLICATION ! (" + ex.Message + ")", ex);
        }
        if (authState == LgAuthenticationState.RequireMfaValidation)
        {
            return RedirectToPage("./LoginWithMfa", new { ReturnUrl = returnUrl, rememberMe = RememberMe && ShowRememberMe });
        }
        else if (authState == LgAuthenticationState.RequireMfaActivation)
        {
            return RedirectToPage("./EnableAuthenticator", new { ReturnUrl = returnUrl, rememberMe = RememberMe && ShowRememberMe });
        }
        else if (identity.GroupChoices.Count > 1)
        {
            return RedirectToPage("./GroupChoice", new { ReturnUrl = returnUrl });
        }
        else
        {
            // No groups choice, return to application
            return LocalRedirect(returnUrl);
        }
    }

    /// <summary>
    /// Launch authentication with external SSO
    /// </summary>
    /// <param name="provider">SSO Provider name</param>
    /// <param name="returnUrl">Application return URL</param>
    /// <param name="rememberMe">Indicate if authentication persists through navigator sessions.</param>
    /// <returns></returns>
    public IActionResult OnPostExternalAuthentication(string provider, string returnUrl = null, bool rememberMe = false)
    {
        return LaunchExternalAuthentication(provider, returnUrl, rememberMe && ShowRememberMe);
    }

    #endregion

    #region Auth config helper

    /// <summary>
    /// Init auth helpers variables
    /// </summary>
    /// <returns></returns>
    private async Task InitConfigAsync()
    {
        _externalAuth = await _authenticationManager.GetExternalAuthenticationSchemesAsync();
        // Init the list of external provider without domain pattern
        DirectExternalSSOProvider = _externalAuth.Where(x => x.DisplayName.ToLower() == "explicit").ToList();
        // Authentication configuration
        _isWindowsAuth = _authenticationOptions.AllowWindowsAuthentication;
        _isFormAuth = _authenticationOptions.AllowFormAuthentication;
        _isOnlyOneSso = _externalAuth.Count == 1;
        _isNoSSO = _externalAuth.Count == 0;
        _isOnlyExplicitSso = DirectExternalSSOProvider.Count == _externalAuth.Count;
    }

    #endregion

}
