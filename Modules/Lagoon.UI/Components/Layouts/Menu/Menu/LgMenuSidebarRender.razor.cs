namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Render the application toolbar.
/// </summary>
public partial class LgMenuSidebarRender : LgMenuRender
{

    #region parameters

    /// <summary>
    /// Menu is collapsed ? (Used for mobile mode with hamburger menu mode)
    /// </summary>
    [Parameter]
    public bool Collapsed { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// Create a new instance of <see cref="LgMenuTopRender"/>
    /// </summary>
    public LgMenuSidebarRender() : base(1)
    {
        MenuPosition = MenuPosition.MenuSidebar;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        HideRootLevelText = Collapsed;
        HideDropDownArrow = Collapsed;
        ShowTextAsTooltip = Collapsed;
        HideTitleItems = Collapsed;
        HideSeparatorItems = Collapsed;
    }

    #endregion

}