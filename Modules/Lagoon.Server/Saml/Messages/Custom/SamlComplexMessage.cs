using System.Xml;

namespace Lagoon.Server.Saml.Messages.Custom;


/// <summary>
/// Adds XML document customization to the common base for creating and reading SAML messages.
/// </summary>
public abstract class SamlComplexMessage : SamlSimpleMessage
{

    private bool _dsign;
    private bool _samlp;
    private bool _samla;

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="samlp">Add XML SAML protocol namespace.</param>
    /// <param name="samla">Add XML SAML protocol namespace.</param>
    /// <param name="dsign">Add XML SAML DSign.</param>
    public SamlComplexMessage(bool samlp = true, bool samla = true, bool dsign = true)
    {
        _samlp = samlp;
        _samla = samla;
        _dsign = dsign;
    }

    /// <summary>
    /// Returns the SAML request in XML format.
    /// </summary>
    /// <returns>The SAML request in XML format.</returns>
    public override string ToXml()
    {
        XmlWriterSettings ws = new()
        {
            NewLineChars = "\n",
            Indent = true
        };
        StringBuilder sb = new();
        using (XmlWriter writer = XmlWriter.Create(sb, ws))
        {
            GetLoadedDocument().WriteTo(writer);
        }
        // Return the text in XML format
        return sb.ToString();
    }

    /// <summary>
    /// Save the XML document to the stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    internal void Save(Stream stream)
    {
        stream.Write(System.Text.Encoding.UTF8.GetBytes(ToXml()));
    }

    /// <summary>
    /// Get the XML document.
    /// </summary>
    /// <returns>The XML document.</returns>
    private XmlDocument GetLoadedDocument()
    {
        // We load the XML document
        XmlDocument xml = new();
        xml.LoadXml(base.ToXml());
        // Loading namespaces for XPath use
        XmlNamespaceManager nsm = new(xml.NameTable);
        {
            if (_samlp)
            {
                nsm.AddNamespace("samlp", XmlHelper.XML_NAMESPACE_PROTOCOL);
            }
            if (_samla)
            {
                nsm.AddNamespace("samla", XmlHelper.XML_NAMESPACE_ASSERTION);
            }
            if (_dsign)
            {
                nsm.AddNamespace("ds", XmlHelper.XML_NAMESPACE_XMLDSIGN);
            }
            nsm.AddNamespace("md", XmlHelper.XML_NAMESPACE_METADATA);
        }
        // We complete the XML document
        OnCompleteXml(xml, nsm);
        return xml;
    }

    /// <summary>
    /// Method to complete the XML document generated from the model.
    /// </summary>
    /// <param name="xml">Content of the SAML message.</param>
    /// <param name="nsm"></param>
    protected abstract void OnCompleteXml(System.Xml.XmlDocument xml, System.Xml.XmlNamespaceManager nsm);

}
