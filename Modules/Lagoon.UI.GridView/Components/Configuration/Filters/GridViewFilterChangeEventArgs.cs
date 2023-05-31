namespace Lagoon.UI.Components;

/// <summary>
/// Filter change event arguments
/// </summary>
public class GridViewFilterChangeEventArgs
{

    #region properties

    /// <summary>
    /// Gets the column unique identifier.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the filter value.
    /// </summary>
    public Filter Filters { get; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="filters">Filter values.</param>
    /// <param name="uniqueKey">Column identifier.</param>
    public GridViewFilterChangeEventArgs(string uniqueKey, Filter filters)
    {
        Key = uniqueKey;
        Filters = filters;
    }

    #endregion

}
