namespace Lagoon.UI.Components;

/// <summary>
/// Gridview state
/// </summary>
internal class GridViewState
{
    /// <summary>
    /// Gets or sets current dragged column
    /// </summary>
    public GridColumnState DraggedColumn { get; set; }

    /// <summary>
    /// Gets or sets group collapse state
    /// </summary>
    public bool GroupCollapsed { get; set; }

    /// <summary>
    /// Gets group state
    /// </summary>
    public readonly Dictionary<string, GridViewGroupState> GroupsState = new();

}
