namespace Lagoon.UI.Components;

/// <summary>
/// Move column event arguments
/// </summary>
internal class GridViewMoveColumnEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets column receiving drop
    /// </summary>
    public GridColumnState DroppedColumn { get; set; }

    /// <summary>
    /// Gets or sets dragged column
    /// </summary>
    public GridColumnState DraggedColumn { get; set; }
}
