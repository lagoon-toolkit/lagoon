namespace Lagoon.UI.Components;

/// <summary>
/// Value changed event arguments
/// </summary>
public class GridViewValueChangeEventArgs<TItem> : EventArgs
{

    /// <summary>
    /// Gets object of the row
    /// </summary>
    /// <value></value>
    public TItem Item { get;}

    /// <summary>
    /// Gets field name
    /// </summary>
    /// <value></value>
    public string ColumnKey { get;}

    /// <summary>
    /// Gets new value of the cell
    /// </summary>
    /// <value></value>
    public object Value { get; }

    /// <summary>
    /// Gets previous value of the cell
    /// </summary>
    /// <value></value>
    public object PreviousValue { get; }

    /// <summary>
    /// Gets or sets cancel update indicator
    /// </summary>
    public bool Cancel { get; set; }

    #region obsolete

    /// <summary>
    /// Gets object of the row
    /// </summary>
    /// <value></value>
    [Obsolete("Use the \"Item\" property.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TItem RowData => Item;

    /// <summary>
    /// Gets field name
    /// </summary>
    /// <value></value>
    [Obsolete("Use the \"ColumnKey\" property.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string FieldName => ColumnKey;

    #endregion

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="columnKey">The column identifier.</param>
    /// <param name="value">The new value.</param>
    /// <param name="previousValue">The old value.</param>
    public GridViewValueChangeEventArgs(TItem item, string columnKey, object value, object previousValue)
    {
        Item = item;
        ColumnKey = columnKey;
        Value = value;
        PreviousValue = previousValue;
    }
}
