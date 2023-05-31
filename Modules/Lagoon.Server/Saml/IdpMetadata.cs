using Lagoon.Server.Saml.Messages;

namespace Lagoon.Server.Saml;

/// <summary>
/// SAML identity provider usage information.
/// </summary>
public class IdpMetadata
{

    #region properties

    /// <summary>
    /// IdP identifier.
    /// </summary>
    /// <value>IdP identifier</value>.
    /// <returns>IdP identifier.</returns>
    public string EntityId { get; set; }

    /// <summary>
    /// URL of the IdP for SAML authentication.
    /// </summary>
    /// <value>URL of the IdP for SAML authentication.</value>
    /// <returns>URL of the IdP for SAML authentication.</returns>
    public string SingleSignOnLocationRedirect { get; set; }

    /// <summary>
    /// URL of the IdP to disconnect.
    /// </summary>
    /// <value>URL of the IdP to disconnect.</value>
    /// <returns>URL of the IdP to disconnect.</returns>
    public string SingleLogoutLocation { get; set; }

    /// <summary>
    /// Certificate used to sign XML messages by the IdP.
    /// </summary>
    /// <value>Certificate used to sign XML messages by the IdP.</value>
    /// <returns>Certificate used to sign XML messages by the IdP.</returns>
    public SigningCertCollection SigningCertificates { get; } = new();

    /// <summary>
    /// The list of identifier formats supported by the IdP.
    /// </summary>
    /// <value>The list of identifier formats supported by the IdP.</value>
    /// <returns>The list of identifier formats supported by the IdP.</returns>
    public List<string> NameIdFormats { get; } = new();

    /// <summary>
    /// Date until which the authentication certificate is valid.
    /// </summary>
    /// <value>Date until which the authentication certificate is valid.</value>
    /// <returns>Date until which the authentication certificate is valid.</returns>
    public DateTime ValidUntil => GetValidUntil();

    #endregion

    #region constructors

    /// <summary>
    /// Returns metadata from a file.
    /// </summary>
    /// <param name="fileName">File path.</param>
    /// <returns>Metadata from a file.</returns>
    public static IdpMetadata FromFile(string fileName)
    {
        IdpMetadata metadata = new();
        System.Xml.XmlDocument xml = new();
        xml.Load(fileName);
        metadata.Load(xml);
        return metadata;
    }

    /// <summary>
    /// Returns the metadata from the content of a text string.
    /// </summary>
    /// <param name="p_s">Chaine.</param>
    /// <returns>Metadata from the content of a text string.</returns>
    public static IdpMetadata FromString(string p_s)
    {
        IdpMetadata metadata = new();
        System.Xml.XmlDocument xml = new();
        xml.LoadXml(p_s);
        metadata.Load(xml);
        return metadata;
    }

    #endregion

    #region methods

    /// <summary>
    /// Loading information from the metadata XML file.
    /// </summary>
    private void Load(System.Xml.XmlDocument xml)
    {
        System.Xml.XmlNamespaceManager nsm;
        System.Xml.XmlNode idpNode;

        // Loading namespaces for XPath use
        nsm = new System.Xml.XmlNamespaceManager(xml.NameTable);
        {
            System.Xml.XmlNamespaceManager withBlock = nsm;
            withBlock.AddNamespace("md", XmlHelper.XML_NAMESPACE_METADATA);
            withBlock.AddNamespace("ds", XmlHelper.XML_NAMESPACE_XMLDSIGN);
        }
        // Identity of the IdP
        EntityId = xml.SelectSingleNode("/md:EntityDescriptor", nsm).Attributes["entityID"].Value;
        // Selection of the node describing the IdP configuration
        idpNode = xml.SelectSingleNode("/md:EntityDescriptor/md:IDPSSODescriptor", nsm);
        // Retrieve the URL to use for authentication
        SingleSignOnLocationRedirect = null;
        foreach (System.Xml.XmlNode node in idpNode.SelectNodes("md:SingleSignOnService", nsm))
        {
            if ("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect".Equals(node.Attributes["Binding"].Value, StringComparison.OrdinalIgnoreCase))
            {
                SingleSignOnLocationRedirect = node.Attributes["Location"].Value;
                break;
            }
        }
        // Retrieve the URL to use for logging out
        SingleLogoutLocation = null;
        foreach (System.Xml.XmlNode node in idpNode.SelectNodes("md:SingleLogoutService", nsm))
        {
            if ("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect".Equals(node.Attributes["Binding"].Value, StringComparison.OrdinalIgnoreCase))
            {
                SingleLogoutLocation = node.Attributes["Location"].Value;
                break;
            }
        }
        // Recovery of the certificates used by the server
        SigningCertificates.Clear();
        foreach (System.Xml.XmlNode node in idpNode.SelectNodes("md:KeyDescriptor", nsm))
        {
            if ("signing".Equals(node.Attributes["use"].Value, StringComparison.OrdinalIgnoreCase))
            {
                foreach (System.Xml.XmlNode certNode in node.SelectNodes("ds:KeyInfo/ds:X509Data/ds:X509Certificate", nsm))
                {
                    SigningCertificates.Add(certNode.InnerText);
                }
            }
        }
        // Retrieval of name formats supported by the server
        NameIdFormats.Clear();
        foreach (System.Xml.XmlNode node in idpNode.SelectNodes("md:NameIDFormat", nsm))
        {
            NameIdFormats.Add(node.InnerText);
        }
        // Checking the initialization of values
        if (string.IsNullOrEmpty(SingleSignOnLocationRedirect))
        {
            throw new Exception("\"SingleSignOnLocation\" attribute is not found.");
        }

        if (SigningCertificates.Count == 0)
        {
            throw new Exception("Certificate to sign the SAML messages was not found.");
        }
    }

    /// <summary>
    /// Returns the latest validity date for one of the certificates in the list.
    /// </summary>
    /// <returns>The latest validity date for one of the certificates in the list.</returns>
    private DateTime GetValidUntil()
    {
        DateTime result = DateTime.MinValue;
        foreach (SigningCert cert in SigningCertificates)
        {
            {
                System.Security.Cryptography.X509Certificates.X509Certificate2 withBlock = cert.ToCert();
                if (withBlock.NotAfter > result)
                {
                    result = withBlock.NotAfter;
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Save the metadata in a file.
    /// </summary>
    /// <param name="fileName">Chemin du fichier.</param>
    public void Save(string fileName)
    {
        using (FileStream file = new(fileName, FileMode.Create, FileAccess.Write))
        {
            Save(file);
        }
    }

    /// <summary>
    /// Save the metadata in a file.
    /// </summary>
    /// <param name="stream">Stream to which to write the file.</param>
    public void Save(Stream stream)
    {
        IdpMetadataMessage xml = new(this);
        xml.Save(stream);
    }

    ///<inheritdoc/>
    public override int GetHashCode()
    {
        return 0;
    }

    /// <summary>
    /// Indicates if the metadata are the same.
    /// </summary>
    /// <param name="obj">Second object to compare.</param>
    /// <returns><c>True</c> if the metadata are the same; <c>False</c> otherwise.</returns>
    public override bool Equals(object obj)
    {
        if (obj is not IdpMetadata other
            || !string.Equals(EntityId, other.EntityId)
            || !string.Equals(SingleSignOnLocationRedirect, other.SingleSignOnLocationRedirect)
            || !string.Equals(SingleLogoutLocation, other.SingleLogoutLocation)
            || SigningCertificates.Count != other.SigningCertificates.Count
            || NameIdFormats.Count != other.NameIdFormats.Count)
        {
            return false;
        }
        foreach (SigningCert key in SigningCertificates)
        {
            if (!other.SigningCertificates.Contains(key))
            {
                return false;
            }
        }
        foreach (string nidf in NameIdFormats)
        {
            if (!other.NameIdFormats.Contains(nidf))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns the certificate to be used to sign messages.
    /// </summary>
    /// <returns>The certificate to be used to sign messages.</returns>
    public SigningCert GetSigningCertificate()
    {
        foreach (SigningCert cert in SigningCertificates)
        {
            if (HasValidDate(cert))
            {
                return cert;
            }
        }
        return null;
    }

    /// <summary>
    /// Indicates if the certificate is valid on the date indicated.
    /// </summary>
    /// <param name="cert">Certificate.</param>
    /// <returns><c>True</c> if the certificate is valid on the specified date; <c>False</c> otherwise.</returns>
    internal static bool HasValidDate(SigningCert cert)
    {
        DateTime l_dt_now = DateTime.Now;
        return l_dt_now >= cert.ToCert().NotBefore && l_dt_now <= cert.ToCert().NotAfter;
    }

    /// <summary>
    /// Returns the description of the object's content.
    /// </summary>
    /// <returns>The description of the object's content.</returns>
    public override string ToString()
    {
        StringBuilder l_sb = new();
        l_sb.Append("EntityID: ");
        l_sb.AppendLine(EntityId);
        l_sb.Append("SingleSignOnLocationRedirect: ");
        l_sb.AppendLine(SingleSignOnLocationRedirect);
        l_sb.Append("SingleLogoutLocation: ");
        l_sb.AppendLine(SingleLogoutLocation);
        foreach (SigningCert key in SigningCertificates)
        {
            l_sb.Append("PublicSigningKey: ");
            l_sb.AppendLine(key.ToString());
        }
        foreach (string nidf in NameIdFormats)
        {
            l_sb.Append("NameIdFormat: ");
            l_sb.AppendLine(nidf);
        }
        return l_sb.ToString();
    }

    #endregion

}