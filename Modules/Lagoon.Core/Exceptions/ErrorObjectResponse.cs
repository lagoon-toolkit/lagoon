using Lagoon.Core.Application;

namespace Lagoon.Core.Exceptions;


/// <summary>
/// Object used to describe a exception which has not been cached and send to the client
/// </summary>
public class ErrorObjectResponse
{

    #region properties

    /// <summary>
    /// The exception message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// The request Id.
    /// </summary>
    public string RequestId { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    [Obsolete("This constructor is only for deserialization. Use the other one.", true)]
    public ErrorObjectResponse()
    { }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="app">The main application.</param>
    /// <param name="ex">The exception.</param>
    /// <param name="requestId">The request id.</param>
    public ErrorObjectResponse(ILgApplicationBase app, Exception ex, string requestId)
    {
        // Get the generic contact admin message if it's as UserException
        Message = app.GetContactAdminMessage(ex, true);
        RequestId = requestId;
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ErrorObjectResponse(string message)
    {
        // Get the generic contact admin message if it's as UserException
        Message = message;
    }

    #endregion

}
