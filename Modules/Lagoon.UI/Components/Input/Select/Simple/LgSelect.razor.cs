namespace Lagoon.UI.Components;

/// <summary>
///  A select component.
/// </summary>
/// <typeparam name="TValue">Type of value</typeparam>
public partial class LgSelect<TValue> : LgSelectBase<TValue, TValue>
{
    #region fields

    /// <summary>
    /// Additional informations about the last select value.
    /// </summary>
    private ListItemData<TValue> _selectedItemData;

    /// <summary>
    /// Value available the last time the list was openned.
    /// </summary>
    private TValue _lastValue;

    #endregion

    #region methods        

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        TrackSelectionUpdate();
    }

    /// <summary>
    /// Get the items to render.
    /// </summary>
    /// <returns></returns>
    private ListItemData<TValue> GetRenderingSelectedItemData()
    {
        HasUnknownValues = _selectedItemData is null || !ValueEqualityComparer.Equals(Value, _selectedItemData.Value);
        if (HasUnknownValues)
        {
            return null;
        }
        return _selectedItemData;
    }

    /// <summary>
    /// Tests whether an item is selected.
    /// </summary>
    /// <param name="item">The item to test.</param>
    /// <returns><c>true</c> if the item is selected; otherwise, <c>false</c>.</returns>
    protected bool IsSelected(LgOptionListItem<TValue> item)
    {
        return Value != null && Value.Equals(item.Value);
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("select-dropdown-simple");
    }

    ///<inheritdoc/>
    internal override bool OnInitItemSelection(LgOptionListItem<TValue> item)
    {
        return KeepItemData(item);
    }

    ///<inheritdoc/>
    internal override bool KeepItemData(IListItemData<TValue> item)
    {
        bool isSelected = ValueEqualityComparer.Equals(Value, item.GetValue());
        if (isSelected)
        {
            _selectedItemData = new ListItemData<TValue>(item);
        }
        return isSelected;
    }

    ///<inheritdoc/>
    internal override Guid OnGetSelectionGuid(bool trackUpdate)
    {
        if (trackUpdate)
        {
            TrackSelectionUpdate();
        }
        return SelectionGuid;
    }

    /// <summary>
    /// Check if selection has changed
    /// </summary>
    private void TrackSelectionUpdate()
    {
        if (!ValueEqualityComparer.Equals(_lastValue, Value))
        {
            SelectionGuid = Guid.NewGuid();
            _lastValue = Value;
        }
    }

    ///<inheritdoc/>
    internal override List<TValue> GetUnknownValues()
    {
        if (Value is null || (_selectedItemData is not null && ValueEqualityComparer.Equals(_selectedItemData.Value, Value)))
        {
            return null;
        }
        else
        {
            return new List<TValue> { Value };
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
        if (OptionItems.TryGetValue(oldValue, out IListItemData<TValue> oldItem))
        {
            if (oldItem == item)
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
    }

    ///<inheritdoc/>
    internal override void OnRemoveItem(LgOptionListItem<TValue> item)
    {
        if (OptionItems.TryGetValue(item.Value, out IListItemData<TValue> oldItem))
        {
            if (oldItem == item)
            {
                OptionItems.Remove(item.Value);
            }
        }
    }

    ///<inheritdoc/>
    internal override bool IsDropdownItemVisible(LgOptionListItem<TValue> item)
    {
        return IsItemDropdownVisible(item);
    }

    ///<inheritdoc/>
    internal override Task OnClickItemAsync(OptionEventArgs<TValue> arg)
    {
        throw new InvalidOperationException();
    }

    ///<inheritdoc/>
    internal override async Task OnSelectItemAsync(LgOptionListItem<TValue> item)
    {
        var keepValue = await SelectItemAsync(item.Value);
        if (keepValue)
        {
            KeepItemData(item);
        }
    }

    /// <summary>
    /// Call on click reset button
    /// </summary>
    /// <returns></returns>
    internal override Task OnResetValueAsync()
    {
        _selectedItemData = null;
        return SelectItemAsync(default);        
    }

    /// <summary>
    /// Call from LgOptionListItem: item selection
    /// </summary>
    /// <param name="value">Selected value</param>
    protected async Task<bool> SelectItemAsync(TValue value)
    {
        await ToggleListAsync(false);
        if (ValueEqualityComparer.Equals(Value, value))
        {
            // Close the tray when the selected option item is clicked
            StateHasChanged();
            return false;
        }
        else
        {                
            await OnValueChangeAsync(value);
        }
        return true;
    }

    /// <summary>
    /// Update selected value
    /// </summary>
    /// <returns></returns>
    private Task OnValueChangeAsync(TValue value)
    {                      
        // Close the list before the user process
        if (!ShowList)
        {
            StateHasChanged();
        }
        // Call a potentially long process
        return BaseChangeValueAsync(new ChangeEventArgs() { Value = value });
    }

    ///<inheritdoc/>
    protected override Task ChangeValueAsync(object value)
    {
        CurrentValue = (TValue)value;
        return Task.CompletedTask;
    }

    #endregion

}