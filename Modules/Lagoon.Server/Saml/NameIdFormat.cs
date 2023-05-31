namespace Lagoon.Server.Saml;


/// <summary>
/// NameId constants.
/// </summary>
public static class NameIdFormat
{
    /// <summary>
    /// The subject NameID value from the identity provider can be any format.
    /// </summary>
    public const string Unspecified = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified";

    /// <summary>
    /// The subject NameID value from the identity provider uses the email address format.
    /// </summary>
    public const string EmailAddress = "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress";

    /// <summary>
    /// The subject NameID is an attribute that is generated randomly for temporary use. The service provider accepts this value as temporary. 
    /// </summary>
    public const string Transient = "urn:oasis:names:tc:SAML:2.0:nameid-format:transient";

    /// <summary>
    /// The subject NameID is a randomly generated unique identifier that retains the same value for that application federation. 
    /// </summary>
    public const string Persistent = "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent";

}
