namespace Lagoon.Core.Exceptions;


/// <summary>
/// Special exception used to display errors in the authentication workflow
/// </summary>
public class AuthenticationException : UserException
{

    /// <summary>
    /// Initialize a new authentication exception message
    /// </summary>
    /// <param name="message">Exception message</param>
    public AuthenticationException(string message) : base(message)
    {

    }

    /// <summary>
    /// Initialize a new authentication exception message with it's inner exception
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="inner">Inner exception</param>
    public AuthenticationException(string message, Exception inner) : base(message, inner)
    {

    }

}
