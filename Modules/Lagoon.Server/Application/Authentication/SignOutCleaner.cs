using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lagoon.Server.Application.Authentication;

/// <summary>
/// List all browser items that need to be removed when a user log out.
/// </summary>
public class SignOutCleaner
{

    #region fields

    private List<CookieIdentifier> _cookies;

    #endregion

    #region methods

    /// <summary>
    /// Add a new name to remove when the user log out the application.
    /// </summary>
    /// <param name="name">The name of the cookie.</param>
    /// <param name="path">The path of the cookie.</param>
    public void AddCookie(string name, string path)
    {
        _cookies ??= new();
        _cookies.Add(new(name, path));
    }

    /// <summary>
    /// Remove the cookie from the HTTP context and return the javascript to run to complete the logout.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="pageModel">The page model.</param>
    /// <returns>The javascript to run to complete the logout.</returns>
    public void Run(ILgAuthApplication app, PageModel pageModel)
    {
        // Remove the group choice cookie
        pageModel.HttpContext.RemoveCookie(app.GroupChoiceCookieName(), pageModel.Url.Content("~/"));
        // Remove additional cookies
        if (_cookies is not null)
        {
            foreach (CookieIdentifier cookie in _cookies)
            {
                pageModel.HttpContext.RemoveCookie(cookie.Name, cookie.Path);
            }
        }
    }

    #endregion

    #region CookieIdentifier class

    /// <summary>
    /// Class to identify a cookie.
    /// </summary>
    public class CookieIdentifier
    {
        /// <summary>
        /// The name of the cookie.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The path of the cookie.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// New instance.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="path">The path of the cookie.</param>
        public CookieIdentifier(string name, string path)
        {
            Name = name;
            Path = path;
        }

    }

    #endregion

}
