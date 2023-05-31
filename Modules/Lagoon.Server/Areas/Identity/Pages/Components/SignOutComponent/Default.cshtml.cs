using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lagoon.Server.Areas.Identity.Pages.Components;

/// <summary>
/// Component to handle the complete signout.
/// </summary>
[AllowAnonymous]
public class SignOutComponent : ViewComponent
{

    /// <summary>
    /// The current application.
    /// </summary>
    public ILgAuthApplication Application { get; }

    /// <summary>
    /// Specify the behavior of post logout.
    /// </summary>
    public string PostLogoutMode { get; }

    /// <summary>
    /// Optional. Define the uri to call after successfully disconnected from the application.
    /// Can be usefull to disconnect from SSO when disconnecting from the application.
    /// </summary>
    public string PostLogoutUri { get; }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="application">The current application.</param>
    public SignOutComponent(ILgApplication application)
    {
        Application = (ILgAuthApplication)application;
        // Check if a post logout action is configured
        PostLogoutMode = Application.Configuration["App:PostLogoutMode"];
        PostLogoutUri = Application.Configuration["App:PostLogoutUri"];
    }

    /// <summary>
    /// Inserting the component.
    /// </summary>
    /// <returns>The component render.</returns>
    public IViewComponentResult Invoke(PageModel pageModel)
    {
        // Clear cookies
        Application.SignOutCleaner.Run(Application, pageModel);
        return View(this);
    }

}
