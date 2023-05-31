using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace Lagoon.Server.Areas.Identity.Pages.Account;


/// <summary>
/// Page to enable an authenticator app for MFA
/// </summary>
[AllowAnonymous]
public class EnableAuthenticatorModel : PageModel
{

    #region Private fields

    /// <summary>
    /// The URL encoder.
    /// </summary>
    private readonly UrlEncoder _urlEncoder;

    /// <summary>
    /// The logger to trace errors.
    /// </summary>
    private ILogger<EnableAuthenticatorModel> _logger;

    /// <summary>
    /// The sign-in service.
    /// </summary>
    private IAuthenticationManager _authenticationManager;

    #endregion

    #region Public properties

    /// <summary>
    /// Authenticator code (to use without scanning the QRCode)
    /// </summary>
    public string SharedKey { get; set; }

    /// <summary>
    /// Authenticator link
    /// </summary>
    public string AuthenticatorUri { get; set; }

    /// <summary>
    /// Flag to persist or not the authentication cookie
    /// </summary>
    public bool RememberMe { get; set; }

    /// <summary>
    /// Model binding
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// Model binded to the view
    /// </summary>
    public class InputModel
    {

        /// <summary>
        /// Verification code
        /// </summary>
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }
    }

    #endregion

    #region Initialisation

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="urlEncoder">Url encoder.</param>
    /// <param name="AuthenticationManager">The main application.</param>
    /// <param name="logger">The logger</param>
    public EnableAuthenticatorModel(UrlEncoder urlEncoder, IAuthenticationManager AuthenticationManager, ILogger<EnableAuthenticatorModel> logger)
    {
        _urlEncoder = urlEncoder;
        _authenticationManager = AuthenticationManager;
        _logger = logger;
    }

    #endregion

    #region Form submission

    /// <summary>
    /// On page load, generate a new authenticator link for the current user
    /// </summary>
    public async Task<IActionResult> OnGetAsync(bool rememberMe)
    {
        try
        {
            RememberMe = rememberMe;
            await LoadSharedKeyAndQrCodeUriAsync();
        }
        catch (AuthenticationException ex)
        {
            ModelState.AddModelError("Input.Code", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured during MFA activation");
            ModelState.AddModelError("Input.Code", "An error occured during MFA activation");
        }

        return Page();
    }

    private async Task LoadSharedKeyAndQrCodeUriAsync()
    {
        // Generate the link wich will be used to generate the QRCode
        AuthenticatorUri = await _authenticationManager.GenerateMfaAuthenticationUriAsync(User, _urlEncoder);
        // Extract the secret from the authentication uri
        // to allow the user to configure the authenticator manually, without to scan the QRCode
        Regex r = new("(?:secret=)(?<secretValue>[^&]+)");
        var res = r.Match(AuthenticatorUri);
        SharedKey = res.Groups["secretValue"].Value;
    }

    /// <summary>
    /// Form submission
    /// </summary>
    /// <param name="rememberMe">RememberMe flag</param>
    /// <param name="returnUrl">Application return url</param>
    public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync();
                return Page();
            }
            // Strip spaces and hypens
            var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
            // Try to enable MFA
            var recoveryCodes = await _authenticationManager.EnableMfaAsync(User, verificationCode);
            // Log the user
            await _authenticationManager.TwoFactorAuthenticatorSignInAsync(verificationCode, rememberMe, false);
            // Check for GroupChoices
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
            ModelState.AddModelError("Input.Code", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured during MFA activation");
            ModelState.AddModelError("Input.Code", "An error occured during MFA activation");
        }
        return Page();
    } 

    #endregion

}
