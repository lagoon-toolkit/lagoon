using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;

namespace Lagoon.UI.Application;

/// <summary>
/// Manager to remove user data from browser storages when the user log out.
/// </summary>
public class SignOutManager
{

    #region dependency injection

    /// <summary>
    /// Give access to LgApplication context
    /// </summary>
    private LgApplication _app;

    /// <summary>
    /// Sign out session state manager.
    /// </summary>
    private SignOutSessionStateManager _signOutSessionStateManager;

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="signOutSessionStateManager">Handles CSRF protection for the logout endpoint. 'Scoped)</param>
    public SignOutManager(LgApplication app, SignOutSessionStateManager signOutSessionStateManager)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(signOutSessionStateManager);
        _app = app;
        _signOutSessionStateManager = signOutSessionStateManager;
    }

    #endregion

    #region methods

    /// <summary>
    /// Disconnect from client.
    /// </summary>
    /// <returns></returns>
    public async Task SignoutAsync()
    {
        // Unsubscribe to potential WebPush notification subscription without waiting
        await _app.UnsubscribeToPushNotificationAsync(true);

        // Clear user data from browser storage
        await _app.SignOutCleaner.RunAsync(_app);
        // Change the sign out state
        await _signOutSessionStateManager.SetSignOutState();
        // Redirect to a loged out page
        _app.NavigateTo("authentication/logout");
    }

    #endregion

}
