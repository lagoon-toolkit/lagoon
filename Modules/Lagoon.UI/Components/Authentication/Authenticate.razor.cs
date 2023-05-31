using Lagoon.UI.Application;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Lagoon.UI.Components;

/// <summary>
/// Authentication management
/// </summary>
public partial class Authenticate: ComponentBase, IFullScreenPage
{

    #region parameters

    /// <summary>
    /// Current authentication step
    /// </summary>
    /// <value></value>
    [Parameter]
    public string Action { get; set; }

    /// <summary>
    /// Used to retrieve current authentication state
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationState { get; set; }

    #endregion

    #region dependencies injections

    /// <summary>
    /// Configuration by the developper.
    /// </summary>
    [Inject]
    private ApplicationBehavior BehaviorConfiguration { get; set; }

    /// <summary>
    /// Configuration from the appsettings.json file.
    /// </summary>
    [Inject]
    private IConfiguration Configuration { get; set; }

    /// <summary>
    /// Used to check if we are in offline config, and if so to clean claims stored on disconnection
    /// </summary>
    /// <value></value>
    [Inject] 
    private AccountClaimsPrincipalFactory<RemoteUserAccount> OfflineAuth { get; set; }

    /// <summary>
    /// Javascript interop
    /// </summary>
    [Inject]
    private IJSRuntime Js { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// On Action paramter set
    /// </summary>
    protected override Task OnParametersSetAsync()
    {
        // Patch for authentication management in tabbed mode (which return url with '?...' while the default router does not return any)
        if (Action.Contains('?'))
        {
            Action = Action.Split('?')[0];
        }
        return base.OnParametersSetAsync();          
    }

    #endregion

}
