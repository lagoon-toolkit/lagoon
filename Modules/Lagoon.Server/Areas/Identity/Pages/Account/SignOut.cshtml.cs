using Lagoon.Server.Pages.Shared;

namespace Lagoon.Server.Areas.Identity.Pages.Account;


/// <summary>
/// Default connection page
/// </summary>
[AllowAnonymous]
public class SignOutModel : AccountPageModel
{

    #region constructor

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="app">The main application.</param>
    public SignOutModel(ILgApplication app) : base(app)
    {
    }

    #endregion

    #region methods

    /// <summary>
    /// Windows authentication endpoint
    /// </summary>
    public IActionResult OnGet()
    {
        try
        {
        }
        catch (Exception ex)
        {
            UpdateErrorMessage(ex);
        }
        // Show the page
        return Page();
    }

    #endregion

}
