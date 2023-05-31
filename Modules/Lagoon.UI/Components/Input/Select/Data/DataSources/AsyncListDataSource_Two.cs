namespace Lagoon.UI.Components;

/// <summary>
/// Provide the list of items to fill a component.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TValue"></typeparam>
public abstract class AsyncListDataSource<TItem, TValue> : ListDataSourceBase<TItem, TValue>
{

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public AsyncListDataSource() : base(true)
    {
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override Task<IEnumerable<TItem>> OnGetItemsAsync(GetItemsAsyncEventArgs<TValue> e)
    {
        if (e.OnlyUnknown)
        {
            return GetItemsFiltredByValueAsync(new GetItemsFiltredByValueAsyncEventArgs<TValue>(e));
        }
        else
        {
            return GetItemsFiltredByTextAsync(new GetItemsFiltredByTextAsyncEventArgs<TValue>(e));
        }
    }

    /// <summary>
    /// Load the list of items corresponding to the argument filter.
    /// </summary>
    /// <param name="e">Event args.</param>
    /// <returns>The loaded list.</returns>
    protected abstract Task<IEnumerable<TItem>> GetItemsFiltredByTextAsync(GetItemsFiltredByTextAsyncEventArgs<TValue> e);

    /// <summary>
    /// Load the list of items corresponding to the values passed in event args.
    /// </summary>
    /// <param name="e">Event args.</param>
    /// <returns>The loaded list.</returns>
    protected abstract Task<IEnumerable<TItem>> GetItemsFiltredByValueAsync(GetItemsFiltredByValueAsyncEventArgs<TValue> e);

    #endregion

}
