namespace Lagoon.Model.Context;


/// <summary>
/// Object used to describe external identities
/// </summary>
public class OidcClientDescriptor
{

    /// <summary>
    /// Get the ClientId
    /// </summary>
    public string ClientId { get; }

    /// <summary>
    /// Get the ClientSecret
    /// </summary>
    public string ClientSecret { get; }

    /// <summary>
    /// Get the Scope
    /// </summary>
    public string Scope { get; }

    /// <summary>
    /// Get the Roles
    /// </summary>
    public IEnumerable<string> Roles { get; }

    /// <summary>
    /// Get the RedirectUris
    /// </summary>
    public IEnumerable<string> RedirectUris { get; }

    /// <summary>
    /// List of supported permission for this oidc client
    /// rq: must be a constant value from OpenIddict.Abstractions.OpenIddictConstants.Permissions.GrantTypes
    /// </summary>
    public IEnumerable<string> PermissionsGrantType { get; }

    /// <summary>
    /// Initialize a new <see cref="OidcClientDescriptor"/>
    /// </summary>
    /// <param name="grantType">List of permission</param>
    /// <param name="clientId">The client identifier</param>
    /// <param name="clientSecret">The client secret</param>
    /// <param name="scope">Scope</param>
    /// <param name="roles">List of application roles</param>
    /// <param name="redirectUris">List of allowed redirect uri</param>
    public OidcClientDescriptor(IEnumerable<string> grantType, string clientId, string clientSecret, string scope, IEnumerable<string> roles, IEnumerable<string> redirectUris)
    {
        PermissionsGrantType = grantType;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Scope = scope;
        Roles = roles;
        RedirectUris = redirectUris;
    }

}
