using Lagoon.Internal;
using Lagoon.UI.Application;

namespace Lagoon.UI.Components;

/// <summary>
/// Tabs service
/// </summary>
public class ClientTabService
{

    #region dependencies injections

    // Http client
    private HttpClient _http;

    // Js runtime
    private readonly IJSInProcessRuntime _js;

    #endregion

    #region fields

    /// <summary>
    /// The name of the Tab Data Store.
    /// </summary>
    private static string _tabDataStoreKey;

    /// <summary>
    /// The cancellation token to save the tab state.
    /// </summary>
    private CancellationTokenSource _saveCancellationTokenSource;

    #endregion

    #region constructor

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="http">Http client</param>
    /// <param name="app">Lagoon application manager</param>
    public ClientTabService(HttpClient http, LgApplication app)
    {
        _js = app.JS;
        _http = http;
        _tabDataStoreKey = GetLocalStorageKey(app);
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
    /// Get the local storage key used to save tabs states.
    /// </summary>
    /// <param name="app">The application.</param>
    private static string GetLocalStorageKey(LgApplication app)
    {
        return app.GetLocalStorageKey("TabDataStore");
    }

    #endregion

    #region Getters

    /// <summary>
    /// Get saved tabs
    /// </summary>
    /// <param name="savingMethod">saved tab location</param>
    /// <returns></returns>
    internal async Task<IEnumerable<Tab>> GetTabsAsync(TabSavingMethod savingMethod)
    {
        IEnumerable<Tab> tabsDB = Enumerable.Empty<Tab>();
        switch (savingMethod)
        {
            case TabSavingMethod.Remote:
                tabsDB = await _http.TryGetAsync<IEnumerable<Tab>>(Routes.TABS_ROUTE);
                break;
            case TabSavingMethod.Local:
                tabsDB = await _js.InvokeAsync<IEnumerable<Tab>>("Lagoon.LgTabDataStore.getTabs", _tabDataStoreKey);
                break;
        }
        return tabsDB;
    }

    /// <summary>
    /// Remove the database from IndexedDB
    /// </summary>
    internal Task DeleteDatabaseAsync()
    {
        return _js.RemoveIndexedDbAsync(_tabDataStoreKey);
    }

    #endregion

    #region Setters

    /// <summary>
    /// Save tabs
    /// </summary>
    /// <param name="savingMethod">saved tab location</param>
    /// <param name="tabs">Tab list to save</param>
    /// <returns></returns>
    internal async Task SaveTabsAsync(TabSavingMethod savingMethod, IEnumerable<Tab> tabs)
    {
        try
        {

            if (savingMethod != TabSavingMethod.None)
            {
                _saveCancellationTokenSource?.Cancel();
                _saveCancellationTokenSource = new CancellationTokenSource();
                await Task.Run(async () =>
                {
                    // Wait 1s before saving tabs
                    await Task.Delay(1000);

                    switch (savingMethod)
                    {
                        case TabSavingMethod.Remote:
                            await _http.TryPostAsync(Routes.TABS_ROUTE, tabs, _saveCancellationTokenSource.Token);
                            break;
                        case TabSavingMethod.Local:
                            await _js.InvokeVoidAsync("Lagoon.LgTabDataStore.saveTabs", _saveCancellationTokenSource.Token, _tabDataStoreKey, tabs);
                            break;
                    }
                }
                , _saveCancellationTokenSource.Token);
            }
        }
        catch (TaskCanceledException)
        {
            // Don't trace Task cancellation exception
        }
    }

    #endregion

}
