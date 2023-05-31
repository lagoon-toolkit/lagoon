using Lagoon.Core.Helpers;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;

namespace Lagoon.Server.Controllers;

/// <summary>
/// Base controller with error handling.
/// </summary>
[Authorize()]
public class LgControllerBase : ControllerBase
{

    #region fields

    private Guid? _userId;

    #endregion

    #region properties

    /// <summary>
    /// Gets the current user ID.
    /// </summary>
    public Guid ContextUserId
    {
        get
        {
            if (!_userId.HasValue)
            {
                _userId = HttpContext.GetUserId();
            }
            return _userId.Value;
        }
    }

    #endregion

    #region Trace management 

    /// <summary>
    /// Trace the underlying exception and return an error object to the client
    /// </summary>
    /// <param name="ex">Exception</param>
    /// <returns>ErrorResponse</returns>
    protected static ObjectResult Problem(Exception ex)
    {
        ExceptionDispatchInfo.Capture(ex).Throw();
        return null;
    }

    /// <summary>
    /// Return an error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <returns>Error response</returns>
    [Obsolete("Use throw new UserException(...).")]
    protected static ObjectResult Problem(string message)
    {
//            HttpContext.Response.StatusCode = 500;
        throw new UserException(message);
    }

    #endregion

    #region File management

    /// <summary>
    /// Send a file to client
    /// </summary>
    /// <param name="filePath">File path</param>
    /// <param name="attachment"><c>true</c> if the browser should download the file, <c>false</c> if the browser should open the file in a window.</param>
    /// <returns>Stream ready to be sent</returns>
    protected ActionResult<FileStream> FileContent(string filePath, bool attachment = true)
    {
        string clientFileName = Path.GetFileName(filePath);
        return FileContent(filePath, Tools.ExtrapolateContentType(clientFileName), clientFileName, attachment);
    }

    /// <summary>
    /// Send a file to client
    /// </summary>
    /// <param name="filePath">File path</param>
    /// <param name="fileNameOrContentType">Filename used on client side</param>
    /// <param name="attachment"><c>true</c> if the browser should download the file, <c>false</c> if the browser should open the file in a window.</param>
    /// <returns>Stream ready to be sent</returns>
    protected ActionResult<FileStream> FileContent(string filePath, string fileNameOrContentType, bool attachment = true)
    {
        string clientFileName = Tools.DetectFileNameOrContentType(ref fileNameOrContentType);
        return FileContent(filePath, fileNameOrContentType, clientFileName, attachment);
    }

    /// <summary>
    /// Send a file to client
    /// </summary>
    /// <param name="filePath">File path</param>
    /// <param name="contentType">Type mime to use when sending response</param>
    /// <param name="clientFileName">Filename used on client side</param>
    /// <param name="attachment"><c>true</c> if the browser should download the file, <c>false</c> if the browser should open the file in a window.</param>
    /// <returns>Stream ready to be sent</returns>
    protected ActionResult<FileStream> FileContent(string filePath, string contentType, string clientFileName, bool attachment = true)
    {
        FileStream stream = new(filePath, FileMode.Open, FileAccess.Read);
        return FileContent(stream, contentType, clientFileName, attachment);
    }

    /// <summary>
    /// Send a file to client
    /// </summary>
    /// <param name="content">File content</param>
    /// <param name="fileNameOrContentType">The filename suggested to the client side OR the mime type to use when sending response.</param>
    /// <param name="attachment"><c>true</c> if the browser should download the file, <c>false</c> if the browser should open the file in a window.</param>
    /// <returns>Stream ready to be sent</returns>
    protected ActionResult<FileStream> FileContent(byte[] content, string fileNameOrContentType, bool attachment = true)
    {
        string clientFileName = Tools.DetectFileNameOrContentType(ref fileNameOrContentType);
        return FileContent(content, fileNameOrContentType, clientFileName, attachment);
    }

    /// <summary>
    /// Send a file to client
    /// </summary>
    /// <param name="content">File content</param>
    /// <param name="contentType">Type mime to use when sending response</param>
    /// <param name="clientFileName">Filename used on client side</param>
    /// <param name="attachment"><c>true</c> if the browser should download the file, <c>false</c> if the browser should open the file in a window.</param>
    /// <returns>Stream ready to be sent</returns>
    protected ActionResult<FileStream> FileContent(byte[] content, string contentType, string clientFileName, bool attachment = true)
    {
        AddContentDispositionHeader(attachment, clientFileName);
        return File(content, contentType, false);
    }

    /// <summary>
    /// Send a file to client
    /// </summary>
    /// <param name="contents">File content</param>
    /// <param name="encoding">The encoding to use to save the contents.</param>
    /// <param name="fileNameOrContentType">The filename suggested to the client side OR the mime type to use when sending response.</param>
    /// <param name="attachment"><c>true</c> if the browser should download the file, <c>false</c> if the browser should open the file in a window.</param>
    /// <returns>Stream ready to be sent</returns>
    protected ActionResult<FileStream> FileContent(string contents, System.Text.Encoding encoding, string fileNameOrContentType, bool attachment = true)
    {
        string clientFileName = Tools.DetectFileNameOrContentType(ref fileNameOrContentType);
        return FileContent(contents, encoding, fileNameOrContentType, clientFileName, attachment);
    }

    /// <summary>
    /// Send a file to client
    /// </summary>
    /// <param name="contents">File content</param>
    /// <param name="encoding">The encoding to use to save the contents.</param>
    /// <param name="contentType">Type mime to use when sending response</param>
    /// <param name="clientFileName">Filename used on client side</param>
    /// <param name="attachment"><c>true</c> if the browser should download the file, <c>false</c> if the browser should open the file in a window.</param>
    /// <returns>Stream ready to be sent</returns>
    protected ActionResult<FileStream> FileContent(string contents, System.Text.Encoding encoding, string contentType, string clientFileName, bool attachment = true)
    {
        AddContentDispositionHeader(attachment, clientFileName);
        byte[] bom = encoding.GetPreamble();
        byte[] text = encoding.GetBytes(contents);
        if (bom is null || bom.Length == 0)
        {
            return File(text, contentType);
        }
        else
        {
            return File(new ConcatenatedArrayStream(bom, text), contentType, false);
        }
    }

    /// <summary>
    /// Send a file to the client.
    /// </summary>
    /// <param name="fileStream">File stream. Disposed after sending file content</param>
    /// <param name="fileNameOrContentType">The filename suggested to the client side OR the mime type to use when sending response.</param>
    /// <param name="attachment"><c>true</c> if the browser should download the file, <c>false</c> if the browser should open the file in a window.</param>
    /// <returns>Stream ready to be sent</returns>
    /// <remarks>The mime type is extrapolated from the filename if it's not defined.</remarks>
    protected ActionResult<FileStream> FileContent(Stream fileStream, string fileNameOrContentType, bool attachment = true)
    {
        string clientFileName = Tools.DetectFileNameOrContentType(ref fileNameOrContentType);
        return FileContent(fileStream, fileNameOrContentType, clientFileName, attachment);
    }

    /// <summary>
    /// Send a file to client
    /// </summary>
    /// <param name="fileStream">File stream. Disposed after sending file content</param>
    /// <param name="contentType">Type mime to use when sending response</param>
    /// <param name="clientFileName">Filename used on client side</param>
    /// <param name="attachment"><c>true</c> if the browser should download the file, <c>false</c> if the browser should open the file in a window.</param>
    /// <returns>Stream ready to be sent</returns>
    protected ActionResult<FileStream> FileContent(Stream fileStream, string contentType, string clientFileName, bool attachment = true)
    {
        AddContentDispositionHeader(attachment, clientFileName);
        return File(fileStream, contentType, false);
    }

    /// <summary>
    /// Send a file to client.
    /// OBSOLETE : Use the <see cref="FileContent(string, string, bool)"/> method that invert the parameter order.
    /// </summary>
    /// <param name="filePath">File path</param>
    /// <param name="attachment"><c>true</c> if the browser should download the file, <c>false</c> if the browser should open the file in a window.</param>
    /// <param name="clientFileName">Filename used on client side</param>
    /// <returns>Stream ready to be sent</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Move the \"attachment\" parameter as the last parameter. WARNING the meaning of \"attachment\" has been fixed !")] //15/07/2022
    protected ActionResult<FileStream> FileContent(string filePath, bool attachment, string clientFileName)
    {
        return FileContent(filePath, Tools.ExtrapolateContentType(clientFileName), clientFileName, attachment);
    }

    /// <summary>
    /// Add a content disposition header to the response
    /// </summary>
    /// <param name="clientFileName">Downloaded file name</param>
    /// <param name="attachment"><c>true</c> if the browser should download the file, <c>false</c> if the browser should open the file in a window.</param>
    private void AddContentDispositionHeader(bool attachment, string clientFileName)
    {
        // FileNameStar supports UTF8 characters https://stackoverflow.com/a/68587104/3568845
        ContentDispositionHeaderValue contentDisposition = new(attachment ? "attachment" : "inline")
        {
            FileNameStar = clientFileName
        };
        Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
    }

    #endregion

}
