using Lagoon.Server.Saml.Messages.Custom;
using System.Xml;

namespace Lagoon.Server.Saml.Messages;

internal class IdpMetadataMessage : SamlComplexMessage
{

    private IdpMetadata _metadata;

    public IdpMetadataMessage(IdpMetadata metadata)
        : base(false, false)
    {
        _metadata = metadata;
    }

    protected override string GetTemplateName()
    {
        return "SamlMetadataIdP.xml";
    }

    protected override void OnCompleteXml(XmlDocument xml, XmlNamespaceManager nsm)
    {
        // Identity of the IdP
        XmlNode entityNode = xml.SelectSingleNode("/md:EntityDescriptor", nsm);
        XmlHelper.XmlAddAttribute(entityNode, "entityID", _metadata.EntityId);
        XmlHelper.XmlAddAttribute(entityNode, "validUntil", XmlConvert.ToString(_metadata.ValidUntil, XmlDateTimeSerializationMode.Utc));
        // IdP descriptor
        XmlNode idpNode = xml.SelectSingleNode("/md:EntityDescriptor/md:IDPSSODescriptor", nsm);
        // "signin" Certificates
        foreach (SigningCert key in _metadata.SigningCertificates)
        {
            // We quit if we are on the date of the certificate is not valid
            if (!IdpMetadata.HasValidDate(key))
            {
                continue;
            }
            // On ajoute le certificat
            XmlNode node = XmlHelper.XmlAddNode(idpNode, "md:KeyDescriptor", "use", "signing");
            node = XmlHelper.XmlAddNode(XmlHelper.XML_NAMESPACE_XMLDSIGN, node, "ds:KeyInfo");
            node = XmlHelper.XmlAddNode(node, "ds:X509Data");
            node = XmlHelper.XmlAddNode(node, "ds:X509Certificate");
            node.InnerText = key.ToBase64(76);
        }
        // NameIdFormat
        foreach (string nidf in _metadata.NameIdFormats)
        {
            XmlHelper.XmlAddNode(idpNode, "md:NameIDFormat", nidf);
        }
        // URL for the connection
        if (!string.IsNullOrEmpty(_metadata.SingleSignOnLocationRedirect))
        {
            XmlHelper.XmlAddNode(idpNode, "md:SingleSignOnService", "Binding", "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect", "Location", _metadata.SingleSignOnLocationRedirect);
        }
        // URL for logging out
        if (!string.IsNullOrEmpty(_metadata.SingleLogoutLocation))
        {
            XmlHelper.XmlAddNode(idpNode, "md:SingleLogoutService", "Binding", "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect", "Location", _metadata.SingleLogoutLocation);
        }
    }

}
