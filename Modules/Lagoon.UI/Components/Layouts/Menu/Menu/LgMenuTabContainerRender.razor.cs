namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Render the application tab container menu.
/// </summary>
public partial class LgMenuTabContainerRender : LgMenuRender
{

    #region constructors

    /// <summary>
    /// Create a new instance of <see cref="LgMenuTopRender"/>
    /// </summary>
    public LgMenuTabContainerRender()
    {
        MenuPosition = MenuPosition.MenuToolbarTabContainer;
        HideRootLevelText = true;
        ShowTextAsTooltip = true;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    internal override string GetDropDownMenuCssClass(int level)
    {
        return level == 0 ? "dropdown-menu dropdown-menu-right" : "dropdown-menu dropdown-menuLeft";
    }

    ///<inheritdoc/>
    internal override string GetNavItemCssClass(int level)
    {
        // Toolbar sub items (children) must be displayed at the left => pinned to the right with the css class from bootstrap : dropdown-menu-right
        return level == 0 ? "dropdown" : "dropleft";
    }

    #endregion

}
