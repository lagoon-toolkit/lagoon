using Lagoon.UI.GridView.Components.Internal;
using Lagoon.UI.GridView.Helpers;

namespace Lagoon.UI.Components.Internal;


/// <summary>
/// The state for one column.
/// </summary>
internal class GridColumnState
{

    #region static fields

    /// <summary>
    /// All the parameter names defined in this class.
    /// </summary>
    internal static readonly HashSet<string> Parameters = GetParameters();

    #endregion

    #region fields

    private Filter _filter;
    private LambdaExpression _modelValueExpression;
    private LambdaExpression _parameterizedValueExpression;
    private LambdaExpression _filterExpression;
    private LambdaExpression _sortExpression;
    private LambdaExpression _groupExpression;
    private Delegate _getGroupValue;

    #endregion

    #region column state render fragments

    /// <summary>
    /// Gets or sets template of the filter cell
    /// </summary>
    [Parameter]
    public RenderFragment<Filter> FilterCellContent { get; set; }

    #endregion

    #region column state parameters

    /// <summary>
    /// Gets or sets if the column has filter
    /// </summary>
    [Parameter]
    public bool AllowFilter { get; set; } = true;

    /// <summary>
    /// Gets or sets if the column can be hidden
    /// </summary>
    [Parameter]
    public bool AllowHide { get; set; } = true;

    /// <summary>
    /// Gets or sets if the column can be sorted
    /// </summary>
    [Parameter]
    public bool AllowSort { get; set; } = true;

    /// <summary>
    /// Gets or sets aria label
    /// </summary>
    [Parameter]
    public string AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets id of component for aria label
    /// </summary>
    [Parameter]
    public string AriaLabelledBy { get; set; }

    /// <summary>
    /// Gets or sets calculation function
    /// </summary>
    [Parameter]
    public virtual DataCalculationType CalculationType { get; set; }

    /// <summary>
    /// Gets or sets default filter value
    /// </summary>
    [Parameter]
    public Filter DefaultFilter { get; set; }

    /// <summary>
    /// Gets or sets if the column is frozen.
    /// </summary>
    [Parameter]
    public bool DefaultFrozen { get; set; }

    /// <summary>
    /// Gets or sets default sort direction
    /// </summary>
    [Parameter]
    public DataSortDirection DefaultSortDirection { get; set; }

    /// <summary>
    /// Gets or sets if the column is displayed
    /// </summary>
    [Parameter]
    public bool DefaultVisible { get; set; } = true;

    /// <summary>
    /// Gets or set width (null == "1fr").
    /// </summary>
    [Parameter]
    public string DefaultWidth { get; set; }

    /// <summary>
    /// Gets or sets if the column is exportable.
    /// </summary>
    [Parameter]
    public bool Exportable { get; set; } = true;

    /// <summary>
    /// Gets or sets the title used when the column is exported.
    /// The column "Title" parameter is used by default.
    /// </summary>
    [Parameter]
    public string ExportTitle { get; set; }

    /// <summary>
    /// Gets or sets if the selection tab is visible.
    /// </summary>
    [Parameter]
    public bool FilterDisableSelection { get; set; }

    /// <summary>
    /// Gets or sets column group name
    /// </summary>
    [Parameter]
    public string GroupName { get; set; }

    /// <summary>
    /// Gets or sets header content.
    /// </summary>
    [Parameter]
    public RenderFragment HeaderCellContent { get; set; }

    /// <summary>
    /// Gets or sets the header CSS class.
    /// </summary>
    [Parameter]
    public string HeaderCssClass { get; set; }

    /// <summary>
    /// Gets or sets tooltip for header cell
    /// </summary>        
    [Parameter]
    public string HeaderTooltip { get; set; }

    /// <summary>
    /// Gets or sets if the tooltip contents for header cell use the HTML format; else, the content use a raw text format.
    /// </summary>        
    [Parameter]
    public bool HeaderTooltipIsHtml { get; set; }

    /// <summary>
    /// Gets or sets unique id of the column
    /// </summary>
    [Parameter]
    [Obsolete("Use UniqueKey property !")]
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets maximal width
    /// </summary>
    [Parameter]
    public int MaxWidth { get; set; }

    /// <summary>
    /// Gets or sets minimal width
    /// </summary>
    [Parameter]
    public int MinWidth { get; set; }

    /// <summary>
    /// Gets or sets if the column can be resized
    /// </summary>
    [Parameter]
    public bool Resizable { get; set; } = true;

    /// <summary>
    /// Gets or sets title
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets sort expression
    /// </summary>
    [Parameter]
    [Obsolete("Use the GetSortExpression() !")]
    public LambdaExpression SortExpression { get => _sortExpression; set => _sortExpression = value; }

    /// <summary>
    /// Gets or sets filter expression
    /// </summary>
    [Parameter]
    [Obsolete("Use the GetFilterExpression() !")]
    public LambdaExpression FilterExpression { get => _filterExpression; set => _filterExpression = value; }

    /// <summary>
    /// Gets or sets group expression
    /// </summary>
    [Parameter]
    [Obsolete("Use the GetGroupExpression() !")]
    public LambdaExpression GroupExpression { get => _groupExpression; set => _groupExpression = value; }

    /// <summary>
    /// Gets or sets filter and sort expression
    /// </summary>
    [Parameter]
    public LambdaExpression LambdaExpression { get; set; }


    /// <summary>
    /// Gets or sets the input prefix.
    /// </summary>
    [Parameter]
    public string PrefixCalculation { get; set; }

    /// <summary>
    /// Indicate if prefix is simple text or icon from library.
    /// </summary>
    [Parameter]
    public InputLabelType PrefixCalculationType { get; set; }

    /// <summary>
    /// Gets or sets the input suffix.
    /// </summary>
    [Parameter]
    public string SuffixCalculation { get; set; }

    /// <summary>
    /// Indicate if suffix is simple text or icon from library.
    /// </summary>
    [Parameter]
    public InputLabelType SuffixCalculationType { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets Column width
    /// </summary>
    public GridViewCssSize CalculatedWidth { get; set; }

    /// <summary>
    /// Gets or sets the default display format
    /// </summary>
    public string CalculationDisplayFormat { get; set; }

    /// <summary>
    /// Type of component to create for the cell rendering.
    /// </summary>
    public Type CellComponentType { get; private set; }

    /// <summary>
    /// The type of value to bound on the cell declaration.
    /// </summary>
    public Type CellValueType { get; private set; }

    /// <summary>
    /// The main class type of the column.
    /// </summary>
    public GridColumnType ColumnBoundType { get; set; }

    /// <summary>
    /// Gets the active filter.
    /// </summary>
    public Filter Filter => _filter;

    /// <summary>
    /// Gets or sets filter cell
    /// </summary>
    public LgGridFilterCellBase FilterCell { get; set; }

    /// <summary>
    /// Gets the complete field name "PropertyName" or "PropertyName.SubPropertyName".
    /// </summary>       
    public string EditContextFieldName { get; set; }

    /// <summary>
    /// Gets or sets the type of the filter for the column.
    /// </summary>
    public Type FilterType { get; set; }

    /// <summary>
    /// Gets or sets the type of the filterbox for the column.
    /// </summary>
    public Type FilterBoxType { get; set; }

    /// <summary>
    /// Gets of sets if the filter box must be loaded with existing items.
    /// </summary>
    public bool FilterShowAllItems { get; set; }

    /// <summary>
    /// Gets or sets if the column is frozen.
    /// </summary>
    public bool Frozen { get; set; }

    /// <summary>
    /// The parent Grid View.
    /// </summary>
    public LgBaseGridView GridView { get; set; }

    /// <summary>
    /// Gets or sets header cell
    /// </summary>
    public LgGridHeaderCell HeaderCell { get; set; }

    /// <summary>
    /// Gets or sets header cell span
    /// </summary>
    public int HeaderColSpan { get; set; }

    /// <summary>
    /// Type of component to create for the cell rendering.
    /// </summary>
    public Type HeaderGroupCellComponentType { get; private set; }

    /// <summary>
    /// Gets or sets header selection checkbox for selection column
    /// </summary>
    public bool HeaderSelectionState { get; set; }

    /// <summary>
    /// Gets the column index
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// Gets or sets initial column width
    /// </summary>
    public GridViewCssSize InitialWidth { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the bound value.
    /// </summary>
    public LambdaExpression ModelValueExpression => _modelValueExpression;

    /// <summary>
    /// Gets an expression to get the bound value with an item parameter.
    /// </summary>
    public LambdaExpression ParameterizedValueExpression => _parameterizedValueExpression;

    /// <summary>
    /// Gets or sets column order
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets sort direction
    /// </summary>
    public DataSortDirection SortDirection { get; set; }

    /// <summary>
    /// Get or sets the sorting of order
    /// </summary>
    public int? SortingOrder { get; set; } = null;

    /// <summary>
    /// Gets the column state id.
    /// </summary>
    public string UniqueKey { get; set; }

    /// <summary>
    /// Gets or sets if the column is displayed
    /// </summary>        
    public bool Visible { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// Define the column index and default order at the creation time.
    /// </summary>
    /// <param name="index"></param>
    internal void SetIndex(int index)
    {
        Index = index;
        Order = index + 1;
    }

    #endregion

    #region static methods

    /// <summary>
    /// Get all the parameter names defined in this class.
    /// </summary>
    /// <returns>All the parameter names defined in this class.</returns>
    private static HashSet<string> GetParameters()
    {
        return new HashSet<string>(
            typeof(GridColumnState).GetProperties()
            .Where(prop => prop.IsDefined(typeof(ParameterAttribute), false)).Select(p => p.Name));
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    internal void Initialize(LgGridBaseColumn column)
    {
        CalculationDisplayFormat = column.DisplayFormat;
        CellValueType = column.OnGetCellValueType();
        GridView = column.GridView;
        UniqueKey = column.GetUniqueKey();
        ColumnBoundType = GridViewHelper.GetColumnType(CellValueType);
        CellComponentType = column.OnGetCellComponentType(column.GridView.ItemType, column.ColumnValueType, CellValueType);
        FilterType = column.OnGetFilterType(CellValueType);
        FilterBoxType = column.OnGetFilterBoxType(CellValueType);
        HeaderGroupCellComponentType = column.OnGetHeaderGroupCellComponentType(column.GridView.ItemType);
        // The GridView.DisplayDefaultTooltipHeader is defined in OnInitialized of the LgGridView
        if (GridView.DisplayDefaultTooltipHeader.Value)
        {
            HeaderTooltip ??= Title;
        }
        LoadDefaultProfile();
        OnInitialized();
    }

    internal virtual void OnInitialized()
    {
    }

    /// <summary>
    /// Load the default profile settings.
    /// </summary>
    internal void LoadDefaultProfile()
    {
        LoadProfile(null, GridFeature.None);
    }

    /// <summary>
    /// Load the profile settings.
    /// </summary>
    /// <param name="profile">The profile to load.</param>
    /// <param name="features">The settings activated for the grid.</param>
    internal void LoadProfile(GridViewColumnProfile profile, GridFeature features)
    {
        GridViewColumnProfile profileIf(GridFeature feature) => GetProfileIf(profile, features, feature);
        SetCurrentFilter(profileIf(GridFeature.Filter)?.Filter ?? DefaultFilter);
        Frozen = profileIf(GridFeature.Freeze)?.Frozen ?? DefaultFrozen;
        Order = profileIf(GridFeature.Move)?.Order ?? Index + 1;
        SetSortDirection(profileIf(GridFeature.Sort)?.Sort ?? DefaultSortDirection);
        Visible = !(profileIf(GridFeature.Visibility)?.Hidden ?? !DefaultVisible);
        InitialWidth = new GridViewCssSize(profileIf(GridFeature.Resize)?.Width ?? DefaultWidth);
        if (profileIf(GridFeature.Sort)?.SortingOrder is not null)
        {
            SortingOrder = profileIf(GridFeature.Sort)?.SortingOrder;
        }
        else
        {
            SortingOrder = DefaultSortDirection == DataSortDirection.None ? null : Index;
        }
    }

    /// <summary>
    /// Get the column profile from the current state.
    /// </summary>
    /// <param name="features">The GridView features.</param>
    /// <param name="isShared">Indicate if it's a shared profile.</param>
    /// <returns>The column profile from the current state.</returns>
    internal GridViewColumnProfile SaveToProfile(GridFeature features, bool isShared)
    {
        GridViewColumnProfile profile = new();
        T setIf<T>(GridFeature feature, T value) => GetProfileIf(value, features, feature);
        profile.UniqueKey = UniqueKey;
        profile.Filter = setIf(GridFeature.Filter, Filter);
        profile.Order = setIf<int?>(GridFeature.Move, Order);
        profile.Sort = setIf<DataSortDirection?>(GridFeature.Sort, SortDirection);
        profile.Hidden = setIf<bool?>(GridFeature.Visibility, !Visible);
        profile.Frozen = setIf<bool?>(GridFeature.Freeze, Frozen);
        if (!isShared && !InitialWidth.Equals(DefaultWidth))
        {
            //We don't save column width for shared profiles
            profile.Width = setIf(GridFeature.Resize, InitialWidth.ToString());
        }
        profile.SortingOrder = setIf(GridFeature.Sort, SortingOrder);
        return profile;
    }

    /// <summary>
    /// Return the profile only the feature is available.
    /// </summary>
    /// <param name="profile">The state profile.</param>
    /// <param name="features">The allowed features.</param>
    /// <param name="feature">The wanted feature.</param>
    /// <returns>The profile only the feature is available.</returns>
    private T GetProfileIf<T>(T profile, GridFeature features, GridFeature feature)
    {
        if (feature == GridFeature.Filter && !AllowFilter)
        {
            return default;
        }
        if (feature == GridFeature.Visibility && !AllowHide)
        {
            return default;
        }
        if (feature == GridFeature.Sort && !AllowSort)
        {
            return default;
        }
        return features.HasFlag(feature) ? profile : default;
    }

    /// <summary>
    /// Indicate if the column can be resizable
    /// </summary>
    /// <returns></returns>
    internal bool IsResizable()
    {
        return Resizable && GridView.Features.HasFlag(GridFeature.Resize);
    }

    /// <summary>
    /// Indicate if the column can be sort
    /// </summary>
    /// <returns></returns>
    internal bool IsSortable()
    {
        return AllowSort && GridView.Features.HasFlag(GridFeature.Sort) && GetSortExpression() is not null;
    }

    /// <summary>
    /// Indicate if column can be frozen
    /// </summary>
    /// <returns></returns>
    internal bool IsFrozen()
    {
        return Frozen && GridView.Features.HasFlag(GridFeature.Freeze);
    }

    /// <summary>
    /// Indicate if the column can be grouped
    /// </summary>
    /// <returns></returns>
    internal bool IsGroupable()
    {
        return GetGroupExpression() != null;
    }

    /// <summary>
    /// Get an expression to get the bound value with an item parameter.
    /// </summary>
    /// <param name="headerValueExpression">Value expression for the column.</param>
    internal void SetParameterizedValueExpression(LambdaExpression headerValueExpression)
    {
        _modelValueExpression = headerValueExpression;
        if (_modelValueExpression is not null)
        {
            _parameterizedValueExpression = GetValueParameterizedExpression();
        }
        else
        {
            _parameterizedValueExpression = null;
        }
    }

    /// <summary>
    /// Return title with dico support
    /// </summary>
    /// <returns></returns>
    internal string GetTitle()
    {
        return Title.CheckTranslate();
    }

    /// <summary>
    /// Return title and subheader title
    /// </summary>
    /// <returns></returns>
    internal string GetFullTitle()
    {
        string subheaderTitle = GetGroupName();
        if (string.IsNullOrEmpty(subheaderTitle))
        {
            return GetTitle();
        }
        return $"{subheaderTitle} - {Title.CheckTranslate()}";
    }

    /// <summary>
    /// Return group name with dico support
    /// </summary>
    /// <returns></returns>
    internal string GetGroupName()
    {
        return GroupName.CheckTranslate();
    }

    /// <summary>
    /// Get the base CSS classes for a cell of the datagrid.
    /// </summary>
    /// <returns></returns>
    internal string GetCellCssClass()
    {
        return $"gridview-cell gridview-col{Index}";
    }

    /// <summary>
    /// Gets the column calculation has prefix or suffix
    /// </summary>
    /// <returns></returns>
    internal bool HasPrefixSuffixValue()
    {
        return !string.IsNullOrEmpty(PrefixCalculation)
            || !string.IsNullOrEmpty(SuffixCalculation);
    }

    /// <summary>
    /// Change column sort direction
    /// </summary>
    /// <param name="value">new sort direction</param>
    public void SetSortDirection(DataSortDirection value)
    {
        SortDirection = value;
        HeaderCell?.Update();
    }

    /// <summary>
    /// Define the current filter.
    /// </summary>
    /// <param name="filter">The Filter.</param>
    public void SetCurrentFilter(Filter filter)
    {
        if (filter is null || FilterType is null)
        {
            _filter = null;
        }
        else if (filter.GetType() == FilterType)
        {
            // The type of filter match the type awaiting by the column
            _filter = filter;
        }
        else
        {
            // The type mismatch, we try a conversion
            _filter = filter.ConvertAs(FilterType);
        }
    }

    /// <summary>
    /// Define the current filter.
    /// </summary>
    /// <param name="value">The new Filter value.</param>
    /// <param name="p_b_reloadData">Indicate if the data must be reloaded to apply the new filter value.</param>
    public async Task SetCurrentFilterAsync(Filter value, bool p_b_reloadData)
    {
        if (AllowFilter)
        {
            _filter = value;
            // Update filter icon and value                
            FilterCell?.Update();
            // Raise the grid event
            if (p_b_reloadData)
            {
                await GridView.OnColumnFilterChangeAsync(new GridViewFilterChangeEventArgs(UniqueKey, _filter));
            }
        }
    }

    /// <summary>
    /// Reset the current filter.
    /// </summary>
    /// <param name="p_b_reloadData">Indicate if the data must be reloaded to apply the new filter value.</param>
    public async Task<bool> ResetFilterAsync(bool p_b_reloadData)
    {
        if (_filter is null)
        {
            return false;
        }
        await SetCurrentFilterAsync(null, p_b_reloadData);
        return true;
    }

    /// <summary>
    /// Return an expression to get the bound value with an item parameter.
    /// </summary>
    /// <returns>An expression to get the bound value with an item parameter.</returns>
    private LambdaExpression GetValueParameterizedExpression()
    {
        return GridView.ModelExpressionResolver.ToParameterizedLambda(ModelValueExpression, CellValueType);
    }

    /// <summary>
    /// Get the filter expression.
    /// </summary>
    /// <returns></returns>
    internal LambdaExpression GetFilterExpression()
    {
        return _filterExpression ?? LambdaExpression ?? ParameterizedValueExpression;
    }

    /// <summary>
    /// Get the sort expression.
    /// </summary>
    /// <returns></returns>
    internal LambdaExpression GetSortExpression()
    {
        return _sortExpression ?? LambdaExpression ?? ParameterizedValueExpression;
    }

    /// <summary>
    /// Get the export information for the column.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the GridView.</typeparam>
    /// <param name="columnTitle">The column title.</param>
    /// <param name="groupTitle">The group title.</param>
    /// <returns></returns>
    internal virtual IExportColumn<TItem> GetExportColumn<TItem>(string columnTitle, string groupTitle)
    {
        if (CellValueType is null || ParameterizedValueExpression is null)
        {
            return null;
        }
        return (IExportColumn<TItem>)Activator
            .CreateInstance(typeof(ExportColumn<,>)
            .MakeGenericType(typeof(TItem), CellValueType), columnTitle, ParameterizedValueExpression, groupTitle);
    }

    /// <summary>
    /// Get filters informations
    /// </summary>
    /// <returns></returns>
    internal virtual GridViewSummaryFilter GetSummaryFilter()
    {
        return new GridViewSummaryFilter(GetTitle(), Filter.ToString());
    }

    /// <summary>
    /// Get the group expression.
    /// </summary>
    /// <returns></returns>
    internal LambdaExpression GetGroupExpression()
    {
        return _groupExpression ?? LambdaExpression ?? ParameterizedValueExpression;
    }

    // TODO : Not display group in options is not 
    /// <summary>
    /// Return row value for the column
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    internal object GetGroupValue(object item)
    {
        _getGroupValue ??= GetGroupExpression().Compile();
        return _getGroupValue.DynamicInvoke(item);
    }

    #endregion
}
