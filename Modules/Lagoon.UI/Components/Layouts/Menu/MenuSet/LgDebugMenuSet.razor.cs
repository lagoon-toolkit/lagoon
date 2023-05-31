namespace Lagoon.UI.Components;


/// <summary>
/// Debug menu item set.
/// </summary>
public partial class LgDebugMenuSet : LgComponentBase
{

    #region dependencies injection

    private void ReloadCss()
    {
        JS.InvokeVoidAsync("Lagoon.refreshCss");
    }

    #endregion

    #region render fragments

    /// <summary>
    /// Menu set composition.
    /// </summary>
    /// <value></value>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Id of this toolbar instance.
    /// </summary>
    [Parameter]
    public string Id { get; set; }

    /// <summary>
    /// Indicate if menu items will be shown.
    /// </summary>
    [Parameter]
    public bool Visible { get; set; } = true;

    #endregion

}
