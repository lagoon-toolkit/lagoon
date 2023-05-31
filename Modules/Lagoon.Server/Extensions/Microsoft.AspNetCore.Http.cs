using System.Diagnostics;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Extension methods class.
/// </summary>
public static class LagoonExtensions
{

    #region HttpContext extensions - Cookies

    /// <summary>
    /// Extract the user id from the claims of httpContext user.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>The user ID found.</returns>
    /// <exception cref="Exception">If the user ID is not found.</exception>
    public static Guid GetUserId(this HttpContext httpContext)
    {
        return TryGetUserId(httpContext, out Guid userId) ? userId : throw new Exception("User ID is not found in the HttpContext.");
    }

    /// <summary>
    /// Try to extract the user id from the claims of httpContext user.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="userId">The user ID.</param>
    /// <returns><c>true</c> if the user ID is found.</returns>
    public static bool TryGetUserId(this HttpContext httpContext, out Guid userId)
    {
        string value = httpContext?.User?.FindFirstValue(Claims.Subject);
        if (string.IsNullOrEmpty(value))
        {
            userId = Guid.Empty;
            return false;
        }
        return Guid.TryParse(value, out userId);
    }

    /// <summary>
    /// Add a new cookie to the Http response
    /// </summary>
    /// <param name="httpContext">Extension for HttpContext</param>
    /// <param name="name">Cookie name</param>
    /// <param name="value">Cookie value</param>
    /// <param name="httpOnly">Only http transport ?</param>
    /// <param name="path">Cookie applicable path</param>
    /// <param name="expiration">Expiration date for cookie</param>
    public static void AddCookie(this HttpContext httpContext, string name, string value, bool httpOnly = true, string path = "/", DateTime? expiration = null)
    {
        CookieOptions options = new()
        {
            HttpOnly = httpOnly,
            Secure = httpContext.Request.IsHttps,
            Path = path,
        };
        if (expiration != null)
        {
            options.Expires = new DateTimeOffset(expiration.Value);
        }

        httpContext.Response.Cookies.Append(name, value, options);
    }

    /// <summary>
    /// Retrieve a cookie value (or defaultValue if unknow cookie name)
    /// </summary>
    /// <param name="httpContext">HttpContext extension method</param>
    /// <param name="name">Cookie name to retrieve</param>
    /// <param name="defaultValue">Default value returned if cookie does not exist</param>
    public static string GetCookieValue(this HttpContext httpContext, string name, string defaultValue = null)
    {
        return httpContext.Request.Cookies.ContainsKey(name) ? httpContext.Request.Cookies[name] : defaultValue;
    }

    /// <summary>
    /// Remove an http cookie
    /// </summary>
    /// <param name="httpContext">HttpContext extension method</param>
    /// <param name="name">Name of cookie to delete</param>
    /// <param name="path">Cookie path applicable</param>
    public static void RemoveCookie(this HttpContext httpContext, string name, string path = "/")
    {
        AddCookie(httpContext, name, "X", true, path, new DateTime(2000, 1, 1));
    }

    #endregion

    #region HttpContext extensions- GetRequestId

    /// <summary>
    /// Get the current request unique trace identifier.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>The current request unique trace identifier.</returns>
    public static string GetRequestId(this HttpContext httpContext)
    {
        return Activity.Current?.Id ?? httpContext.TraceIdentifier;
    }

    #endregion

}