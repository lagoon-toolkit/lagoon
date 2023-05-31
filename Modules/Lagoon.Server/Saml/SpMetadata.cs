using Lagoon.Server.Saml.Internal;
using Lagoon.Server.Saml.Messages;

namespace Lagoon.Server.Saml;


/// <summary>
/// Information d'utilisation du fournisseur d'identité SAML.
/// </summary>
public class SpMetadata
{

    #region properties

    /// <summary>
    /// URL of the application receiving the response to an authentication request (POST).
    /// </summary>
    /// <value>URL of the application receiving the response to an authentication request (POST).</value>
    /// <returns>URL of the application receiving the response to an authentication request (POST).</returns>
    public string AssertionConsumerLocationUrl { get; set; }

    /// <summary>
    /// The contacts.
    /// </summary>
    internal List<ContactPerson> Contacts { get; private set; }

    /// <summary>
    /// SP identifier.
    /// </summary>
    /// <value>SP identifier.</value>
    /// <returns>SP identifier.</returns>
    public string EntityId { get; set; }

    /// <summary>
    /// Identifier format expected by the SP.
    /// </summary>
    /// <value>Identifier format expected by the SP.</value>
    /// <returns>Identifier format expected by the SP.</returns>
    public string NameIdFormat { get; set; } = Saml.NameIdFormat.Unspecified;

    /// <summary>
    /// The organization.
    /// </summary>
    internal Organization Organization { get; private set; }

    /// <summary>
    /// URL of the application to process the disconnection of the SP (GET).
    /// </summary>
    /// <value>URL of the application to process the disconnection of the SP (GET).</value>
    /// <returns>URL of the application to process the disconnection of the SP (GET).</returns>
    public string SingleLogoutLocation { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="entityId">SP identifier.</param>
    /// <param name="nameIdFormat"></param>
    /// <param name="assertionConsumerLocationUrl"></param>
    public SpMetadata(string entityId, string nameIdFormat, string assertionConsumerLocationUrl)
    {
        EntityId = entityId;
        NameIdFormat = nameIdFormat;
        AssertionConsumerLocationUrl = assertionConsumerLocationUrl;
    }

    /// <summary>
    /// New instance.
    /// </summary>
    public SpMetadata()
    { }

    #endregion

    #region methods

    /// <summary>
    /// Save the metadata in a file.
    /// </summary>
    /// <param name="fileName">File path.</param>
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
        SpMetatadataMessage message = new(this);
        message.Save(stream);
    }

    /// <summary>
    /// Defines the company for the SP.
    /// </summary>
    /// <param name="name">Name of the organization.</param>
    /// <param name="displayName">Name displayed.</param>
    /// <param name="url">URL for the organization.</param>
    /// <param name="language">Language.</param>
    public void SetOrganisation(string name, string displayName, string url = null, string language = "en-US")
    {
        Organization = new Organization()
        {
            Name = name,
            DisplayName = displayName,
            URL = url,
            Language = language
        };
    }

    /// <summary>
    /// Adds a person to the contact list.
    /// </summary>
    /// <param name="type">Type of contact. ("technical", "support", ...).</param>
    /// <param name="givenName">Contact name.</param>
    /// <param name="emailAddress">E-mail address.</param>
    public void AddContactPerson(string type, string givenName, string emailAddress)
    {
        Contacts ??= new();
        Contacts.Add(new ContactPerson() { Type = type, GivenName = givenName, EmailAddress = emailAddress });
    }

    #endregion

}
