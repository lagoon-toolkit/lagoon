namespace Lagoon.UI.Components.Internal;

/// <summary>
/// A component to edit filters.
/// </summary>
/// <typeparam name="TValue">The type of value filtred.</typeparam>
/// <typeparam name="TFilter">The type of filter for the value.</typeparam>
public abstract class LgFilterEditorBase<TValue, TFilter> : LgComponentBase, ILgFilterEditor
    where TFilter : Filter<TValue>
{

    #region fields

    /// <summary>
    /// Search cancellation token
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource = new();

    /// <summary>
    /// List of the tabs.
    /// </summary>
    private List<FilterTab> _tabs;

    /// <summary>
    /// The items to display in the list.
    /// </summary>
    private List<TValue> _workingItems;

    #endregion

    #region cascading parameters

    /// <summary>
    /// The FilterBox component that contains the filter input zone.
    /// </summary>
    [CascadingParameter]
    public LgFilterBoxBase<TValue, TFilter> FilterBox { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Get or sets the selected tab.
    /// </summary>
    public FilterTab SelectedTab { get; set; }

    /// <summary>
    /// Gets the tabs shown in the editor.
    /// </summary>
    public IEnumerable<FilterTab> Tabs => _tabs;

    /// <summary>
    /// Get the availlable values.
    /// </summary>
    protected List<TValue> WorkingItems => _workingItems;

    #endregion

    #region render fragments

    /// <summary>
    /// Gets or sets filter content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region methods               

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        FilterBox.FilterEditor = this;
        // Load the tab list
        FilterTab activeTabs = FilterBox.ActiveTabs;
        _tabs = new();
        if (activeTabs.HasFlag(FilterTab.Selection))
        {
            _tabs.Add(FilterTab.Selection);
        }
        if (activeTabs.HasFlag(FilterTab.Rules))
        {
            _tabs.Add(FilterTab.Rules);
        }

        if (_tabs.Count == 0)
        {
            _tabs = null;
        }
        // Initialize editor with the current filter value
        LoadFilterParameter(FilterBox.Filter);
        // Initialize the selected tab
        if (_tabs is not null)
        {
            if (!_tabs.Contains(SelectedTab))
            {
                SelectedTab = FilterBox.DefaultTab;
            }
            if (!_tabs.Contains(SelectedTab))
            {
                SelectedTab = _tabs[0];
            }
        }
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        _cancellationTokenSource.Cancel();
        FilterBox.FilterEditor = null;
        base.Dispose(disposing);
    }

    /// <summary>
    /// Initialize editor from filter value.
    /// </summary>
    protected abstract void LoadFilterParameter(TFilter Filter);

    ///<inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        // Check if the working items will be used
        if (WorkingItemsRequired())
        {
            // Load the data
            try
            {
                IEnumerable<TValue> items = await GetWorkingItemsAsync(_cancellationTokenSource.Token);
                // Work on a list
                if (items is null)
                {
                    _workingItems = new();
                }
                else
                {
                    _workingItems = items.ToList();
                }
            }
            catch (TaskCanceledException)
            {
#if DEBUG
                App.TraceDebug("Filter cancelled.");
#endif
            }
        }
    }

    /// <summary>
    /// Indicate if the list of existing items must be loaded.
    /// </summary>
    /// <returns><c>true</c> if the list of existing items must be loaded.</returns>
    protected virtual bool WorkingItemsRequired()
    {
        return _tabs is null || _tabs.Contains(FilterTab.Selection);
    }

    /// <summary>
    /// Get the available values for the filter.
    /// </summary>
    /// <returns>The available values for the filter.</returns>
    protected virtual async Task<IEnumerable<TValue>> GetWorkingItemsAsync(CancellationToken cancellationToken)
    {
        if (FilterBox.GetItems is null)
        {
#pragma warning disable CS0618 // This parameter can be used only here
            return FilterBox.Items;
#pragma warning restore CS0618
        }
        else
        {
            return await FilterBox.GetItems(cancellationToken);
        }
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("filterbox-filters");
    }

    /// <summary>
    /// Get the new filter from the filter editor.
    /// </summary>
    /// <returns>The new filter.</returns>
    internal TFilter GetFilter()
    {
        return BuildFilter();
    }

    /// <summary>
    /// Method to build the filter from the current user selection.
    /// </summary>
    /// <returns>The new filter.</returns>
    protected abstract TFilter BuildFilter();


    #endregion

}
