using Lagoon.Server.Application.IdentitySources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lagoon.Server.Areas.Identity.Account;


/// <summary>
/// Page used to show group choice (if some group choice has been set for a particular user)
/// </summary>
public class GroupChoiceModel : PageModel
{

    #region fields

    /// <summary>
    /// Access to application manager for signin user.
    /// </summary>
    private readonly ILgAuthApplication _app;

    #endregion

    #region properties

    /// <summary>
    /// List of group choice for connected user
    /// </summary>
    [BindProperty]
    public IList<GroupChoice> GroupChoices { get; set; }

    /// <summary>
    /// Default choice
    /// </summary>
    [BindProperty]
    public string DefaultChoice { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// Initialize instance.
    /// </summary>
    /// <param name="application">Application manager.</param>
    public GroupChoiceModel(ILgApplication application)
    {
        _app = (ILgAuthApplication)application;
    }

    #endregion

    #region methods

    /// <summary>
    /// Initialization
    /// </summary>
    public IActionResult OnGet()
    {
        string groupsValuesEncrypted = HttpContext.GetCookieValue("LgGroupSelector");
        if (!string.IsNullOrEmpty(groupsValuesEncrypted))
        {
            string groupsValuesDecrypted = _app.Unprotect(groupsValuesEncrypted, "LgGroupSelector");
            GroupChoices = JsonSerializer.Deserialize<List<GroupChoice>>(groupsValuesDecrypted);
            DefaultChoice = GroupChoices.First().Description;
            return Page();
        }
        throw new UnauthorizedAccessException();
    }

    #endregion

    #region User group choice selection 

    /// <summary>
    /// On group selection
    /// </summary>
    /// <param name="groupChoice">Group value choosed by the user</param>
    /// <param name="returnUrl">Applicaiton return url</param>
    public async Task<IActionResult> OnPostAsync(string groupChoice, string returnUrl)
    {
        AuthenticateResult userAuth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (userAuth.Succeeded)
        {
            HttpContext.AddCookie(_app.GroupChoiceCookieName(), _app.Protect(groupChoice, ILgAuthApplication.GROUP_CHOICE_PROTECT_KEY),
                                    true, Url.Content("~/"), DateTime.Now.AddYears(2));
            HttpContext.RemoveCookie("LgGroupSelector", Url.Content("~/"));
            // Return to application
            return LocalRedirect(returnUrl);
        }
        throw new UnauthorizedAccessException();
    }

    #endregion
}
