namespace Lagoon.UI.Components;

/// <summary>
/// Provide the list of items to fill a component.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TValue"></typeparam>
public abstract class ListDataSourceBase<TItem, TValue> : IListDataSource<TValue>
{
    #region fields

    private readonly IEqualityComparer<TValue> _valueEqualityComparer;
    private readonly bool _hasAsyncLoading;

    #endregion

    #region constructors

    /// <summary>
    /// New instance from an override.
    /// </summary>
    protected ListDataSourceBase(bool hasAsyncLoading)
    {
        _hasAsyncLoading = hasAsyncLoading;
        _valueEqualityComparer = EqualityComparer<TValue>.Default;
    }

    #endregion

    #region properties

    /// <summary>
    /// Push the "StartWith" items on the top of the list after a search.
    /// </summary>
    public bool DisableStartsWithAsFirstResults { get; set; }

    #endregion

    #region IListItemSource interface

    /// <summary>
    /// Indicate if the loading is asynchrone
    /// </summary>
    bool IListDataSource<TValue>.HasAsyncLoading => _hasAsyncLoading;

    IEnumerable<IListItemData<TValue>> IListDataSource<TValue>.GetItems()
    {
        return GetItemDataList();
    }

    Task<IEnumerable<IListItemData<TValue>>> IListDataSource<TValue>.GetItemsAsync(GetItemsAsyncEventArgs<TValue> e)
    {
        return GetItemDataListAsync(e);
    }

    #endregion

    #region IEqualityComparer

    bool IEqualityComparer<TValue>.Equals(TValue x, TValue y)
    {
        return ValueEquals(x, y);
    }

    int IEqualityComparer<TValue>.GetHashCode(TValue obj)
    {
        return ValueGetHashCode(obj);
    }

    /// <summary>
    /// Determines whether the specified objects are equal.
    /// </summary>
    /// <param name="x">The first object of type T to compare.</param>
    /// <param name="y">The second object of type T to compare.</param>
    /// <returns>true if the specified objects are equal; otherwise, false.</returns>
    /// <remarks>WARNING : If ValueEquals is overide, GetValueHashCode must be override too.</remarks>
    public virtual bool ValueEquals(TValue x, TValue y)
    {
        return _valueEqualityComparer.Equals(x, y);
    }

    /// <summary>
    /// Returns a hash code for the specified object.
    /// </summary>
    /// <param name="obj">The System.Object for which a hash code is to be returned.</param>
    /// <returns>A hash code for the specified object.</returns>
    public virtual int ValueGetHashCode(TValue obj)
    {
        return _valueEqualityComparer.GetHashCode(obj);
    }

    #endregion

    #region methods

    /// <summary>
    /// Return a IListItemData(Of TValue) enumeration.
    /// </summary>
    /// <returns>A a IListItemData(Of TValue) enumeration.</returns>
    protected virtual IEnumerable<IListItemData<TValue>> GetItemDataList()
    {
        return ToIListItemDataEnumerable(GetItemList());
    }

    /// <summary>
    /// Load the list of items corresponding to the argument filter.
    /// </summary>
    /// <returns>The loaded list.</returns>
    internal virtual IEnumerable<TItem> GetItemList()
    {
        throw new InvalidOperationException($"The \"{GetType().Name}.{nameof(GetItemList)}\" method must be override and implemented.");
    }

    /// <summary>
    /// Load the list of items corresponding to the argument filter.
    /// </summary>
    /// <param name="e">Event args.</param>
    /// <returns>The loaded list.</returns>
    private async Task<IEnumerable<IListItemData<TValue>>> GetItemDataListAsync(GetItemsAsyncEventArgs<TValue> e)
    {
        // We keep the list in memory
        return ToIListItemDataEnumerable(await OnGetItemsAsync(e)).ToList();
    }

    /// <summary>
    /// Load the list of items corresponding to the argument filter.
    /// </summary>
    /// <param name="e">Event args.</param>
    /// <returns>The loaded list.</returns>
    protected internal virtual Task<IEnumerable<TItem>> OnGetItemsAsync(GetItemsAsyncEventArgs<TValue> e)
    {
        throw new InvalidOperationException($"The \"{nameof(OnGetItemsAsync)}\" method must be override and implemented.");
    }

    /// <summary>
    /// Wrap each value into a property reader.
    /// </summary>
    /// <param name="items">Items.</param>
    /// <returns>The new enumerable.</returns>
    private IEnumerable<IListItemData<TValue>> ToIListItemDataEnumerable(IEnumerable<TItem> items)
    {
        return items.Select(x => (IListItemData<TValue>)new ListDataSourceItem<TItem, TValue>(x, this));
    }

    /// <summary>
    /// Gets the value representing the item.
    /// </summary>
    /// <param name="item">The source item.</param>
    /// <returns>The value representing the item.</returns>
    public abstract TValue GetItemValue(TItem item);

    /// <summary>
    /// Gets the text representing the item.
    /// </summary>
    /// <param name="item">The source item.</param>
    /// <returns>The text representing the item.</returns>
    public abstract string GetItemText(TItem item);

    /// <summary>
    /// Gets if the item is disabled.
    /// </summary>
    /// <param name="item">The source item.</param>
    /// <returns><c>true</c> if the item is disabled.</returns>
    public virtual bool GetItemDisabled(TItem item)
    {
        return false;
    }

    /// <summary>
    /// Gets the tooltip of the item.
    /// </summary>
    /// <param name="item">The source item.</param>
    /// <returns>The tooltip of the item.</returns>
    public virtual string GetItemTooltip(TItem item)
    {
        return null;
    }

    /// <summary>
    /// Gets the CSS classes for the item.
    /// </summary>
    /// <param name="item">The source item.</param>
    /// <returns>The CSS classes for the item.</returns>
    public virtual string GetItemCssClass(TItem item)
    {
        return null;
    }

    /// <summary>
    /// Gets the icon name for the item.
    /// </summary>
    /// <param name="item">The source item.</param>
    /// <returns>The icon name for the item.</returns>
    public virtual string GetItemIconName(TItem item)
    {
        return null;
    }



    #endregion

}
