namespace Lagoon.UI.Components;

/// <summary>
/// Menu item separator.
/// </summary>
public partial class LgMenuSeparator : LgCustomMenuItem
{

    #region cascading parameters

    /// <summary>
    /// Menu position (MenuTop, MenuToolbar, MenuSidebar, MenuAppTabContainer)
    /// </summary>
    [CascadingParameter]
    public LgMenuRender MenuRender { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Get or set the title text
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Gets if the component must be visible.
    /// </summary>
    /// <returns><c>true</c> if the component must be visible.</returns>
    private bool IsVisible()
    {
        return !MenuRender?.HideSeparatorItems ?? true;
    }

    #endregion

}
