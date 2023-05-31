using Microsoft.AspNetCore.Components.Authorization;

namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Handle the unauthize page access.
/// </summary>
public partial class DisplayNotAuthorized: LgPage
{

    #region fields

    private static readonly string _title = "#pgNotAuthorizedTitle";
    private static readonly string _iconName = IconNames.Error;

    #endregion

    #region cascading parameters

    /// <summary>
    /// LgApp instance.
    /// </summary>
    [CascadingParameter]
    public LgApp AppComponent { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the authentication state.
    /// </summary>
    [Parameter]
    public AuthenticationState AuthenticationState { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        return SetTitleAsync(_title, _iconName);
    }

    /// <summary>
    /// Gets a value that indicates whether the user has been authenticated.
    /// </summary>
    /// <returns>A value that indicates whether the user has been authenticated.</returns>
    public bool IsAuthenticated()
    {            
        System.Security.Principal.IIdentity identity = AuthenticationState?.User?.Identity;
        if (identity is null)
        {
            return false;
        }
        return identity.IsAuthenticated;
    }

    #endregion
}
