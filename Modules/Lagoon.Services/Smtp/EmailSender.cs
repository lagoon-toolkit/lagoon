using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Lagoon.Services;


/// <summary>
/// ISmtp implementation
/// </summary>
public class EmailSender : ISmtp
{

    #region fields

    /// <summary>
    /// Smtp address
    /// </summary>
    private readonly string _host;

    /// <summary>
    /// Smtp port
    /// </summary>
    private readonly int _port;

    /// <summary>
    /// Use secure connexion to access smtp server
    /// </summary>
    private readonly bool _enableSSL;

    /// <summary>
    /// Username if smtp secured
    /// </summary>
    private readonly string _userName;

    /// <summary>
    /// User password for smtp secure
    /// </summary>
    private readonly string _password;

    #endregion

    #region properties

    /// <summary>
    /// Default sender mail address.
    /// </summary>
    public string DefaultSender { get; }

    #endregion

    #region initialization

    /// <summary>
    /// Initialize a new email smtp
    /// </summary>
    /// <param name="host">Smtp address</param>
    /// <param name="port">Smtp port</param>
    /// <param name="enableSSL">Use secure connexion to access smtp server</param>
    /// <param name="userName">Username if smtp secured</param>
    /// <param name="password">User password for smtp secure</param>
    /// <param name="defaultSender">A default address used if no sender specified</param>
    public EmailSender(string host, int port, bool enableSSL, string userName, string password, string defaultSender)
    {
        _host = host;
        _port = port;
        _enableSSL = enableSSL;
        _userName = userName;
        _password = password;
        DefaultSender = defaultSender;
    }

    #endregion

    #region ISmtp implementation

    /// <inheritdoc />
    public Task SendEmailAsync(string from, string to, string subject, string htmlMessage, IEnumerable<string> files = null)
    {
        return InternalSendEmailAsync(from, new string[] { to }, null, subject, htmlMessage, (attachmentCollection) =>
        {
            if (files is not null)
            {
                foreach (string file in files)
                {
                    attachmentCollection.Add(file);
                }
            }
        });
    }

    /// <inheritdoc />
    public Task SendEmailAsync(string from, string to, string subject, string htmlMessage, IEnumerable<Tuple<string, byte[]>> files = null)
    {
        return InternalSendEmailAsync(from, new string[] { to }, null, subject, htmlMessage, (attachmentCollection) =>
        {
            if (files is not null)
            {
                foreach (Tuple<string, byte[]> file in files)
                {
                    attachmentCollection.Add(file.Item1, file.Item2);
                }
            }
        });
    }

    /// <inheritdoc />
    public Task SendEmailAsync(string from, string to, string subject, string htmlMessage, IEnumerable<Tuple<string, Stream>> files = null)
    {
        return InternalSendEmailAsync(from, new string[] { to }, null, subject, htmlMessage, (attachmentCollection) =>
        {
            if (files is not null)
            {
                foreach (Tuple<string, Stream> file in files)
                {
                    attachmentCollection.Add(file.Item1, file.Item2);
                }
            }
        });
    }

    /// <inheritdoc />
    public Task SendEmailAsync(string from, IEnumerable<string> to, string subject, string htmlMessage, IEnumerable<string> files = null)
    {
        return InternalSendEmailAsync(from, to, null, subject, htmlMessage, (attachmentCollection) =>
        {
            if (files is not null)
            {
                foreach (string file in files)
                {
                    attachmentCollection.Add(file);
                }
            }
        });
    }

    /// <inheritdoc />
    public Task SendEmailAsync(string from, IEnumerable<string> to, string subject, string htmlMessage, IEnumerable<Tuple<string, byte[]>> files = null)
    {
        return InternalSendEmailAsync(from, to, null, subject, htmlMessage, (attachmentCollection) =>
        {
            if (files is not null)
            {
                foreach (Tuple<string, byte[]> file in files)
                {
                    attachmentCollection.Add(file.Item1, file.Item2);
                }
            }
        });
    }

    /// <inheritdoc />
    public Task SendEmailAsync(string from, IEnumerable<string> to, string subject, string htmlMessage, IEnumerable<Tuple<string, Stream>> files = null)
    {
        return InternalSendEmailAsync(from, to, null, subject, htmlMessage, (attachmentCollection) =>
        {
            if (files is not null)
            {
                foreach (Tuple<string, Stream> file in files)
                {
                    attachmentCollection.Add(file.Item1, file.Item2);
                }
            }
        });
    }

    /// <inheritdoc />
    public Task SendEmailAsync(string from, IEnumerable<string> to, IEnumerable<string> cc, string subject, string htmlMessage, IEnumerable<string> files = null)
    {
        return InternalSendEmailAsync(from, to, cc, subject, htmlMessage, (attachmentCollection) =>
        {
            if (files is not null)
            {
                foreach (string file in files)
                {
                    attachmentCollection.Add(file);
                }
            }
        });
    }

    /// <inheritdoc />
    public Task SendEmailAsync(string from, IEnumerable<string> to, IEnumerable<string> cc, string subject, string htmlMessage, IEnumerable<Tuple<string, byte[]>> files = null)
    {
        return InternalSendEmailAsync(from, to, cc, subject, htmlMessage, (attachmentCollection) =>
        {
            if (files is not null)
            {
                foreach (Tuple<string, byte[]> file in files)
                {
                    attachmentCollection.Add(file.Item1, file.Item2);
                }
            }
        });
    }

    /// <inheritdoc />
    public Task SendEmailAsync(string from, IEnumerable<string> to, IEnumerable<string> cc, string subject, string htmlMessage, IEnumerable<Tuple<string, Stream>> files = null)
    {
        return InternalSendEmailAsync(from, to, cc, subject, htmlMessage, (attachmentCollection) =>
        {
            if (files is not null)
            {
                foreach (Tuple<string, Stream> file in files)
                {
                    attachmentCollection.Add(file.Item1, file.Item2);
                }
            }
        });
    }

    #endregion

    #region Internal SendMail

    /// <summary>
    /// Send email with MailKit
    /// </summary>
    /// <param name="from">The sender email</param>
    /// <param name="to">The 'To' recipients</param>
    /// <param name="cc">The 'Cc' recipients</param>
    /// <param name="subject">The mail subject</param>
    /// <param name="htmlMessage">The mail body</param>
    /// <param name="onAttachFiles">Action used to add files attachments</param>
    /// <returns>An asynchronous task context.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="from"/> and <paramref name="to"/> parameters must not be empty</exception>
    private async Task InternalSendEmailAsync(string from, IEnumerable<string> to, IEnumerable<string> cc, string subject, string htmlMessage, Action<AttachmentCollection> onAttachFiles)
    {
        if (string.IsNullOrEmpty(from))
        {
            throw new ArgumentNullException(nameof(from));
        }
        if (to == null || !to.Any())
        {
            throw new ArgumentNullException(nameof(to));
        }
        using (MimeMessage message = new())
        {
            // Add recipients
            message.From.Add(MailboxAddress.Parse(from));
            foreach (string toAdd in to)
            {
                message.To.Add(MailboxAddress.Parse(toAdd));
            }
            if (cc is not null)
            {
                foreach (string ccAdd in cc)
                {
                    message.Cc.Add(MailboxAddress.Parse(ccAdd));
                }
            }
            // Add body
            message.Subject = subject;
            BodyBuilder builder = new()
            {
                HtmlBody = htmlMessage
            };
            // Delegate files attachments
            onAttachFiles(builder.Attachments);
            // Build & Send message
            message.Body = builder.ToMessageBody();
            using (SmtpClient smtp = new())
            {
                await smtp.ConnectAsync(_host, _port, _enableSSL ? SecureSocketOptions.Auto : SecureSocketOptions.None);
                if (_enableSSL)
                {
                    await smtp.AuthenticateAsync(_userName, _password);
                }
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
        }
    }

    #endregion

}
