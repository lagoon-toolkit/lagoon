using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using AuthenticationOptions = Lagoon.Server.Application.AuthenticationOptions;

namespace Lagoon.Server.Areas.Identity.Account;


/// <summary>
/// Page used to update the password of an authenticated user 
/// or changing the password for an unauthenticated user with a recovery code (sended by mail to the user by the ForgotPassword page)
/// </summary>
public class ResetPasswordModel : PageModel
{

    #region Private properties

    // Lagoon application authentication options
    private readonly AuthenticationOptions _authenticationOptions;

    // Lagoon application
    private readonly ILgAuthApplication _app;

    // Log errors
    private readonly ILogger<ResetPasswordModel> _logger;

    #endregion

    #region View model

    /// <summary>
    /// Actual user password (in case of updating the password of an authenticated user)
    /// </summary>
    [BindProperty]
    [DataType(DataType.Password)]
    public string ActualPassword { get; set; }

    /// <summary>
    /// New user password
    /// </summary>
    [BindProperty]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    /// <summary>
    /// Confirm user password
    /// </summary>
    [BindProperty]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

    /// <summary>
    /// Reset password code
    /// </summary>
    [BindProperty]
    public string Code { get; set; }

    /// <summary>
    /// Reset password code
    /// </summary>
    [BindProperty]
    public string UserId { get; set; }

    /// <summary>
    /// Flag to handle if the user is authenticated or not
    /// </summary>
    [BindProperty]
    public bool IsUserAuthenticated { get; set; } = false;

    /// <summary>
    /// Flag to handle if the password has been successfully updated
    /// </summary>
    public bool Complete { get; set; } = false;

    /// <summary>
    /// Application name
    /// </summary>
    public string ApplicationName;

    /// <summary>
    /// Application version
    /// </summary>
    public string ApplicationVersion;

    #endregion

    #region Page initialisation

    /// <summary>
    /// Page initialisation
    /// </summary>
    /// <param name="authenticationOptions">Retrieve application authentication options</param>
    /// <param name="application">Lagoon application</param>
    /// <param name="logger">App logger</param>
    public ResetPasswordModel(AuthenticationOptions authenticationOptions, ILgApplication application, ILogger<ResetPasswordModel> logger)
    {
        _authenticationOptions = authenticationOptions;
        _app = (ILgAuthApplication)application;
        ApplicationName = _app.ApplicationInformation.Name;
        ApplicationVersion = _app.ApplicationInformation.Version.ToString();
        _logger = logger;
    }

    /// <summary>
    /// Check if a reset code is supplied
    /// </summary>
    /// <param name="userId">User unique identifier</param>
    /// <param name="code">Validation code</param>
    public async Task<IActionResult> OnGetAsync(string userId, string code)
    {
        var userAuth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (userAuth.Succeeded)
        {
            IsUserAuthenticated = userAuth.Principal.Identity.IsAuthenticated;
        }
        // rq: this page is used for two purposes
        // from the forgot password page => not authenticated
        //      => must have a code provided and this code must be validated BEFORE updating the user password
        // from the application, to allow the user to change his password
        //      => authenticated => update password without validation since the user is already authenticated
        //                       => don't use the userId query string and extract claim from current request
        if (string.IsNullOrWhiteSpace(_authenticationOptions.ForgotPasswordUrl)
            || ((string.IsNullOrEmpty(code) || string.IsNullOrEmpty(userId)) && !IsUserAuthenticated))
        {
            throw new UnauthorizedAccessException();
        }
        Code = code;
        UserId = userId;
        return Page();
    }

    #endregion

    #region Form actions

    /// <summary>
    /// Reset user password
    /// </summary>
    public async Task<IActionResult> OnPostNewPasswordAsync()
    {
        try
        {
            if (ConfirmPassword == Password)
            {
                var userAuth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
                IdentityResult result = null;
                if (userAuth.Succeeded && userAuth.Principal.Identity.IsAuthenticated)
                {
                    result = await _app.ChangePasswordAsync(userAuth.Principal, ActualPassword, Password);
                }
                else if (!string.IsNullOrEmpty(Code) && !string.IsNullOrEmpty(UserId))
                {
                    var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
                    result = await _app.ResetPasswordAsync(UserId, code, Password);
                }
                if (result != null)
                {
                    if (result.Succeeded)
                    {
                        Complete = true;
                    }
                    // Display errors related to password update
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "lblWrongPasswordConfirm".Translate());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            ModelState.AddModelError(string.Empty, "lblAuthErr".Translate());
        }
        return Page();
    }

    /// <summary>
    /// Return to the login page
    /// </summary>
    public IActionResult OnPostReturnLogin()
    {
        return RedirectToPage("Login");
    }

    #endregion

}
