namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Render the application toolbar.
/// </summary>
public partial class LgMenuToolbarRender : LgMenuRender
{

    #region constructors

    /// <summary>
    /// Create a new instance of <see cref="LgMenuTopRender"/>
    /// </summary>
    public LgMenuToolbarRender()
    {
        MenuPosition = MenuPosition.MenuToolbar;
        HideRootLevelText = true;
        ShowTextAsTooltip = false;
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

    ///<inheritdoc/>
    internal override bool GetIsTooltipAsText(int level)
    {
        return (level == 0 && HideRootLevelText) ? true : ShowTextAsTooltip;
    }

    #endregion

}