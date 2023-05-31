using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Lagoon.UI.Components;

/// <summary>
/// Policy state for a component.
/// </summary>
internal class PolicyState
{

    #region properties

    /// <summary>
    /// Gets or sets if the authentication is done.
    /// </summary>
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// Gets or sets if the component display is allowed.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Gets or sets if the component edition mode is allowed.
    /// </summary>
    public bool Editable { get; set; }

    /// <summary>
    /// Gets or sets informations about the user.
    /// </summary>
    public AuthenticationState AuthenticationState { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    public PolicyState()
    {
    }

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="visible">Component is allowed to display.</param>
    /// <param name="editable">Component is allowed to edit.</param>
    public PolicyState(bool visible, bool editable)
    {
        Visible = visible;
        Editable = editable;
    }

    #endregion

    #region methods

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal ClaimsPrincipal GetAuthentificationUser()
    {
        if (AuthenticationState is null)
        {
            throw new InvalidOperationException($"Authorization requires a cascading parameter of type Task<{nameof(AuthenticationState)}>. Consider using {typeof(CascadingAuthenticationState).Name} to supply this.");
        }
        return AuthenticationState.User;
    }

    #endregion

}
