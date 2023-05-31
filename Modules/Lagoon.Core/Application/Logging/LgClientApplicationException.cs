namespace Lagoon.Core.Application.Logging;

/// <summary>
/// A trace of a Client Side exception.
/// </summary>
public class LgClientApplicationException : Exception
{

    /// <summary>
    /// The client error.
    /// </summary>
    public ClientLogData Error { get; }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="error">The client error.</param>
    public LgClientApplicationException(ClientLogData error)
    {
        Error = error;
    }

}
