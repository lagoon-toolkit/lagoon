using Microsoft.AspNetCore;
using OpenIddict.Validation;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Lagoon.Server.Application.Authentication;

/// <summary>
/// Handler used by Openiddict to extract the token from a cookie when using download from client side
/// </summary>
public class TokenFromCookieHandler : IOpenIddictValidationHandler<OpenIddictValidationEvents.ProcessAuthenticationContext>
{

    /// <summary>
    /// Check if the 'lgndld' identifier is set in the query string and if so retrieve the token value from 
    /// </summary>
    /// <param name="context">Authentication context</param>
    public ValueTask HandleAsync(OpenIddictValidationEvents.ProcessAuthenticationContext context)
    {
        var request = context.Transaction.GetHttpRequest();

        // Get the cookie name from the url (Warning : The use of request.Query broke the $batch query sometimes...)
        if (request.QueryString.Value.Contains("lgndld="))
        {
            string cookieName = request.Query["lgndld"];
            string token = request.Cookies[cookieName + "T"];
            if (!string.IsNullOrEmpty(token) && context.Token is null)
            {
                // Get the token from the cookie
                context.Token = token;
                // Must be explicitly set to avoid error when enabling DataProtection on Openiddict
                context.TokenType = TokenTypeHints.AccessToken;
            }

            // Remove the cookie (to indicate that the download has started)
            request.HttpContext.Response.Cookies.Append(cookieName, "x",
                new CookieOptions()
                {
                    Expires = DateTimeOffset.Now.AddYears(-1),
                    HttpOnly = false,
                    Path = "/",
                    SameSite = SameSiteMode.Lax
                });
        }
        return default;
    }

}
