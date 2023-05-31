using Lagoon.Server.Saml.Messages.Custom;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Lagoon.Server.Saml;


/// <summary>
/// Response to the SAML connection request.
/// </summary>
public partial class SamlResponse : SamlComplexMessage
{

    #region properties

    /// <summary>
    /// List of attributes to add to the assertion.
    /// </summary>
    /// <value>List of attributes to add to the assertion.</value>
    /// <returns>List of attributes to add to the assertion.</returns>
    public SamlAttributeCollection Attributes { get; } = new();

    /// <summary>
    /// Time the user first authenticated to the IdP.
    /// </summary>
    /// <value>Time at which the user first authenticated on the IdP.</value>
    /// <returns>Time at which the user first authenticated on the IdP.</returns>
    public DateTime AuthnInstant { get; set; }

    /// <summary>
    /// URL to which the browser should redirect the response.
    /// </summary>
    /// <value>URL to which the browser should redirect the response.</value>
    /// <returns>URL to which the browser should redirect the response.</returns>
    public string Destination { get; set; }

    /// <summary>
    /// IdP identifier.
    /// </summary>
    /// <value>Identifier of the IdP.</value>
    /// <returns>Identifier of the IdP.</returns>
    public string IdpEntityId { get; set; }

    /// <summary>
    /// Identifier of the message that made the request.
    /// </summary>
    /// <value>Identifier of the message that made the request.</value>
    /// <returns>Identifier of the message that made the request.</returns>
    public string InResponseTo { get; set; }

    /// <summary>
    /// Identity of the logged in user.
    /// </summary>
    /// <value>Identity of the logged in user.</value>
    /// <returns>Identity of the logged in user.</returns>
    public string NameId { get; set; }

    /// <summary>
    /// The relay state.
    /// </summary>
    public byte[] RelayState { get; set; }

    /// <summary>
    /// Certificate to be used for signing the XML.
    /// </summary>
    /// <value>Certificate to be used for signing the XML.</value>
    /// <returns>Certificate to be used for signing the XML.</returns>
    public X509Certificate2 SigningCert { get; set; }

    /// <summary>
    /// SP ID.
    /// </summary>
    /// <value>Identifier of SP.</value>
    /// <returns>Identifier of SP.</returns>
    public string SpEntityId { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// Load the response from the HTTP context.
    /// </summary>
    /// <param name="idpMetadata">The expected IdP metadata.</param>
    /// <param name="spEntityId">The application SP entity Id.</param>
    /// <param name="request">The HTTP request.</param>
    /// <returns>The loaded SAML response.</returns>
    public static SamlResponse FromHttpRequest(IdpMetadata idpMetadata, string spEntityId, HttpRequest request)
    {
        switch (request.Method)
        {
            case "GET":
                {
                    return new(idpMetadata, spEntityId, request.Query["SAMLResponse"], request.Query["RelayState"]);
                }
            case "POST":
                {
                    return new(idpMetadata, spEntityId, request.Form["SAMLResponse"], request.Form["RelayState"]);
                }
            default:
                {
                    throw new InvalidOperationException();
                }
        }
    }

    /// <summary>
    /// Initialization of a new instance of the class.
    /// </summary>
    /// <param name="idpMetadata">Identity provider metadata.</param>
    /// <param name="spEntityId">Application identifier.</param>
    /// <param name="samlResponse">SAML response.</param>
    /// <param name="relayState">The relay state.</param>
    public SamlResponse(IdpMetadata idpMetadata, string spEntityId, string samlResponse, string relayState)
    {
        Load(idpMetadata, spEntityId, samlResponse);
        RelayState = Microsoft.AspNetCore.WebUtilities.Base64UrlTextEncoder.Decode(relayState);
    }

    /// <summary>
    /// Initialization of a new instance of the class.
    /// </summary>
    /// <param name="idpMetadata">IdP metadata.</param>
    /// <param name="spMetadata">"Service Provider" metadata.</param>
    /// <param name="inResponseTo">Identifier of the message that made the request.</param>
    /// <param name="p_dt_authnInstant">Time at which the user first authenticated on
    /// the IdP.</param>
    /// <param name="nameId">Identity of the logged in user.</param>
    public SamlResponse(IdpMetadata idpMetadata, SpMetadata spMetadata, string inResponseTo, DateTime p_dt_authnInstant, string nameId)
    {
        IdpEntityId = idpMetadata.EntityId;
        SigningCert = idpMetadata.GetSigningCertificate().ToCert();
        SpEntityId = spMetadata.EntityId;
        Destination = spMetadata.AssertionConsumerLocationUrl;
        AuthnInstant = p_dt_authnInstant;
        InResponseTo = inResponseTo;
        NameId = nameId;
    }

    #endregion

    #region methods - read response

    /// <summary>
    /// Loading the response following an authentication request.
    /// </summary>
    /// <param name="idpMetadata"></param>
    /// <param name="spEntityId"></param>
    /// <param name="samlReponse"></param>
    public void Load(IdpMetadata idpMetadata, string spEntityId, string samlReponse)
    {
        XmlNamespaceManager nsm;
        XmlDocument xml = new();
        XmlNode assertion;
        XmlNode node;
        SignedXml signed;
        X509Certificate2 cert;
        string cert64;
        bool validAudience = false;
        string audiance;

        // We decode the SAML request
        if (string.IsNullOrEmpty(samlReponse))
        {
            throw new Exception("\"SAMLResponse\" value is not found.");
        }

        samlReponse = FromBase64Zip(samlReponse);
        // Loading the XML document
        xml.LoadXml(samlReponse);
        // Loading namespaces for XPath use
        nsm = new XmlNamespaceManager(xml.NameTable);
        nsm.AddNamespace("samla", XmlHelper.XML_NAMESPACE_ASSERTION);
        nsm.AddNamespace("ds", XmlHelper.XML_NAMESPACE_XMLDSIGN);
        // Recovery of the assertion
        assertion = xml.SelectSingleNode(".//samla:Assertion", nsm);
        // Name retrieval
        node = assertion.SelectSingleNode(".//samla:NameID", nsm);
        if (node is null)
        {
            throw new Exception("\"NameID\" node not found in SAML Response.");
        }
        NameId = node.InnerText;
        // Attribute retrieval
        Attributes.LoadFromXmlNode(assertion.SelectSingleNode(".//samla:AttributeStatement", nsm), nsm);
        // We check that the URL of the site is the destination URL of the assertion
        foreach (XmlNode currentnode in assertion.SelectNodes(".//samla:Conditions/samla:AudienceRestriction/samla:Audience", nsm))
        {
            node = currentnode;
            audiance = node.InnerText;
            if (string.IsNullOrEmpty(audiance))
            {
                continue;
            }
            // Azure prefixes the SP identifier with "spn:".
            if (audiance.StartsWith("spn:", StringComparison.OrdinalIgnoreCase))
            {
                audiance = audiance[4..];
            }
            // We see if the target is the right one
            if (spEntityId.Equals(audiance, StringComparison.OrdinalIgnoreCase))
            {
                validAudience = true;
                break;
            }
        }
        if (!validAudience)
        {
            throw new Exception("The assertion was not issued for this service.");
        }
        // We check the validity date of the assertion
        foreach (XmlNode currentnode1 in assertion.SelectNodes(".//samla:Conditions[@NotOnOrAfter]", nsm))
        {
            node = currentnode1;
            DateTime date = UtcStringToDate(node.Attributes["NotOnOrAfter"].Value);
            if (!(DateTime.Now.ToUniversalTime() < date))
            {
                throw new Exception("The answer is out of date (NotOnOrAfter).");
            }
        }
        // Loading the signature
        node = assertion.SelectSingleNode(".//ds:Signature", nsm);
        if (node is null)
        {
            throw new Exception("Signature was not found in the SAML response.");
        }
        // Check that it is a known certificate
        cert64 = Saml.SigningCert.CleanupBase64(node.SelectSingleNode("ds:KeyInfo/ds:X509Data/ds:X509Certificate", nsm).InnerText);
        if (!idpMetadata.SigningCertificates.Contains(cert64))
        {
            throw new Exception("Signature was not found in the SAML response.");
        }
        // Signature verification
        signed = new SignedXml(xml.DocumentElement);
        signed.LoadXml((XmlElement)node);
        cert = new X509Certificate2(Convert.FromBase64String(cert64));
        if (!signed.CheckSignature(cert, true))
        {
            throw new Exception("Signature used in the SAML response is invalid.");
        }
    }

    #endregion

    #region methods - build response

    /// <summary>
    /// Returns the name of the template to use.
    /// </summary>
    /// <returns>The name of the template to use.</returns>
    protected override string GetTemplateName()
    {
        return "SamlResponse.xml";
    }

    /// <summary>
    /// Initialization of the message template used for message creation.
    /// </summary>
    /// <param name="template">template content.</param>
    protected override void OnInitTemplate(SamlMessageBuilder template)
    {
        // Id et IssueInstant
        base.OnInitTemplate(template);
        // others
        template.Replace("IssueInstant+5m", DateToUtcString(IssueInstant.AddMinutes(5)));
        template.Replace("IssueInstant+1h", DateToUtcString(IssueInstant.AddHours(1)));
        template.Replace("AssertionId", GetNewMessageId());
        template.Replace("AuthnInstant", DateToUtcString(AuthnInstant));
        template.Replace("Destination", Destination);
        template.Replace("InResponseTo", InResponseTo);
        template.Replace("IdpEntityId", IdpEntityId);
        template.Replace("SpEntityId", SpEntityId);
        template.Replace("NameId", NameId);
    }

    /// <summary>
    /// Method to complete the XML document generated from the template.
    /// </summary>
    /// <param name="xml">SAML message content.</param>
    /// <param name="nsm">Namespaces for XML nodes.</param>
    protected override void OnCompleteXml(XmlDocument xml, XmlNamespaceManager nsm)
    {
        XmlNode assertionNode;

        // We add the signature on the assertion
        assertionNode = xml.SelectSingleNode(".//samla:Assertion", nsm);
        // We add the attributes to the assertion
        if (Attributes.Count > 0)
        {
            AddAttributes(assertionNode);
        }
        // We sign the node containing the assertion using the certificate
        SignXmlNode(assertionNode, SigningCert);
    }

    /// <summary>
    /// Adds the attributes to the assertion.
    /// </summary>
    /// <param name="node">Node of the assertion.</param>
    private void AddAttributes(XmlNode node)
    {
        XmlNode stmt = XmlHelper.XmlAddNode(node, "AttributeStatement");
        foreach (SamlAttribute attr in Attributes)
        {
            XmlNode nodeAttribute = XmlHelper.XmlAddNode(stmt, "Attribute", "Name", attr.Name, "NameFormat", "urn:oasis:names:tc:SAML:2.0:attrname-format:basic");
            XmlHelper.XmlAddNode(nodeAttribute, "AttributeValue", "xsi:type", "xs:string", attr.Value);
        }
    }

    /// <summary>
    /// We sign an XML node using a certificate.
    /// </summary>
    /// <param name="node">XML node.</param>
    /// <param name="cert">Certificate.</param>
    private void SignXmlNode(XmlNode node, X509Certificate2 cert)
    {
        SignedXml signed = new((XmlElement)node);
        Reference reference = new();
        KeyInfo ki = new();
        XmlElement SignatureNode;

        // Certificat pour générer la signature
        signed.SigningKey = cert.GetRSAPrivateKey();
        // Référence
        reference.Uri = "#" + node.Attributes["ID"].Value;
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        signed.AddReference(reference);
        // Certificat pour la clé publique
        ki.AddClause(new KeyInfoX509Data(cert));
        signed.KeyInfo = ki;
        // On génère la signature
        signed.ComputeSignature();
        SignatureNode = signed.GetXml();
        // On ajoute la signature dans le noeud
        node.AppendChild(SignatureNode);
    }

    /// <summary>
    /// Write the SAML response to the HTTP response.
    /// </summary>
    /// <param name="httpContext">HTTP context.</param>
    /// <param name="relayState">Contextual state transmitted by the application at the origin of the request.</param>
    public void SendAsHttpPost(HttpContext httpContext, string relayState = null)
    {
        SamlMessageBuilder msgb = new("SamlResponse.html");
        msgb.Replace("{CallbackUrl}", System.Web.HttpUtility.HtmlEncode(Destination));
        msgb.Replace("{SAMLResponse}", ToBase64Zip());
        msgb.Replace("{RelayState}", System.Web.HttpUtility.HtmlEncode(relayState));
        httpContext.Response.Body.Write(msgb.ToArray());
    }

    #endregion

}
