namespace Lagoon.Server.Application;

/// <summary>
/// Accepted authentication mode
/// </summary>
public enum AuthenticationMode
{

    /// <summary>
    /// No authentication
    /// </summary>
    None,

    /// <summary>
    /// Form authentication
    /// </summary>
    Forms,

    /// <summary>
    /// SSO Authentication
    /// </summary>
    SSO,

    /// <summary>
    /// Windows authentication
    /// </summary>
    Windows

}
