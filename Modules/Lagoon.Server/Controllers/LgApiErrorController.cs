using Microsoft.AspNetCore.Diagnostics;

namespace Lagoon.Server.Controllers;


/// <summary>
/// Controller used to catch all unhandled exception
/// </summary>
[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class LgApiErrorController : LgControllerBase
{
    #region fields

    /// <summary>
    /// The main application.
    /// </summary>
    private readonly ILgApplication _app;

    #endregion

    #region constructor

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="app">The application.</param>
    public LgApiErrorController(ILgApplication app)
    {
        _app = app;
    }

    #endregion

    #region methods

    /// <summary>
    /// Catch unhandled exception and return error message in DEBUG
    /// </summary>
    /// <returns>Error message in DEBUG, generic message in RELEASE</returns>
    [Route("ApiError")]
    public ErrorObjectResponse Get()
    {
        IExceptionHandlerFeature context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        Response.StatusCode = 500;
        return new ErrorObjectResponse(_app, context?.Error, HttpContext.GetRequestId());
    }

    #endregion

}
