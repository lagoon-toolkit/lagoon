using System.Security.Claims;

namespace Lagoon.Server.Application.IdentitySources;

/// <summary>
/// The identity come from a Windows authentication.
/// </summary>
public class WindowsIdentitySource : IdentitySource
{
    #region constants

    /// <summary>
    /// The identifier of this identity source.
    /// </summary>
    public const string SOURCE_NAME = "Windows";

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="userName">The windows login.</param>
    /// <param name="removeDomain">Indicate if the domain must be ignored.</param>
    public WindowsIdentitySource(string userName, bool removeDomain)
        : base(AuthenticationMode.Windows, removeDomain ? RemoveDomain(userName) : userName, new Claim[] { new(ClaimTypes.Name, userName) }, SOURCE_NAME, false)
    { }

    private static string RemoveDomain(string name)
    {
        string[] parts = name.Split('\\');
        return parts[^1];
    }

    #endregion

}
