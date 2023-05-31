namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Content of the dropdown list for a select.
/// </summary>
public partial class LgSelectTray<TValue, TItemValue> : LgComponentBase
{

    #region fields

    /// <summary>
    /// Gets or sets the last state.
    /// </summary>
    private bool _wasOpen;

    /// <summary>
    /// State of the async load items method.
    /// </summary>
    private readonly AsyncLoadState<TItemValue> _loadState = new();

    /// <summary>
    /// Last text searched.
    /// </summary>
    private string _lastSearchText = "";

    /// <summary>
    /// Message icon name.
    /// </summary>
    private string _msgIconName;

    /// <summary>
    /// Message description.
    /// </summary>
    private string _msgDescription;

    /// <summary>
    /// Is opening the dropdown list
    /// </summary>
    private bool _isOpening;

    /// <summary>
    /// Is closing the dropdown list
    /// </summary>
    private bool _isClosing;

    /// <summary>
    /// Last update guid
    /// </summary>
    private Guid _lastSelectionGuid;

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets the ILgSelect interface.
    /// </summary>
    [CascadingParameter]
    public ILgSelect<TItemValue> ILgSelect { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets if the bar is opened.
    /// </summary>
    [Parameter]
    public bool IsOpen { get; set; }

    /// <summary>
    /// Gets or sets the filter text.
    /// </summary>
    [Parameter]
    public string SearchText { get; set; }

    /// <summary>
    /// Guid for tacking selection changes.
    /// </summary>
    [Parameter]
    public Guid SelectionGuid { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets the parent LgSelectBase component.
    /// </summary>
    public LgSelectBase<TValue, TItemValue> Select => (LgSelectBase<TValue, TItemValue>)ILgSelect;

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        // Configuration du message
        UpdateMessage();
        // Check the list visibility state
        if (IsOpen != _wasOpen)
        {
            // Keep the last state
            _wasOpen = IsOpen;
            _isOpening = IsOpen;
            _isClosing = !IsOpen;
        }
        else
        {
            _isOpening = false;
            _isClosing = false;
        }
        //// Prepare the async loading
        if (Select.HasAsyncLoading)
        {
            if (_isOpening)
            {
                // Retry after loading errors
                if (_loadState.HasLoadingError)
                {
                    Select.WorkingItems = null;
                    _loadState.HasLoadingError = false;
                }
                // Refresh data on open
                if (Select.WorkingItems is null)
                {
                    // We start a data download only if a download is not running
                    _loadState.IsPending = !_loadState.IsLoading;
                }
            }
            else if (IsOpen && Select.MinSearchCharacter > 0 && SearchText != _lastSearchText)
            {
                // We download data corresponding to the new filter
                Select.WorkingItems = null;
                _loadState.IsPending = true;
            }
        }
        else if(Select.OnFilterChange.HasDelegate && IsOpen && Select.MinSearchCharacter > 0 && SearchText != _lastSearchText)
        {
            // Handle the OnFilterChange when items are defined into the ChildContent
            _loadState.IsPending = true;
        }
    }

    ///<inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        if (_isClosing)
        {
            await JS.InvokeVoidAsync("Lagoon.LgSelect.onClosing", Select.ElementRef);
        }
        if (_loadState.IsPending)
        {
            await LoadItemsAsync();
        }
    }

    /// <summary>
    /// Call the async method to load items.
    /// </summary>
    /// <returns>The async method to load items.</returns>
    private async Task LoadItemsAsync()
    {
        _lastSearchText = SearchText ?? "";
        EventCallback<FilterChangeEventArgs>? onFilterChange = Select.OnFilterChange.HasDelegate 
            && SearchText?.Length >= Select.MinSearchCharacter ? Select.OnFilterChange : null;
        await _loadState.LoadAndProcessItemsAsync(this, Select.HasAsyncLoading ? Select.OnLoadItemsAsync : null,
            onFilterChange, _lastSearchText);
        UpdateMessage();
    }

    /// <summary>
    /// Cancel the rendering (except the first one) if the list is not visible.
    /// </summary>
    protected override bool ShouldRender()
    {
        var shouldRender = IsOpen || !_lastSelectionGuid.Equals(SelectionGuid);
        _lastSelectionGuid = SelectionGuid;
        return shouldRender;
    }

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (_isOpening)
        {
            await JS.InvokeVoidAsync("Lagoon.LgSelect.onOpening", Select.ElementRef, Select._dotnetRef);
        }
        // Try to refresh the "Select All" state
        if (Select.ShowList)
        {
            Select.SelectAllManager?.UpdateRender();
        }
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (JS is IJSInProcessRuntime ijs)
        {
            ijs.InvokeVoid("Lagoon.LgSelect.dispose", Select.ElementRef);
        }
        base.Dispose(disposing);
    }

    private void UpdateMessage()
    {
        if (SearchText?.Length < Select.MinSearchCharacter)
        {
            _msgIconName = null;
            _msgDescription = "lgSelectInputTooShort".Translate(Select.MinSearchCharacter);
        }
        else if (_loadState.HasLoadingError)
        {
            _msgIconName = IconNames.Error;
            _msgDescription = "lgSelectErrorLoading".Translate();
        }
        else
        {
            _msgIconName = IconNames.Empty;
            _msgDescription = "lgSelectNoResult".Translate();
        }
    }

    #endregion
}
