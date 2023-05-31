namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Button of a Select commponent.
/// </summary>
public partial class LgSelectButton<TValue, TItemValue> : LgComponentBase
{

    #region fields

    /// <summary>
    /// Value with unknown extra informations (Title, IconName, ...)
    /// </summary>
    private List<TItemValue> _unknownValues;

    /// <summary>
    /// The last unknown value list.
    /// </summary>
    private List<TItemValue> _lastUnknownValues;

    /// <summary>
    /// State of the async load items method.
    /// </summary>
    private readonly AsyncLoadState<TItemValue> _loadState = new();

    #endregion

    #region parameters

    /// <summary>
    /// Content of the button.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Content of the readonly button.
    /// </summary>
    [Parameter]
    public RenderFragment ReadOnlyContent { get; set; }

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets the ILgSelect interface.
    /// </summary>
    [CascadingParameter]
    public ILgSelect<TItemValue> ILgSelect { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets the parent LgSelectBase component.
    /// </summary>
    public LgSelectBase<TValue, TItemValue> Select => (LgSelectBase<TValue, TItemValue>)ILgSelect;

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        Select.OnUpdateSelection += OnUpdateSelection;
        base.OnInitialized();
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        Select.OnUpdateSelection -= OnUpdateSelection;
        base.Dispose(disposing);
    }

    /// <summary>
    /// Refreshing the selection when it's change.
    /// </summary>
    private void OnUpdateSelection()
    {
        StateHasChanged();
    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _unknownValues = Select.GetUnknownValues();
        if (_unknownValues?.Count > 0)
        {
            if(Select.WorkingDataSource is not null && Select.WorkingDataSource.HasAsyncLoading)
            {
                // Ask to load the unknow items (Cancel the current loading only if the unknown items aren't the same)
                _loadState.IsPending = _lastUnknownValues is null || !Enumerable.SequenceEqual(_lastUnknownValues, _unknownValues);
            }
            else
            {
                // Load the missings items
                Select.OnKeepItemsData(_unknownValues);
            }
        }
    }

    ///<inheritdoc/>
    protected override Task OnParametersSetAsync()
    {
        if (_loadState.IsPending)
        {
            return LoadItemsAsync();
        }
        else
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Call the async method to load items.
    /// </summary>
    /// <returns>The async method to load items.</returns>
    private async Task LoadItemsAsync()
    {
        _lastUnknownValues = _unknownValues;
        await _loadState.LoadAndProcessItemsAsync(this, Select.OnKeepItemsDataAsync, null, null, _unknownValues);
        Select.UpdateSelection();
    }

    #endregion

}
