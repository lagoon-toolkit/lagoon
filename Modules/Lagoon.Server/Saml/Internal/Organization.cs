namespace Lagoon.Server.Saml.Internal;


/// <summary>
/// Information sur le client.
/// </summary>
internal class Organization
{

    /// <summary>
    /// Name of the organization.
    /// </summary>
    /// <value>Name of the organization.</value>
    /// <returns>Name of the organization.</returns>
    public string Name { get; set; }

    /// <summary>
    /// Name displayed.
    /// </summary>
    /// <value>Name displayed.</value>
    /// <returns>Name displayed.</returns>
    public string DisplayName { get; set; }

    /// <summary>
    /// URL for the organization.
    /// </summary>
    /// <value>URL for the organization.</value>
    /// <returns>URL for the organization.</returns>
    public string URL { get; set; }

    /// <summary>
    /// Language.
    /// </summary>
    /// <value>Language.</value>
    /// <returns>Language.</returns>
    public string Language { get; set; }

}
