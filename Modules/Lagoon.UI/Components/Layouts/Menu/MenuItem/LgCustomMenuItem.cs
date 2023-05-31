namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Base class for LgMenuItem, LgMenuTitle and LgMenuSeparator.
/// </summary>
public class LgCustomMenuItem : LgComponentBase
{

    #region cascading parameters

    /// <summary>
    /// Parent menu item.
    /// </summary>
    [CascadingParameter]
    public LgCustomMenuItem ParentMenuItem { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Level of the menu item.
    /// </summary>
    public int Level { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Level = ParentMenuItem is null ? 0 : ParentMenuItem.Level + 1;
    }

    #endregion

}
