namespace Lagoon.UI.Components;

/// <summary>
/// Provide the list of items to fill a component.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class ListDataSource<TValue> : ListDataSource<TValue, TValue>
{

    #region constructors

    /// <summary>
    /// New instance from a preload item list.
    /// </summary>
    /// <param name="items">Item list.</param>
    public ListDataSource(IEnumerable<TValue> items) : base(items)
    {
    }

    /// <summary>
    /// New instance from a preload item list.
    /// </summary>
    public ListDataSource()
    {
    }

    ///<inheritdoc/>
    public override TValue GetItemValue(TValue item)
    {
        return item;
    }

    #endregion

}
