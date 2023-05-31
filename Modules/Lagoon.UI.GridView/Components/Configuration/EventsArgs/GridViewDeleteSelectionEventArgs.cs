namespace Lagoon.UI.Components;

/// <summary>
/// Action on selection event arguments
/// </summary>
public class GridViewSelectionActionEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Gets or sets selected items
    /// </summary>
    public ICollection<TItem> Selection { get; }

    /// <summary>
    /// Gets or sets cancel action indicator
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="selection"></param>
    public GridViewSelectionActionEventArgs(ICollection<TItem> selection)
    {
        Selection = selection;
    }
}
