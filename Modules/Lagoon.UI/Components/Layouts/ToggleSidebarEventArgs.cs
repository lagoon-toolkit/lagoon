namespace Lagoon.UI.Components;

/// <summary>
/// Toggle side bar events args.
/// </summary>
public class ToggleSidebarEventArgs : EventArgs
{
    /// <summary>
    /// Sidebar is collasped ?
    /// </summary>
    public bool Collapsed { get; set; }
}
