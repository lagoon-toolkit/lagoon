using Lagoon.Internal;
using System.Diagnostics;

namespace Lagoon.Server.Middlewares;


/// <summary>
/// Handle the Lagoon resources.
/// </summary>
public class LagoonResourceMiddleware
{

    #region fields

    /// <summary>
    /// The main application.
    /// </summary>
    private readonly ILgApplication _app;

    /// <summary>
    /// Next middleware to execute
    /// </summary>
    private readonly RequestDelegate _next;

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="next">Next middleware in the pipeline to execute.</param>
    /// <param name="app">Application manager</param>
    public LagoonResourceMiddleware(RequestDelegate next, ILgApplication app)
    {
        _next = next;
        _app = app;
    }

    #endregion

    #region methods

    /// <summary>
    /// Handle the Lagoon resources.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>Next middleware in the pipeline</returns>
    [StackTraceHidden]
    public Task InvokeAsync(HttpContext context)
    {
        // Apply the client language from the x-lang cookie
        ApplyClientLanguage(context);
        string path = context.Request.Path.Value.ToLowerInvariant();
        // Block direct access to the generated file folder, the virtual path must be used.
        if (path.StartsWith(Routes.VIRTUAL_ROOT_PATH))
        {
            throw new Exception($"The acces to the path \"{path}\" is not allowed. Try to remove the \"{Routes.VIRTUAL_ROOT_PATH[..^1]}\" from the path.");
        }
        // Redirect all index*.html to the good environment one
        if (path.StartsWith("/index.", StringComparison.Ordinal) && path.EndsWith(".html", StringComparison.Ordinal))
        {
            context.Request.Path = _app.GetIndexPath();
        }
        // Check if the path is a virtual one
        else if (_app.VirtualPaths.TryGetValue(path, out string newPath))
        {
            context.Request.Path = newPath;
        }
        // Invoke the next delegate/middleware in the pipeline
        return _next(context);
    }

        /// <summary>
        /// Check if the request have an 'x-lang' cookie and if so try to apply the requested culture.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        private void ApplyClientLanguage(HttpContext context)
        {
            string xLangCookieName = $".{_app.ApplicationInformation.RootName}.Lang";
            CultureInfo culture = null;
            // Check if 'x-lang' cookie is present 
            if (context.Request.Cookies.ContainsKey(xLangCookieName))
            {
                // Retrieve the culture name to apply  from cookie
                try
                {
                    // Set the culture for the current request
                    culture = new CultureInfo(context.GetCookieValue(xLangCookieName));
                }
                catch (Exception ex)
                {
                    // Ignore the error if cookie value is invalid
                    _app.TraceWarning($"Failed to apply client language from {xLangCookieName} cookie. ({ex.Message}).");
                }
            }
            // Init selected culture for the current thread
            _app.SetCulture(culture);
        }

#endregion
}
