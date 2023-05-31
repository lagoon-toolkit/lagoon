using Lagoon.Core.Application;
using Lagoon.Core.Application.Logging;

namespace Lagoon.Server.Application.Logging;

/// <summary>
/// Console logger provider that show only warning and errors except for the current application messages.
/// </summary>
internal class LgConsoleLoggerProvider : LgLoggerProvider<LgConsoleLogger>
{

    #region fields

    /// <summary>
    /// The main application.
    /// </summary>
    private readonly ILgApplication _app;

    /// <summary>
    /// Indicate if the queries must be traced.
    /// </summary>
    private readonly bool _showQueries;

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="app">The Main class.</param>
    public LgConsoleLoggerProvider(ILgApplication app)
        : base(app as LgApplicationBase)
    {
        _app = app;
        _showQueries = !"false".Equals(_app.Configuration["Lagoon:LightConsoleLogger:ShowQueries"], StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region methods

    /// <summary>
    /// Create a console logger that show only warning and errors except for the current application messages.
    /// </summary>
    /// <param name="categoryName">The category name.</param>
    /// <param name="isAppCategory">Indicate if the category come from the current application.</param>
    /// <returns>The new logger.</returns>
    protected override LgConsoleLogger CreateNewLogger(string categoryName, bool isAppCategory)
    {
        bool alwaysTrace = _showQueries && !isAppCategory && categoryName == "Microsoft.EntityFrameworkCore.Database.Command";
        return new LgConsoleLogger(_app, categoryName, isAppCategory, alwaysTrace);
    }

    ///<inheritdoc/>
    protected override bool IsNullCategory(string category)
    {
        return base.IsNullCategory(category) || LgLoggerHelper.IsUnhandleExceptionCategory(category);
    }

    #endregion

}
