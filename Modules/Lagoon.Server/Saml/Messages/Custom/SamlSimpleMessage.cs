namespace Lagoon.Server.Saml.Messages.Custom;

/// <summary>
/// Common basis for creating and reading SAML messages.
/// </summary>
public abstract class SamlSimpleMessage
{

    #region properties

    /// <summary>
    /// Identifier of the request.
    /// </summary>
    /// <value>Identifier of the request.</value>
    /// <returns>Identifier of the request.</returns>
    public string MessageId { get; set; }

    /// <summary>
    /// Date of creation of the SAML request.
    /// </summary>
    /// <value>Date of creation of the SAML request.</value>
    /// <returns>Date of creation of the SAML request.</returns>
    public DateTime IssueInstant { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Returns the name of the template to use.
    /// </summary>
    /// <returns>The name of the template to use.</returns>
    protected abstract string GetTemplateName();

    /// <summary>
    /// Returns the SAML request in XML format.
    /// </summary>
    /// <returns>The SAML request in XML format.</returns>
    public virtual string ToXml()
    {
        SamlMessageBuilder builder = new(GetTemplateName());
        // We initialize the variables for a new message
        OnCreateNewMessage();
        // Initialization of the message template used for message creation
        OnInitTemplate(builder);
        // Return the message with the replaced tags
        return builder.ToString();
    }

    /// <summary>
    /// Initialization of a new request.
    /// </summary>
    protected virtual void OnCreateNewMessage()
    {
        MessageId = GetNewMessageId();
        IssueInstant = DateTime.Now;
    }

    /// <summary>
    /// Initialization of the message template used for message creation.
    /// </summary>
    /// <param name="template">Contenu du modèle.</param>
    protected virtual void OnInitTemplate(SamlMessageBuilder template)
    {
        {
            var withBlock = template;
            withBlock.Replace("ID", MessageId);
            withBlock.Replace("IssueInstant", DateToUtcString(IssueInstant));
        }
    }

    /// <summary>
    /// Returns the compressed message formatted in Base64.
    /// </summary>
    /// <returns>The compressed message formatted in Base64.</returns>
    public string ToBase64Zip()
    {
        return ToBase64Zip(ToXml());
    }

    /// <summary>
    /// Return a byte array with XML encoded in UTF8.
    /// </summary>
    /// <returns></returns>
    protected byte[] ToUTF8Bytes()
    {
        return System.Text.Encoding.UTF8.GetBytes(ToXml());
    }

    /// <summary>
    /// Returns a new random identifier.
    /// </summary>
    /// <returns>Un nouvel identifiant aléatoire.</returns>
    public static string GetNewMessageId()
    {
        return $"_{Guid.NewGuid():D}";
    }

    #endregion

    #region tools

    /// <summary>
    /// Returns the date in UTC format.
    /// </summary>
    /// <param name="p_dt">Date.</param>
    /// <returns>The date in UTC format.</returns>
    protected static string DateToUtcString(DateTime p_dt)
    {
        return System.Xml.XmlConvert.ToString(p_dt, System.Xml.XmlDateTimeSerializationMode.Utc);
    }

    /// <summary>
    /// Returns the date from a date in UTC format.
    /// </summary>
    /// <param name="date">Date in UTC format.</param>
    /// <returns>The date.</returns>
    protected static DateTime UtcStringToDate(string date)
    {
        if (string.IsNullOrEmpty(date))
            return default;
        return System.Xml.XmlConvert.ToDateTime(date, System.Xml.XmlDateTimeSerializationMode.Utc);
    }

    /// <summary>
    /// Encodes the SAML request to transmit it in a URL.
    /// </summary>
    /// <returns>Encodes the SAML request to pass it in a URL.</returns>
    protected static string ToBase64Zip(string samlRequest)
    {
        byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(samlRequest);
        return Convert.ToBase64String(Compress(utf8));
    }

    /// <summary>
    /// Decodes the SAML request received from the server.
    /// </summary>
    /// <param name="samlResponse">SAML response.</param>
    /// <returns>Decodes the SAML request received from the server.</returns>
    protected static string FromBase64Zip(string samlResponse)
    {
        byte[] zipped = Convert.FromBase64String(samlResponse);
        // Check if response is not compressed
        if (ContainsSamlSignature(zipped))
            return System.Text.Encoding.UTF8.GetString(zipped);
        // Uncompress response
        return System.Text.Encoding.UTF8.GetString(Decompress(zipped));
    }

    /// <summary>
    /// Compress a byte array to a new byte array.
    /// </summary>
    /// <param name="bytes">The decompressed byte array.</param>
    /// <returns>The compressed byte array.</returns>
    internal static byte[] Compress(byte[] bytes)
    {
        using (System.IO.MemoryStream buffer = new())
        {
            using (System.IO.Compression.DeflateStream compressor = new(buffer, System.IO.Compression.CompressionMode.Compress))
            {
                compressor.Write(bytes, 0, bytes.Length);
            }
            return buffer.ToArray();
        }
    }
    /// <summary>
    /// Decompress a byte array to a new byte array.
    /// </summary>
    /// <param name="bytes">The compressed byte array.</param>
    /// <returns>The decompressed byte array.</returns>
    internal static byte[] Decompress(byte[] bytes)
    {
        using (System.IO.MemoryStream buffer = new())
        {
            using (System.IO.MemoryStream zippedStream = new(bytes))
            {
                using (System.IO.Compression.DeflateStream decompressor = new(zippedStream, System.IO.Compression.CompressionMode.Decompress))
                {
                    decompressor.CopyTo(buffer);
                }
            }
            return buffer.ToArray();
        }
    }

    /// <summary>
    /// Indicates if the byte array contains the string ":SAML:" which should be in the namespace of the first
    /// XML element.
    /// </summary>
    /// <param name="p_ab_samlResponse">SAML response.</param>
    /// <returns>
    /// <c>true</c> if the byte array contains the string ":SAML:" which should be in the namespace
    /// of the first XML element; <c>false</c> otherwise.
    /// </returns>
    private static bool ContainsSamlSignature(byte[] p_ab_samlResponse)
    {
        byte[] signature = System.Text.Encoding.UTF8.GetBytes(":SAML:");
        bool found = false;
        for (int start = 0; start <= p_ab_samlResponse.Length - signature.Length; start++)
        {
            for (int l_i = 0; l_i <= signature.Length - 1; l_i++)
            {
                if (signature[l_i] == p_ab_samlResponse[start + l_i])
                    found = true;
                else
                {
                    found = false;
                    break;
                }
            }
            if (found)
                return true;
        }
        return false;
    }

    #endregion
}