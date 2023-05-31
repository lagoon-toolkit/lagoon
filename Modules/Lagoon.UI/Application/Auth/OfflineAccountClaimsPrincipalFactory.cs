using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System.Security.Claims;

namespace Lagoon.UI.Application.Auth;


/// <summary>
/// Allow an authenticated user to go Offline with it's actual claims
/// </summary>
public class OfflineAccountClaimsPrincipalFactory : ArrayClaimsPrincipalFactory<RemoteUserAccount>
{

    #region fields

    /// <summary>
    /// The main application.
    /// </summary>
    private readonly LgApplication _app;

    /// <summary>
    /// Local storage key
    /// </summary>
    private readonly string _claimsKey;

    #endregion

    #region Initialization

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="accessor">For base contructor</param>
    /// <param name="app">Lagoon application manager</param>
    /// <returns></returns>
    public OfflineAccountClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor, LgApplication app) : base(accessor)
    {
        _app = app;
        _claimsKey = GetLocalStorageKey(app);
    }

    /// <summary>
    /// Add the key to delete to the signout manager when the user log out.
    /// </summary>
    /// <param name="app">The application.</param>
    internal static void RegisterToSignoutManager(LgApplication app)
    {
        app.SignOutCleaner.AddLocalStorageKey(GetLocalStorageKey(app));
    }

    /// <summary>
    /// Get the local storage key used.
    /// </summary>
    /// <param name="app">The application.</param>
    private static string GetLocalStorageKey(LgApplication app)
    {
        return app.GetLocalStorageKey("ApplicationClaims");
    }

    #endregion

    #region methods

    /// <summary>
    /// When an building user claims:
    /// - if online, saved current user claims to localStorage
    /// - if offline try to load claims from localStorage
    /// </summary>
    /// <param name="account">User</param>
    /// <param name="options">Options</param>
    /// <returns></returns>
    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
    {
        ClaimsPrincipal result = await base.CreateUserAsync(account, options);
        if (result.Identity.IsAuthenticated)
        {
            SaveUserToLocalStorage(result);
        }
        else
        {
            result = LoadUserFromLocalStorage();
        }
        return result;
    }

    /// <summary>
    /// Save user claims to localStorage
    /// </summary>
    /// <param name="user">Claims to save</param>
    private void SaveUserToLocalStorage(ClaimsPrincipal user)
    {
        _app.LocalStorage.SetItem(_claimsKey, user.Claims.Select(c => new ClaimData { Type = c.Type, Value = c.Value }));
    }

    /// <summary>
    /// Try to local user claims from localstorage and if not available return an empty claim (no authenticated user) 
    /// </summary>
    private ClaimsPrincipal LoadUserFromLocalStorage()
    {
        ClaimData[] storedClaims = _app.LocalStorage.GetItem<ClaimData[]>(_claimsKey);
        ClaimsIdentity claimsIdentity = storedClaims is null ? new() : new(storedClaims.Select(c => new Claim(c.Type, c.Value)), "appAuth", "name", "role");
        return new ClaimsPrincipal(claimsIdentity);
    }

#endregion

#region internal struct

    /// <summary>
    /// Claim storage infos
    /// </summary>
    private class ClaimData
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

#endregion

}
