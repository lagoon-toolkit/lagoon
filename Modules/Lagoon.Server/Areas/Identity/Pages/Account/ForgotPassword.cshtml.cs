using Lagoon.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Lagoon.Server.Areas.Identity.Account;


/// <summary>
/// Page for updating a password for 'Form' authentication. 
/// User should have a valid and verified email.
/// </summary>
public class ForgotPasswordModel : PageModel
{
    #region Private properties

    // Retrieve config
    private readonly IConfiguration _config;

    // Send mail
    private readonly ISmtp _sendMail;

    // Authentication methods
    private ILgAuthApplication _app;

    // Lagoon application authentication options
    private readonly AuthenticationOptions _authenticationOptions;

    // Trace errors
    private readonly ILogger<ForgotPasswordModel> _logger;

    #endregion

    #region Page inputs model

    /// <summary>
    /// User login
    /// </summary>
    [Display(Name = "Login")]
    [BindProperty]
    [EmailAddress]
    public string UserLogin { get; set; }

    /// <summary>
    /// Flag to handle if the reset mail as been successfully sended
    /// </summary>
    public bool Completed { get; set; } = false;

    /// <summary>
    /// Application name
    /// </summary>
    public string ApplicationName;

    /// <summary>
    /// Application version
    /// </summary>
    public string ApplicationVersion;

    #endregion

    #region Initialisation

    /// <summary>
    /// Page initialisation
    /// </summary>
    /// <param name="configuration">Access to application configuration</param>
    /// <param name="emailSender">Used to send reset password ling</param>
    /// <param name="authenticationOptions">Retrieve application authentication options</param>
    /// <param name="application">LgApplication</param>
    /// <param name="logger">The logger</param>
    public ForgotPasswordModel(IConfiguration configuration, ISmtp emailSender, AuthenticationOptions authenticationOptions, ILgApplication application, ILogger<ForgotPasswordModel> logger)
    {
        _config = configuration;
        _sendMail = emailSender;
        _app = (ILgAuthApplication)application;
        _authenticationOptions = authenticationOptions;
        ApplicationName = _app.ApplicationInformation.Name;
        ApplicationVersion = _app.ApplicationInformation.Version.ToString();
        _logger = logger;
    }

    /// <summary>
    /// Check if user can access this page
    /// </summary>
    public ActionResult OnGet()
    {
        return string.IsNullOrWhiteSpace(_authenticationOptions.ForgotPasswordUrl) || !_authenticationOptions.AllowFormAuthentication
            ? RedirectToPage("Login")
            : Page();
    }

    #endregion

    #region Form actions

    /// <summary>
    /// Update password: generate and send a link to user
    /// </summary>
    public async Task<IActionResult> OnPostResetPasswordAsync()
    {
        try
        {
            string baseUrl = string.IsNullOrEmpty(_app.ApplicationInformation.PublicURL)
                  ? $"{Request.Scheme}://{Request.Host}{Request.PathBase}"
                   : _app.ApplicationInformation.PublicURL.TrimEnd('/');

            if (ModelState.IsValid)
            {
                string resetLink = await _app.GenerateResetPasswordLinkAsync(UserLogin);
                if (!string.IsNullOrEmpty(resetLink))
                {
                    resetLink = $"{baseUrl}{resetLink}";


                    await _sendMail.SendEmailAsync(UserLogin, "msgResetPwdObject".Translate(ApplicationName), "msgResetPwdBody".Translate(resetLink));

                    // Redirect to login page (to the confirmation handler, to inform user his account is created)
                    Completed = true;
                    return Page();
                }
                else
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("Login");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OnPostResetPasswordAsync : {Message}", ex.Message);
            ModelState.AddModelError(nameof(UserLogin), "lblAuthErr".Translate());
        }
        return Page();
    }

    /// <summary>
    /// Return to the login page
    /// </summary>
    /// <returns></returns>
    public IActionResult OnPostReturnLogin()
    {
        return RedirectToPage("Login");
    }

    #endregion
}
