namespace Lagoon.UI.Components;

/// <summary>
/// A select multiple component.
/// </summary>
/// <typeparam name="TValue">Type of value</typeparam>
public partial class LgSelectMultiple<TValue> : LgSelectBase<List<TValue>, TValue>
{

    #region fields

    /// <summary>
    /// Additional informations about the last selecte values.
    /// </summary>
    private readonly Dictionary<TValue, ListItemData<TValue>> _selectedItemsData = new();

    /// <summary>
    /// Value available the last time the list was openned.
    /// </summary>
    private List<TValue> _lastValue;

    /// <summary>
    /// Gets the number of selected and unselected items in the list.
    /// </summary>
    private SelectAllManager _selectionCounter = new();

    #endregion

    #region parameters

    /// <summary>
    /// Fire when the user click on an item
    /// </summary>
    /// <value></value>
    [Parameter]
    public EventCallback<OptionEventArgs<TValue>> OnItemClick { get; set; }

    /// <summary>
    /// Fire when the user remove an item
    /// </summary>
    /// <value></value>
    [Parameter]
    public EventCallback<OptionEventArgs<TValue>> OnItemRemove { get; set; }

    /// <summary>
    /// Gets or sets the number of selected elements displayed
    /// </summary>
    [Parameter]
    public int? VisibleItemCount { get; set; }

    #endregion

    #region properties

    ///<inheritdoc/>
    internal override bool HasCheckBoxes => true;

    ///<inheritdoc/>
    internal override bool ReadOnlyAsBubble => true;

    ///<inheritdoc/>
    internal override bool Multiple => true;

    ///<inheritdoc/>
    internal override SelectAllManager SelectAllManager => _selectionCounter;

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("select-dropdown-multiple");
        if(VisibleItemCount is not null)
        {
            builder.Add("select-no-wrap");
        }
    }

    #endregion

    #region interface

    ///<inheritdoc/>
    internal override bool OnInitItemSelection(LgOptionListItem<TValue> item)
    {
        bool selected = KeepItemData(item);
        return selected;
    }

    ///<inheritdoc/>
    internal override bool KeepItemData(IListItemData<TValue> item)
    {
        bool isSelected = Value != null && Value.Contains(item.GetValue());
        if (isSelected)
        {
            ListItemData<TValue> itemData = new(item);
            if (!_selectedItemsData.TryAdd(itemData.Value, itemData))
            {
                _selectedItemsData[itemData.Value] = itemData;
            }
        }
        return isSelected;
    }

    ///<inheritdoc/>
    internal override Guid OnGetSelectionGuid(bool trackUpdate)
    {
        if (Value is null && _lastValue is null)
        {
            return SelectionGuid;
        }
        if (trackUpdate && (_lastValue is null || Value is null || !Enumerable.SequenceEqual(_lastValue, Value)))
        {
            SelectionGuid = Guid.NewGuid();
            _lastValue = Value is null ? null : new List<TValue>(Value);
        }
        return SelectionGuid;
    }

    ///<inheritdoc/>
    internal override List<TValue> GetUnknownValues()
    {
        if (Value is null)
        {
            return null;
        }
        else
        {
            return Value.Where(v => !_selectedItemsData.ContainsKey(v)).ToList();
        }
    }

    ///<inheritdoc/>
    internal override void OnAddItem(LgOptionListItem<TValue> item)
    {
        if (!OptionItems.ContainsKey(item.Value))
        {
            OptionItems.Add(item.Value, item);
        }
    }

    ///<inheritdoc/>
    internal override void OnUpdateItem(LgOptionListItem<TValue> item, TValue oldValue, TValue newValue)
    {
        if (OptionItems.TryGetValue(oldValue, out IListItemData<TValue> oldItem) && oldItem == item)
        {
            OptionItems.Remove(oldValue);
            if (!OptionItems.TryAdd(newValue, item))
            {
                // Value already exists => force update with remove and add item
                OptionItems.Remove(newValue);
                OptionItems.Add(newValue, item);
            }
        }
    }

    ///<inheritdoc/>
    internal override void OnRemoveItem(LgOptionListItem<TValue> item)
    {
        if (OptionItems.TryGetValue(item.Value, out IListItemData<TValue> savedItem) && savedItem == item)
        {
            OptionItems.Remove(item.Value);
        }
    }

    ///<inheritdoc/>
    internal override Task OnSelectItemAsync(LgOptionListItem<TValue> item)
    {
        SavePreviousValue();
        if (Value is null || !Value.Contains(item.Value))
        {
            Value ??= new List<TValue>();
            Value.Add(item.Value);
            KeepItemData(item);
        }
        else
        {
            // Remove duplicated values of the item
            while (Value.Remove(item.Value))
            { }
            if (Value.Count == 0)
            {
                Value = null;
            }
        }
        return OnValueChangeAsync();
    }

    ///<inheritdoc/>
    internal override bool IsDropdownItemVisible(LgOptionListItem<TValue> item)
    {
        return IsItemDropdownVisible(item);
    }

    ///<inheritdoc/>
    internal override async Task OnClickItemAsync(OptionEventArgs<TValue> arg)
    {
        if (OnItemClick.HasDelegate)
        {
            await OnItemClick.TryInvokeAsync(App, arg);
        }
    }

    #endregion

    #region events

    /// <summary>
    /// Update selected value
    /// </summary>
    /// <returns></returns>
    private Task OnValueChangeAsync()
    {
        return BaseChangeValueAsync(new ChangeEventArgs { Value = Value });
    }

    ///<inheritdoc/>
    protected override async Task ChangeValueAsync(object value)
    {
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.TryInvokeAsync(App, Value);
        }
        // Clear/Show validation message
        EditContext?.NotifyFieldChanged(FieldIdentifier.Value);
        // Update the SelectAll checkbox
        SelectAllManager.UpdateRender();
    }

    /// <summary>
    /// Remove an item from the selection
    /// </summary>
    /// <param name="value">selected value</param>
    /// <returns></returns>
    private async Task OnRemoveItemAsync(TValue value)
    {
        SavePreviousValue();
        if (Value is not null && Value.Remove(value))
        {
            if (Value.Count == 0)
            {
                Value = null;
            }
            await OnValueChangeAsync();
            await ToggleListAsync(false);
        }
    }

    /// <summary>
    /// Clear field - remove all selected values
    /// </summary>
    /// <returns></returns>
    internal override async Task OnResetValueAsync()
    {            
        if (Value is not null)
        {
            SavePreviousValue();
            Value = null;
            _selectedItemsData.Clear();
            await ToggleListAsync(false);
            await OnValueChangeAsync();
        }
    }

    /// <summary>
    /// Change the selection state for the item list.
    /// </summary>
    /// <param name="optionItems">List of items to update.</param>
    /// <param name="select">The new selection state</param>
    internal override Task ChangeSelectionAsync(IEnumerable<LgOptionListItem<TValue>> optionItems, bool select)
    {
        SavePreviousValue();
        if (select)
        {
            Value ??= new List<TValue>();                
            Value.AddRange(optionItems.Select(x => x.Value));
        }
        else if (Value is not null)
        {
            foreach (LgOptionListItem<TValue> optionItem in optionItems)
            {
                Value.Remove(optionItem.Value);
            }
            if (Value.Count == 0)
            {
                Value = null;
            }
        }
        return OnValueChangeAsync();
    }

    /// <summary>
    /// Keep previous list
    /// </summary>
    private void SavePreviousValue()
    {
        PreviousValue = Value?.ToList();
    }

    ///<inheritdoc/>
    public override Task CancelValueActionAsync()
    {
        Value = PreviousValue?.ToList();
        return ChangeValueAsync(null);
    }

    ///<inheritdoc/>
    protected override void RestoreOriginalValue()
    {
        base.RestoreOriginalValue();
        CancelValueAsync().GetAwaiter().GetResult();
    }

    #endregion

}