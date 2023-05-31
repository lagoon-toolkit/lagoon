namespace Lagoon.UI.Application.Logging;

/// <summary>
/// Exception handling options.
/// </summary>
public class LgClientLoggerOptions
{
    #region Logger configuration

    /// <summary>
    /// Delay (in seconds) between two attempts to send local errors to the server.
    /// Default value : 30 minutes.
    /// </summary>
    public int SyncErrorDelay { get; set; } = 1800;

    /// <summary>
    /// If this counter is reach, a message will warn the user.
    /// Default value : 10 times.
    /// </summary>
    public int MaxResendErrorAttempts { get; set; } = 10;

    /// <summary>
    /// Max number of errors stored on the client side.
    /// Default value : 200.
    /// </summary>
    public int MaxLocalErrorsCount { get; set; } = 200;

    #endregion
}
