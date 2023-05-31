namespace Lagoon.Core.Application.Logging;

/// <summary>
/// An abstract class to handle application logs.
/// </summary>
public abstract class LgLogger : ILogger
{
    #region fields

    /// <summary>
    /// Indicate if the category come from the current application.
    /// </summary>
    private bool IsApplicationCategory { get; }

    /// <summary>
    /// The category name.
    /// </summary>
    private string CategoryName { get; }

    #endregion

    #region constructor

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="name">The category name.</param>
    /// <param name="isAppCategory">Indicate if the category come from the current application.</param>
    public LgLogger(string name, bool isAppCategory)
    {
        CategoryName = name;
        IsApplicationCategory = isAppCategory;
    }

    #endregion

    #region ILogger implementation

    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    /// <summary>
    /// Check if we need to log the exception according to appSetting configuration
    /// </summary>
    /// <param name="logLevel">The log lovel to check.</param>
    /// <returns>True if log enable, false otherwise</returns>
    public abstract bool IsEnabled(LogLevel logLevel);


    /// <summary>
    /// If trace is enable, write the exception in a file
    /// </summary>
    /// <param name="logLevel">Error type (Error, Warning, Trace, ...)</param>
    /// <param name="eventId">Optional EventId</param>
    /// <param name="state">Additional messages on the exception</param>
    /// <param name="exception">Exception to trace</param>
    /// <param name="formatter">Function to format the exception</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        //  Abort if the trace is disabled or if the exception is an UserException
        if (!IsEnabled(logLevel) || IsExceptionExcluded(exception))
        {
            return;
        }
        if (exception is LgClientApplicationException cex)
        {
            ClientLogData clientError = cex.Error;
            Log(logLevel,
                clientError.Time,
                clientError.Message,
                clientError.StackTrace,
                clientError.Category,
                clientError.IsAppCategory,
                LogSide.Client,
                exception);
        }
        else
        {
            Log(logLevel,
                DateTime.Now,
                CategoryName is null && exception is not null ? exception?.Message : formatter(state, exception),
                Lagoon.Helpers.Tools.GetLogStackTrace(logLevel, exception),
                CategoryName,
                IsApplicationCategory,
                LogSide.Server,
                exception);
        }
    }

    /// <summary>
    /// Indicate if the exception must be excluded from the trace.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns><c>true</c>if the exception must be excluded from the trace; <c>false</c> otherwise.</returns>
    protected virtual bool IsExceptionExcluded(Exception exception)
    {
        return exception is UserException;
    }

    /// <summary>
    /// Log the event details.
    /// </summary>
    /// <param name="logLevel">The level of the event.</param>
    /// <param name="time">The time of the event.</param>
    /// <param name="message">The message to show.</param>
    /// <param name="stackTrace">The stack trace associated to the message.</param>
    /// <param name="category">The category name.</param>
    /// <param name="isAppCategory">Indicate if the category come from the current application.</param>
    /// <param name="side">The application source of the event.</param>
    /// <param name="exception">The orginal exception.</param>
    protected abstract void Log(LogLevel logLevel, DateTime time, string message, string stackTrace, string category, bool isAppCategory, LogSide side, Exception exception);

    #endregion

}
