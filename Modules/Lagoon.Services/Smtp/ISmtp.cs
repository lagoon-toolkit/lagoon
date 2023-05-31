namespace Lagoon.Services;


/// <summary>
/// Interface used by email sender implementation
/// </summary>
public interface ISmtp
{
    /// <summary>
    /// Default sender mail address.
    /// </summary>
    string DefaultSender { get; }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// The sender (from) will be retrieved from appsettings.json
    /// </summary>
    /// <param name="to">Recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list that contains file paths</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string to, string subject, string htmlMessage, IEnumerable<string> files = null)
    {
        return SendEmailAsync(DefaultSender, to, subject, htmlMessage, files);
    }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// The sender (from) will be retrieved from appsettings.json
    /// </summary>
    /// <param name="to">Recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list with filename/filecontent</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string to, string subject, string htmlMessage, IEnumerable<Tuple<string, byte[]>> files = null)
    {
        return SendEmailAsync(DefaultSender, to, subject, htmlMessage, files);
    }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// The sender (from) will be retrieved from appsettings.json
    /// </summary>
    /// <param name="to">Recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list with filename/filecontent</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string to, string subject, string htmlMessage, IEnumerable<Tuple<string, Stream>> files = null)
    {
        return SendEmailAsync(DefaultSender, to, subject, htmlMessage, files);
    }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// The sender (from) will be retrieved from appsettings.json
    /// </summary>
    /// <param name="to">Recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string to, string subject, string htmlMessage)
    {
        return SendEmailAsync(DefaultSender, to, subject, htmlMessage, (IEnumerable<string>)null);
    }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// The sender (from) will be retrieved from appsettings.json
    /// </summary>
    /// <param name="to">List of recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of files path</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(IEnumerable<string> to, string subject, string htmlMessage, IEnumerable<string> files = null)
    {
        return SendEmailAsync(DefaultSender, to, subject, htmlMessage, files);
    }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// The sender (from) will be retrieved from appsettings.json
    /// </summary>
    /// <param name="to">List of recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of filename/filecontent</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(IEnumerable<string> to, string subject, string htmlMessage, IEnumerable<Tuple<string, byte[]>> files = null)
    {
        return SendEmailAsync(DefaultSender, to, subject, htmlMessage, files);
    }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// The sender (from) will be retrieved from appsettings.json
    /// </summary>
    /// <param name="to">List of recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of filename/filecontent</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(IEnumerable<string> to, string subject, string htmlMessage, IEnumerable<Tuple<string, Stream>> files = null)
    {
        return SendEmailAsync(DefaultSender, to, subject, htmlMessage, files);
    }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// The sender (from) will be retrieved from appsettings.json
    /// </summary>
    /// <param name="to">Recipient address list</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(IEnumerable<string> to, string subject, string htmlMessage)
    {
        return SendEmailAsync(DefaultSender, to, subject, htmlMessage, (IEnumerable<string>)null);
    }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// The sender (from) will be retrieved from appsettings.json
    /// </summary>
    /// <param name="to">List of recipient address</param>
    /// <param name="cc">List of cc recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of file path</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(IEnumerable<string> to, IEnumerable<string> cc, string subject, string htmlMessage, IEnumerable<string> files = null)
    {
        return SendEmailAsync(DefaultSender, to, cc, subject, htmlMessage, files);
    }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// The sender (from) will be retrieved from appsettings.json
    /// </summary>
    /// <param name="to">List of recipient address</param>
    /// <param name="cc">List of cc recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of filename/filecontent</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(IEnumerable<string> to, IEnumerable<string> cc, string subject, string htmlMessage, IEnumerable<Tuple<string, byte[]>> files = null)
    {
        return SendEmailAsync(DefaultSender, to, cc, subject, htmlMessage, files);
    }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// The sender (from) will be retrieved from appsettings.json
    /// </summary>
    /// <param name="to">List of recipient address</param>
    /// <param name="cc">List of cc recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of filename/filecontent</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(IEnumerable<string> to, IEnumerable<string> cc, string subject, string htmlMessage, IEnumerable<Tuple<string, Stream>> files = null)
    {
        return SendEmailAsync(DefaultSender, to, cc, subject, htmlMessage, files);
    }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// The sender (from) will be retrieved from appsettings.json
    /// </summary>
    /// <param name="to">List of recipient address</param>
    /// <param name="cc">List of cc recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(IEnumerable<string> to, IEnumerable<string> cc, string subject, string htmlMessage)
    {
        return SendEmailAsync(DefaultSender, to, cc, subject, htmlMessage, (IEnumerable<string>) null);
    }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// </summary>
    /// <param name="from">Sender address</param>
    /// <param name="to">Recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of file path</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string from, string to, string subject, string htmlMessage, IEnumerable<string> files = null);

    /// <summary>
    /// Send an email to a recipient with html support.
    /// </summary>
    /// <param name="from">Sender address</param>
    /// <param name="to">Recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of filename/filecontent</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string from, string to, string subject, string htmlMessage, IEnumerable<Tuple<string, byte[]>> files = null);

    /// <summary>
    /// Send an email to a recipient with html support.
    /// </summary>
    /// <param name="from">Sender address</param>
    /// <param name="to">Recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of filename/filecontent</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string from, string to, string subject, string htmlMessage, IEnumerable<Tuple<string, Stream>> files = null);

    /// <summary>
    /// Send an email to a recipient with html support.
    /// </summary>
    /// <param name="from">Sender address</param>
    /// <param name="to">Recipient address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string from, string to, string subject, string htmlMessage)
    {
        return SendEmailAsync(from, to, subject, htmlMessage, (IEnumerable<string>)null);
    }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// </summary>
    /// <param name="from">Sender address</param>
    /// <param name="to">List of recipients address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of file path</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string from, IEnumerable<string> to, string subject, string htmlMessage, IEnumerable<string> files = null);

    /// <summary>
    /// Send an email to a recipient with html support.
    /// </summary>
    /// <param name="from">Sender address</param>
    /// <param name="to">List of recipients address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of filename/filecontent</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string from, IEnumerable<string> to, string subject, string htmlMessage, IEnumerable<Tuple<string, byte[]>> files = null);

    /// <summary>
    /// Send an email to a recipient with html support.
    /// </summary>
    /// <param name="from">Sender address</param>
    /// <param name="to">List of recipients address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of filename/filecontent</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string from, IEnumerable<string> to, string subject, string htmlMessage, IEnumerable<Tuple<string, Stream>> files = null);

    /// <summary>
    /// Send an email to a recipient with html support.
    /// </summary>
    /// <param name="from">Sender address</param>
    /// <param name="to">List of recipients address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string from, IEnumerable<string> to, string subject, string htmlMessage)
    {
        return SendEmailAsync(from, to, subject, htmlMessage, (IEnumerable<string>)null);
    }

    /// <summary>
    /// Send an email to a recipient with html support.
    /// </summary>
    /// <param name="from">Sender address</param>
    /// <param name="to">List of recipients address</param>
    /// <param name="cc">List of cc address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of file path</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string from, IEnumerable<string> to, IEnumerable<string> cc, string subject, string htmlMessage, IEnumerable<string> files = null);

    /// <summary>
    /// Send an email to a recipient with html support.
    /// </summary>
    /// <param name="from">Sender address</param>
    /// <param name="to">List of recipients address</param>
    /// <param name="cc">List of cc address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of filename/filecontent</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string from, IEnumerable<string> to, IEnumerable<string> cc, string subject, string htmlMessage, IEnumerable<Tuple<string, byte[]>> files = null);

    /// <summary>
    /// Send an email to a recipient with html support.
    /// </summary>
    /// <param name="from">Sender address</param>
    /// <param name="to">List of recipients address</param>
    /// <param name="cc">List of cc address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <param name="files">Attachments : list of filename/filecontent</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string from, IEnumerable<string> to, IEnumerable<string> cc, string subject, string htmlMessage, IEnumerable<Tuple<string, Stream>> files = null);

    /// <summary>
    /// Send an email to a recipient with html support.
    /// </summary>
    /// <param name="from">Sender address</param>
    /// <param name="to">List of recipients address</param>
    /// <param name="cc">List of cc address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">Email body content with html support</param>
    /// <returns>An asynchronous task context.</returns>
    Task SendEmailAsync(string from, IEnumerable<string> to, IEnumerable<string> cc, string subject, string htmlMessage)
    {
        return SendEmailAsync(from, to, cc, subject, htmlMessage, (IEnumerable<string>)null);
    }

}
