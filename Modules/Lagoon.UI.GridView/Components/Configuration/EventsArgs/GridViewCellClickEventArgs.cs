namespace Lagoon.UI.Components;

/// <summary>
/// Cell click event arguments (For columns).
/// </summary>
public class GridViewCellClickEventArgs : EventArgs
{

    /// <summary>
    /// Gets object of the row.
    /// </summary>
    /// <value></value>
    [Obsolete("Use the \"Item\" property.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object Data => Item;

    /// <summary>
    /// Gets item of the row.
    /// </summary>
    /// <value></value>
    public object Item { get; }

    /// <summary>
    /// Gets object of the cell
    /// </summary>
    /// <value></value>
    [Obsolete("Use the \"Item\" property.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object Value { get => null; set { } }

    /// <summary>
    /// Get the column identifier of the clicked cell.
    /// </summary>
    public string ColumnKey { get; }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="columnKey"></param>
    public GridViewCellClickEventArgs(object item, string columnKey)
    {
        Item = item;
        ColumnKey = columnKey;
    }

}

/// <summary>
/// Cell click event arguments (For grid).
/// </summary>
public class GridViewCellClickEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Gets object of the row.
    /// </summary>
    /// <value></value>
    [Obsolete("Use the \"Item\" property.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TItem Data => Item;

    /// <summary>
    /// Gets item of the row.
    /// </summary>
    /// <value></value>
    public TItem Item { get; }

    /// <summary>
    /// Gets object of the cell
    /// </summary>
    /// <value></value>
    [Obsolete("Use the \"Item\" property.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object Value { get => null; set { } }

    /// <summary>
    /// Get the column identifier of the clicked cell.
    /// </summary>
    public string ColumnKey { get; }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="columnKey"></param>
    public GridViewCellClickEventArgs(TItem item, string columnKey)
    {
        Item = item;
        ColumnKey = columnKey;
    }
}
