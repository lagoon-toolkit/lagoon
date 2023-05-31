namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Class for the LgGridColumn
/// </summary>
public abstract class LgGridBaseColumn : IComponent
{

    #region constants

    internal const string COLUMN_KEY_PROPERTY_NAME = "Key";

    #endregion

    #region fields

    private RenderHandle _renderHandle;
    private string _quickKey;
    private GridColumnState _state;
    private LgBaseGridView _gridView;
    private GridRenderContext _renderContext;

    #endregion

    #region cell parameters

    /// <summary>
    /// Gets or sets if the cells are editable
    /// </summary>
    [Parameter]
    public bool CanEdit { get; set; } = true;

    /// <summary>
    /// Gets or sets if we can add new lines.
    /// </summary>        
    [Parameter]
    public bool CanAdd { get; set; } = true;

    /// <summary>
    /// Gets or sets if the cells contents are displayed
    /// </summary>
    [Parameter]
    public bool HideContent { get; set; }

    /// <summary>
    /// Gets or sets field with column span value
    /// </summary>
    [Parameter]
    public int ColSpan { get; set; } = -1;

    /// <summary>
    /// Gets or sets css class for all column cells
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the display format.
    /// </summary>
    [Parameter]
    public string DisplayFormat { get; set; }

    /// <summary>
    /// Gets or sets the input dispaly format.
    /// </summary>
    [Parameter]
    public string InputDisplayFormat { get; set; }

    /// <summary>
    /// Gets or sets input edit mask options
    /// </summary>
    [Parameter]
    public InputMaskOptions InputMaskOptions { get; set; }

    /// <summary>
    /// Gets or sets unique id of the column
    /// </summary>
    [Parameter]
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets cell click event
    /// </summary>
    [Parameter]
    public EventCallback<GridViewCellClickEventArgs> OnCellClick { get; set; }

    /// <summary>
    /// Gets or sets confirmation message in cell edit mode only
    /// </summary>
    [Parameter]
    public string Confirmation { get; set; }

    #endregion

    #region column state parameters

    /// <summary>
    /// Gets or sets if the column has filter
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public bool AllowFilter { get; set; } = true;

    /// <summary>
    /// Gets or sets if the column can be hidden
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public bool AllowHide { get; set; } = true;

    /// <summary>
    /// Gets or sets if the column can be sorted
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public bool AllowSort { get; set; } = true;

    /// <summary>
    /// Gets or sets aria label
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public string AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets id of component for aria label
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public string AriaLabelledBy { get; set; }

    /// <summary>
    /// Gets or sets calculation function
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public virtual DataCalculationType CalculationType { get; set; }

    /// <summary>
    /// Gets or sets default filter value
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public Filter DefaultFilter { get; set; }

    /// <summary>
    /// Gets or sets if the column is frozen.
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public bool DefaultFrozen { get; set; }

    /// <summary>
    /// Gets or sets default sort direction
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public DataSortDirection DefaultSortDirection { get; set; }

    /// <summary>
    /// Gets or sets if the column is displayed
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public bool DefaultVisible { get; set; } = true;

    /// <summary>
    /// Gets or set width of the column.
    /// Default value : "1fr"
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public string DefaultWidth { get; set; } = "1fr";

    /// <summary>
    /// Gets or sets if the column is exportable.
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public bool Exportable { get; set; } = true;

    /// <summary>
    /// Gets or sets the title used when the column is exported.
    /// The column "Title" parameter is used by default.
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public string ExportTitle { get; set; }

    /// <summary>
    /// Gets or sets if the selection tab is visible.
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public bool FilterDisableSelection { get; set; }

    /// <summary>
    /// Gets or sets column group name
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public string GroupName { get; set; }

    /// <summary>
    /// Gets or sets header content.
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public RenderFragment HeaderCellContent { get; set; }

    /// <summary>
    /// Gets or sets the header CSS class.
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public string HeaderCssClass { get; set; }

    /// <summary>
    /// Gets or sets tooltip for header cell
    /// </summary>    
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public string HeaderTooltip { get; set; }

    /// <summary>
    /// Gets or sets maximal width
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public int MaxWidth { get; set; }

    /// <summary>
    /// Gets or sets minimal width
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public int MinWidth { get; set; }

    /// <summary>
    /// Gets or sets if the column can be resized
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public bool Resizable { get; set; } = true;

    /// <summary>
    /// Gets or sets title
    /// </summary>
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets sort expression
    /// </summary>     
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public LambdaExpression SortExpression { get; set; }

    /// <summary>
    /// Gets or sets filter expression
    /// </summary>     
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public LambdaExpression FilterExpression { get; set; }

    /// <summary>
    /// Gets or sets filter and sort expression
    /// </summary>     
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public LambdaExpression LambdaExpression { get; set; }

    /// <summary>
    /// Gets or sets group expression
    /// </summary>     
#if DEBUG
    [Obsolete("Use the state property", true)]
#endif
    [Parameter]
    public LambdaExpression GroupExpression { get; set; }

    #endregion

    #region column state render fragments

    /// <summary>
    /// Gets or sets template of the filter cell
    /// </summary>
    [Parameter]
    public RenderFragment<FilterContext> FilterCellContent { get; set; }

    #endregion

    #region cascading parameters

    /// <summary>
    /// GridView parent
    /// </summary>
    [CascadingParameter]
    public LgBaseGridView GridView { get => _gridView; set { } }

    /// <summary>
    /// Gets or sets if the render is for the header, or the filter, or the body, ...
    /// </summary>
    [CascadingParameter]
    internal GridRenderContext RenderContext { get => _renderContext; set { } }

    #endregion

    #region properties

    /// <summary>
    /// Gets column state
    /// </summary>        
    internal GridColumnState State => _state;

    /// <summary>
    /// The type of value bound to the column.
    /// </summary>
    internal virtual Type ColumnValueType => null;

    #endregion

    #region IComponent interface

    /// <summary>
    /// Attaches the component to a Microsoft.AspNetCore.Components.RenderHandle.
    /// </summary>
    /// <param name="renderHandle">A Microsoft.AspNetCore.Components.RenderHandle that allows the component to be rendered.</param>
    void IComponent.Attach(RenderHandle renderHandle)
    {
        _renderHandle = renderHandle;
    }

    /// <summary>
    /// The content that should be rendered.
    /// </summary>
    /// <param name="builder">Render builder.</param>
    protected virtual void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (State.Visible)
        {
            builder.AddCascadingValueComponent(0, this, GetRenderContextContent(), true);
        }
    }

    #endregion

    #region methods

    /// <summary>
    /// Returns the working type for the cell.
    /// </summary>
    /// <returns>The working type for the cell.</returns>
    internal virtual Type OnGetCellValueType()
    {
        return null;
    }

    /// <summary>
    /// Return the type of filter for the column.
    /// </summary>
    /// <param name="cellValueType">The type of value.</param>
    /// <returns>The type of filter for the column.</returns>
    internal abstract Type OnGetFilterType(Type cellValueType);

    /// <summary>
    /// Return the component to use for filters on the column.
    /// </summary>
    /// <param name="cellValueType">The type of value.</param>
    /// <returns>The component to use for filters on the column.</returns>
    internal abstract Type OnGetFilterBoxType(Type cellValueType);

    /// <summary>
    /// Returns the working type for the cell.
    /// </summary>
    /// <param name="itemType">The type of data in the list bound to the grid.</param>
    /// <param name="columnValueType">The type of value bound on the column declaration.</param>
    /// <param name="cellValueType">The type of value bound on the cell declaration.</param>
    /// <returns>The working type for the cell.</returns>
    /// <remarks>Used because for the <see cref="LgDynamicGridView"/>, the TColumnValue is always <see cref="object"/></remarks>
    internal abstract Type OnGetCellComponentType(Type itemType, Type columnValueType, Type cellValueType);

    /// <summary>
    /// Returns the working type for the cell.
    /// </summary>
    /// <param name="itemType">The type of data in the list bound to the grid.</param>
    /// <returns>The working type for the cell.</returns>
    /// <remarks>Used because for the <see cref="LgDynamicGridView"/>, the TColumnValue is always <see cref="object"/></remarks>
    internal virtual Type OnGetHeaderGroupCellComponentType(Type itemType)
    {
        return typeof(LgGridHeaderGroupCell<>).MakeGenericType(itemType);
    }

    /// <summary>
    /// Get an automatic unique key if the key isn't specified.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    protected virtual string OnGetQuickKey(ParameterView parameters)
    {
        return parameters.GetValueOrDefault<string>(COLUMN_KEY_PROPERTY_NAME);
    }

    /// <summary>
    /// Get the key to identify the column.
    /// </summary>
    /// <returns></returns>
    internal string GetUniqueKey()
    {
        return OnGetUniqueKey();
    }

    /// <summary>
    /// Get the key to identify the column.
    /// </summary>
    /// <returns></returns>
    protected virtual string OnGetUniqueKey()
    {
#pragma warning disable CS0618 // (Obsolete warning) : The "Key" property must be used only here !
        return CleanUniqueKey(State.Key);
#pragma warning restore CS0618
    }

    /// <summary>
    /// Remove invlid chars from the unique key.
    /// (The Key is used as parameter name in PageDataLoader)
    /// </summary>
    /// <param name="id">The input key.</param>
    /// <returns>The new id cleaned.</returns>
    protected static string CleanUniqueKey(string id)
    {
        if (id is null)
        {
            return null;
        }
        StringBuilder sb = new(id.Length);
        foreach (char c in id)
        {
            if ("\"]".Contains(c))
            {
                continue;
            }
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(c);
            }
            else
            {
                sb.Append('_');
            }
        }
        return sb.ToString();
    }

    ///<inheritdoc/>
    public virtual Task SetParametersAsync(ParameterView parameters)
    {
        // Load cascading parameters
        bool result = parameters.TryGetValue(nameof(GridView), out _gridView);
        parameters.TryGetValue(nameof(RenderContext), out _renderContext);
        //Get the unique key
        string newQuickKey = OnGetQuickKey(parameters);
        if (string.IsNullOrEmpty(newQuickKey))
        {
            throw new InvalidOperationException($"The \"{COLUMN_KEY_PROPERTY_NAME}\" parameter must be specified for the {GetType().FriendlyName()} column. (Position : {State?.Index})");
        }
        bool initialization = _quickKey is null;
        bool newState = false;
        // Check the initialisation or the UniqueKey change
        if (newQuickKey != _quickKey)
        {
#if DEBUG
            if (!initialization)
            {
                Lagoon.Helpers.Trace.ToConsole(this, $"QuickKey changed ! {_quickKey} --> {newQuickKey}");
            }
#endif
            // Keep the unique key
            _quickKey = newQuickKey;
            // Try to get the existing column state
            newState = !GridView.ColumnDico.TryGetValue(_quickKey, out _state);
            if (newState)
            {
                _state = CreateState();
                _state.SetIndex(GridView.ColumnDico.Count);
            }
        }
        // Dispatch the parameters between the column state object and the column object (for cell level properties)
        Dictionary<string, object> stateParameters = null;
        if (RenderContext == GridRenderContext.Header)
        {
            stateParameters = new();
        }
        Dictionary<string, object> columnParameters = new();
        foreach (ParameterValue parameter in parameters)
        {
            if (parameter.Cascading)
            {
                continue;
            }
            if (GridColumnState.Parameters.Contains(parameter.Name))
            {
                stateParameters?.Add(parameter.Name, parameter.Value);
            }
            else
            {
                columnParameters.Add(parameter.Name, parameter.Value);
            }
        }
        ParameterView columnParameterView = ParameterView.FromDictionary(columnParameters);
        columnParameterView.SetParameterProperties(this);
#if DEBUG
        // We create state only for the header line
        System.Diagnostics.Debug.Assert(!newState || RenderContext == GridRenderContext.Header);
#endif
        if (RenderContext == GridRenderContext.Header)
        {
            ParameterView stateParameterView = ParameterView.FromDictionary(stateParameters);
            stateParameterView.SetParameterProperties(_state);
            if (newState)
            {
                _state.Initialize(this);
                OnStateInitialized();
            }
        }
        // Add the initialized new state
        if (newState)
        {               
            if(GridView.EditMode == GridEditMode.Cell && this is LgGridEditColumn)
            {
                throw new Exception("The cell edition mode not need a edition column. Remove the LgGridEditColumn.");
            }
            GridView.ColumnDico.Add(_quickKey, _state);
        }
        // Equivalent code part of OnInitialized for ComponentBase objects
        if (initialization)
        {
            OnInitialized();
        }
        OnParametersSet();
        // Notifies the renderer that the component should be rendered
        _renderHandle.Render(BuildRenderTree);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Method invoked when the column state is initialized.
    /// </summary>
    internal virtual void OnStateInitialized() { }

    /// <inheritdoc/>
    protected virtual void OnInitialized() { }

    ///<inheritdoc/>
    protected virtual void OnParametersSet() { }

    /// <summary>
    /// Initialise a new column state.
    /// </summary>
    /// <returns>The new column state.</returns>
    internal virtual GridColumnState CreateState()
    {
        return new GridColumnState();
    }

    /// <summary>
    /// Sort change event
    /// </summary>
    public Task OnSortChangeAsync(DataSortDirection direction)
    {
        State.SetSortDirection(direction);
        return GridView.OnColumnSortChangeAsync(new GridViewSortEventArgs(State.UniqueKey, State.SortDirection));
    }

    /// <summary>
    /// Get the rendering depending on the context.
    /// </summary>
    /// <returns></returns>
    internal RenderFragment GetRenderContextContent()
    {
        return RenderContext switch
        {
            GridRenderContext.Header => GetHeaderCellContent(),
            GridRenderContext.HeaderGroup => GetHeaderGroupCellContent(),
            GridRenderContext.Filter => GetFilterCellContent(),
            GridRenderContext.Body => GetCellContent(),
            _ => null,
        };
    }

    /// <summary>
    /// Get the cell content defined by the user.
    /// </summary>
    /// <returns></returns>
    internal abstract RenderFragment GetCustomCellContent();

    /// <summary>
    /// Get the cell content for edition defined by the user.
    /// </summary>
    /// <returns></returns>
    internal abstract RenderFragment GetCustomEditCellContent();

    /// <summary>
    /// Normal cell render.
    /// </summary>        
    /// <returns></returns>
    protected virtual RenderFragment GetCellContent()
    {
        return builder =>
        {
            builder.OpenComponent(1, State.CellComponentType);
            builder.AddAttribute(2, nameof(LgGridBaseCell<object>.CssClass), CssClass);

            builder.CloseComponent();
        };
    }

    /// <summary>
    /// Header cell render.
    /// </summary>
    /// <returns></returns>
    protected virtual RenderFragment GetHeaderCellContent()
    {
        return builder =>
        {
            builder.OpenComponent(1, typeof(LgGridHeaderCell));
            builder.AddComponentReferenceCapture(2, reference =>
            {
                State.HeaderCell = (LgGridHeaderCell)reference;
            });
            builder.CloseComponent();
        };
    }

    /// <summary>
    /// Filter cell render.
    /// </summary>
    /// <returns></returns>
    protected virtual RenderFragment GetFilterCellContent()
    {
        return builder =>
        {
            Type filterCellType;
            if (State.FilterCellContent is not null)
            {
                filterCellType = typeof(LgGridFilterCellCustom);
            }
            else
            {
                filterCellType = GetFilterCellType();
            }
            builder.OpenComponent(1, filterCellType);
            builder.AddComponentReferenceCapture(2, inst =>
            {
                State.FilterCell = (LgGridFilterCellBase)inst;
            });
            builder.CloseComponent();
        };
    }

    /// <summary>
    /// Gets the filter cell component type.
    /// </summary>
    /// <returns></returns>
    protected virtual Type GetFilterCellType()
    {
        return typeof(LgGridFilterCellEmpty);
    }

    /// <summary>
    /// Header group cell render.
    /// </summary>
    /// <returns></returns>
    protected virtual RenderFragment GetHeaderGroupCellContent()
    {
        return builder =>
        {
            builder.OpenComponent(1, State.HeaderGroupCellComponentType);
            builder.AddAttribute(2, nameof(CssClass), CssClass);
            builder.CloseComponent();
        };
    }

    /// <summary>
    /// Raise cell click event
    /// </summary>
    /// <param name="item">The row item.</param>
    internal async Task CellClickAsync(object item)
    {
        // Raise the column event
        if (OnCellClick.HasDelegate)
        {
            await OnCellClick.TryInvokeAsync(GridView.App, new GridViewCellClickEventArgs(item, State.UniqueKey));
        }
    }

    #endregion

}
