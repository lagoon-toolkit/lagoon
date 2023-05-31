namespace Lagoon.UI.Components;

/// <summary>
/// Value add or update event arguments
/// </summary>
public class GridViewItemActionEventArgs<TItem> : EventArgs
{

    /// <summary>
    /// The working item.
    /// </summary>
    public TItem Item { get; }        

    /// <summary>
    /// Gets object of the row
    /// </summary>
    /// <value></value>
    [Obsolete("Use the \"Item\" property.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TItem RowData => Item;


    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="item">The target item of the action.</param>
    public GridViewItemActionEventArgs(TItem item)
    {
        Item = item;
    }       
    
    /// <summary>
    /// Gets or sets cancel action indicator
    /// </summary>
    public bool Cancel { get; set; }

}