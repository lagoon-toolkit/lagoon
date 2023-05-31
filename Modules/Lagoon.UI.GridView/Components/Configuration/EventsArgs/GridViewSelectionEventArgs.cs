namespace Lagoon.UI.Components;

/// <summary>
/// Row selection event arguments
/// </summary>
public class GridViewSelectionEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Gets or sets the last selected row item.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TItem SelectedItem { get; set; }

    /// <summary>
    /// Gets or sets list of selected data
    /// </summary>
    /// <returns></returns>
    public ICollection<TItem> Selection { get; }

    /// <summary>
    /// Object initialization 
    /// </summary>
    /// <param name="selection">List of all selected items.</param>
    /// <param name="selectedItem">The last checked item.</param>
    public GridViewSelectionEventArgs(ICollection<TItem> selection, TItem selectedItem)
    {
        Selection = selection;
        SelectedItem = selectedItem;
    }

}