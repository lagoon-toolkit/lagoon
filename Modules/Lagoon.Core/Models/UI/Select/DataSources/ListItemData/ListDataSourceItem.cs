namespace Lagoon.UI.Components.Internal;

internal class ListDataSourceItem<TSourceItem, TItemValue> : IListItemData<TItemValue>
{

    #region fields

    private readonly TSourceItem _sourceItem;
    private readonly ListDataSourceBase<TSourceItem, TItemValue> _listSource;

    #endregion

    #region methods

    string IListItemData<TItemValue>.GetCssClass()
    {
        return _listSource.GetItemCssClass(_sourceItem);
    }

    bool IListItemData<TItemValue>.GetDisabled()
    {
        return _listSource.GetItemDisabled(_sourceItem);
    }

    string IListItemData<TItemValue>.GetIconName()
    {
        return _listSource.GetItemIconName(_sourceItem);
    }

    string IListItemData<TItemValue>.GetText()
    {
        return _listSource.GetItemText(_sourceItem);
    }

    string IListItemData<TItemValue>.GetTooltip()
    {
        return _listSource.GetItemTooltip(_sourceItem);
    }

    TItemValue IListItemData<TItemValue>.GetValue()
    {
        return _listSource.GetItemValue(_sourceItem);
    }

    #endregion

    #region constructors

    public ListDataSourceItem(TSourceItem source, ListDataSourceBase<TSourceItem, TItemValue> listSource)
    {
        _sourceItem = source;
        _listSource = listSource;
    }

    #endregion

}
