namespace Lagoon.UI.Components;


/// <summary>
/// Item to do a global search.
/// </summary>
public partial class LgDebugMenuItem
{

    #region dependencies injection

    /// <summary>
    /// Give access to the JS Runtime
    /// </summary>
    [Inject]
    private IJSRuntime JS { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// Menu set composition.
    /// </summary>
    /// <value></value>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Force to reload the application CSS.
    /// </summary>
    private void ReloadCss()
    {
        JS.InvokeVoidAsync("Lagoon.refreshCss");
    }

    #endregion
}
