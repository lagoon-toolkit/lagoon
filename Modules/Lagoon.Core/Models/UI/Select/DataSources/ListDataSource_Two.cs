//TODO Alix TranslateOption ?

namespace Lagoon.UI.Components;

/// <summary>
/// Provide the list of items to fill a component.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TValue"></typeparam>
public abstract class ListDataSource<TItem, TValue> : ListDataSourceBase<TItem, TValue>
{

    #region properties

    /// <summary>
    /// Items of the data source.
    /// </summary>
    public IEnumerable<TItem> Items { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance from a preload item list.
    /// </summary>
    /// <param name="items">Item list.</param>
    public ListDataSource(IEnumerable<TItem> items) : base(false)
    {
        Items = items;
    }

    /// <summary>
    /// New instance from a preload item list.
    /// </summary>
    public ListDataSource() : base(false)
    {
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    public override string GetItemText(TItem value)
    {
        return value.ToString();
    }

    ///<inheritdoc/>
    internal override IEnumerable<TItem> GetItemList()
    {
        return Items;
    }

    #endregion

}
