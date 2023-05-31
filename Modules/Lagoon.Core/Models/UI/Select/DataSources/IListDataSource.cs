namespace Lagoon.UI.Components;

/// <summary>
/// Interface to get items for the LgSelect, LgSelectMultiple, ... components.
/// </summary>
/// <typeparam name="TItemValue">The type of component <c>Value</c> property.</typeparam>
public interface IListDataSource<TItemValue> : IEqualityComparer<TItemValue>
{

    /// <summary>
    /// Indicate if the item list will be from an asynchrone method or a synchrone method.
    /// </summary>
    bool HasAsyncLoading { get; }        

    /// <summary>
    /// Load the list of items corresponding to the argument filter.
    /// </summary>
    /// <returns>The loaded list.</returns>
    IEnumerable<IListItemData<TItemValue>> GetItems();

    /// <summary>
    /// Load the list of items corresponding to the argument filter.
    /// </summary>
    /// <param name="e">Event args.</param>
    /// <returns>The loaded list.</returns>
    Task<IEnumerable<IListItemData<TItemValue>>> GetItemsAsync(GetItemsAsyncEventArgs<TItemValue> e);

}
