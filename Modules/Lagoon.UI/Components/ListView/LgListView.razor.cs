namespace Lagoon.UI.Components;

/// <summary>
/// List view component.
/// </summary>
/// <typeparam name="TItem">Items type.</typeparam>
public partial class LgListView<TItem> : LgAriaComponentBase
{

    #region fields

    /// <summary>
    /// sort model
    /// </summary>
    private SortListViewModel _sortModel;

    private IList<string> _sortProperties;

    /// <summary>
    /// Indicate sortproteprties must be ordered by asc or desc
    /// </summary>
    private bool _sortAscending = true;

    /// <summary>
    /// Corresponding button to sort ascending choice
    /// </summary>
    private string _sortButtonIcon = IconNames.All.ArrowDown;

    /// <summary>
    /// State of the load/sort items method.
    /// </summary>
    private bool _isLoading = false;

    /// <summary>
    /// Collapse state of groups
    /// </summary>
    private Dictionary<object, bool> _groupsCollapseState;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the CSS classes.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the view title
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    /// <summary>
    /// Get or seets the item template (card for example)
    /// </summary>
    [Parameter]
    public RenderFragment<TItem> ItemTemplate { get; set; }

    /// <summary>
    /// Gets or sets the list of items
    /// </summary>
    [Parameter]
    public List<TItem> Items { get; set; }

    /// <summary>
    /// Gets or sets the list of selected items
    /// </summary>
    [Parameter]
    public List<TItem> SelectedItems { get; set; }

    /// <summary>
    /// Gets or sets the selection toolbar (bottom)
    /// </summary>
    [Parameter]
    public RenderFragment Toolbar { get; set; }

    /// <summary>
    /// Gets or sets the selection toolbar (bottom)
    /// </summary>
    [Parameter]
    public RenderFragment ToolbarSelection { get; set; }

    /// <summary>
    /// Sorting with properties list
    /// </summary>
    [Parameter]
    [Obsolete("Use \"SortOptions\" parameter to specify value and display label for each sort option.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IList<string> SortProperties { get => _sortProperties; set => _sortProperties = value; }

    /// <summary>
    /// Gets or sets the default property selected.
    /// </summary>
    [Parameter]
    public string DefaultSortProperty { get; set; }

    /// <summary>
    /// Gets or set the sort dropdown content.
    /// </summary>
    [Parameter]
    public RenderFragment SortOptions { get; set; }

    /// <summary>
    /// Gets or sets fields names for the grouping
    /// </summary>
    [Parameter]
    public string GroupBy { get; set; }

    /// <summary>
    /// Gets or sets group header content
    /// </summary>
    [Parameter]
    public RenderFragment<TItem> GroupHeaderContent { get; set; }

    #endregion
   
    #region Private classes

    private class SortListViewModel
    {
        /// <summary>
        /// Search ter into list
        /// </summary>
        public string SortProperty { get; set; }
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _sortModel = new SortListViewModel()
        {
            SortProperty = DefaultSortProperty
        };            
    }

    ///<inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (Items.Any())
            await SortListViewAsync(null);
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("list-view-content", CssClass);
    }

    /// <summary>
    /// Call on item selection 
    /// </summary>
    /// <param name="item"></param>
    protected void ItemSelection(TItem item)
    {
        if (!HasSelection())
        {
            return;
        }
        // select / unselect element
        if (SelectedItems.Contains(item))
        {
            // Unselect item
            SelectedItems.Remove(item);
        }
        else
        {
            // Select item
            SelectedItems.Add(item);
        }
    }

    /// <summary>
    /// Select all items
    /// </summary>
    protected void SelectAll()
    {
        SelectedItems.Clear();
        SelectedItems.AddRange(Items);
    }

    /// <summary>
    /// Unselect all items
    /// </summary>
    protected void UnSelectAll()
    {
        SelectedItems.Clear();
    }

    /// <summary>
    /// Return if item is selected
    /// </summary>
    /// <param name="item">Item</param>
    /// <returns></returns>
    protected bool IsSelected(TItem item)
    {
        return HasSelection() && SelectedItems.Contains(item);
    }

    /// <summary>
    /// Return if there is a selection item list
    /// </summary>
    /// <returns></returns>
    protected bool HasSelection()
    {
        return SelectedItems is not null;
    }

    /// <summary>
    /// Return if there is at least one item selected 
    /// </summary>
    /// <returns></returns>
    protected bool HasSelectedItems()
    {
        return HasSelection() && SelectedItems.Any();
    }

    /// <summary>
    /// Return value following the class property name
    /// </summary>
    /// <param name="item"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    private static object GetPropertyValue(TItem item, string propertyName)
    {            
        var propertyInfo = item.GetType().GetProperty(propertyName);
        return propertyInfo.PropertyType.IsEnum
            ? Lagoon.Helpers.Extensions.GetDisplayName((Enum)propertyInfo.GetValue(item, null))
            : propertyInfo.GetValue(item, null);                       
    }

    /// <summary>
    /// Sort items
    /// </summary>
    /// <param name="e">Change arguments</param>
    public async Task SortListViewAsync(ChangeEventArgs e)
    {
        _isLoading = true;
        await Task.Run(() =>
        {                             
            IOrderedQueryable<TItem> queryOrdered = null;               
            if (!string.IsNullOrEmpty(GroupBy) && !_sortModel.SortProperty.Equals(GroupBy))
            {                    
                queryOrdered = Items.AsQueryable().OrderBy(s => GetPropertyValue(s, GroupBy));                                        
            }
            if (!string.IsNullOrEmpty(_sortModel.SortProperty) || queryOrdered is null)
            {
                if (queryOrdered is null)
                {
                    IQueryable<TItem> query = Items.AsQueryable();
                    if (_sortAscending)
                    {
                        Items = query.OrderBy(x => GetPropertyValue(x, _sortModel.SortProperty)).ToList();
                    }
                    else
                    {
                        Items = query.OrderByDescending(x => GetPropertyValue(x, _sortModel.SortProperty)).ToList();
                    }
                }
                else
                {
                    if (_sortAscending)
                    {
                        Items = queryOrdered.ThenBy(s => GetPropertyValue(s, _sortModel.SortProperty)).ToList();
                    }
                    else
                    {
                        Items = queryOrdered.ThenByDescending(s => GetPropertyValue(s, _sortModel.SortProperty)).ToList();
                    }                                
                }                    
            }
        });
        _isLoading = false;
    }

    /// <summary>
    /// Set sort properties order and button icon
    /// </summary>
    private Task SetSortOrderAsync()
    {
        _sortAscending = !_sortAscending;
        _sortButtonIcon = _sortAscending ? IconNames.All.ArrowDown : IconNames.All.ArrowUp;
        return SortListViewAsync(null);

    }

    /// <summary>
    /// Return data to display
    /// </summary>
    /// <returns></returns>
    private IEnumerable<IGrouping<object, TItem>> GetData()
    {            
        return Items.GroupBy(g => string.IsNullOrEmpty(GroupBy) ? g : GetPropertyValue(g, GroupBy));            
    }

    /// <summary>
    /// render fragment for group name
    /// </summary>
    private RenderFragment GroupNameRender(IGrouping<object, TItem> group)
    {
        return builder =>
        {
            builder.OpenElement(0, "span");                
            builder.AddContent(1, $"{GroupBy} : {group.Key}");
            builder.CloseElement();

        };
    }

    /// <summary>
    /// Return group collapse state
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    private bool GetGroupState(IGrouping<object, TItem> group)
    {
        _groupsCollapseState ??= new();
        if (!_groupsCollapseState.ContainsKey(group.Key))
        {
            _groupsCollapseState.Add(group.Key, false);                
        }
        return _groupsCollapseState[group.Key];                        
    }

    /// <summary>
    /// Group change collapse state event
    /// </summary>
    /// <param name="group"></param>
    private void OnChangeCollapse(IGrouping<object, TItem> group)
    {
        _groupsCollapseState[group.Key] = !_groupsCollapseState[group.Key];
    }

    #endregion

}
