using Microsoft.AspNetCore.Authentication;

namespace Lagoon.Server.Saml;

/// <summary>
/// The SSO options.
/// </summary>
public class SamlOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// Service provider identity.
    /// </summary>
    public string EntityId { get; set; }

    /// <summary>
    /// The Idp metatadata location.
    /// </summary>
    public string IdpMetadataLocation { get; set; }

    /// <summary>
    /// The name ID policy.
    /// </summary>
    public string NameIdPolicy { get; set; } = NameIdFormat.EmailAddress;

    /// <summary>
    /// The sign-in scheme to use after user identification.
    /// </summary>
    public string SignInScheme { get; set; }

    /// <summary>
    /// The sign-out scheme to use after user identification.
    /// </summary>
    public string SignOutScheme { get; set; }

}
