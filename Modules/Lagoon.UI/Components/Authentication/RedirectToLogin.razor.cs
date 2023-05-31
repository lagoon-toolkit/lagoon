using Microsoft.AspNetCore.Components.Authorization;

namespace Lagoon.UI.Components;

/// <summary>
/// Redirect to login page component
/// </summary>
public partial class RedirectToLogin : ComponentBase
{
    #region constants

    /// <summary>
    /// Route for user connection.
    /// </summary>
    public const string ROUTE = "authentication/login";

    #endregion

    #region cascading parameters

    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationStateTask { get; set; }

    #endregion

    #region dependency injection

    /// <summary>
    /// Navigation manager.
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        System.Security.Principal.IIdentity identity = (await AuthenticationStateTask)?.User?.Identity;
        if (identity is null || !identity.IsAuthenticated)
        {
            Navigate(NavigationManager, NavigationManager.Uri);
        }
    }

    #endregion

    #region static methods

    /// <summary>
    /// Navigate to the connect page.
    /// </summary>
    /// <param name="navigationManager"></param>
    /// <param name="returnUri"></param>
    public static void Navigate(NavigationManager navigationManager, string returnUri)
    {
        navigationManager.NavigateTo($"{ROUTE}?returnUrl={Uri.EscapeDataString(returnUri)}");
    }

    #endregion

}
