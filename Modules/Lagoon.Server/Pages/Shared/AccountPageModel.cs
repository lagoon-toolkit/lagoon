using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lagoon.Server.Pages.Shared;


/// <summary>
/// Default connection page
/// </summary>
[AllowAnonymous]
public class AccountPageModel : PageModel
{

    #region ViewData

    /// <summary>
    /// The application display name.
    /// </summary>
    [ViewData]
    public string ApplicationName => App.ApplicationInformation.Name;

    #endregion

    #region properties

    /// <summary>
    /// The application display name.
    /// </summary>
    public ILgAuthApplication App { get; }

    /// <summary>
    /// The last error message.
    /// </summary>
    public string ErrorMessage { get; set; }

    #endregion

    #region constructor

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="app">The main application.</param>
    public AccountPageModel(ILgApplication app)
    {
        if (app is null)
        {
            throw new ArgumentException(nameof(app));
        }
        App = (ILgAuthApplication)app;
    }

    #endregion

    #region methods

    /// <summary>
    /// Trace exception and set description in ErrorMessage property.
    /// </summary>
    /// <param name="ex">The exception</param>
    protected void UpdateErrorMessage(Exception ex)
    {
        // Trace exception
        App.TraceException(ex);
        // Show error to user
        ErrorMessage = App.GetContactAdminMessage(ex, true);
    }

    #endregion

}
