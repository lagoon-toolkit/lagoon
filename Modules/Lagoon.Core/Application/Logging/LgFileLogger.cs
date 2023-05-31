namespace Lagoon.Core.Application.Logging;

/// <summary>
/// Server-side logger. Log all exception (except UserException) to a file
/// </summary>
public class LgFileLogger : LgLogger
{
    #region properties

    /// <summary>
    /// The file stream manager.
    /// </summary>
    protected LgFileLoggerManager Manager { get; }

    #endregion

    #region constructor

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="manager">The file stream manager.</param>
    /// <param name="name">The category name.</param>
    /// <param name="isAppCategory">Indicate if the category come from the current application.</param>
    public LgFileLogger(LgFileLoggerManager manager, string name, bool isAppCategory)
        : base(name, isAppCategory)
    {
        Manager = manager;
    }

    #endregion

    #region ILogger implementation

    ///<inheritdoc/>
    public override bool IsEnabled(LogLevel _)
    {
        return true;
    }

    ///<inheritdoc/>
    protected override void Log(LogLevel logLevel, DateTime time, string message, string stackTrace, string category, bool isAppCategory, LogSide side, Exception exception)
    {
        Manager.Log(logLevel, time, message, stackTrace, category, isAppCategory, side, GetContext(exception));
    }

    /// <summary>
    /// Return the current context informations.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetContext(Exception exception)
    {
        return null;
    }

    #endregion

}
