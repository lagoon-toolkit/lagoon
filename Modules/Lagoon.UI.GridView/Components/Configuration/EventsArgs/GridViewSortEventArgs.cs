namespace Lagoon.UI.Components;

/// <summary>
/// Event argument for column sort change
/// </summary>
public class GridViewSortEventArgs : EventArgs
{

    /// <summary>
    /// Gets the column identifier.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the new sort direction.
    /// </summary>
    public DataSortDirection SortDirection { get; }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="key">The column identifier.</param>
    /// <param name="sortDirection">The new sort direction.</param>
    public GridViewSortEventArgs(string key, DataSortDirection sortDirection)
    {
        Key = key;
        SortDirection = sortDirection;
    }

}
