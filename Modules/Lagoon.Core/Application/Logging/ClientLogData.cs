namespace Lagoon.Core.Application.Logging;

/// <summary>
/// Describe an exception which will be send to server
/// </summary>
public class ClientLogData : LogData
{

    /// <summary>
    /// The number of times the exception has been raised.
    /// </summary>
    public int Count { get; set; } = 1;

    /// <summary>
    /// Exception side : Always CLient.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public override LogSide Side
    {
        get => LogSide.Client;
        set { }
    }

    /// <summary>
    /// Return <c>true</c> if the two instance represents the same exception source.
    /// </summary>
    /// <param name="other">The oher exception.</param>
    /// <returns><c>true</c> if the two instance represents the same exception source</returns>
    public bool HasTheSameSource(ClientLogData other)
    {
        return
            LogLevel == other.LogLevel &&
            StackTrace == other.StackTrace;
    }

}
