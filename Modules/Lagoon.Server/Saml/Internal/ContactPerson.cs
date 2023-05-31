namespace Lagoon.Server.Saml.Internal;


/// <summary>
/// Contact pour le SP.
/// </summary>
internal class ContactPerson
{

    /// <summary>
    /// Type of contact.
    /// </summary>
    /// <value>Type of contact.</value>
    /// <returns>Type of contact.</returns>
    public string Type { get; set; }

    /// <summary>
    /// Contact name.
    /// </summary>
    /// <value>Contact name.</value>
    /// <returns>Contact name.</returns>
    public string GivenName { get; set; }

    /// <summary>
    /// E-mail address.
    /// </summary>
    /// <value>E-mail address.</value>
    /// <returns>E-mail address.</returns>
    public string EmailAddress { get; set; }

}
