namespace Lagoon.UI.Components;

/// <summary>
/// Exception raised after a server side exception.
/// </summary>
/// <remarks>This exception type inherits from UserException and will never be logged.</remarks>
public class LgRemoteException : UserException
{

    /// <summary>
    /// The stack trace is not availlable for this kind of exception.
    /// </summary>
    public override string StackTrace => null;

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="message">Exception description.</param>
    public LgRemoteException(string message)
        : base(message)
    { }

}
