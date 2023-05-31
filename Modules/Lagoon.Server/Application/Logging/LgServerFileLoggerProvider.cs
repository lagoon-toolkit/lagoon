using Lagoon.Core.Application.Logging;

namespace Lagoon.Server.Application.Logging;

/// <summary>
/// File logger with HTTP context informations.
/// </summary>
public class LgServerFileLoggerProvider : LgFileLoggerProvider
{

    #region fields

    /// <summary>
    /// The main application.
    /// </summary>
    private readonly ILgApplication _app;

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="app">The main application</param>
    /// <param name="options">Logger options.</param>
    public LgServerFileLoggerProvider(ILgApplication app, LgFileLoggerOptions options)
        : base(app, options)
    {
        _app = app;
    }

    #endregion

    #region methods

    /// <summary>
    /// Create a console logger that show only warning and errors except for the current application messages.
    /// </summary>
    /// <param name="categoryName">The category name.</param>
    /// <param name="isAppCategory">Indicate if the category come from the current application.</param>
    /// <returns>The new logger.</returns>
    protected override LgFileLogger CreateNewLogger(string categoryName, bool isAppCategory)
    {
        return new LgServerFileLogger(_app, Manager, categoryName, isAppCategory);
    }

    ///<inheritdoc/>
    protected override bool IsNullCategory(string category)
    {
        return base.IsNullCategory(category) || LgLoggerHelper.IsUnhandleExceptionCategory(category);
    }

    #endregion

}
