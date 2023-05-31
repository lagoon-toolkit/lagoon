namespace Lagoon.UI.Components;

/// <summary>
/// Refresh menu event args.
/// </summary>
public class RefreshMenuEventArgs : EventArgs
{
    #region public properties

    /// <summary>
    /// Menu position 
    /// </summary>
    public MenuPosition MenuPosition { get; }

    #endregion

    /// <summary>
    /// Instance initialization.
    /// </summary>
    /// <param name="menuPosition">Menu position.</param>
    public RefreshMenuEventArgs(MenuPosition menuPosition)
    {
        MenuPosition = menuPosition;
    }


}
