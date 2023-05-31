using Lagoon.Core;
using Lagoon.Core.Application;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lagoon.Server.Pages;

/// <summary>
/// The production error page.
/// </summary>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{

    /// <summary>
    /// The application informations.
    /// </summary>
    public IApplicationInformation ApplicationInformation { get; }

    /// <summary>
    /// The request id.
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// The request id link.
    /// </summary>
    public string RequestIdLink { get; set; }

    /// <summary>
    /// The exception message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="app">The application informations.</param>
    public ErrorModel(ILgApplication app)
    {
        ApplicationInformation = app.ApplicationInformation;
    }

    /// <summary>
    /// Show the current error.
    /// </summary>
    /// <returns>The page.</returns>
    public IActionResult OnGet()
    {
        RequestId = HttpContext.GetRequestId();
        IExceptionHandlerFeature context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        Exception exception = context.Error;
        exception.Data.Add("RequestId", RequestId);
        Message = LgApplicationBase.Current.GetContactAdminMessage(exception, false);
        RequestIdLink = GetRequestIdLink();
        return Page();
    }

    /// <summary>
    /// Returns the mailto: adress for the RequestId.
    /// </summary>
    /// <returns>he mailto: adress for the RequestId.</returns>
    protected virtual string GetRequestIdLink()
    {
        string subject = $"[{ApplicationInformation?.Name}] An unexpected error has occurred";
        string body = $"{Message}\nRequest ID: {RequestId}";
        return $"mailto:{Uri.EscapeDataString(ApplicationInformation?.SupportEmail ?? "")}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}";
    }

}