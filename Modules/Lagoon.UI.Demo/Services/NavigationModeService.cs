using Microsoft.JSInterop;
using System;


namespace Lagoon.UI.Demo.Pages;

public class NavigationModeService
{
    #region fields

    private bool _isTabbed;

    private readonly IJSInProcessRuntime _js;

    #endregion

    #region properties

    public bool IsTabbed => _isTabbed;

    #endregion

    #region events

    /// <summary>
    /// Event called when tab navigation mode is changed.
    /// </summary>
    public event Action OnNavigationModeChanged;

    #endregion

    #region constructors

    public NavigationModeService(IJSRuntime js)
    {
        _js = (IJSInProcessRuntime)js;
        _isTabbed = _js.Invoke<string>("Lagoon.JsUtils.GetCookie", "IsTabbed") != "false";
    }

    #endregion

    #region methods

    public void ChangeNavigationMode(bool isTabbed)
    {
        _isTabbed = isTabbed;
        _js.InvokeVoid("Lagoon.JsUtils.CreateCookie", "IsTabbed", isTabbed, 365 * 5);
        OnNavigationModeChanged?.Invoke();
    }

    #endregion

}

