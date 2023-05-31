using Lagoon.Server.Saml.Internal;
using Lagoon.Server.Saml.Messages.Custom;
using System.Net;
using System.Xml;

namespace Lagoon.Server.Saml.Messages;

internal class SpMetatadataMessage : SamlComplexMessage
{

    private SpMetadata _metadata;

    public SpMetatadataMessage(SpMetadata metadata)
        : base(false, false, false)
    {
        _metadata = metadata;
    }

    protected override string GetTemplateName()
    {
        return "SamlMetadataSP.xml";
    }

    protected override void OnCompleteXml(XmlDocument xml, XmlNamespaceManager nsm)
    {
        // Identité du SP
        XmlNode entityNode = xml.SelectSingleNode("/md:EntityDescriptor", nsm);
        XmlHelper.XmlAddAttribute(entityNode, "entityID", _metadata.EntityId);
        // SP descriptor
        XmlNode spNode = xml.SelectSingleNode("/md:EntityDescriptor/md:SPSSODescriptor", nsm);
        // NameIdFormat
        XmlHelper.XmlAddNode(spNode, "md:NameIDFormat", _metadata.NameIdFormat);
        // URL pour la déconnexion
        if (!string.IsNullOrEmpty(_metadata.SingleLogoutLocation))
        {
            XmlHelper.XmlAddNode(spNode, "md:SingleLogoutService", "Binding", "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect", "Location", _metadata.SingleLogoutLocation);
        }
        // URL pour la connexion
        if (!string.IsNullOrEmpty(_metadata.AssertionConsumerLocationUrl))
        {
            XmlHelper.XmlAddNode(spNode, "md:AssertionConsumerService", "Binding", "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST", "Location", _metadata.AssertionConsumerLocationUrl, "index", "0", "isDefault", "true");
        }
        // Organisation
        Organization org = _metadata.Organization;
        if (org is not null)
        {
            XmlNode node = XmlHelper.XmlAddNode(entityNode, "md:Organization");
            XmlHelper.XmlAddNode(node, "md:OrganizationName", "xml:lang", org.Language).InnerText = org.Name;
            XmlHelper.XmlAddNode(node, "md:OrganizationDisplayName", "xml:lang", org.Language).InnerText = org.DisplayName;
            XmlHelper.XmlAddNode(node, "md:OrganizationURL", "xml:lang", org.Language).InnerText = org.URL;
        }
        // Contacts
        if (_metadata.Contacts is not null)
        {
            foreach (ContactPerson contact in _metadata.Contacts)
            {
                XmlNode node = XmlHelper.XmlAddNode(entityNode, "md:ContactPerson", "contactType", contact.Type);
                XmlHelper.XmlAddNode(node, "md:GivenName").InnerText = contact.GivenName;
                XmlHelper.XmlAddNode(node, "md:EmailAddress").InnerText = contact.EmailAddress;
            }
        }
    }

    public void WriteTo(HttpResponse response)
    {
        response.StatusCode = (int)HttpStatusCode.OK;
        response.Headers.Add("Content-Type", "application/samlmetadata+xml");
        response.Headers.Add("Content-Disposition", "filename=SpMetadata.xml");
        response.Body.Write(ToUTF8Bytes());
    }

}
