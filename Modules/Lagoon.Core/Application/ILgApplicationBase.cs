namespace Lagoon.Core.Application;

/// <summary>
/// Interface to the main lagoon application..
/// </summary>
public interface ILgApplicationBase
{

    #region properties

    /// <summary>
    /// Get the current application informations.
    /// </summary>
    IApplicationInformation ApplicationInformation { get; }

    /// <summary>
    /// The configuration settings for the application.
    /// </summary>
    IConfiguration Configuration { get; }

    /// <summary>
    /// Service provider
    /// </summary>
    IServiceScopeFactory ServiceScopeFactory { get; }

    #endregion

    #region methods

    /// <summary>
    /// Get a generic message exception if we aren't in debug mode and it's not an exception for the end user.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <param name="includeOriginalMessage">Indicate if the original message must be included in DEBUG.</param>
    /// <returns>The genreic message.</returns>
    string GetContactAdminMessage(Exception ex, bool includeOriginalMessage);

    /// <summary>
    /// Set the current application culture.
    /// </summary>
    /// <param name="culture">The new culture</param>
    void SetCulture(CultureInfo culture);

    /// <summary>
    /// Formats and writes an informational log message for the application.
    /// </summary>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    void TraceInfo(string message, params object[] args);

    /// <summary>
    /// Formats and writes a warning log message for the application.
    /// </summary>
    /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    void TraceWarning(string message, params object[] args);

    /// <summary>
    /// Formats and writes an error log message for the application.
    /// </summary>
    /// <param name="ex">The exception to trace.</param>
    void TraceException(Exception ex);

    #endregion

}
