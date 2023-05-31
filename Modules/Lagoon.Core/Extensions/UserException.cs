namespace Lagoon.Helpers;

/// <summary>
/// An exception which not be traced
/// </summary>
public class UserException : Exception
{

    /// <summary>
    /// New empty exception
    /// </summary>
    public UserException()
    { }

    /// <summary>
    /// Initialize a new user exception
    /// </summary>
    /// <param name="message">Message to display</param>
    public UserException(string message) : base(message.CheckTranslate())
    { }

    /// <summary>
    /// Initialize a new user exception
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <param name="innerException">Inner exception</param>
    public UserException(string message, Exception innerException) : base(message.CheckTranslate(), innerException)
    { }

}
