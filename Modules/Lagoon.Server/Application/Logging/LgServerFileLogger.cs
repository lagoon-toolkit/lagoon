using Lagoon.Core.Application.Logging;
using Lagoon.Server.Exceptions;

namespace Lagoon.Server.Application.Logging;

/// <summary>
/// A file logger with HTTP request informations.
/// </summary>
public class LgServerFileLogger : LgFileLogger
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
    /// <param name="app">The main application</param>
    /// <param name="manager">The file stream manager.</param>
    /// <param name="name">The category name.</param>
    /// <param name="isAppCategory">Indicate if the category come from the current application.</param>
    public LgServerFileLogger(ILgApplication app, LgFileLoggerManager manager, string name, bool isAppCategory)
        : base(manager, name, isAppCategory)
    {
        _app = app;
    }

    #endregion

    #region methods

    /// <summary>
    /// Return the HTTP request context if exists.
    /// </summary>
    /// <returns>The HTTP request context if exists.</returns>
    protected override string GetContext(Exception exception)
    {
        HttpContext context = exception is ContextProxyException ex
            ? ex.Context
            : _app.HttpContextAccessor?.HttpContext;
        if (context is null)
        {
            return null;
        }
        else
        {
            HttpRequest request = context.Request;
            StringBuilder sb = new();
            sb.Append(request.Method);
            sb.Append(' ');
            sb.Append(request.Path);
            sb.Append(request.QueryString);
            if (Manager.Options.ShowUser)
            {
                string user = context.User?.Identity?.Name;
                if (user is not null)
                {
                    sb.Append("\nUser: ");
                    sb.Append(user);
                }
            }
            sb.Append("\nRequestId: ");
            sb.Append(context.GetRequestId());
            return sb.ToString();
        }
    }

    #endregion

}
