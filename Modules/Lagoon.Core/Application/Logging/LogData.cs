namespace Lagoon.Core.Application.Logging;

/// <summary>
/// Describe an exception which will be send to server
/// </summary>
[JsonConverter(typeof(LogDataConverter<LogData>))]
public class LogData
{

    #region properties

    /// <summary>
    /// The category of the message.
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// The context of the message.
    /// </summary>
    public string Context { get; set; }

    /// <summary>
    /// Indicate if the event category is from the application root namespace.
    /// </summary>
    /// <remarks>It's <c>true</c> when the event is raised from "Trace..." method of the main app, or when a custom logger is used.</remarks>
    public bool IsAppCategory  { get; set; }

    /// <summary>
    /// Exception level
    /// </summary>
    public LogLevel LogLevel { get; set; }

    /// <summary>
    /// Exception message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Exception side (Server or CLient)
    /// </summary>
    public virtual LogSide Side { get; set; }

    /// <summary>
    /// Exception stack trace
    /// </summary>
    public string StackTrace { get; set; }

    /// <summary>
    /// Exception date
    /// </summary>
    /// <value></value>
    public DateTime Time { get; set; }

    #endregion

}
