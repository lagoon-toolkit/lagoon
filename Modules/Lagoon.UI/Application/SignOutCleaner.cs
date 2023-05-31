namespace Lagoon.UI.Application;

/// <summary>
/// Remove browser data store for the user when he log out.
/// </summary>
public class SignOutCleaner
{

    #region fields

    private readonly List<string> _sessionStorageKeys = new();
    private readonly List<string> _localStorageKeys = new();
    private readonly List<string> _indexDbKeys = new();

    #endregion

    #region methods

    /// <summary>
    /// Add a new name to remove when the user log out the application.
    /// </summary>
    /// <param name="key">The key of the SessionStorage.</param>
    public void AddSessionStorageKey(string key)
    {
        _sessionStorageKeys.Add(key);
    }

    /// <summary>
    /// Add a new name to remove when the user log out the application.
    /// </summary>
    /// <param name="key">The key of the LocalStorage.</param>
    public void AddLocalStorageKey(string key)
    {
        _localStorageKeys.Add(key);
    }

    /// <summary>
    /// Add a new name to remove when the user log out the application.
    /// </summary>
    /// <param name="key">The name of the IndexDb database.</param>
    public void AddIndexDbKey(string key)
    {
        _indexDbKeys.Add(key);
    }

    /// <summary>
    /// Remove data store for the user when he log out.
    /// </summary>
    public async Task RunAsync(LgApplication app)
    {
        foreach (string key in _sessionStorageKeys)
        {
#if DEBUG //TOCLEAN
            Lagoon.Helpers.Trace.ToConsole(this, $"sessionStorage.removeItem: {key}");
#endif
            app.JS.InvokeVoid("sessionStorage.removeItem", key);
        }
        foreach (string key in _localStorageKeys)
        {
#if DEBUG //TOCLEAN
            Lagoon.Helpers.Trace.ToConsole(this, $"LocalStorage.removeItem: {key}");
#endif
            app.LocalStorage.RemoveItem(key);
        }
        foreach (string key in _indexDbKeys)
        {
#if DEBUG //TOCLEAN
            Lagoon.Helpers.Trace.ToConsole(this, $"IndexDB.removeItem: {key}");
#endif
            await app.JS.RemoveIndexedDbAsync(key);
        }
    }

    #endregion

}
