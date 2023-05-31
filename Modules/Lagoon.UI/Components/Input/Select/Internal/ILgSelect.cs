namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Interface to expose the LgSelect property
/// </summary>
public interface ILgSelect<TItemValue>
{

    #region properties

    /// <summary>
    /// Indicate if the dropdown list has checkboxes.
    /// </summary>
    bool HasCheckBoxes { get; }

    /// <summary>
    /// Gets if select is in readonly.
    /// </summary>
    bool ReadOnly { get; }

    /// <summary>
    /// Gets the value equality comparer.
    /// </summary>
    IEqualityComparer<TItemValue> ValueEqualityComparer { get; }

    #endregion

    #region methods

    #endregion

    /// <summary>
    /// Add an item to the current selected item list
    /// </summary>
    /// <param name="item">item</param>
    void AddItem(LgOptionListItem<TItemValue> item);

    /// <summary>
    /// Remove an item to the current selected item list
    /// </summary>
    /// <param name="item">item</param>
    void RemoveItem(LgOptionListItem<TItemValue> item);

    /// <summary>
    /// Update an item to the current selected item list
    /// </summary>
    /// <param name="item">item</param>
    /// <param name="oldValue">previous item value</param>
    /// <param name="newValue">new item value</param>
    void UpdateItem(LgOptionListItem<TItemValue> item, TItemValue oldValue, TItemValue newValue);

    /// <summary>
    /// Call from LgOptionListItem: item selection
    /// </summary>
    /// <param name="item">selected item</param>
    /// <returns></returns>
    Task SelectItemAsync(LgOptionListItem<TItemValue> item);

    /// <summary>
    /// Does item is visible into dropdown list ?
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool IsDropdownItemVisible(LgOptionListItem<TItemValue> item);

    /// <summary>
    /// item is selected ?
    /// </summary>
    /// <param name="item">item</param>
    /// <returns></returns>
    bool InitItemSelection(LgOptionListItem<TItemValue> item);

    /// <summary>
    /// on click selected item (multiple only)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task OnClickItemAsync(OptionEventArgs<TItemValue> item);

}
