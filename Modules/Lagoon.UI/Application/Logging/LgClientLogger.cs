using Lagoon.Core.Application.Logging;

namespace Lagoon.UI.Application.Logging;

/// <summary>
/// Represents a type used to perform logging.
/// </summary>
public class LgClientLogger : LgLogger
{

    #region fields

    /// <summary>
    /// The file stream manager.
    /// </summary>
    private LgClientLoggerManager _manager;

    #endregion

    #region constructor

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="manager">The fie stream.</param>
    /// <param name="name">The category name.</param>
    /// <param name="isAppCategory">Indicate if the category come from the current application.</param>
    internal LgClientLogger(LgClientLoggerManager manager, string name, bool isAppCategory)
        : base(name, isAppCategory)
    {
        _manager = manager;
    }

    #endregion

    #region ILogger implementation

    ///<inheritdoc/>
    public override bool IsEnabled(LogLevel logLevel)
    {
        //Catch only Error or critical errors
        return logLevel >= LogLevel.Error;
    }

    ///<inheritdoc/>
    protected override bool IsExceptionExcluded(Exception exception)
    {
        return base.IsExceptionExcluded(exception) || exception is LgHttpFetchException;
    }

    ///<inheritdoc/>
    protected override void Log(LogLevel logLevel, DateTime time, string message, string stackTrace, string category, bool isAppCategory, LogSide side, Exception _)
    {
        _manager.LogAsync(logLevel, time, message, stackTrace, category, isAppCategory);
    }

    #endregion



}
