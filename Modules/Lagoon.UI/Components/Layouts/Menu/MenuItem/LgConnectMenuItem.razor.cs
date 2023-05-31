using Lagoon.UI.Application;
using Microsoft.AspNetCore.Components.Authorization;

namespace Lagoon.UI.Components;


/// <summary>
/// Menu item to connect or disconnect user.
/// </summary>
public partial class LgConnectMenuItem : LgComponentBase, ICommandListener
{
    #region constants

    private const string LOGIN_COMMAND = "LOGIN";
    private const string LOGOUT_COMMAND = "LOGOUT";

    #endregion

    #region dependency injection

    /// <summary>
    /// The signout manager.
    /// </summary>
    [Inject]
    private SignOutManager SignOutManager { get; set; }

    #endregion

    #region cascading parameters

    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationStateTask { get; set; }

    /// <summary>
    /// The parent command listener.
    /// </summary>
    [CascadingParameter]
    private ICommandListener ParentCommandListener { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// By default, the text display depends of the menu container : The text is not displayed in toolbar by default.
    /// </summary>
    [Parameter]
    public bool? HideRootLevelText { get; set; }

    #endregion

    #region property

    /// <summary>
    /// Gets if a current user is authenticated.
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// The parent command listener.
    /// </summary>
    ICommandListener ICommandListener.ParentCommandListener => ParentCommandListener;

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        System.Security.Principal.IIdentity identity = (await AuthenticationStateTask)?.User?.Identity;
        IsConnected = (identity != null) && identity.IsAuthenticated;
    }

    /// <summary>
    /// Method invoked when a command is received from a sub component.
    /// </summary>
    /// <param name="args">The command event args.</param>
    public async Task BubbleCommandAsync(CommandEventArgs args)
    {
        switch (args.CommandName)
        {
            case LOGIN_COMMAND:
                App.NavigateTo("Identity/Account/Login", true);
                args.Handled = true;
                break;
            case LOGOUT_COMMAND:
                await SignOutManager.SignoutAsync();
                args.Handled = true;
                break;
        }
    }

    #endregion

}
