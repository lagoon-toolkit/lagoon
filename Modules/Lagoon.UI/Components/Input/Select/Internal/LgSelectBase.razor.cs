using Lagoon.UI.Components.Input.Internal;

namespace Lagoon.UI.Components.Internal;


/// <summary>
/// Base class for LgSelect and LgSelectMultiple component
/// </summary>
/// <typeparam name="TValue">Select type</typeparam>
/// <typeparam name="TItemValue">Item value type</typeparam>
public abstract partial class LgSelectBase<TValue, TItemValue> : LgInputRenderBase<TValue>, ILgSelect<TItemValue>, IInputCommonProperties
{
    #region constants

    internal const StringComparison STRING_COMPARISON = StringComparison.CurrentCultureIgnoreCase;

    #endregion

    #region fields

    /// <summary>
    /// Display or not the items list
    /// </summary>
    private bool _showList;

    /// <summary>
    /// Gets or sets the last state.
    /// </summary>
    private bool _wasOpen;

    /// <summary>
    /// Text searched into items list (it can't be null to optimize options visibility test).
    /// </summary>
    private string _searchText = "";

    /// <summary>
    /// Reset button text
    /// </summary>
    private string _resetButtonText;
    /// <summary>
    /// Reset button arialabel
    /// </summary>
    private string _resetButtonAriaLabel;

    /// <summary>
    /// Hide / Show items list token
    /// </summary>
    private CancellationTokenSource _tokenDropdown;

    /// <summary>
    /// Data source from enumeration.
    /// </summary>
    private IListDataSource<TItemValue> _autoDS;

    /// <summary>
    /// Rise focus JS 
    /// </summary>
    private bool _focus;

    /// <summary>
    /// Rise focus JS on last option 
    /// </summary>
    private bool _focusLast;

    /// <summary>
    /// Rise focus JS on first option 
    /// </summary>
    private bool _focusFirst;

    /// <summary>
    /// DotNet object reference
    /// </summary>
    internal IDisposable _dotnetRef;

    /// <summary>
    /// Cancellation token source to cancel the search.
    /// </summary>
    private CancellationTokenSource _searchCancellationTokenSource;

    /// <summary>
    /// Indicate if the reset focus JS must be executed
    /// </summary>
    private bool _resetFocus;

    #endregion

    #region properties

    /// <summary>
    /// Dropdown button content
    /// </summary>
    protected RenderFragment ButtonContent { get; set; }

    /// <summary>
    /// Gets or sets select render
    /// </summary>
    protected RenderFragment Select { get; set; }

    /// <summary>
    /// Display or not the items list
    /// </summary>
    internal bool ShowList => _showList;

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    internal ElementReference ElementRef { get; set; }

    ///<inheritdoc/>
    protected override ElementReference FocusElementRef => ElementRef;

    /// <summary>
    /// Options item list (in dropdown)
    /// </summary>
    internal Dictionary<TItemValue, IListItemData<TItemValue>> OptionItems { get; set; } = new();

    /// <summary>
    /// List of the items from the source.
    /// </summary>
    internal IEnumerable<IListItemData<TItemValue>> WorkingItems { get; set; }

    /// <summary>
    /// Data source from enum or from parameter.
    /// </summary>
    internal IListDataSource<TItemValue> WorkingDataSource { get; set; }

    /// <summary>
    /// Gets if the data are loaded when the tray is openned.
    /// </summary>
    internal bool HasAsyncLoading { get; private set; }

    /// <summary>
    /// Indicate if the dropdown list has checkboxes.
    /// </summary>
    internal virtual bool HasCheckBoxes { get; }

    /// <summary>
    /// Indicate if the readonly mode is text.
    /// </summary>
    internal virtual bool ReadOnlyAsBubble { get; }

    /// <summary>
    /// Method to compare two values.
    /// </summary>
    internal IEqualityComparer<TItemValue> ValueEqualityComparer { get; set; }

    /// <summary>
    /// Indicate if a selected value is unknown.
    /// </summary>
    internal bool HasUnknownValues { get; set; }

    /// <summary>
    /// Guid for tacking selection changes.
    /// </summary>
    internal Guid SelectionGuid { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets indicator of multiple selection
    /// </summary>

    internal virtual bool Multiple => false;

    /// <summary>
    /// Gets the number of selected and unselected items in the list.
    /// </summary>
    internal virtual SelectAllManager SelectAllManager => null;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    [Parameter]
    public IListDataSource<TItemValue> DataSource { get; set; }

    /// <summary>
    /// Gets or set the items of the list.
    /// </summary>
    [Parameter]
    public IEnumerable<TItemValue> DataSourceItems { get; set; }

    /// <summary>
    /// Gets or sets the items.
    /// </summary>
    [Parameter]
    public RenderFragment Items { get; set; }

    /// <summary>
    /// Gets or sets the select placeholder
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; }

    /// <summary>
    /// Fire when the user do a search
    /// </summary>
    /// <value></value>
    [Parameter]
    public EventCallback<FilterChangeEventArgs> OnFilterChange { get; set; }

    /// <summary>
    /// Minimal input length before 'OnFilterChanged' is fired (used in LoadOnDemand mode)
    /// </summary>
    /// <value>3 by default</value>
    [Parameter]
    public int MinSearchCharacter { get; set; } = 0;

    /// <summary>
    /// Disaply/Hide the search box which allow filter items
    /// </summary>
    /// <value><c>true</c> if activated (the default), <c>false</c> otherwise</value>
    [Parameter]
    public bool ShowSearchBox { get; set; } = true;

    /// <summary>
    /// CSS class to add to search box.
    /// </summary>
    [Parameter]
    public string SearchBoxCssClass { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the input mask
    /// </summary>
    [Parameter]
    public string InputMask { get; set; }

    /// <summary>
    /// Gets or sets the input mask kind
    /// </summary>
    [Parameter]
    public InputMaskKind InputMaskKind { get; set; }

    /// <summary>
    /// Gets or sets the input mask
    /// </summary>
    [Parameter]
    public string InputMaskPlaceholder { get; set; }

    #endregion

    #region events

    /// <summary>
    /// Event called when the extra-informations about the selected items are loaded (Text, IconName, ...)
    /// </summary>
    internal event Action OnUpdateSelection;

    /// <summary>
    /// Event called when the select refreshed and selected items don't have the "Text" value. 
    /// </summary>
    internal event Action OnReloadItemsContent;

    #endregion

    #region abstract method

    /// <summary>
    /// Add an item to the current selected item list
    /// </summary>
    /// <param name="item">item</param>
    internal abstract void OnAddItem(LgOptionListItem<TItemValue> item);

    /// <summary>
    /// Remove an item to the current selected item list
    /// </summary>
    /// <param name="item">item</param>
    internal abstract void OnRemoveItem(LgOptionListItem<TItemValue> item);

    /// <summary>
    /// Update an item to the current selected item list
    /// </summary>
    /// <param name="item">item</param>
    /// <param name="oldValue">previous item value</param>
    /// <param name="newValue">new item value</param>
    internal abstract void OnUpdateItem(LgOptionListItem<TItemValue> item, TItemValue oldValue, TItemValue newValue);

    /// <summary>
    /// Call from LgOptionListItem: item selection
    /// </summary>
    /// <param name="item">selected item</param>
    /// <returns></returns>
    internal abstract Task OnSelectItemAsync(LgOptionListItem<TItemValue> item);

    /// <summary>
    /// Does item is visible into dropdown list ?
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    internal abstract bool IsDropdownItemVisible(LgOptionListItem<TItemValue> item);

    /// <summary>
    /// item is selected ?
    /// </summary>
    /// <param name="item">item</param>
    /// <returns></returns>
    internal abstract bool OnInitItemSelection(LgOptionListItem<TItemValue> item);

    /// <summary>
    /// on click selected item (multiple only)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    internal abstract Task OnClickItemAsync(OptionEventArgs<TItemValue> item);

    /// <summary>
    /// Get the GUID corresponding the current version of selected values.
    /// </summary>
    /// <param name="trackUpdate">Check if the selection have been updated.</param>
    /// <returns>The GUID corresponding the current version of selected values.</returns>
    internal abstract Guid OnGetSelectionGuid(bool trackUpdate);

    #endregion

    #region ILgSelect implementation

    bool ILgSelect<TItemValue>.HasCheckBoxes => HasCheckBoxes;

    bool ILgSelect<TItemValue>.ReadOnly => ReadOnly;

    IEqualityComparer<TItemValue> ILgSelect<TItemValue>.ValueEqualityComparer => ValueEqualityComparer;

    void ILgSelect<TItemValue>.RemoveItem(LgOptionListItem<TItemValue> item)
    {
        OnRemoveItem(item);
    }

    void ILgSelect<TItemValue>.AddItem(LgOptionListItem<TItemValue> item)
    {
        item.OptionId = $"select-{ElementId}-{OptionItems.Count + 1}";
        OnAddItem(item);
    }

    void ILgSelect<TItemValue>.UpdateItem(LgOptionListItem<TItemValue> item, TItemValue oldValue, TItemValue newValue)
    {
        OnUpdateItem(item, oldValue, newValue);
    }

    Task ILgSelect<TItemValue>.SelectItemAsync(LgOptionListItem<TItemValue> item)
    {
        return OnSelectItemAsync(item);
    }

    bool ILgSelect<TItemValue>.IsDropdownItemVisible(LgOptionListItem<TItemValue> item)
    {
        return IsDropdownItemVisible(item);
    }

    bool ILgSelect<TItemValue>.InitItemSelection(LgOptionListItem<TItemValue> item)
    {
        return OnInitItemSelection(item);
    }

    Task ILgSelect<TItemValue>.OnClickItemAsync(OptionEventArgs<TItemValue> item)
    {
        return OnClickItemAsync(item);
    }

    #endregion

    #region methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (!ResetButton.HasValue)
        {
            ResetButton = !App.BehaviorConfiguration.Select.HideAlwaysResetButton;
        }
        _resetButtonText = App.BehaviorConfiguration.Select.ResetText;
        _resetButtonAriaLabel = App.BehaviorConfiguration.Select.ResetTextAriaLabel;
        _dotnetRef = DotNetObjectReference.Create(this);
    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Items is null)
        {
            if (DataSourceItems is not null)
            {
                if (_autoDS is ListDataSource<TItemValue> simpleDataSource)
                {
                    simpleDataSource.Items = DataSourceItems;
                }
                else
                {
                    _autoDS = new ListDataSource<TItemValue>(DataSourceItems);
                }
            }
            else
            {
                if (DataSource is null && _autoDS is null)
                {

                    Type type = typeof(TItemValue);
                    if (type.IsEnum)
                    {
                        _autoDS = (IListDataSource<TItemValue>)Activator.CreateInstance(typeof(EnumListDataSource<>).MakeGenericType(type));
                    }
                    else
                    {
                        type = Nullable.GetUnderlyingType(type);
                        if (type is not null && type.IsEnum)
                        {
                            _autoDS = (IListDataSource<TItemValue>)Activator.CreateInstance(typeof(NullableEnumListDataSource<>).MakeGenericType(type));
                        }
                    }

                }
            }
            WorkingDataSource = _autoDS ?? DataSource;
            // Check if the data are loaded dynamically
            HasAsyncLoading = WorkingDataSource is not null && WorkingDataSource.HasAsyncLoading;
            // If the items are from the load on demand, we keep values from cache (null the first time), else we use the "DataItems" parameter
            if (WorkingDataSource is not null && !WorkingDataSource.HasAsyncLoading)
            {
                WorkingItems = WorkingDataSource.GetItems();
            }
        }
        else
        {
            // Check the item list source
            if (DataSource is not null || DataSourceItems is not null)
            {
                throw new Exception($"You can't specify {nameof(DataSource)} / {nameof(DataSourceItems)} and {nameof(Items)} at the same time for a {GetType().Name} component.");
            }
        }
        // Get the equality comparison method
        ValueEqualityComparer = WorkingDataSource is null ? EqualityComparer<TItemValue>.Default : WorkingDataSource;
        // If there is unknow values
        if (HasUnknownValues)
        {
            ReloadItemsContent();
        }
    }

    ///<inheritdoc/>
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        // Show the selected item(s) with they're Text property in the button of the select component          
        if (!firstRender && HasUnknownValues)
        {
            ReloadItemsContent();
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Keyboard controller for Rgaa
            await JS.InvokeVoidAsync("Lagoon.LgSelect.keyBoardController", ElementRef, ShowSearchBox);
        }
        // Reset focus
        if (_resetFocus)
        {
            await JS.InvokeVoidAsync("Lagoon.LgSelect.resetFocus", ElementRef);
            _resetFocus = false;
        }
        // Focus option in list
        if (_focus)
        {
            await JS.InvokeVoidAsync("Lagoon.LgSelect.initFocus", ElementRef, _focusLast, _focusFirst);
            _focus = false;
            _focusLast = false;
            _focusFirst = false;
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        _dotnetRef?.Dispose();
        _searchCancellationTokenSource?.Dispose();
        base.Dispose(disposing);
    }

    /// <summary>
    /// Gets the items to be rendered.
    /// </summary>
    /// <returns>Items to be rendered.</returns>
    internal IEnumerable<IListItemData<TItemValue>> GetRenderingDataItems()
    {
        if (WorkingItems is null || _searchText.Length < MinSearchCharacter)
        {
            return Enumerable.Empty<IListItemData<TItemValue>>();
        }
        else if (_searchText.Length > 0)
        {
            return WorkingItems
                .Where(i => (i.GetText() ?? "").Contains(_searchText, STRING_COMPARISON))
                .OrderBy(i => (i.GetText() ?? "").IndexOf(_searchText, STRING_COMPARISON) != 0);
        }
        else
        {
            return WorkingItems;
        }
    }

    /// <summary>
    /// Gets the selected items extra informations (Title, IconName, ...)
    /// </summary>
    /// <param name="unknownValues">The values without additional informations.</param>
    internal void OnKeepItemsData(List<TItemValue> unknownValues)
    {
        foreach (IListItemData<TItemValue> item in GetItemsFiltredByValue(unknownValues))
        {
            KeepItemData(item);
        }
    }

    /// <summary>
    /// Load the list of items corresponding to the argument filter.
    /// </summary>
    /// <param name="unknownValues">The values without additional informations.</param>
    /// <returns>The loaded list.</returns>
    protected virtual IEnumerable<IListItemData<TItemValue>> GetItemsFiltredByValue(List<TItemValue> unknownValues)
    {
        if (WorkingItems is not null)
        {
            // From DataSource
            return WorkingItems.Where(c => unknownValues.Contains(c.GetValue(), ValueEqualityComparer));
        }
        else if (OptionItems is not null)
        {
            // From LgOptionListItem 
            return OptionItems.Values.Where(c => unknownValues.Contains(c.GetValue(), ValueEqualityComparer));
        }
        else
        {
            return Enumerable.Empty<IListItemData<TItemValue>>();
        }
    }

    /// <summary>
    /// Gets the selected items extra informations (Title, IconName, ...)
    /// </summary>
    internal async Task OnKeepItemsDataAsync(List<TItemValue> unknownValues, string searchText, Progress progress, CancellationToken cancellationToken)
    {
        foreach (IListItemData<TItemValue> item in await LoadItemsAsync(unknownValues, searchText, progress, cancellationToken))
        {
            KeepItemData(item);
        }
    }

    /// <summary>
    /// Gets the items to be rendered.
    /// </summary>
    internal async Task OnLoadItemsAsync(List<TItemValue> unknownValues, string searchText, Progress progress, CancellationToken cancellationToken)
    {
        WorkingItems = await LoadItemsAsync(unknownValues, searchText, progress, cancellationToken);
    }

    /// <summary>
    /// Gets the items to be rendered.
    /// </summary>
    /// <returns>Items to be rendered.</returns>
    internal async Task<IEnumerable<IListItemData<TItemValue>>> LoadItemsAsync(List<TItemValue> unknownValues, string searchText, Progress progress, CancellationToken cancellationToken)
    {
        bool onlyUnknown = unknownValues?.Count > 0;
        bool hasValue = Value is not null;

        // Empty list cases
        if ((onlyUnknown && !hasValue) || (!onlyUnknown && searchText.Length < MinSearchCharacter))
        {
            return Enumerable.Empty<IListItemData<TItemValue>>();
        }
        // Get items from the IListItemData provider
        if (WorkingItems is not null)
        {
            return WorkingItems;
        }
        // Get items from controller...
        if (HasAsyncLoading)
        {
            return await WorkingDataSource.GetItemsAsync(new GetItemsAsyncEventArgs<TItemValue>(unknownValues, searchText, progress,
                cancellationToken));
        }
        //No data source
        return Enumerable.Empty<IListItemData<TItemValue>>();
    }

    /// <summary>
    /// Keep item additional informations.
    /// </summary>
    /// <param name="item">Item with it's additionnal informations.</param>
    /// <returns><c>true</c> if the Value is in selection.</returns>
    internal virtual bool KeepItemData(IListItemData<TItemValue> item)
    {
        throw new InvalidOperationException();
    }

    /// <summary>
    /// Return current Guid selection
    /// </summary>
    /// <param name="ignoreListState"></param>
    /// <returns></returns>
    internal Guid GetSelectionGuid(bool ignoreListState)
    {
bool trackUpdate = ignoreListState || _showList;
        _wasOpen = _showList;
        return OnGetSelectionGuid(trackUpdate);
    }

    /// <summary>
    /// Gets the value with unknown extra informations (Title, IconName, ...)
    /// </summary>
    internal virtual List<TItemValue> GetUnknownValues() { throw new InvalidOperationException(); }

    /// <summary>
    /// Check if an item is visible into list
    /// </summary>
    /// <param name="item">item tested</param>
    /// <returns></returns>
    internal bool IsItemDropdownVisible(LgOptionListItem<TItemValue> item)
    {
        if (ReadOnly || Disabled)
        {
            return false;
        }
        if (_searchText.Length < MinSearchCharacter)
        {
            return false;
        }
        if (!IsItemMatchFilter(item))
        {
            return false;
        }
        if (item.Disabled && !item.IsSelected)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Check if the item match the searched text.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> if the item match the searched text.</returns>
    private bool IsItemMatchFilter(LgOptionListItem<TItemValue> item)
    {
        return Items is null || _searchText == "" || (item?.Text?.Contains(_searchText, STRING_COMPARISON) ?? false);
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.AddIf(string.IsNullOrEmpty(Label), "select-without-lbl");
        builder.AddIf(Disabled, "disabled");
        builder.AddIf(ReadOnly, "readonly");
        builder.Add("select-dropdown");
    }

    /// <inheritdoc/>
    protected override bool HasActiveLabel()
    {
        if (!String.IsNullOrEmpty(Placeholder) ||
            Value is not null)
        {
            return true;
        }
        return base.HasActiveLabel();
    }

    /// <summary>
    /// Toggle items list
    /// </summary> 
    [JSInvokable]
    public async Task CloseDropdownAsync()
    {
        await ToggleListAsync(false);
        StateHasChanged();
    }

    /// <summary>
    /// Toggle items list
    /// </summary> 
    internal Task ToggleListAsync()
    {
        return ToggleListAsync(!ShowList);
    }

    /// <summary>
    /// Toggle items list
    /// </summary> 
    internal Task ToggleListAsync(bool showList)
    {
        if (!Disabled && !ReadOnly && _showList != showList)
        {
            // Toggle the dropdown visibility
            _showList = showList;
            if (_showList)
            {
                // Reset the search text
                if (MinSearchCharacter == 0)
                {
                    _searchText = "";
                }                    
            }
            else
            {
                // Clear the temporary TabIndex
                _resetFocus = true;                    
            }
            _focus = true;
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Try to hide item list
    /// </summary>
    private void KeepListOpen()
    {
        _tokenDropdown?.Cancel();
    }

    /// <summary>
    /// Hide item list
    /// </summary>
    /// <returns></returns>
    [JSInvokable]
    public Task TryCloseListAsync()
    {
        _tokenDropdown = new CancellationTokenSource();
        return Task.Factory.StartNew(async () =>
        {
            // Wait 100 ms before hidding the tray in case of the target focus is another element inside the select
            await Task.Delay(100);
            await ToggleListAsync(false);
            // "InvokeAsync" is for blazor server or when Wasm will use more then one thread :  https://stackoverflow.com/a/65231017/3568845
            await InvokeAsync(StateHasChanged);
        }, _tokenDropdown.Token);
    }

    /// <summary>
    /// Call on searching value into select
    /// </summary>
    /// <param name="e">Event on change argument</param>
    /// <returns></returns>
    private async Task OnSearchAsync(ChangeEventArgs e)
    {
        // Wait a while before searching
        if (_searchCancellationTokenSource is not null)
        {
            _searchCancellationTokenSource.Cancel();
            _searchCancellationTokenSource.Dispose();
        }
        _searchCancellationTokenSource = new();
        await Task.Delay(App.BehaviorConfiguration.Select.DebounceTime, _searchCancellationTokenSource.Token);
        // Get the search text (it can't be null to optimize options visibility test)
        string newSearchText = (string)e.Value ?? "";
        if (newSearchText.Length < MinSearchCharacter)
        {
            newSearchText = "";
        }
        // Filter on the new text
        if (newSearchText != _searchText)
        {
            _searchText = newSearchText;
            if (!HasAsyncLoading && !OnFilterChange.HasDelegate)
            {
                // Force refresh each item to check if it must be display into dropdown list items
                foreach (LgOptionListItem<TItemValue> item in OptionItems.Values.Cast<LgOptionListItem<TItemValue>>())
                {
                    item.Refresh();
                }
            }
        }
    }

    /// <summary>
    /// Capture the key pressed events.
    /// </summary>
    /// <param name="args">Key pressed informations.</param>
    private async Task OnKeyDownAsync(KeyboardEventArgs args)
    {
        if (!ShowList)
        {
            // Open list
            if (args.Code == "ArrowDown" || args.Code == "ArrowUp"
                || args.Code == "Enter" || args.Code == "Space"
                || args.Code == "Home" || args.Code == "End")
            {
                await ToggleListAsync();
                _focus = true;
                _focusLast = args.Code == "End";
                _focusFirst = args.Code == "Home";
            }
            else if (args.CtrlKey && args.Code == "Delete")
            {
                await OnResetAsync();
            }
        }
    }

    /// <summary>
    /// Ask for the "Items" render fragment to re-render.
    /// </summary>
    internal void ReloadItemsContent()
    {
        OnReloadItemsContent?.Invoke();
    }

    internal void UpdateSelection()
    {
        OnUpdateSelection?.Invoke();
    }

    /// <summary>
    /// Return list of the options selected text
    /// </summary>
    /// <returns></returns>
    private string GetSelectedItemsText()
    {
        var selectedText = OptionItems?.Values
            .Where(o => ((LgOptionListItem<TItemValue>)o).IsSelected)
            .Select(o => o.GetText()).ToList();
        return string.Join(", ", selectedText);
    }

    /// <summary>
    /// Change the selection state for the item list.
    /// </summary>
    /// <param name="optionItems">List of items to update.</param>
    /// <param name="select">The new selection state</param>
    internal virtual Task ChangeSelectionAsync(IEnumerable<LgOptionListItem<TItemValue>> optionItems, bool select)
    {
        throw new InvalidOperationException();
    }

    ///<inheritdoc/>
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {
        builder.AddContent(0, Select);
    }

    #endregion

    #region parsing

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string value, out TValue result, out string validationErrorMessage)
    {
        throw new InvalidOperationException($"This component does not parse string inputs. Bind to the '{nameof(CurrentValue)}' property, not '{nameof(CurrentValueAsString)}'.");
    }

    ///<inheritdoc/>
    internal override bool TryParseValueFromString(string value, out TValue parsedValue)
    {
        parsedValue = default;
        return true;            
    }

    #endregion
}
