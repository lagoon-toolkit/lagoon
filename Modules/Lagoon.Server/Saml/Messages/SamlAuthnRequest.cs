using Lagoon.Server.Saml.Messages.Custom;
using System.Xml;

namespace Lagoon.Server.Saml.Messages;


/// <summary>
/// Requête de demande de connection SAML.
/// </summary>
public class SamlAuthnRequest : SamlComplexMessage
{

    #region fields

    /// <summary>
    /// IdP metadata.
    /// </summary>
    private readonly IdpMetadata _idpMetadata;

    #endregion

    #region properties

    /// <summary>
    /// Application identifier.
    /// </summary>
    /// <value>Application identifier.</value>
    /// <returns>Application identifier.</returns>
    public string SpEntityId { get; set; }

    /// <summary>
    /// Returns the type of identifier to return for the user.
    /// </summary>
    /// <value>The type of identifier to return for the user.</value>
    /// <returns>The type of identifier to return for the user.</returns>
    public string NameIdFormat { get; set; }

    /// <summary>
    /// URL to which to post the response.
    /// </summary>
    /// <value>URL to which to post the response.</value>
    /// <returns>URL to which to post the response.</returns>
    public string AssertionConsumerServiceUrl { get; set; }

    /// <summary>
    /// Contextual state passed by the application at the request org.
    /// </summary>
    /// <value>Contextual state passed by the application to the request org.</value>
    /// <returns>Contextual state passed by the application to the request orgine.</returns>
    public byte[] RelayState { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// Nouvelle instance.
    /// </summary>
    /// <param name="idp">Les métadonnées du fournisseur d'identité.</param>
    /// <param name="sp">Les métadonnées de l'application.</param>
    /// <param name="relayState">Etat contextuel transmis par l'application à l'origine de la requête.</param>
    public SamlAuthnRequest(IdpMetadata idp, SpMetadata sp, byte[] relayState)
    {
        _idpMetadata = idp;
        SpEntityId = sp.EntityId;
        NameIdFormat = sp.NameIdFormat;
        AssertionConsumerServiceUrl = sp.AssertionConsumerLocationUrl;
        RelayState = relayState;
    }

    /// <summary>
    /// Initialization of a new instance of the class.
    /// </summary>
    /// <param name="context">HTTP context.</param>
    public SamlAuthnRequest(HttpContext context)
    {
        switch (context.Request.Method)
        {
            case "GET":
                {
                    Init(context.Request.Query["SAMLRequest"], context.Request.Query["RelayState"]);
                    break;
                }

            case "POST":
                {
                    Init(context.Request.Form["SAMLRequest"], context.Request.Form["RelayState"]);
                    break;
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
    /// <param name="samlRequest">Content of the SAML request.</param>
    /// <param name="relayState">Contextual state passed by the application to the request org.</param>
    public SamlAuthnRequest(string samlRequest, string relayState)
    {
        Init(samlRequest, relayState);
    }

    #endregion

    #region methods

    /// <summary>
    /// Initialization on an authentication request.
    /// </summary>
    /// <param name="samlRequest">Content of the SAML request.</param>
    /// <param name="relayState">Contextual state transmitted by the application to the request org.</param>
    private void Init(string samlRequest, string relayState)
    {
        // We check that the SAML request has been passed
        if (samlRequest == null)
        {
            throw new InvalidOperationException();
        }
        // Loading the SAML message content
        LoadFromXml(FromBase64Zip(samlRequest));
        // Load the report to be relayed
        RelayState = Microsoft.AspNetCore.WebUtilities.Base64UrlTextEncoder.Decode(relayState);
    }

    /// <summary>
    /// Loading the contents of the SAML request.
    /// </summary>
    /// <param name="samlRequest">SAML request content.</param>
    private void LoadFromXml(string samlRequest)
    {
        XmlDocument xml = new();
        // Loading namespaces for XPath use
        XmlNamespaceManager nsm = new(xml.NameTable);
        {
            XmlNamespaceManager withBlock = nsm;
            withBlock.AddNamespace("md", XmlHelper.XML_NAMESPACE_METADATA);
            withBlock.AddNamespace("ds", XmlHelper.XML_NAMESPACE_XMLDSIGN);
            withBlock.AddNamespace("samlp", XmlHelper.XML_NAMESPACE_PROTOCOL);
            withBlock.AddNamespace("samla", XmlHelper.XML_NAMESPACE_ASSERTION);
        }
        // Loading the XML document
        xml.LoadXml(samlRequest);
        OnLoadFromXml(xml, nsm);
    }

    /// <summary>
    /// Loading the information contained in the SAML request.
    /// </summary>
    /// <param name="xml">Contents of the SAML request.</param>
    /// <param name="nsm">Namespaces for XML nodes.</param>
    protected virtual void OnLoadFromXml(XmlDocument xml, XmlNamespaceManager nsm)
    {
        // Retrieve information from the request
        XmlNode node = xml.DocumentElement;
        MessageId = XmlHelper.XmlAttributeValue(node, "ID");
        IssueInstant = UtcStringToDate(XmlHelper.XmlAttributeValue(node, "IssueInstant"));
        // Retrieve the URL of the Sp receiving the response from the IdP
        AssertionConsumerServiceUrl = XmlHelper.XmlAttributeValue(node, "AssertionConsumerServiceURL");
        // Recovering the SP identifier
        node = xml.SelectSingleNode(".//samla:Issuer", nsm);
        SpEntityId = node.InnerText;
        // Retrieving the NameIDPolicy
        node = xml.SelectSingleNode(".//samlp:NameIDPolicy", nsm);
        NameIdFormat = XmlHelper.XmlAttributeValue(node, "Format");
    }

    /// <summary>
    /// Returns the name of the template to use.
    /// </summary>
    /// <returns>The name of the template to use.</returns>
    protected override string GetTemplateName()
    {
        return "SamlAuthnRequest.xml";
    }

    /// <summary>
    /// Initialization of the message template used for message creation.
    /// </summary>
    /// <param name="template">Template content.</param>
    protected override void OnInitTemplate(SamlMessageBuilder template)
    {
        // Id and IssueInstant
        base.OnInitTemplate(template);
        // Issuer
        template.Replace("SpEntityId", SpEntityId);
        // NameIdFormat
        template.Replace("NameIdFormat", NameIdFormat);
    }

    /// <summary>
    /// Method to complete the XML document generated from the template.
    /// </summary>
    /// <param name="xml">SAML message content.</param>
    /// <param name="nsm">Namespaces for XML nodes.</param>
    protected override void OnCompleteXml(XmlDocument xml, XmlNamespaceManager nsm)
    {
        XmlNode node;

        // URL to which to post the answer
        if (AssertionConsumerServiceUrl != null)
        {
            node = xml.DocumentElement;
            XmlHelper.XmlAddAttribute(node, "ProtocolBinding", "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST");
            XmlHelper.XmlAddAttribute(node, "AssertionConsumerServiceURL", AssertionConsumerServiceUrl);
        }
    }

    /// <summary>
    /// Returns the URL with the SAML request and the state to relay as parameters.
    /// </summary>
    /// <returns>The URL with the SAML request in parameter.</returns>
    public string BuildRequestUrl()
    {
        return AddToUrl(_idpMetadata.SingleSignOnLocationRedirect, RelayState);
    }

    /// <summary>
    /// Returns the URL with the SAML request and the state to relay in parameter.
    /// </summary>
    /// <param name="targetURL">Destination URL.</param>
    /// <param name="relayState">Contextual state transmitted by the application at the origin of the request.</param>
    /// <returns>The URL with the SAML request and the state to relay in parameter.</returns>
    private string AddToUrl(string targetURL, byte[] relayState)
    {
        if (string.IsNullOrEmpty(targetURL))
        {
            throw new Exception("SAML auth URL is not found.");
        }
        UriBuilder uri = new(targetURL);
        uri.AddParameter("SAMLRequest", ToBase64Zip());
        if (relayState is not null)
        {
            uri.AddParameter("RelayState", Microsoft.AspNetCore.WebUtilities.Base64UrlTextEncoder.Encode(relayState), false);
        }
        return uri.AbsoluteUri();
    }

}

#endregion
