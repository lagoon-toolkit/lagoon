namespace Lagoon.Core.Application.Logging;


/// <summary>
/// Source of a log entry.
/// </summary>
public enum LogSide
{
    /// <summary>
    /// The entry has been created on the server.
    /// </summary>
    Server,
    /// <summary>
    /// The entry has been created in the web browser.
    /// </summary>
    Client
}
