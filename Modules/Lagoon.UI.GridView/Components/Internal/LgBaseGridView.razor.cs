namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Base gridview
/// </summary>
public abstract class LgBaseGridView : LgFrame
{
    #region fields

    /// <summary>
    /// Authenticated Json HttpClient instance.
    /// </summary>
    private HttpClient _authenticatedJsonHttpClient;

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets if the header group row must be displayed
    /// </summary>
    protected bool HeaderGroup { get; set; }

    /// <summary>
    /// Gets list of the grouping field
    /// </summary>
    internal Dictionary<int, List<string>> GroupByColumnKeyList { get; } = new();

    /// <summary>
    /// Authenticated Json HttpClient instance.
    /// </summary>    
    internal HttpClient JsonHttp
    {
        get
        {
            _authenticatedJsonHttpClient ??= App.HttpClientFactory.CreateJsonAuthenticatedClient();            
            return _authenticatedJsonHttpClient;
        }
    }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets Auto resize column at startup.
    /// </summary>
    [Parameter]
    public GridLayoutMode LayoutMode { get; set; } = GridLayoutMode.FitHeader;

    /// <summary>
    /// Gets or sets unique id of the model
    /// </summary>
    /// <remarks>Default value is Id</remarks>
    [Parameter]
    public string KeyField { get; set; }

    /// <summary>
    /// Gets or sets unique identifiant to keep grid state.
    /// </summary>
    [Parameter]
    public string StateId { get; set; }

    /// <summary>
    /// Gets or sets gridview height
    /// </summary>
    [Parameter]
    [Obsolete("You should use style atribute.")]
    public string Height { get; set; }

    /// <summary>
    /// Gets or sets fields names for the grouping
    /// Separator is ,
    /// </summary>
    [Parameter]
    public string GroupBy { get; set; }

    /// <summary>
    /// Gets or sets if groups are collapsed by default
    /// </summary>
    [Parameter]
    public bool DefaultGroupCollapsed { get; set; }

    /// <summary>
    /// Gets or sets editing mode
    /// </summary>
    [Parameter]
    public GridEditMode EditMode { get; set; } = GridEditMode.None;

    #region Filters

    /// <summary>
    /// Gets or sets if filter are visible.
    /// </summary>
    [Parameter]
    public bool DefaultShowFilters { get; set; }

    /// <summary>
    /// Gets or sets if filter are visible.
    /// </summary>
    [Parameter]
    public bool ShowFilters { get; set; }

    /// <summary>
    /// Gets or sets event for the binding
    /// </summary>
    /// <value></value>
    [Parameter]
    public EventCallback<bool> ShowFiltersChanged { get; set; }

    /// <summary>
    /// Gets or sets event indicate filters has changed
    /// </summary>
    /// <value></value>
    [Parameter]
    public EventCallback OnFilterChanged { get; set; }

    #endregion        

    #region Pagination       

    /// <summary>
    /// Gets or sets current page
    /// </summary> 
    [Parameter]
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets current page binding event
    /// </summary>
    [Parameter]
    public EventCallback<int> CurrentPageChanged { get; set; }

    /// <summary>
    /// Gets or sets  the default number of the row by page when the paging feature is activated.
    /// </summary>
    /// <remarks>If 0 or less, the minimum value of <see cref="PaginationSizeSelector" /> is used.</remarks>
    [Parameter]
    public int DefaultPageSize { get; set; }

    /// <summary>
    /// Gets or sets if the row number selector is shown
    /// </summary>
    /// <remarks>0 define All value</remarks>
    [Parameter]
    public int[] PaginationSizeSelector { get; set; } // = new int[] { 5, 10, 25, 50, 100 };

    /// <summary>
    /// Gets or sets number of pagination buttons
    /// </summary>
    [Parameter]
    public int PaginationButtonCount { get; set; } = 5;

    /// <summary>
    /// Gets count of displayed rows
    /// </summary>
    /// <value></value>
    [Parameter]
    public GridViewDataCounter GridViewDataCounter { get; set; }

    /// <summary>
    /// Gets or sets event for binding count
    /// </summary>
    /// <value></value>
    [Parameter]
    public EventCallback<GridViewDataCounter> GridViewDataCounterChanged { get; set; }

    #endregion

    #region Export

    /// <summary>
    /// Gets or sets the export activation
    /// </summary>
    [Parameter]
    public bool? Exportable { get; set; }

    /// <summary>
    /// Gets or sets the name of the export file
    /// </summary>
    [Parameter]
    public string ExportFileName { get; set; } = "Data";

    /// <summary>
    /// Gets or sets the list of column keys representing the columns to export.
    /// </summary>
    [Parameter]
    public List<string> ExportableColumns { get; set; }

    /// <summary>
    /// Gets or sets the GroupName export activation
    /// </summary>
    [Parameter]
    public bool ExportGroupName { get; set; }

    /// <summary>
    /// Gets or sets the export column title format (by default : "{0}({1})").
    ///  {0} is the column title.
    ///  {1} is the group name.
    /// </summary>
    /// <remarks>Used only if ExportGroupName is True and GroupName is not null or empty.</remarks>
    [Parameter]
    public string ExportGroupNameTitleFormat { get; set; } = "{0}({1})";


    /// <summary>
    /// Gets or sets the show filters summary activation
    /// </summary>
    [Parameter]
    public bool? ShowFiltersSummary { get; set; }

    #endregion

    /// <summary>
    /// Define allowed options
    /// </summary>
    [Parameter]
    public GridFeature Features { get; set; } = GridFeature.Default;

    /// <summary>
    /// Define grid style 
    /// </summary>
    [Parameter]
    public GridStyleType GridStyleType { get; set; } = GridStyleType.Grid;

    /// <summary>
    /// Force the display of tooltips on the current Gridview headers
    /// </summary>
    [Parameter]
    public bool? DisplayDefaultTooltipHeader { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Type of item bound to the current grid.
    /// </summary>
    internal abstract Type ItemType { get; }

    /// <summary>
    /// Dictionnary of columns by unique key.
    /// </summary>
    internal Dictionary<string, GridColumnState> ColumnDico { get; } = new();

    /// <summary>
    /// Columns list.
    /// </summary>
    internal IReadOnlyCollection<GridColumnState> ColumnList => ColumnDico.Values;

    /// <summary>
    /// Gets or sets current parameters
    /// </summary>
    internal GridViewState State { get; set; } = new();

    /// <summary>
    /// Gets or sets if update buttons must be displayed
    /// </summary>
    internal bool DisplayEdit { get; set; }

    /// <summary>
    /// Gets or sets if the edit and add management buttons are displayed in toolbar
    /// </summary>
    internal bool HasEditColumn { get; set; }

    /// <summary>
    /// Gets or sets if the line selection is simple or multiple
    /// </summary>
    internal bool HasSelectionColumn { get; set; }

    /// <summary>
    /// Gets or sets if at least one group is active
    /// </summary>
    internal bool HasActiveGroup => GroupByColumnKeyList.Any();

    /// <summary>
    /// Class to translate bounded expression as parametrized lambda expression.
    /// </summary>
    internal ModelExpressionVisitor ModelExpressionResolver { get; set; }

    #endregion

    #region events

    /// <summary>
    /// Called when the filter summary must be updated.
    /// </summary>
    internal event Action OnFilterSummaryStateChange;

    /// <summary>
    /// Ask to update the filter summary.
    /// </summary>
    internal async Task FilterStateChangeAsync()
    {
        OnFilterSummaryStateChange?.Invoke();
        if (OnFilterChanged.HasDelegate)
        {
            await OnFilterChanged.TryInvokeAsync(App);
        }
    }

    #endregion

    #region methods

    /// <summary>
    /// Returns the working type for the cell.
    /// </summary>
    /// <returns>The working type for the cell.</returns>
    /// <remarks>Used because for the <see cref="LgDynamicGridView"/>, the TColumnValue is always <see cref="object"/></remarks>
    internal virtual Type OnGetCellValueType(Type columnValueType, LambdaExpression valueExpression)
    {
        return columnValueType;
    }

    /// <summary>
    /// Get a column informations by it's unique key.
    /// </summary>
    /// <param name="uniqueKey">The identifier of the column..</param>
    /// <returns></returns>
    internal GridColumnState GetColumn(string uniqueKey)
    {
        return ColumnList.First(c => c.UniqueKey == uniqueKey);
    }

    /// <summary>
    /// Get a column informations by it's unique key.
    /// </summary>
    /// <param name="uniqueKey">The identifier of the column..</param>
    /// <returns></returns>
    internal GridColumnState GetColumnOrDefault(string uniqueKey)
    {
        return ColumnList.FirstOrDefault(c => c.UniqueKey == uniqueKey);
    }

    /// <summary>
    /// Get a column informations by it's unique key.
    /// </summary>
    /// <param name="uniqueKey">The identifier of the column..</param>
    /// <param name="columnState"></param>
    /// <returns></returns>
    internal bool TryGetColumn(string uniqueKey, out GridColumnState columnState)
    {
        columnState = ColumnList.FirstOrDefault(c => c.UniqueKey == uniqueKey);
        return columnState is not null;
    }

    /// <summary>
    /// Get all the distinct existing values for the column.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="columnState">The column informations.</param>
    /// <returns>The distinct existing values for the column.</returns>
    /// <exception cref="NotImplementedException"></exception>
    internal virtual Task<IEnumerable<T>> GetColumnValuesAsync<T>(GridColumnState columnState)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Return name of the property binded to column relative to the item.
    /// </summary>
    /// <param name="valueExpression">The value expression.</param>
    /// <param name="parameterizedValueExpression">The expression representing the method to obtain the value.</param>
    /// <returns></returns>
    internal virtual string BuildColumnKey(LambdaExpression valueExpression, LambdaExpression parameterizedValueExpression)
    {
        const string MODEL_EXPRESSION = ModelExpressionVisitor.PARAMETER_NAME + ".";
        if (parameterizedValueExpression is not null)
        {
            string bodyExpression = parameterizedValueExpression.Body.ToString();
            if (bodyExpression.StartsWith(MODEL_EXPRESSION))
            {
                return bodyExpression[MODEL_EXPRESSION.Length..];
            }
            else if (bodyExpression == ModelExpressionVisitor.PARAMETER_NAME)
            {
                return "//item";
            }
        }
        return null;
    }

    /// <summary>
    /// Add group field
    /// </summary>
    /// <param name="groupLevel"></param>
    /// <param name="field"></param>
    internal void AddGroupField(int groupLevel, string field)
    {
        if (GroupByColumnKeyList.TryGetValue(groupLevel, out List<string> group))
        {
            group.Add(field);
        }
        else
        {
            GroupByColumnKeyList.Add(groupLevel, new() { field });
        }
    }

    /// <summary>
    /// JS Focus element
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    internal virtual void SetFocus(string selector)
    {
    }

    #region Events

    /// <summary>
    /// Change data sort
    /// </summary>
    /// <param name="args"></param>
    internal abstract Task OnColumnSortChangeAsync(GridViewSortEventArgs args);

    /// <summary>
    /// Change column filter value event
    /// </summary>
    /// <param name="args"></param>
    internal abstract Task OnColumnFilterChangeAsync(GridViewFilterChangeEventArgs args);

    /// <summary>
    /// Move column before droppedcolumn event
    /// </summary>
    /// <param name="args"></param>
    internal abstract Task OnColumnMove(GridViewMoveColumnEventArgs args);

    /// <summary>
    /// Add or remove displayed rows from selection
    /// </summary>        
    /// <param name="state"></param>
    internal abstract Task OnChangeAllSelectionAsync(bool state);

    /// <summary>
    /// Remove all row from the selection
    /// </summary>        
    /// <returns></returns>
    internal abstract Task OnRemoveAllSelectionAsync();

    /// <summary>
    /// Post cell value changed event
    /// </summary>
    /// <param name="item">The item bound to the row.</param>
    /// <param name="columnKey">Identifier of the column.</param>
    /// <param name="value">The new value.</param>
    /// <param name="previousValue">The old value.</param>
    internal abstract Task<bool> OnValueChangedAsync(object item, string columnKey, object value, object previousValue);

    /// <summary>
    /// Cell value changed event
    /// </summary>
    /// <param name="item"></param>
    /// <param name="columnKey"></param>
    /// <param name="value"></param>
    /// <param name="previousValue"></param>
    /// <returns></returns>
    internal abstract Task OnPatchValueAsync(object item, string columnKey, object value, object previousValue);

    /// <summary>
    /// Add row data event
    /// </summary>        
    /// <returns></returns>
    internal abstract Task<bool> OnAddValueAsync();

    /// <summary>
    /// Update row data event
    /// </summary>
    /// <returns></returns>
    internal abstract Task<bool> OnUpdateValueAsync();

    /// <summary>
    /// Delete selectionned rows
    /// </summary>
    /// <returns></returns>
    internal abstract Task OnDeleteSelectionAsync();

    /// <summary>
    /// Command click event
    /// </summary>
    /// <param name="command">Command name</param>
    /// <param name="row">Row data</param>
    internal abstract Task CommandClickAsync(string command, object row);

    /// <summary>
    /// Indicate if edit cell on click is active
    /// </summary>
    /// <returns></returns>
    internal abstract bool CellCanFocus();

    #endregion

    /// <summary>
    /// Render the active edit row or the add row
    /// </summary>
    internal abstract void RefreshEditRow();

    #endregion

}
