using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lagoon.Server.Areas.Identity.Pages.Account;


/// <summary>
/// Page to login with MFA
/// </summary>
[AllowAnonymous]
public class LoginWithMfaModel : PageModel
{

    #region Private fields

    // Logger
    private ILogger<LoginWithMfaModel> _logger;

    /// <summary>
    /// The sign-in service.
    /// </summary>
    private readonly IAuthenticationManager _authenticationManager;

    #endregion

    #region Public properties

    /// <summary>
    /// Binded data
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// The remember flag
    /// </summary>
    public bool RememberMe { get; set; }

    /// <summary>
    /// The returnUrl after a successfull login attempt
    /// </summary>
    public string ReturnUrl { get; set; }

    /// <summary>
    /// Inputs binding
    /// </summary>
    public class InputModel
    {

        /// <summary>
        /// MFA code
        /// </summary>
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string TwoFactorCode { get; set; }

        /// <summary>
        /// Flag to remember or not the browser (and not ask for MFA again)
        /// </summary>
        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }
    }

    #endregion

    #region Initialisation

    /// <summary>
    /// Page initialisation
    /// </summary>
    /// <param name="AuthenticationManager">The sign-in service.</param>
    /// <param name="logger">The logger.</param>
    public LoginWithMfaModel(IAuthenticationManager AuthenticationManager, ILogger<LoginWithMfaModel> logger)
    {
        _authenticationManager = AuthenticationManager;
        _logger = logger;
    }

    #endregion

    #region Form submission

    /// <summary>
    /// Store remember parameter
    /// </summary>
    /// <param name="rememberMe">true if the application cookie must be persisted, false otherwise</param>
    public IActionResult OnGet(bool rememberMe)
    {
        RememberMe = rememberMe;
        return Page();
    }

    /// <summary>
    /// MFA code submission
    /// </summary>
    /// <param name="rememberMe">RemeberMe flag</param>
    /// <param name="returnUrl">The returnUrl page</param>
    public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            returnUrl ??= Url.Content("~/");
            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            await _authenticationManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);
            // Check for groups selection
            if (HttpContext.GetCookieValue("LgGroupSelector") != null)
            {
                return RedirectToPage("./GroupChoice", new { returnUrl });
            }
            else
            {
                return LocalRedirect(returnUrl);
            }
        }
        catch (AuthenticationException ex)
        {
            ModelState.AddModelError("Input.TwoFactorCode", ex.Message);
            return Page();
        }
        catch (System.Security.Cryptography.CryptographicException ex)
        {
            // Probably : The payload was invalid...
            foreach (string cookieName in HttpContext.Request.Cookies.Keys)
            {
                HttpContext.RemoveCookie(cookieName, "/");
            }
            throw new Exception("You have probably reused a port previously used by another application: Authentication cookies have been reset.  The site cookies have been clean up, please TRY TO RELOAD APPLICATION ! (" + ex.Message + ")", ex);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("Input.TwoFactorCode", "An error occured during MFA code verification");
            _logger.LogError(ex, "An error occured during MFA code verification");
            return Page();
        }
    } 

    #endregion

}
