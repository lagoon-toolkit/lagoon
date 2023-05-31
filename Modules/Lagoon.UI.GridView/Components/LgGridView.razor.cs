using Lagoon.Helpers.Data;
using Lagoon.UI.Application;
using Lagoon.UI.GridView.Helpers;
using System.Runtime.CompilerServices;

namespace Lagoon.UI.Components;

/// <summary>
/// Gridview
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class LgGridView<TItem> : LgBaseGridView
{
    #region constant
    private const int COLUMN_SORTABLE_COUNT = 4;
    #endregion

    #region private classes

    /// <summary>
    /// Comparer for selection based to KeyField property
    /// </summary>        
    private class SelectionComparer : IEqualityComparer<TItem>
    {
        /// <summary>
        /// KeyField property
        /// </summary>
        private PropertyInfo _property;

        /// <summary>
        /// The KeyField property name
        /// </summary>
        private string _propertyName;

        private PropertyInfo Property => _property is null
                    ? throw new InvalidOperationException($"The property {_propertyName} defined in {nameof(KeyField)} doesn't exist in {typeof(TItem).FriendlyName()}.")
                    : _property;

        /// <summary>
        /// Object initialization
        /// </summary>
        /// <param name="keyField"></param>
        public SelectionComparer(string keyField)
        {
            _propertyName = keyField;
            _property = typeof(TItem).GetProperty(keyField,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// Item equality check
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(TItem x, TItem y)
        {
            object xValue = x is null ? null : Property.GetValue(x);
            object yValue = y is null ? null : Property.GetValue(y);
            return xValue is null ? yValue is null : xValue.Equals(yValue);
        }

        /// <summary>
        /// Return key hashcode
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(TItem obj)
        {
            object value = obj is null ? null : Property.GetValue(obj);
            return value?.GetHashCode() ?? 0;
        }
    }

    #endregion

    #region fields

#if DEBUG //TOCLEAN
    private System.Diagnostics.Stopwatch _chrono = new();
    private int _chronoCount = 0;
    private int _renderCount;
#endif

    /// <summary>
    /// Item used to add
    /// </summary>
    private TItem _addItem = default;

    /// <summary>
    /// Active edit row for add line
    /// </summary>
    private LgGridRow<TItem> _addRow;

    /// <summary>
    /// Data to diplaying
    /// </summary>
    private IEnumerable<TItem> _data;

    /// <summary>
    /// Dot net object reference
    /// </summary>
    private DotNetObjectReference<LgGridView<TItem>> _dotNetObjectReference;

    /// <summary>
    /// Active edit row for update line
    /// </summary>
    private LgGridRow<TItem> _editRow;

    /// <summary>
    /// List of field definition for dynamics columns.
    /// </summary>
    private List<LgGridValueDefinition<TItem>> _fieldDefinitionList;

    /// <summary>
    /// Define selector to focus element
    /// </summary>
    private string _focusElementSelector;

    /// <summary>
    /// Group definition separator
    /// </summary>
    private readonly char _groupSeparator = ',';

    /// <summary>
    /// The task to insert the Javascript file.
    /// </summary>
    private Task _includeJsTask;

    /// <summary>
    /// Tracker to detect if the GridView data source has changed.
    /// </summary>
    private CollectionChangeTracker<TItem> _itemsChangeTraker;

    /// <summary>
    /// Type of the datasource model
    /// </summary>
    private TItem _itemModel;

    /// <summary>
    /// The loaded item model parameter.
    /// </summary>
    private TItem _itemModelParameter;

    /// <summary>
    /// Level definition separator
    /// </summary>
    private readonly char _levelSeparator = '/';

    /// <summary>
    /// LgExportOptions modal ref
    /// </summary>
    private LgExportOptions _lgExportOptions;

    /// <summary>
    /// Indicate than data must be loaded
    /// </summary>
    private bool _loadData;

    /// <summary>
    /// Indicate than loading data is working
    /// </summary>
    internal bool _loadingData;

    /// <summary>
    /// Indicate the number of group level
    /// </summary>
    private int _maxGroupLevel;

    /// <summary>
    /// Tracker to detect if the GridView data source has changed.
    /// </summary>
    private CollectionChangeTracker<TItem> _selectionChangeTraker;

    /// <summary>
    /// Selection comparer
    /// </summary>
    private IEqualityComparer<TItem> _selectionComparer;

    /// <summary>
    /// Change selection indicator
    /// </summary>
    private Guid _selectionState;

    /// <summary>
    /// Indicate if the focus must be set to the last focused element
    /// </summary>
    private bool _setLastFocus;

    /// <summary>
    /// Keep previous selected row
    /// </summary>
    private LgGridRow<TItem> _previousSelectedRow;

    /// <summary>
    /// Indicate if the profile is loading
    /// </summary>
    private bool _profileLoading;

    #endregion

    #region render fragments

    /// <summary>
    /// Columns content
    /// </summary>
    [Parameter]
    public RenderFragment<TItem> Columns { get; set; }

    /// <summary>
    /// Gets or sets the additionnal actions for the toolbar (bottom)
    /// </summary>
    [Parameter]
    public RenderFragment ToolbarSelection { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets if the add line is shown on top or not
    /// </summary>
    [Parameter]
    public bool? AddItemOnTop { get; set; }

    /// <summary>
    /// List of provider separated by comma. Example : "xlsx,csv"
    /// </summary>
    [Parameter]
    public string HiddenExportList { get; set; }

    /// <summary>
    /// Instance of <typeparamref name="TItem"/> used to build columns structure.
    /// </summary>
    [Obsolete("Use the \"EmptyItemModel\" property to get an empty item.")]
    [Parameter]
    public TItem ItemModel { get; set; }

    /// <summary>
    /// Method called to crate a new item when adding a new line.
    /// </summary>
    [Parameter]
    public Func<TItem> NewItem { get; set; }


    /// <summary>
    /// Method called to initialise the export providers.
    /// </summary>
    [Parameter]
    public Action<ExportProviderManager> OnLoadExportProviders { get; set; }

    /// <summary>
    /// Gets or sets event rised when selection has changed
    /// </summary>
    /// <value></value>
    [Parameter]
    public EventCallback<GridViewSelectionEventArgs<TItem>> OnRowSelection { get; set; }

    /// <summary>
    /// Gets or sets if the pager is shown on top or not
    /// </summary>
    [Parameter]
    public bool? PagerOnTop { get; set; }

    /// <summary>
    /// Gets or sets the row CSS class selector callback.
    /// </summary>
    /// <value></value>
    [Parameter]
    public Func<int, TItem, string> RowCssClassSelector { get; set; }

    /// <summary>
    /// Gets or sets if the input page selector is shown
    /// </summary>
    [Parameter]
    public bool? DisplayInputPageSelector { get; set; }

    /// <summary>
    /// Gets or sets if the success message is shown when cell is edited
    /// </summary>
    [Parameter]
    public bool? DisplaySuccessSaveMessage { get; set; }

    /// <summary>
    /// Gets or sets the dictionnary key for empty data title
    /// </summary>
    [Parameter]
    public string NoDataTitle { get; set; } = "#GridViewNoDataTitle";

    /// <summary>
    /// Gets or sets the dictionnary key for empty data description
    /// </summary>
    [Parameter]
    public string EmptyDescription { get; set; } = "#GridViewEmptyDesc";

    #region Data

    /// <summary>
    /// Gets or sets list of the data
    /// </summary>
    [Parameter]
    public ICollection<TItem> Items { get; set; }

    /// <summary>
    /// Gets or sets wrapping of the content. 
    /// (Default is <c>false</c>)
    /// </summary>
    [Parameter]
    public bool WrapContent { get; set; }

    /// <summary>
    /// Gets or sets weither rows are selectable or not
    /// (Default is <c>false</c>)
    /// </summary>
    [Parameter]
    public bool SelectableRows { get; set; }

    /// <summary>
    /// Gets or sets remote data url
    /// </summary>
    [Parameter]
    public string ControllerUri { get; set; }

    /// <summary>
    /// Gets or sets remote data complementary class
    /// </summary>
    [Parameter]
    public object ControllerQueryArg { get; set; }

    /// <summary>
    /// Gets or sets list of the selected items
    /// </summary>
    /// <value></value>
    [Parameter]
    public ICollection<TItem> Selection { get; set; }

    /// <summary>
    /// Cell click event
    /// </summary>
    [Parameter]
    public EventCallback<GridViewCellClickEventArgs<TItem>> OnCellClick { get; set; }

    /// <summary>
    /// Command click event
    /// </summary>
    [Parameter]
    public EventCallback<GridViewCommandClickEventArgs<TItem>> OnCommandClick { get; set; }

    /// <summary>
    /// Cell value change event
    /// </summary>
    [Parameter]
    public EventCallback<GridViewValueChangeEventArgs<TItem>> OnValueChange { get; set; }

    /// <summary>
    /// Add data event
    /// </summary>
    [Parameter]
    public EventCallback<GridViewItemActionEventArgs<TItem>> OnValueAdd { get; set; }

    /// <summary>
    /// Row update data event
    /// </summary>
    [Parameter]
    public EventCallback<GridViewItemActionEventArgs<TItem>> OnRowUpdate { get; set; }

    /// <summary>
    /// Gets or sets group header content
    /// </summary>
    [Parameter]
    public RenderFragment<GridGroupData<TItem>> GroupHeaderContent { get; set; }

    /// <summary>
    /// Delete selected item(s) event
    /// </summary>
    [Parameter]
    public EventCallback<GridViewSelectionActionEventArgs<TItem>> OnDelete { get; set; }

#if SHRINK_DATA
    /// <summary>
    /// Gets or sets if the return data is limited to visible columns
    /// </summary>
    /// <remarks>Useful in remote data mode.</remarks>
    [Parameter]
    public bool ShrinkData { get; set; }

    /// <summary>
    /// Gets or sets list of the properties name complementary to field used with ShrinkData parameter
    /// </summary>
    [Parameter]
    public string RequiredFields { get; set; }
#endif

    #endregion

    #region profiles

    /// <summary>
    /// Policy required to add and edit shared profile
    /// </summary>
    [Parameter]
    public string SharedProfileAdministratorPolicy { get; set; }

    /// <summary>
    /// Profile storage
    /// </summary>
    [Parameter]
    public GridViewProfileStorage? StorageMode { get; set; }

    /// <summary>
    /// Gets or sets profile save mode
    /// </summary>
    [Parameter]
    public GridViewProfileSave? ProfileSaveMode { get; set; }

    #endregion

    #endregion

    #region cascading parameters

    /// <summary>
    /// Potential policies defined by an ancestor 
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    public LgAuthorizeView ParentPolicy { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Get the type of items used to generate rows.
    /// </summary>
    internal override Type ItemType { get; } = typeof(TItem);

    /// <summary>
    /// Get an empty item for building specials rows.
    /// </summary>
    internal TItem EmptyItem => _itemModel;

    /// <summary>
    /// Determine if data must be loaded from a controller.
    /// </summary>
    protected bool HasController => !string.IsNullOrEmpty(ControllerUri);

    /// <summary>
    /// Gets the list of fields with their property names and their property types.
    /// </summary>
    internal List<LgGridValueDefinition<TItem>> FieldDefinitionList
    {
        get
        {
            _fieldDefinitionList ??= GetFieldDefinitionList();

            return _fieldDefinitionList;
        }
    }

    /// <summary>
    /// Rebuild rows state indicator
    /// </summary>
    internal bool RebuildRows { get; set; }

    /// <summary>
    /// Identifiant unique de la grille en cours.
    /// </summary>
    protected string ElementId { get; } = GetNewElementId();

    /// <summary>
    /// Gets or sets unique identifier for the current columns layout
    /// </summary>
    internal Guid ColumnsLayoutState { get; set; }

    /// <summary>
    ///  Get the Policy Edit
    /// </summary>
    internal bool IsEditable { get; private set; } = true;

    /// <summary>
    /// Gets or sets active cell in the cell edit mode
    /// </summary>
    internal IActiveCell ActiveCell { get; set; }

    #region Style

    /// <summary>
    /// Gets or sets css placeholder
    /// </summary>
    private LgGridCss<TItem> GridViewCss { get; set; }

    /// <summary>
    /// Gets or sets style for rows
    /// </summary>                        
    internal string RowsStyle { get; set; }

    /// <summary>
    /// Gets or sets css variables
    /// </summary>                        
    internal string CssVariables { get; set; }

    /// <summary>
    /// Gets or set style for cells
    /// </summary>        
    internal List<string> ColumnsStyle { get; set; } = new List<string>();

    #endregion

    #region Pagination

    /// <summary>
    /// Gets or sets the state of the pagination.
    /// </summary>
    internal GridViewPaginationState<TItem> PaginationState { get; set; } = new();

    #endregion

    #region Toolbar

    /// <summary>
    /// Toolbar render
    /// </summary>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override RenderFragment RenderToolbarContent()
    {
        return (builder) =>
        {
            builder.OpenComponent(1, typeof(LgAuthorizeView));
            builder.AddAttribute(2, nameof(LgAuthorizeView.PolicyEdit), "*");
            builder.AddAttribute(3, nameof(LgAuthorizeView.PolicyVisible), "*");
            builder.AddAttribute(4, nameof(LgAuthorizeView.AllowAnnonymous), true);
            builder.AddAttribute(5, nameof(LgAuthorizeView.ChildContent), RenderAuhtorizeContent());
            builder.CloseComponent();
        };
    }

    /// <summary>
    /// Toolbar menus and buttons
    /// </summary>
    /// <returns></returns>
    private RenderFragment RenderAuhtorizeContent()
    {
        return (builder) =>
        {
            // Gridview toolbar should be show only if frame is expanded
            builder.OpenComponent(10, typeof(LgCollapsableView));
            builder.AddAttribute(11, nameof(LgCollapsableView.ViewMode), ViewMode.OnlyExpanded);
            builder.AddAttribute(12, nameof(LgCollapsableView.ChildContent), (RenderFragment)((subBuilder) =>
            {
                // Edit/Add buttons
                subBuilder.AddContent(13, RenderToolbarEdit());
            }));
            builder.CloseComponent();
            // Developper custom toolbar
            builder.AddCascadingValueComponent(14, Selection.Count, Toolbar, false);
            // Gridview toolbar should be show only if frame is expanded
            builder.OpenComponent(15, typeof(LgCollapsableView));
            builder.AddAttribute(16, nameof(LgCollapsableView.ViewMode), ViewMode.OnlyExpanded);
            builder.AddAttribute(17, nameof(LgCollapsableView.ChildContent), (RenderFragment)((subBuilder) =>
            {
                // Group buttons
                if (Features.HasFlag(GridFeature.Group) && HasActiveGroup)
                {
                    subBuilder.AddContent(30, RenderToolbarGroup());
                }
                // Display export buttons if export is activated
                if (Exportable.Value)
                {
                    subBuilder.AddContent(40, RenderToolbarExportMenu());
                }
                // Display filter button if there is one filter at least                
                if (Features.HasFlag(GridFeature.Filter))
                {
                    subBuilder.AddContent(50, RenderToolbarFilter());
                }
                // Column options button                
                subBuilder.AddContent(60, RenderToolbarColumnOptionsMenu());
                // Profiles button
                if (Features.HasFlag(GridFeature.Profile))
                {
                    subBuilder.AddContent(70, RenderToolbarProfilesMenu());
                }
            }));
            builder.CloseComponent();
        };
    }

    /// <summary>
    /// Render gridview toolbar
    /// </summary>
    /// <returns></returns>
    private RenderFragment RenderToolbarFilter()
    {
        return (groupBuilder) =>
        {
            groupBuilder.OpenComponent<LgToolbarGroup>(1);
            groupBuilder.AddAttribute(2, nameof(LgToolbarMenu.ChildContent), (RenderFragment)((builder) =>
            {
                builder.OpenComponent<LgToolbarButton>(10);
                builder.AddAttribute(11, "class", "filter-toolbar-style");
                builder.AddAttribute(12, nameof(LgToolbarButton.IconName), ShowFilters ? IconNames.All.FilterOff : IconNames.All.FilterOn);
                builder.AddAttribute(13, nameof(LgToolbarButton.Text), "GridViewToogleFilter".Translate());
                builder.AddAttribute(14, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, ToggleFiltersAsync));
                builder.AddAttribute(15, nameof(LgToolbarButton.AriaLabel), ShowFilters ? "#GridViewToogleFilterOnAria" : "#GridViewToogleFilterOffAria");
                builder.CloseComponent();
                if (ShowFilters)
                {
                    builder.OpenComponent<LgToolbarButton>(50);
                    builder.AddAttribute(51, "class", "filter-toolbar-style");
                    builder.AddAttribute(52, nameof(LgToolbarButton.IconName), IconNames.All.ArrowCounterclockwise);
                    builder.AddAttribute(53, nameof(LgToolbarButton.Text), "GridViewClearFilters".Translate());
                    builder.AddAttribute(54, nameof(LgToolbarButton.Tooltip), "GridViewClearFiltersLayout".Translate());
                    builder.AddAttribute(55, nameof(LgToolbarButton.TooltipPosition), GridViewBehaviour.Options.TooltipPosition);
                    builder.AddAttribute(56, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, ClearFiltersAsync));
                    builder.AddAttribute(57, nameof(LgToolbarButton.AriaLabel), "#GridViewClearFiltersLayoutAria");
                    builder.CloseComponent();
                }
            }));
            groupBuilder.CloseComponent();
        };
    }

    private RenderFragment RenderToolbarEdit()
    {
        return (builder) =>
        {
            string parentPolicyEdit = ParentPolicy?.PolicyEdit ?? "*";

            builder.OpenComponent<LgAuthorizeView>(50);
            builder.AddAttribute(51, nameof(LgAuthorizeView.PolicyEdit), parentPolicyEdit);
            builder.AddAttribute(52, nameof(LgAuthorizeView.PolicyVisible), parentPolicyEdit);
            //builder.AddAttribute(53, nameof(LgAuthorizeView.AllowAnnonymous), true);
            builder.AddAttribute(54, nameof(LgAuthorizeView.ChildContent), (RenderFragment)((subBuilder) =>
            {
                // Add button                
                if (Features.HasFlag(GridFeature.Add))
                {
                    subBuilder.OpenElement(55, "span");
                    subBuilder.AddAttribute(56, "class", HasEditColumn ? "" : "gridview-buttons-add");
                    subBuilder.OpenComponent<LgToolbarButton>(57);
                    subBuilder.AddAttribute(58, "class", "add-toolbar-style");
                    subBuilder.AddAttribute(59, nameof(LgToolbarButton.IconName), _addItem is null || HasEditColumn ? IconNames.All.PlusCircleFill : IconNames.Save);
                    subBuilder.AddAttribute(60, nameof(LgToolbarButton.Text), (_addItem is null || HasEditColumn ? "GridViewAdd" : "GridViewAddSave").Translate());
                    subBuilder.AddAttribute(61, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, OnAddValueAsync));
                    subBuilder.AddAttribute(62, nameof(LgToolbarButton.AriaLabel), _addItem is null || HasEditColumn ? "#GridViewAddTooltip" : "#GridViewAddSaveTooltip");
                    subBuilder.AddAttribute(63, nameof(LgToolbarButton.Tooltip), _addItem is null || HasEditColumn ? "#GridViewAddTooltip" : "#GridViewAddSaveTooltip");
                    subBuilder.AddAttribute(64, nameof(LgToolbarButton.TooltipPosition), GridViewBehaviour.Options.TooltipPosition);
                    subBuilder.AddAttribute(65, nameof(LgToolbarButton.Kind), _addItem is not null ? ButtonKind.Success : ButtonKind.Primary);
                    //TODO Add shortcut
                    //subBuilder.AddAttribute(66, "aria-keyshortcuts", "Ctrl+Alt+a");                    
                    subBuilder.CloseComponent();
                    if (!HasEditColumn && _addItem is not null)
                    {
                        subBuilder.OpenComponent<LgToolbarButton>(70);
                        subBuilder.AddAttribute(71, nameof(LgToolbarButton.IconName), IconNames.Cancel);
                        subBuilder.AddAttribute(72, nameof(LgToolbarButton.Text), "GridViewAddCancel".Translate());
                        subBuilder.AddAttribute(73, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, CancelAddAsync));
                        subBuilder.AddAttribute(74, nameof(LgToolbarButton.AriaLabel), "#GridViewAddCancelAddTooltip");
                        subBuilder.AddAttribute(75, nameof(LgToolbarButton.Tooltip), "#GridViewAddCancelAddTooltip");
                        subBuilder.AddAttribute(76, nameof(LgToolbarButton.TooltipPosition), GridViewBehaviour.Options.TooltipPosition);
                        subBuilder.CloseComponent();
                    }
                    subBuilder.CloseElement();
                }
                // Edit button                
                if (DisplayEdit)
                {
                    subBuilder.OpenElement(80, "span");
                    subBuilder.AddAttribute(81, "class", "gridview-buttons-edit");
                    subBuilder.OpenComponent<LgToolbarButton>(82);
                    subBuilder.AddAttribute(83, nameof(LgToolbarButton.IconName), IconNames.Save);
                    subBuilder.AddAttribute(84, nameof(LgToolbarButton.Text), "GridViewEditSave".Translate());
                    subBuilder.AddAttribute(85, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, OnUpdateValueAsync));
                    subBuilder.AddAttribute(86, nameof(LgToolbarButton.AriaLabel), _editRow is null ? "#GridViewEditTooltip" : "#GridViewEditSaveTooltip");
                    subBuilder.AddAttribute(87, nameof(LgToolbarButton.Tooltip), _editRow is null ? "#GridViewEditTooltip" : "#GridViewEditSaveTooltip");
                    subBuilder.AddAttribute(87, nameof(LgToolbarButton.Kind), _editRow is null ? ButtonKind.Secondary : ButtonKind.Success);
                    subBuilder.AddAttribute(88, nameof(LgToolbarButton.TooltipPosition), GridViewBehaviour.Options.TooltipPosition);
                    subBuilder.CloseComponent();
                    subBuilder.OpenComponent<LgToolbarButton>(91);
                    subBuilder.AddAttribute(92, nameof(LgToolbarButton.IconName), IconNames.Cancel);
                    subBuilder.AddAttribute(93, nameof(LgToolbarButton.Text), "GridViewEditCancel".Translate());
                    subBuilder.AddAttribute(94, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, CancelEditAsync));
                    subBuilder.AddAttribute(95, nameof(LgToolbarButton.AriaLabel), "#GridViewEditCancelTooltip");
                    subBuilder.AddAttribute(96, nameof(LgToolbarButton.Tooltip), "#GridViewEditCancelTooltip");
                    subBuilder.AddAttribute(97, nameof(LgToolbarButton.TooltipPosition), GridViewBehaviour.Options.TooltipPosition);
                    subBuilder.CloseComponent();
                    subBuilder.CloseElement();
                }
            }));
            builder.CloseComponent();
        };
    }

    private RenderFragment RenderToolbarColumnOptionsMenu()
    {
        return (builder) =>
        {
            builder.OpenComponent<LgToolbarMenu>(0);
            builder.AddAttribute(1, nameof(LgToolbarMenu.IconName), IconNames.All.GridColumns);
            builder.AddAttribute(2, nameof(LgToolbarMenu.Text), "GridViewColumnMenu".Translate());
            builder.AddAttribute(3, nameof(LgToolbarMenu.ChildContent), RenderToolbarColumnOptions());
            builder.AddAttribute(4, nameof(LgToolbarMenu.AriaLabel), "#GridViewColumnMenuAria");
            builder.CloseComponent();
        };
    }

    private RenderFragment RenderToolbarProfilesMenu()
    {
        return (builder) =>
        {
            bool defaultSelected = _currentProfileItem is null || _currentProfileItem.IsDefault();
            builder.OpenComponent<LgToolbarMenu>(0);
            builder.AddAttribute(1, nameof(LgToolbarMenu.IconName), IconNames.All.StarFill);
            builder.AddAttribute(2, nameof(LgToolbarMenu.Text), defaultSelected ? "GridViewProfileDisplayed".Translate() : _currentProfileItem?.Label);
            builder.AddAttribute(3, nameof(LgToolbarMenu.ChildContent), RenderToolbarProfiles());
            builder.AddAttribute(4, nameof(LgToolbarMenu.AriaLabel), "GridViewProfiles"
                .Translate(defaultSelected ? "GridViewProfileDefault".Translate() : _currentProfileItem?.Label));
            builder.CloseComponent();
        };
    }

    private RenderFragment RenderToolbarColumnOptions()
    {
        return (builder) =>
        {
            // Column selection
            if (Features.HasFlag(GridFeature.Move)
            || Features.HasFlag(GridFeature.Visibility)
            || Features.HasFlag(GridFeature.Freeze))
            {
                builder.OpenComponent<LgToolbarButton>(10);
                builder.AddAttribute(11, nameof(LgToolbarButton.IconName), IconNames.All.Tools);
                builder.AddAttribute(12, nameof(LgToolbarButton.Text), "GridViewColumnOptions".Translate());
                builder.AddAttribute(13, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, OpenColumnOptionsModalAsync));
                builder.AddAttribute(14, nameof(LgToolbarButton.AriaLabel), "#GridViewColumnVisibleAria");
                builder.CloseComponent();
            }
            // Resize columns
            if (Features.HasFlag(GridFeature.Resize))
            {
                builder.OpenComponent<LgToolbarButton>(50);
                builder.AddAttribute(51, nameof(LgToolbarButton.IconName), IconNames.All.DistributeHorizontal);
                builder.AddAttribute(52, nameof(LgToolbarButton.Text), "GridViewResizeColumns".Translate());
                builder.AddAttribute(53, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, RestoreDefaultColumnWidthsAsync));
                builder.AddAttribute(54, nameof(LgToolbarButton.AriaLabel), "#GridViewColumnResizeAria");
                builder.CloseComponent();
            }
            // GroupBy rows
            if (Features.HasFlag(GridFeature.Group))
            {
                builder.OpenComponent<LgToolbarButton>(50);
                builder.AddAttribute(61, nameof(LgToolbarButton.IconName), IconNames.All.DistributeVertical);
                builder.AddAttribute(62, nameof(LgToolbarButton.Text), "GridViewGroupOptions".Translate());
                builder.AddAttribute(63, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, OpenGroupOptionsModalAsync));
                builder.AddAttribute(64, nameof(LgToolbarButton.AriaLabel), "#GridViewGroupOptionsAria");
                builder.CloseComponent();
            }
            // Reset
            builder.OpenComponent<LgToolbarButton>(60);
            builder.AddAttribute(71, nameof(LgToolbarButton.IconName), IconNames.All.ArrowCounterclockwise);
            builder.AddAttribute(72, nameof(LgToolbarButton.Text), "GridViewResetLayout".Translate());
            builder.AddAttribute(73, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, ResetPersistenceAsync));
            builder.AddAttribute(74, nameof(LgToolbarButton.AriaLabel), "#GridViewColumnResetAria");
            builder.CloseComponent();
        };
    }

    private RenderFragment RenderToolbarProfiles()
    {
        return (builder) =>
        {
            // Profile list
            int index = 0;
            int j = 0;
            bool sharedProfil = false;
            bool localProfil = false;
            foreach (ProfileItem profile in _profileItems.OrderBy(o => o.IsSharedProfile).ThenBy(o => o.Label))
            {

                if (profile.IsSharedProfile && !sharedProfil)
                {
                    sharedProfil = true;
                    builder.OpenComponent<LgMenuSeparator>(index);
                    builder.CloseComponent();
                    builder.OpenComponent<LgMenuTitle>(index + 1);
                    builder.AddAttribute(index + 2, nameof(LgMenuTitle.Text), "GridViewSharedProfile".Translate());
                    builder.CloseComponent();
                }
                else if (!profile.IsSharedProfile && !localProfil)
                {
                    localProfil = true;
                    builder.OpenComponent<LgMenuTitle>(index + 3);
                    builder.AddAttribute(index + 4, nameof(LgMenuTitle.Text), "GridViewMyProfile".Translate());
                    builder.CloseComponent();
                }

                bool selected = profile.Id == _currentProfileItem?.Id;
                selected = selected && profile.IsSharedProfile == _currentProfileItem.IsSharedProfile;
                builder.OpenComponent<LgCrudToolbarButton>(index);
                builder.AddAttribute(index + 5, nameof(LgCrudToolbarButton.IconName), selected ? IconNames.All.RecordCircleFill : IconNames.All.Circle);
                builder.AddAttribute(index + 6, nameof(LgCrudToolbarButton.Text), profile.Label);

                // Remove profile button
                if (CanRemoveProfile(profile))
                {
                    builder.AddAttribute(index + 7, nameof(LgCrudToolbarButton.OnRemove),
                        EventCallback.Factory.Create<ActionEventArgs>(this, arg => RemoveAndLoadDefaultProfileIfNecessaryAsync(profile)));
                    builder.AddAttribute(index + 8, nameof(LgCrudToolbarButton.OnRemoveConfirmationMessage), "GridViewProfileOnRemoveConfirmationMsg".Translate());
                    builder.AddAttribute(index + 9, nameof(LgCrudToolbarButton.RemoveButtonTooltip), "GridViewProfileRemoveButtonTooltip".Translate());
                    builder.AddAttribute(index + 10, nameof(LgCrudToolbarButton.RemoveButtonAria), "GridViewProfileRemoveButtonAria".Translate(profile.Label));
                }
                // Save profile button
                if (CanSaveProfile(profile))
                {
                    builder.AddAttribute(index + 11, nameof(LgCrudToolbarButton.OnSave), EventCallback.Factory.Create<ActionEventArgs>(this, arg => UpdateProfileAsync(profile, true)));
                    builder.AddAttribute(index + 12, nameof(LgCrudToolbarButton.OnSaveConfirmationMessage), "GridViewProfileOnSaveConfirmationMsg".Translate());
                    builder.AddAttribute(index + 13, nameof(LgCrudToolbarButton.SaveButtonTooltip), "GridViewProfileSaveButtonTooltip".Translate());
                    builder.AddAttribute(index + 14, nameof(LgCrudToolbarButton.SaveButtonAria), "GridViewProfileSaveButtonAria".Translate(profile.Label));
                }

                builder.AddAttribute(index + 15, nameof(LgCrudToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, arg => LoadProfileAsync(profile)));
                builder.CloseComponent();
                index += 16 * j;
                j++;
            }
            builder.OpenComponent<LgMenuSeparator>(9999);
            builder.CloseComponent();
            // Add new profile button                
            builder.OpenComponent<LgToolbarButton>(10000);
            builder.AddAttribute(10001, nameof(LgToolbarButton.IconName), IconNames.All.Plus);
            builder.AddAttribute(10002, nameof(LgToolbarButton.Text), "GridViewNewProfileBtn".Translate());
            builder.AddAttribute(10003, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, OpenAddNewProfilePopupAsync));
            builder.CloseComponent();
        };
    }

    /// <summary>
    /// Render the export menu.
    /// </summary>
    /// <returns></returns>
    private RenderFragment RenderToolbarExportMenu()
    {
        return (builder) =>
        {
            builder.OpenComponent<LgToolbarButton>(200);
            builder.AddAttribute(201, nameof(LgToolbarButton.IconName), IconNames.Export);
            builder.AddAttribute(202, nameof(LgToolbarButton.Text), "toolbarExport".Translate());
            builder.AddAttribute(203, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, ShowExportOptionsAsync));
            builder.AddAttribute(204, nameof(LgToolbarButton.AriaLabel), "#GridViewExportOptionsAria");
            builder.CloseComponent();
        };
    }

    /// <summary>
    /// Render of the group management buttons
    /// </summary>
    /// <returns></returns>
    private RenderFragment RenderToolbarGroup()
    {
        return (builder) =>
        {
            builder.OpenComponent<LgToolbarGroup>(300);
            builder.AddAttribute(301, nameof(LgToolbarGroup.CssClass), "gridview-group");
            //builder.AddAttribute(302, "aria-label", "GridViewGroupMng".Translate());
            builder.AddAttribute(303, nameof(LgToolbarMenu.ChildContent), (RenderFragment)((builder) =>
            {
                builder.OpenComponent<LgToolbarButton>(10);
                builder.AddAttribute(11, nameof(LgToolbarButton.IconName), IconNames.All.ViewStacked);
                builder.AddAttribute(12, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, OnExpandGroup));
                builder.AddAttribute(13, nameof(LgToolbarButton.AriaLabel), "#GridViewGroupExpandTooltip");
                builder.AddAttribute(14, nameof(LgToolbarButton.Tooltip), "#GridViewGroupExpandTooltip");
                builder.AddAttribute(15, nameof(LgToolbarButton.Kind), ButtonKind.Secondary);
                builder.CloseComponent();

                builder.OpenComponent<LgToolbarButton>(20);
                builder.AddAttribute(21, nameof(LgToolbarButton.IconName), IconNames.All.ViewList);
                builder.AddAttribute(22, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, OnCollapseGroup));
                builder.AddAttribute(23, nameof(LgToolbarButton.AriaLabel), "#GridViewGroupCollapseTooltip");
                builder.AddAttribute(24, nameof(LgToolbarButton.Tooltip), "#GridViewGroupCollapseTooltip");
                builder.AddAttribute(25, nameof(LgToolbarButton.Kind), ButtonKind.Secondary);
                builder.CloseComponent();

                builder.OpenComponent<LgToolbarButton>(30);
                builder.AddAttribute(31, nameof(LgToolbarButton.IconName), IconNames.All.X);
                builder.AddAttribute(32, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, OnRemoveGroupAsync));
                builder.AddAttribute(33, nameof(LgToolbarButton.AriaLabel), "#GridViewGroupClearTooltip");
                builder.AddAttribute(34, nameof(LgToolbarButton.Tooltip), "#GridViewGroupClearTooltip");
                builder.AddAttribute(35, nameof(LgToolbarButton.Kind), ButtonKind.Secondary);
                builder.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }

    #endregion

    /// <summary>
    /// Gets or sets list of the columns calculations values
    /// </summary>
    internal Dictionary<string, object> CalculationValues { get; set; }

    /// <summary>
    /// Gets if there is one calculation at least
    /// </summary>
    private bool HasCalculation => CalculationValues is not null && CalculationValues.Any();

    /// <summary>
    /// Gets if gridview has column filter
    /// </summary>
    internal bool HasFilter => Features.HasFlag(GridFeature.Filter) && ShowFilters;

    /// <summary>
    /// Export manager
    /// </summary>
    private ExportProviderManager _exportProviderManager;

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // Initialized the items change tracker
        _itemsChangeTraker = new(() => _loadData = true);
        _selectionChangeTraker = new(OnSelectionUpdated);
        // Exportable
        Exportable ??= GridViewBehaviour.Options.Exportable;
        // Initializing exprt providers
        _exportProviderManager = new ExportProviderManager(App.BehaviorConfiguration.ExportProviderManager);
        OnLoadExportProviders?.Invoke(_exportProviderManager);
        _exportProviderManager.Remove(HiddenExportList);
        // Load the default user policy indicate that can we save shared profils                   
        SharedProfileAdministratorPolicy ??= GridViewBehaviour.Options.SharedProfileAdministratorPolicy
#pragma warning disable CS0618 // BehaviorConfiguration.SharedProfileAdministratorPolicy have been replaced by GridViewBehaviour.Options.SharedProfileAdministratorPolicy (01/06/2022)
                ?? App.BehaviorConfiguration.SharedProfileAdministratorPolicy;
#pragma warning restore CS0618
        // The storage mode
        StorageMode ??= GridViewBehaviour.Options.ProfileStorageMode;
        // Save profile mode
        ProfileSaveMode ??= GridViewBehaviour.Options.ProfileSaveMode;
        // Display summary filters
        ShowFiltersSummary ??= GridViewBehaviour.Options.ShowSummaryFilters;
        ShowFilters = DefaultShowFilters;
        // Load the default value for displaying header Tooltips
        DisplayDefaultTooltipHeader ??= GridViewBehaviour.Options.DisplayDefaultTooltipHeader;
        // Load the default value for displaying page input (go to page)
        DisplayInputPageSelector ??= GridViewBehaviour.Options.DisplayInputPageSelector;
        // Default size  page
        DefaultPageSize = DefaultPageSize > 0 ? DefaultPageSize : GridViewBehaviour.Options.DefaultPageSize;
        // Default pagination
        PaginationSizeSelector = PaginationSizeSelector is not null && PaginationSizeSelector.Any() ? PaginationSizeSelector : GridViewBehaviour.Options.PaginationSizeSelector;
        // Group is collapsed ?
        State.GroupCollapsed = DefaultGroupCollapsed;
        // Display add item on top
        AddItemOnTop ??= GridViewBehaviour.Options.AddItemOnTop;
        // Display pager on top
        PagerOnTop ??= GridViewBehaviour.Options.PagerOnTop;
        // Default success save message visibility
        DisplaySuccessSaveMessage ??= GridViewBehaviour.Options.DisplaySuccessSaveMessage;
        if (string.IsNullOrEmpty(StateId))
        {
            ShowException(new ArgumentNullException("StateId parameter is mandatory."));
        }
        // Check if the KeyField is defined, else we use the Equals/GetHashCode of the TItem
        _selectionComparer = string.IsNullOrEmpty(KeyField) ? EqualityComparer<TItem>.Default : CreateKeyFieldSelectionComparer();
        _loadData = true;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        _itemsChangeTraker?.Dispose();
        _selectionChangeTraker?.Dispose();
        _dotNetObjectReference?.Dispose();
        JS.InvokeVoid("Lagoon.GridView.Action", StateId, "Dispose");
        base.Dispose(disposing);
    }

    /// <summary>
    /// Return comparer used to check selected items
    /// </summary>
    /// <returns></returns>
    protected virtual IEqualityComparer<TItem> CreateKeyFieldSelectionComparer()
    {
        return new SelectionComparer(KeyField);
    }

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        //DO NOT USE TO AVOID ADDITIONAL RENDERING !
        return Task.CompletedTask;
    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        InitProfileKeysPattern();
        _itemsChangeTraker.Track(Items);

        // Load informations about the data model
#pragma warning disable CS0618 // The only place where ItemModel parameter must be used, else use EmptyItem.
        if (_itemModel is null || !ReferenceEquals(ItemModel, _itemModelParameter))
        {
            _itemModelParameter = ItemModel;
            _itemModel = _itemModelParameter is null ? CreateItemModel() : _itemModelParameter;
            ModelExpressionResolver = new(_itemModel, true);
        }
#pragma warning restore CS0618 // Le type ou le membre est obsolète
        // Initialize selection list
        Selection ??= new HashSet<TItem>();
        // Check if selection list has changed
        _selectionChangeTraker.Track(Selection);
    }

    /// <summary>
    /// Indicate that the item model must be renewed.
    /// </summary>
    internal void ResetItemModel()
    {
        _itemModel = default;
    }

    /// <inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        // Check if profile is loading
        if (!_profileLoading)
        {
            try
            {
                _profileLoading = true;
                SharedProfileManagement = HasAccessToSharedProfile && await IsInPolicyAsync(SharedProfileAdministratorPolicy);
                if (_currentProfileItem == null)
                {
                    // Load current or default profile
                    await InitializeProfileAsync(HasAccessToSharedProfile);
                }
                // Page active check
                int currentPage = CurrentPage;
                if (currentPage < 1)
                {
                    currentPage = 1;
                }
                if (PaginationState.CurrentPage != currentPage)
                {
                    _loadData = true;
                }
                // Reset the dynamic column definition
                _fieldDefinitionList = default;
                // Check policy edit
                IsEditable = ParentPolicy == null || await IsInPolicyAsync(ParentPolicy.PolicyEdit);
                // Reload data if needed
                if (_loadData && !_loadingData)
                {
                    await LoadDataAsync(true);
                }
            }
            finally
            {
                _profileLoading = false;
            }
        }
    }

    /// <summary>
    /// Invokes the specified JavaScript function asynchronously.
    /// </summary>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments.</param>
    private async ValueTask GridJsInvokeVoidAsync(string identifier, params object[] args)
    {
        // Include the javascript file if it hasn't already been done
        await EnsureJsIncluded();
        // Execute the method
        await JS.InvokeVoidAsync(identifier, args);
    }

    /// <summary>
    /// Invokes the specified JavaScript function asynchronously.
    /// </summary>
    /// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments.</param>
    /// <returns>An instance of TValue obtained by JSON-deserializing the return value.</returns>
    private async Task<TValue> GridJsInvokeAsync<TValue>(string identifier, params object[] args)
    {
        // Include the javascript file if it hasn't already been done
        await EnsureJsIncluded();
        // Execute the method
        return await JS.InvokeAsync<TValue>(identifier, args);
    }

    /// <summary>
    /// Include the javascript file if it hasn't already been done.
    /// </summary>
    private Task EnsureJsIncluded()
    {
        _includeJsTask ??= IncludeJavacriptAsync();
        return _includeJsTask;
    }

    /// <summary>
    /// Include the GridView Javascript functions.
    /// </summary>
    private async Task IncludeJavacriptAsync()
    {
        await JS.ScriptIncludeAsync("_content/Lagoon.UI.GridView/js/main.min.js");
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
#if DEBUG //TOCLEAN
        _chronoCount--;
        if (_chronoCount == 0)
        {
            _chrono.Stop();
            Trace.ToConsole(this, $"ZZZ {_chrono.ElapsedMilliseconds} ms");
        }
#endif
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            // Load profile columns data after columns render
            LoadCurrentProfileColumnsState();
        }
        // Set column style
        await BuildGridViewStyleAsync();
        if (firstRender)
        {
            _dotNetObjectReference ??= DotNetObjectReference.Create(this);
            await GridJsInvokeVoidAsync("Lagoon.GridView.AddGridView", StateId, _dotNetObjectReference);
        }
        // Simulate "sticky" for Firefox
        if (_addItem is not null)
        {
            await GridJsInvokeAsync<bool>("Lagoon.GridView.Sticky", new object[] { StateId, !AddItemOnTop });
        }
        // Set focus on last focused element
        if (_setLastFocus)
        {
            await GridJsInvokeVoidAsync("Lagoon.GridView.Action", StateId, "SetLastFocus");
            _setLastFocus = false;
        }
        // Focus element
        if (!string.IsNullOrEmpty(_focusElementSelector))
        {
            await GridJsInvokeVoidAsync("Lagoon.GridView.Action", StateId, "SetFocus", _focusElementSelector);
            _focusElementSelector = null;
        }
    }

    /// <summary>
    /// Return the title with counter replaced.
    /// </summary>
    /// <returns>The title with counter replaced.</returns>
    protected override string GetRenderTitle()
    {
        string title = Title.CheckTranslate();
        if (string.IsNullOrEmpty(title))
        {
            return null;
        }
        StringBuilder sb = new();
        char one = '?';
        char two = '?';
        bool found = false;
        foreach (char c in title)
        {
            if (c == '}' && one == '{')
            {
                if (two == '0')
                {
                    sb.Length -= 2;
                    if (PaginationState.TotalRows < 0)
                    {
                        sb.Append("...");
                    }
                    else
                    {
                        sb.Append(PaginationState.TotalRows);
                    }
                    found = true;
                }
                else if (two == '1')
                {
                    sb.Length -= 2;
                    if (PaginationState.ActiveRows < 1)
                    {
                        sb.Append("...");
                    }
                    else
                    {
                        sb.Append(PaginationState.ActiveRows);
                    }
                    found = true;
                }
                else
                {
                    sb.Append(c);
                }
            }
            else
            {
                sb.Append(c);
            }
            one = two;
            two = c;
        }
        return found ? sb.ToString() : title;
    }

    /// <summary>
    /// Change column position
    /// </summary>
    /// <param name="key">column identifier</param>
    /// <param name="newPosition">new position</param>
    public Task SetColumnOrderAsync(string key, int newPosition)
    {
        if (newPosition < 1 || newPosition > ColumnList.Count)
        {
            ShowException(new ArgumentOutOfRangeException(nameof(newPosition)));
        }
        GridColumnState destination = ColumnList.First(c => c.Order == newPosition);
        return SetColumnOrderByColumnAsync(GetColumn(key), destination);
    }

    /// <summary>
    /// Change column position
    /// </summary>
    /// <param name="current"></param>
    /// <param name="destination"></param>
    private Task SetColumnOrderByColumnAsync(GridColumnState current, GridColumnState destination)
    {
#pragma warning disable IDE0180 // Utilisez le tuple pour échanger des valeurs
        int prevOrder = current.Order;
        current.Order = destination.Order;
        destination.Order = prevOrder;
#pragma warning restore IDE0180 // Utilisez le tuple pour échanger des valeurs
        return BuildGridViewStyleAsync();

    }

    /// <summary>
    /// Build grdi view style
    /// </summary>
    private async Task BuildGridViewStyleAsync()
    {
        _maxGroupLevel = GroupByColumnKeyList.Count > 0 ? GroupByColumnKeyList.Keys.Max() : 0;
        BuildColumnsStyle(_maxGroupLevel);
        RowsStyle = GetRowStyle(_maxGroupLevel);
        CssVariables = await GetCssVariablesAsync();
        GridViewCss.Refresh();
    }

    /// <summary>
    /// Build column style
    /// </summary>
    private void BuildColumnsStyle(int maxGroupLevel)
    {
        // Build column style
        ColumnsStyle.Clear();
        IEnumerable<GridColumnState> freezedColumns = ColumnList.Where(x => x.Frozen && x.Visible);
        int maxOrderFreezedColumns = freezedColumns.Any() ? freezedColumns.Max(x => x.Order) : -1;
        HeaderGroup = false;
        int order = 1 + maxGroupLevel;
        foreach (GridColumnState state in ColumnList.Where(c => c.Visible).OrderBy(c => c.Order))
        {
            ColumnsStyle.Add(GetColumnStyle(state, order, maxOrderFreezedColumns));
            HeaderGroup = HeaderGroup || !string.IsNullOrEmpty(state.GetGroupName());
            order++;
        }
    }

    /// <summary>
    /// Insert column before another
    /// </summary>
    /// <param name="current"></param>
    /// <param name="destination"></param>
    private Task InsertColumnBeforeAsync(GridColumnState current, GridColumnState destination)
    {
        int newOrder = destination.Order;
        current.Order = newOrder;
        destination.Order += 1;
        int order = 1;
        foreach (GridColumnState column in ColumnList.OrderBy(c => c.Order))
        {
            column.Order = order;
            order++;
        }
        return BuildGridViewStyleAsync();
    }

    /// <summary>
    /// Find is the column key is used to grouping
    /// </summary>
    /// <param name="key">The Key of the column.</param>
    /// <returns></returns>
    public bool IsInGroup(string key)
    {
        return !string.IsNullOrEmpty(key) && GroupByColumnKeyList is not null
&& GroupByColumnKeyList.Any(g => g.Value.Any(d => string.Equals(d, key, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    /// Resize the colunms widths callable by JS.
    /// </summary>
    /// <returns></returns>
    [JSInvokable]
    public async Task JsResizeColumnsAsync()
    {
        await ResizeColumnsWidthsAsync();
        //x            await UpdateCurrentProfileAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Resize the colunm width.
    /// </summary>        
    private async Task ResizeColumnsWidthsAsync()
    {
        int totalWidth = await GetTotalWidthAsync();
        int frCount = 0;
        int usedWidth = 0;
        IEnumerable<GridColumnState> columnVisibles = ColumnList.Where(c => c.Visible);
        // Calculate the width for columns with pixel or percent sizes
        foreach (GridColumnState column in columnVisibles)
        {
            // Save the amount of column that need to be calculated based on free space
            if (column.InitialWidth.Unit == GridCssSizeUnit.Fr)
            {
                frCount += column.InitialWidth.Value;
            }
            else
            {
                // Convert each width to a PX value
                int columnWidth = column.InitialWidth.Unit switch
                {
                    GridCssSizeUnit.Percent => column.InitialWidth.Value * totalWidth / 100,
                    GridCssSizeUnit.Px => column.InitialWidth.Value,
                    _ => throw new NotImplementedException()
                };
                // Check that the final width is larger than the minimum required for the header.
                await SetColumnWidthAsync(column, columnWidth);
                // Save the used width from px & percent width
                usedWidth += column.CalculatedWidth.Value;
            }
        }
        // Calculate the width for columns with fragment of left space sizes
        int leftWidth = totalWidth - usedWidth;
        List<GridColumnState> columnsLeftWidth = new(columnVisibles.Where(c => c.InitialWidth.Unit == GridCssSizeUnit.Fr));
        int totalDiffWidth;
        int totalDiffFrCount;
        do
        {
            totalDiffWidth = 0;
            totalDiffFrCount = 0;
            for (int i = columnsLeftWidth.Count - 1; i >= 0; i--)
            {
                GridColumnState column = columnsLeftWidth[i];
                // Convert each Fr width to a pixel value
                int desiredWidth = leftWidth / frCount * column.InitialWidth.Value;
                await SetColumnWidthAsync(column, desiredWidth);
                // Get the difference between the desired width and the applied width
                int diffWidth = desiredWidth - column.CalculatedWidth.Value;
                if (diffWidth != 0)
                {
                    // Column width is set
                    columnsLeftWidth.RemoveAt(i);
                    totalDiffFrCount += column.InitialWidth.Value;
                    totalDiffWidth += column.CalculatedWidth.Value;
                }
            }
            // Remove the width of columns that reach their size limit
            leftWidth -= totalDiffWidth;
            frCount -= totalDiffFrCount;
            // Continue as long as there is space to distribute and columns to receive it
        } while (totalDiffWidth != 0 && columnsLeftWidth.Count > 0);
    }

    /// <summary>
    /// Resize the colunms widths.
    /// </summary>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the \"RestoreDefaultColumnWidthsAsync()\" method.")]
    public Task ResizeColumnsAsync()
    {
        return RestoreDefaultColumnWidthsAsync();
    }

    /// <summary>
    /// Resize the colunms widths.
    /// </summary>
    /// <returns></returns>
    public async Task RestoreDefaultColumnWidthsAsync()
    {
        // We restore the default_ width
        foreach (GridColumnState columnState in ColumnList)
        {
            columnState.InitialWidth = new GridViewCssSize(columnState.DefaultWidth);
        }
        // We update the render
        await ResizeColumnsWidthsAsync();
        await UpdateCurrentProfileAsync();
    }

    private async Task SetColumnWidthAsync(GridColumnState column, int pixelWidth)
    {
        // Check that the width is larger than the minimum required for the header.
        pixelWidth = Math.Max(pixelWidth, await GetColumnHeaderWidthAsync(column));
        // Check that the width is larger than the minimum for the column and the maximum for the column
        column.CalculatedWidth = new GridViewCssSize(pixelWidth, column.MinWidth, column.MaxWidth);
    }

    /// <summary>
    /// Return real width of the column
    /// </summary>
    /// <param name="columnState"></param>
    /// <returns></returns>
    private async Task<int> GetColumnHeaderWidthAsync(GridColumnState columnState)
    {
        return columnState.InitialWidth.Unit == GridCssSizeUnit.Px
            ? columnState.InitialWidth.Value
            : await JS.InvokeAsync<int>("Lagoon.JsUtils.GetElementScrollWidth", $".gridview-{ElementId} .gridview-row.gridview-header .gridview-cell.gridview-col{columnState.Index}", columnState.InitialWidth.Value);
    }

    /// <summary>
    /// Return width of the gridview 
    /// </summary>
    /// <returns></returns>
    private Task<int> GetTotalWidthAsync()
    {
        return GridJsInvokeAsync<int>("Lagoon.GridView.GetTotalWidth", ElementId);
    }

    /// <summary>
    /// Render the active edit row or the add row
    /// </summary>
    internal override void RefreshEditRow()
    {
        (_editRow ?? _addRow)?.Refresh();
    }

    #region Build styles

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);

        builder.Add("gridview");
        builder.Add($"gridview-{ElementId}");
        if (GridStyleType == GridStyleType.Card)
        {
            builder.Add("gridview-card");
        }
        if (_addRow is not null)
        {
            builder.Add("gridview-add");
        }
        if (_editRow is not null)
        {
            builder.Add("gridview-edit");
        }
        if (SelectableRows)
        {
            builder.Add("gridview-rowsel");
        }
        builder.Add($"gridview-content-{(WrapContent ? "wrap" : "normal")}");
    }

    /// <summary>
    /// Get row style
    /// </summary>
    /// <returns></returns>
    private string GetRowStyle(int maxGroupLevel)
    {
        GridColumnState groupFirstColumn = null;
        List<string> widths = new();
        StringBuilder style = new();
        string lastGroupName = null;
        int lastColSpan = 0;
        IEnumerable<GridColumnState> columsListOrder = ColumnList.Where(c => c.Visible).OrderBy(c => c.Order);

        // Add group level offset column
        if (maxGroupLevel > 0)
        {
            for (int i = 1; i <= maxGroupLevel; i++)
            {
                widths.Add($"var(--group-margin-left)");
            }
        }

        // Columns widths and order
        foreach (GridColumnState column in columsListOrder)
        {
            widths.Add(column.CalculatedWidth.ToString());
            // Header groups
            string groupName = column.GetGroupName();
            if (!string.IsNullOrEmpty(groupName))
            {
                if (lastGroupName != groupName)
                {
                    groupFirstColumn = column;
                    lastColSpan = 0;
                }
                else
                {
                    // Hide header cell
                    column.HeaderColSpan = -1;
                }
                lastColSpan += 1;
                groupFirstColumn.HeaderColSpan = lastColSpan;
                lastGroupName = groupName;
            }
            else
            {
                lastColSpan = 0;
                lastGroupName = null;
                column.HeaderColSpan = 0;
            }
        }
        style.AppendLine($".gridview-{ElementId} .gridview-row {{");
        style.AppendLine($"grid-template-columns: {string.Join(' ', widths)};");
        style.AppendLine("grid-auto-flow: column;");
        style.AppendLine("}");
        style.AppendLine();
        // Header group style
        int order = 1 + maxGroupLevel;
        foreach (GridColumnState state in columsListOrder)
        {
            style.Append(GetHeaderGroupStyle(state, order));
            order++;
        }
        return style.ToString();
    }

    /// <summary>
    /// Return style definition for the column
    /// </summary>        
    /// <param name="column">Column</param>
    /// <param name="realOrder">Display order</param>
    /// <param name="maxOrderFreezedColumns">Max order of freezed columns</param>
    /// <returns></returns>
    private string GetColumnStyle(GridColumnState column, int realOrder, int maxOrderFreezedColumns)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine($".gridview-{ElementId} .gridview-col{column.Index} {{");
        stringBuilder.AppendLine($"grid-column: {realOrder};");
        if (column.Visible)
        {
            // Fix min and max width                        
            if (column.MinWidth > 0)
            {
                stringBuilder.AppendLine($"min-width: {column.MinWidth}px !important;");
            }
            if (column.MaxWidth > 0)
            {
                stringBuilder.AppendLine($"max-width: {column.MaxWidth}px !important;");
            }
        }
        // Add group border
        if (!string.IsNullOrEmpty(column.GroupName))
        {
            string groupName = column.GroupName;
            // Check if the previous column is in same group
            GridColumnState prevColumn = ColumnList.Where(c => c.Order == column.Order - 1).FirstOrDefault();
            // Fix double border problem if two groups are following
            if (prevColumn is not null && string.IsNullOrEmpty(prevColumn?.GroupName))
            {
                stringBuilder.AppendLine("border-left: var(--group-columns-border-left) !important;");
            }
            // Check if the next column is in same group
            GridColumnState nextColumn = ColumnList.Where(c => c.Order == column.Order + 1).FirstOrDefault();
            if (nextColumn is not null && nextColumn?.GroupName != groupName)
            {
                stringBuilder.AppendLine("border-right: var(--group-columns-border-right) !important;");
            }
        }

        if (column.Order <= maxOrderFreezedColumns)
        {
            stringBuilder.AppendLine("position: sticky !important;");
            stringBuilder.AppendLine("left: var(" + GetCssVariableName(column) + ");");
            stringBuilder.AppendLine("background: inherit;");
            stringBuilder.AppendLine("z-index: var(--frozen-z-index) !important;");
        }
        if (column.Order == maxOrderFreezedColumns)
        {
            stringBuilder.AppendLine("border-right: var(--frozen-columns-border-right) !important;");
        }
        stringBuilder.AppendLine("}");
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Return Css variables for the column
    /// </summary>        
    /// <returns></returns>
    private async Task<string> GetCssVariablesAsync()
    {
        StringBuilder stringBuilder = new();
        List<GridColumnState> visibleColumns = ColumnList.Where(x => x.Visible && x.Frozen).OrderBy(x => x.Order).ToList();
        if (visibleColumns.Any())
        {
            int leftPosition = 0;
            for (int index = 0; index < visibleColumns.Count; index++)
            {
                if (index > 0)
                {
                    GridColumnState previousColumn = visibleColumns[index - 1];
                    int previousColumnOffsetWidth = await JS.InvokeAsync<int>("Lagoon.JsUtils.GetElementOffsetWidth", $"#gridview-{StateId}",
                        $"div[data-col=\"{previousColumn.Index}\"]");
                    leftPosition += previousColumnOffsetWidth;
                }
                stringBuilder.AppendLine(string.Format("{0}: {1}px;", GetCssVariableName(visibleColumns[index]), leftPosition));
            }
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Return Css variable name
    /// </summary>        
    /// <param name="column">Column</param>
    /// <returns></returns>
    private string GetCssVariableName(GridColumnState column)
    {
        return $"--gridview-{ElementId}-gridview-col{column.Index}";
    }

    /// <summary>
    /// Return style for sub header title
    /// </summary>
    /// <param name="state">Column state</param>
    /// <param name="realOrder">Display order</param>
    /// <returns></returns>
    private string GetHeaderGroupStyle(GridColumnState state, int realOrder)
    {
        StringBuilder stringBuilder = new();
        int index = state.Index;
        if (state.HeaderColSpan > 0)
        {
            // apply span style
            stringBuilder.AppendLine($".gridview-{ElementId} .gridview-header-group .gridview-col{index}{{");
            stringBuilder.AppendLine($"grid-column: {realOrder} / span {state.HeaderColSpan}");
            stringBuilder.AppendLine("}");
        }
        else if (state.HeaderColSpan < 0)
        {
            // Hide sub header cell
            stringBuilder.AppendLine($".gridview-{ElementId} .gridview-header-group .gridview-col{index}{{");
            stringBuilder.AppendLine("display: none;");
            stringBuilder.AppendLine("}");
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Update column width
    /// </summary>
    /// <param name="key">column identifier</param>
    /// <param name="width">new width</param>
    [JSInvokable]
    public async Task UpdateColumnWidthAsync(string key, double width)
    {
        if (TryGetColumn(key, out GridColumnState resizedColumnState))
        {
            resizedColumnState.CalculatedWidth = new((int)width);
            // We save all width in pixel to get back with the same display the next time we open the page
            foreach (GridColumnState columnState in ColumnList.Where(c => c.Visible))
            {
                columnState.InitialWidth = columnState.CalculatedWidth;
            }
            await BuildGridViewStyleAsync();
        }
    }

    /// <summary>
    /// Refresh css variables
    /// </summary>
    [JSInvokable]
    public async Task RefreshCssVariablesAsync()
    {
        CssVariables = await GetCssVariablesAsync();
        GridViewCss.Refresh();
    }

    #endregion

    #region Displaying data

    /// <summary>
    /// Refresh the GridView data.
    /// </summary>
    [Obsolete("Use the RefreshAsync method.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task Refresh()
    {
        return RefreshAsync();
    }

    /// <summary>
    /// Refresh the GridView data.
    /// </summary>
    public Task RefreshAsync()
    {
        return LoadDataAsync(true);
    }

    /// <summary>
    /// Rise data loading
    /// </summary>        
    /// <param name="calculation">Indicate that calculated values must be reloaded too.</param>
    /// <param name="needStateAsChanged">Indicate if the datagrid must be refreshed.</param>
    private async Task LoadDataAsync(bool calculation, bool needStateAsChanged = false)
    {
#if DEBUG //TOCLEAN
        System.Diagnostics.Stopwatch chrono = new();
        chrono.Start();
#endif
        _loadingData = true;
        try
        {
            // Displaying loading spinner
            if (_data is not null)
            {
                _data = null;
                RebuildRows = true;
                StateHasChanged();
            }
            // Load data
            using (WaitingContext wc = GetNewWaitingContext())
            {
                RebuildRows = await GetDataAsync(wc.CancellationToken);
            }
            // Need to refresh calculation and filter summary   
            if (needStateAsChanged)
            {
                StateHasChanged();
#if DEBUG //TOCLEAN		
                Trace.ToConsole(this, $"XXX STATECHANGED");
#endif
            }
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
        _loadData = false;
        _loadingData = false;
#if DEBUG //TOCLEAN
        chrono.Stop();
        Trace.ToConsole(this, $"ZZZ {chrono.ElapsedMilliseconds} ms");
#endif
        ShowScreenReaderInformation("#GridViewLoadingDataEnd");
    }

    /// <summary>
    /// Return the list of data.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of data.</returns>
    private async Task<bool> GetDataAsync(CancellationToken cancellationToken)
    {
        DataPage<TItem> dataPage = await GetDataPageAsync(cancellationToken);
        if (dataPage is null)
        {
            return false;
        }
        PaginationState = new(dataPage);
        if (CurrentPage != PaginationState.CurrentPage)
        {
            CurrentPage = PaginationState.CurrentPage;
            if (CurrentPageChanged.HasDelegate)
            {
                await CurrentPageChanged.TryInvokeAsync(App, CurrentPage);
            }
        }
        _data = dataPage.Data;
        // Update the bounded counter variable
        if (GridViewDataCounterChanged.HasDelegate)
        {
            await GridViewDataCounterChanged.TryInvokeAsync(App, PaginationState);
        }
        // Get columns calculation results
        if (dataPage.CalculationValues is not null && dataPage.CalculationValues.Any())
        {
            CalculationValues = dataPage.CalculationValues;
        }
        return true;
    }

    /// <summary>
    /// Return the items to load the current page of the grid view.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The items to load the current page of the grid view.</returns>
    private Task<DataPage<TItem>> GetDataPageAsync(CancellationToken cancellationToken = default)
    {
        CustomDataPageLoader<TItem> dataLoader = GetDataLoader(false, false, false);
        return dataLoader is null ? Task.FromResult<DataPage<TItem>>(null) : dataLoader.GetDataPageAsync(GetFieldsUsedInData(), cancellationToken);
    }

    /// <summary>
    /// Return all model properties name used in GridView
    /// </summary>
    /// <returns></returns>
    private static List<string> GetFieldsUsedInData()
    {
#if SHRINK_DATA
        if (!ShrinkData)
        {
            return null;
        }
        List<string> visibleColumns = CurrentProfile.Columns.Where(c => c.Hidden == false).Select(c => c.UniqueKey).ToList();
        if(RequiredFields is not null)
        {
            visibleColumns.AddRange(RequiredFields.Split(";", StringSplitOptions.TrimEntries));
        }            
        return visibleColumns;
#else
        return null;
#endif
    }

    ///<inheritdoc/>
    internal override async Task<IEnumerable<T>> GetColumnValuesAsync<T>(GridColumnState columnState)
    {
        if (columnState.GetFilterExpression() is Expression<Func<TItem, T>> expression)
        {
            CustomDataPageLoader<TItem> dataLoader = GetDataLoader(true, true, true);
            if (dataLoader is not null)
            {
                // Get the filter except the one on the working column
                dataLoader.ModelFilter = GetModelFilter(columnState);
                return await dataLoader.GetSelectorValuesAsync(expression);
            }
        }
#if DEBUG
        else
        {
            Trace.ToConsole(this, $"!!!!!!!!!!!! Invalid expression : {columnState.GetFilterExpression()}");
        }
#endif
        // No values found.
        return Enumerable.Empty<T>();
    }

    /// <summary>
    /// Get a new data loader.
    /// </summary>
    internal CustomDataPageLoader<TItem> GetDataLoader(bool ignoreFilter = false, bool ignoreSort = false, bool allItems = false)
    {
        CustomDataPageLoader<TItem> dataLoader;
        if (HasController)
        {
            dataLoader = new RemoteDataPageLoader<TItem>()
            {
                ControllerUri = ControllerUri,
                ControllerQueryArg = ControllerQueryArg,
                HttpClient = Http
            };
        }
        else if (Items is null)
        {
            return null;
        }
        else
        {
            dataLoader = new LocalDataPageLoader<TItem>()
            {
                Items = Items.AsQueryable()
            };
        }
        dataLoader.AllowCount = Features.HasFlag(GridFeature.Count);
        dataLoader.CurrentPage = allItems ? 1 : CurrentPage;
        dataLoader.PageSize = allItems ? 0 : PaginationState.PageSize;
        dataLoader.SortOptions = ignoreSort ? null : GetSortOptions();
        dataLoader.ModelFilter = ignoreFilter ? null : GetModelFilter();
        dataLoader.CalculationOptions = GetCalculationOptions();
        return dataLoader;
    }

    /// <summary>
    /// Return columns visible count
    /// </summary>
    /// <returns></returns>
    private int GetVisibleColumnsNumber()
    {
        return ColumnList.Where(c => c.Visible).Count();
    }

    /// <summary>
    /// Get all the active filters.
    /// </summary>
    /// <param name="ignoredColumn">Le filtre de la colonne spécifié est ignorée.</param>
    /// <returns>The active filters.</returns>
    private ModelFilter<TItem> GetModelFilter(GridColumnState ignoredColumn = null)
    {
        ModelFilter<TItem> modelFilter = new(default);
        foreach (GridColumnState columnState in ColumnList)
        {
            if (columnState.Filter != null && columnState != ignoredColumn && columnState.AllowFilter)
            {
                modelFilter.AddFilterUnsafe(columnState.GetFilterExpression(), columnState.Filter);
            }
        }
        return modelFilter;
    }

    /// <summary>
    /// Return gridview sort parameters
    /// </summary>
    /// <returns></returns>
    private List<SortOption> GetSortOptions()
    {
        List<SortOption> sortOptions = new();
        List<GridColumnState> columnsSorted = ColumnList.Where(c => c.SortDirection != DataSortDirection.None)
            .OrderBy(c => c.SortingOrder)
            .Take(COLUMN_SORTABLE_COUNT)
            .ToList();
        // We first add the group by columns
        foreach (KeyValuePair<int, List<string>> level in GroupByColumnKeyList)
        {
            foreach (string key in level.Value)
            {
                if (TryGetColumn(key, out GridColumnState state))
                {
                    LambdaExpression sortExpression = state.GetSortExpression();
                    if (sortExpression is not null)
                    {
                        SortOption sortFinded = sortOptions.Find(c => c.ParameterizedValueExpression.ToString().Equals(sortExpression.ToString()));
                        if (sortFinded is null)
                        {
                            sortOptions.Add(new(sortExpression, state.SortDirection));
                        }
                    }
                }
            }
        }
        if (columnsSorted.Any())
        {
            columnsSorted.ForEach(column =>
            {
                LambdaExpression sortExpression = column.GetSortExpression();
                if (sortExpression is not null)
                {
                    SortOption sortFinded = sortOptions.Find(c => c.ParameterizedValueExpression.ToString().Equals(sortExpression.ToString()));
                    if (sortFinded is not null)
                    {
                        sortFinded.Direction = column.SortDirection;
                    }
                    else
                    {
                        sortOptions.Add(new SortOption(sortExpression, column.SortDirection));
                    }
                }
            });
        }

        return sortOptions;
    }

    /// <summary>
    /// Render fragment of columns
    /// </summary>
    /// <param name="item"></param>        
    /// <returns></returns>
    internal RenderFragment RenderColumns(TItem item)
    {
        RenderFragment childContent = Columns != null ? Columns(item) : RenderAutoColumnList(item);
        return childContent;
    }

    /// <summary>
    /// Render fragment of columns
    /// </summary>
    /// <param name="item"></param>
    /// <param name="renderContext"></param>
    /// <returns></returns>
    internal RenderFragment RenderColumns(TItem item, GridRenderContext renderContext)
    {
        return builder =>
        {
            builder.AddCascadingValueComponent(0, renderContext, RenderColumns(item), true, ColumnsLayoutState);
        };
    }

    /// <summary>
    /// Render fragment of auto columns
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private static RenderFragment RenderAutoColumnList(TItem item)
    {
        return builder =>
        {
            builder.OpenComponent<LgGridAutoColumnList<TItem>>(0);
            builder.AddAttribute(1, nameof(LgGridAutoColumnList<TItem>.Item), item);
            builder.CloseComponent();
        };
    }

    #endregion

    #region Events

    /// <summary>
    /// Change data sort
    /// </summary>
    /// <param name="args"></param>
    internal override async Task OnColumnSortChangeAsync(GridViewSortEventArgs args)
    {
        List<int> columnToKeepOrder = ColumnList.Where(c => c.SortDirection != DataSortDirection.None)
            .OrderBy(c => c.SortingOrder)
            .GroupBy(c => c.GetSortExpression().ToString())
            .Select(c => c.First())
            .Take(COLUMN_SORTABLE_COUNT)
            .Select(c => c.Index)
            .ToList();
        foreach (GridColumnState state in ColumnList)
        {
            if (state.UniqueKey == args.Key)
            {
                state.SortingOrder = 0;
            }
            if (state.SortDirection == DataSortDirection.None)
            {
                state.SortingOrder = null;
            }
            else
            {
                if (columnToKeepOrder.Contains(state.Index))
                {
                    state.SortingOrder += 1;
                }
                else
                {
                    state.SortingOrder = null;
                    state.SortDirection = DataSortDirection.None;
                }
            }
        }
        await UpdateCurrentProfileAsync();
        await LoadDataAsync(false, true);
    }

    /// <summary>
    /// Cell click event
    /// </summary>
    /// <param name="row">Row clicked</param>
    /// <param name="columnKey">Identifier of the column.</param>
    internal async Task RowClickAsync(LgGridRow<TItem> row, string columnKey)
    {
        TItem item = row.Item;
        if (SelectableRows)
        {
            // Unselect row only in the multiple selection mode
            if (HasSelectionColumn && IsSelected(item))
            {
                await OnRemoveSelectionAsync(item);
            }
            else
            {
                await OnAddSelectionAsync(item);
                _previousSelectedRow = row;
            }
        }
        if (OnCellClick.HasDelegate)
        {
            await OnCellClick.TryInvokeAsync(App, new GridViewCellClickEventArgs<TItem>(item, columnKey));
        }
    }

    /// <summary>
    /// Command click event
    /// </summary>
    /// <param name="command">Command name</param>
    /// <param name="row">Row data</param>
    internal override Task CommandClickAsync(string command, object row)
    {
        return command switch
        {
            LgGridEditColumn.ADDCOMMAND => OnAddValueAsync(),
            LgGridEditColumn.ADDCANCELCOMMAND => CancelAddAsync(),
            LgGridEditColumn.EDITCANCELCOMMAND => CancelEditAsync(),
            LgGridEditColumn.EDITCOMMAND => OnEditAsync((LgGridRow<TItem>)row),
            LgGridEditColumn.SAVECOMMAND => OnUpdateValueAsync(),
            LgGridSelectionColumn.DELETE_SELECTION_COMMAND => OnDeleteSelectionAsync(),
            _ => OnCommandClick.HasDelegate ? OnCustomCommandAsync(command, (LgGridRow<TItem>)row) : Task.CompletedTask,
        };
    }

    /// <summary>
    /// Exécute a custom command.
    /// </summary>
    /// <param name="command">The custom command.</param>
    /// <param name="row">The row.</param>
    private async Task OnCustomCommandAsync(string command, LgGridRow<TItem> row)
    {
        GridViewCommandClickEventArgs<TItem> eventArgs = new(command, row.Item);
        await OnCommandClick.TryInvokeAsync(App, eventArgs);
        // Force to reload data
        if (eventArgs.Refresh)
        {
            await LoadDataAsync(true, true);
        }
        else
        {
            // Force to refresh active row
            row.Refresh();
        }
    }

    /// <summary>
    /// Set the row in edit mode.
    /// </summary>
    /// <param name="row">The row.</param>
    private static Task OnEditAsync(LgGridRow<TItem> row)
    {
        row.SetRowInEdit();
        return Task.CompletedTask;
    }

    ///<inheritdoc/>
    internal override bool CellCanFocus()
    {
        return !HasEditColumn && IsEditable;
    }

    #endregion

    #region Pagination

    /// <summary>
    /// Move page to value
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    private async Task MoveToPageAsync(int page)
    {
        if (PaginationState.CurrentPage != page)
        {
            PaginationState.CurrentPage = page;
            CurrentPage = page;
            if (CurrentPageChanged.HasDelegate)
            {
                await CurrentPageChanged.TryInvokeAsync(App, page);
            }
        }
    }

    /// <summary>
    /// Change page event
    /// </summary>
    /// <param name="args"></param>
    private async Task OnPagerChangeAsync(ChangeEventArgs args)
    {
        await MoveToPageAsync((int)args.Value);
        await LoadDataAsync(false);
    }

    /// <summary>
    /// Page size change event
    /// </summary>
    /// <param name="args"></param>
    private async Task OnPageSizeChangeAsync(ChangeEventArgs args)
    {
        PaginationState.PageSize = (int)args.Value;
        await UpdateCurrentProfileAsync();
        await MoveToPageAsync(1);
        await LoadDataAsync(false);
    }

    /// <summary>
    /// Move column before droppedcolumn event
    /// </summary>
    /// <param name="args"></param>
    internal override async Task OnColumnMove(GridViewMoveColumnEventArgs args)
    {
        await InsertColumnBeforeAsync(args.DraggedColumn, args.DroppedColumn);
        State.DraggedColumn = null;
        await UpdateCurrentProfileAsync();
        StateHasChanged();
    }

    /// <inheritdoc/>
    internal override async Task<bool> OnValueChangedAsync(object item, string columnKey, object value, object previousValue)
    {
        // Raise change event
        GridViewValueChangeEventArgs<TItem> arguments =
            new((TItem)item, columnKey, value, previousValue);
        if (OnValueChange.HasDelegate)
        {
            await OnValueChange.TryInvokeAsync(App, arguments);
        }
        return arguments is null || !arguments.Cancel;
    }

    /// <summary>
    /// On change value event
    /// </summary>
    /// <param name="item"></param>
    /// <param name="columnKey"></param>
    /// <param name="value"></param>
    /// <param name="previousValue"></param>
    /// <returns></returns>
    internal override async Task OnPatchValueAsync(object item, string columnKey, object value, object previousValue)
    {
        // Remote mode
        if (HasController)
        {
            try
            {
                await Http.TryPatchAsync(ControllerUri, new UpdateDataParametersSerializer<TItem>(
                (TItem)item, columnKey, value, previousValue));
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }
        ShowScreenReaderInformation("#GridViewCellSavedSr");
        if (DisplaySuccessSaveMessage.Value)
        {
            ShowSuccess("#GridViewCellSaved");
        }
        using (WaitingContext wc = GetNewWaitingContext())
        {
            await GetCalculationAsync(wc.CancellationToken);
            StateHasChanged();
        }
    }

    /// <inheritdoc/>
    internal override async Task<bool> OnUpdateValueAsync()
    {
        // Form validation            
        if (_editRow is null)
        {
            ShowException(new NullReferenceException("#EditFormNullException".Translate()));
        }
        bool validated = _editRow.ValidateForm();
        if (validated)
        {
            LgValidator validator = _editRow.GetValidator();
            TItem item = _editRow.Item;
            // Rise add event
            GridViewItemActionEventArgs<TItem> arguments = null;
            if (OnRowUpdate.HasDelegate)
            {
                arguments = new GridViewItemActionEventArgs<TItem>(item);
                await OnRowUpdate.TryInvokeAsync(App, arguments);
            }
            if (arguments is null || !arguments.Cancel)
            {
                // Remote mode
                if (HasController)
                {
                    await Http.TryPutAsync(ControllerUri, item);
                    if (validator.HasError)
                    {
                        return false;
                    }
                }
                using (WaitingContext wc = GetNewWaitingContext())
                {
                    await GetCalculationAsync(wc.CancellationToken);
                }
                ShowScreenReaderInformation("#GridViewRowSavedSr");
                ShowSuccess("#GridViewRowSaved");
            }
            await ResetEditAsync(false);
            return true;
        }
        return false;
    }

    /// <inheritdoc/>
    internal override async Task<bool> OnAddValueAsync()
    {
        // Display add line
        if (_addItem is null || _addItem.Equals(default))
        {
            _addItem = NewItem is null ? Activator.CreateInstance<TItem>() : NewItem();
            // Select first cell of the add line
            SetFocus("div.gridview-row-add div.gridview-cell-edit");
            RebuildRows = true;
            return false;
        }
        // Form validation            
        if (_addRow is null)
        {
            ShowException(new NullReferenceException("#EditFormNullException".Translate()));
        }
        bool validated = _addRow.ValidateForm();
        if (validated)
        {
            LgValidator validator = _addRow.GetValidator();
            GridViewItemActionEventArgs<TItem> arguments = null;
            // Rise add event
            if (OnValueAdd.HasDelegate)
            {
                arguments = new GridViewItemActionEventArgs<TItem>(_addItem);
                await OnValueAdd.TryInvokeAsync(App, arguments);
            }
            if (arguments is null || !arguments.Cancel)
            {
                // Remote or local mode
                if (HasController)
                {
                    await Http.TryPostAsync(ControllerUri, _addItem);
                    if (validator.HasError)
                    {
                        return false;
                    }
                }
                else
                {
                    Items.Add(_addItem);
                }
                await LoadDataAsync(true);
                ShowScreenReaderInformation("#GridViewRowAddedSr");
                ShowSuccess("#GridViewRowAdded");
            }
            _addItem = default;
            await CancelAddAsync();
            return true;
        }
        return false;
    }

    ///<inheritdoc/>
    internal override async Task OnDeleteSelectionAsync()
    {
        if (Selection.Any())
        {
            // Raise the OnDelete event
            GridViewSelectionActionEventArgs<TItem> arguments = null;
            if (OnDelete.HasDelegate)
            {
                arguments = new GridViewSelectionActionEventArgs<TItem>(Selection);
                await OnDelete.TryInvokeAsync(App, arguments);
            }
            if (arguments is null || !arguments.Cancel)
            {
                // Remote mode
                if (!string.IsNullOrEmpty(ControllerUri))
                {
                    await Http.TryDeleteAsync(ControllerUri, Selection);
                }
                else
                {
                    foreach (TItem item in Selection)
                    {
                        Items.Remove(item);
                    }
                }
                await LoadDataAsync(true);
                Selection.Clear();
                ShowScreenReaderInformation("#GridViewSelectionDeletedSr");
                ShowSuccess("#GridViewSelectionDeleted");
            }
        }
    }

    /// <summary>
    /// Define edit form for update by line
    /// </summary>
    /// <param name="editRow"></param>
    internal void SetEditRow(LgGridRow<TItem> editRow)
    {
        _editRow = editRow;
        DisplayEdit = !HasEditColumn;
        StateHasChanged();
    }

    /// <summary>
    /// Define edit form for add line
    /// </summary>
    /// <param name="addRow"></param>
    internal void SetAddRow(LgGridRow<TItem> addRow)
    {
        _addRow = addRow;
        _addRow.RowEditing = true;
        StateHasChanged();
    }

    /// <summary>
    /// Indicate if row is already in edit mode
    /// </summary>
    /// <returns></returns>
    internal bool HasEditRowActive()
    {
        return _editRow is not null;
    }

    /// <summary>
    /// Cancel add row
    /// </summary>
    private async Task CancelAddAsync()
    {
        _addItem = default;
        _addRow = null;
        // Manage sticky for add line
        // TOREMOVE: When sticky with element fixed problem will be fixed in Firefox
        await GridJsInvokeAsync<bool>("Lagoon.GridView.StickyDispose", new object[] { StateId });
        RebuildRows = true;
        StateHasChanged();
    }

    /// <summary>
    /// Cancel update row
    /// </summary>
    private Task CancelEditAsync()
    {
        return ResetEditAsync(true);
    }

    /// <summary>
    /// Reinitialize update row
    /// </summary>
    private async Task ResetEditAsync(bool fullReset = false)
    {
        if (_editRow is not null)
        {
            await _editRow.CancelEditAsync(fullReset);
            _editRow = null;
        }
        DisplayEdit = false;
        StateHasChanged();
    }

    #endregion

    #region Toolbar

    /// <summary>
    /// Reset grid layout, filters and sorts
    /// </summary>
    private Task ResetPersistenceAsync()
    {
        // Reinitialize group collapse state and page size
        State.GroupCollapsed = DefaultGroupCollapsed;
        PaginationState.PageSize = DefaultPageSize > 0 ? DefaultPageSize : PaginationSizeSelector[0];
        return ResetColumnsDefinitionAsync();
    }

    /// <summary>
    /// Remove all filters
    /// </summary>
    private async Task ClearFiltersAsync()
    {
        bool reset = false;
        foreach (GridColumnState state in ColumnList)
        {
            reset |= await state.ResetFilterAsync(false);
        }
        if (reset)
        {
            await FilterStateChangeAsync();
            await MoveToPageAsync(1);
            await LoadDataAsync(true);
            await UpdateCurrentProfileAsync();
        }
    }

    /// <summary>
    /// Show or hide filters row
    /// </summary>
    private async Task ToggleFiltersAsync()
    {
        ShowFilters = !ShowFilters;
        if (!ShowFilters)
        {
            await ClearFiltersAsync();
        }
        if (ShowFiltersChanged.HasDelegate)
        {
            await ShowFiltersChanged.TryInvokeAsync(App, ShowFilters);
        }
    }

    /// <summary>
    /// Focus an gridview element
    /// </summary>
    /// <param name="selector"></param>
    internal override void SetFocus(string selector)
    {
        _focusElementSelector = selector;
    }

    /// <summary>
    /// Show export option popup
    /// </summary>
    public Task ShowExportOptionsAsync()
    {
        return _lgExportOptions.ShowAsync();
    }

    #endregion

    #region Dynamic columns

    /// <summary>
    /// Return the list of fields with their property names and their property types.
    /// </summary>
    /// <returns></returns>
    internal virtual List<LgGridValueDefinition<TItem>> GetFieldDefinitionList()
    {
        GridColumnType columnType;
        List<LgGridValueDefinition<TItem>> list = new();
        // Browse public properties of the instance
        foreach (PropertyInfo prop in ItemType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            columnType = GridViewHelper.GetColumnType(prop.PropertyType);
            // Detect if a column type is defined for the property type
            if (columnType != GridColumnType.None)
            {
                // Register the column definition
                list.Add((LgGridValueDefinition<TItem>)Activator.CreateInstance(typeof(LgGridStaticValueDefinition<,>)
                    .MakeGenericType(ItemType, prop.PropertyType), prop, columnType));
            }
        }
        return list;
    }

    internal virtual TItem CreateItemModel()
    {
        return (TItem)RuntimeHelpers.GetUninitializedObject(typeof(TItem));
    }

    /// <summary>
    /// Display confirmation is needed
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    private async Task ActionConfirmationAsync(Func<Task> callback)
    {
        if (_editRow is not null)
        {
            ShowConfirm("GridViewActionConfirmation".Translate(), callback);
        }
        else
        {
            await callback.Invoke();
        }
    }

    #endregion

    #region Selection        

    /// <inheritdoc/>
    internal async Task OnAddSelectionAsync(TItem item)
    {
        if (!Selection.Contains(item, _selectionComparer))
        {
            // Remove selection in single selection mode without custom selection toolbar
            if (!HasSelectionColumn && ToolbarSelection is null)
            {
                Selection.Clear();
                // Render previous row to remove the selected CSS class
                _previousSelectedRow?.Refresh();
            }
            if (HasSelectionColumn || !Selection.Any())
            {
                Selection.Add(item);
                await RowSelectionChangedAsync(item);
            }
        }
    }

    ///<inheritdoc/>
    internal Task OnRemoveSelectionAsync(TItem item)
    {
        SelectionRemove(item);
        return RowSelectionChangedAsync(item);
    }

    /// <summary>
    /// Remove item from selection
    /// </summary>
    /// <param name="item"></param>
    private void SelectionRemove(TItem item)
    {
        for (int i = 0; i < Selection.Count; i++)
        {
            if (_selectionComparer.Equals(item, Selection.ElementAt(i)))
            {
                Selection.Remove(Selection.ElementAt(i));
                break;
            }
        }
    }

    /// <summary>
    /// Add or remove displayed rows from selection
    /// </summary>        
    /// <param name="state"></param>
    internal override async Task OnChangeAllSelectionAsync(bool state)
    {
        foreach (TItem item in _data)
        {
            if (state)
            {
                if (!Selection.Contains(item, _selectionComparer))
                {
                    Selection.Add(item);
                }
            }
            else
            {
                SelectionRemove(item);
            }
        }
        await RowSelectionChangedAsync(default);
        RebuildRows = true;
    }

    /// <summary>
    /// Remove all rows from the selection
    /// </summary>
    /// <returns></returns>
    internal override async Task OnRemoveAllSelectionAsync()
    {
        Selection.Clear();
        await RowSelectionChangedAsync(default);
        RebuildRows = true;
    }

    /// <summary>
    /// Call selection events
    /// </summary>
    /// <returns></returns>
    private async Task RowSelectionChangedAsync(TItem item)
    {
        OnSelectionUpdated();
        if (OnRowSelection.HasDelegate)
        {
            await OnRowSelection.TryInvokeAsync(App, new GridViewSelectionEventArgs<TItem>(Selection, item));
        }
        else
        {
            StateHasChanged();
        }
    }

    /// <summary>
    /// Indicate than selection has changed
    /// </summary>
    private void OnSelectionUpdated()
    {
        _selectionState = Guid.NewGuid();
    }

    /// <summary>
    /// Indicate if the data item is selected
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    internal bool IsSelected(TItem item)
    {
        return Selection.Contains(item, _selectionComparer);
    }

    /// <summary>
    /// Indicates the selection contains elements 
    /// </summary>
    /// <returns>Returns true if at least one row is selected</returns>
    private bool HasSelectedItems()
    {
        return (HasSelectionColumn || ToolbarSelection is not null) && Selection.Any();
    }

    #endregion

    #region Calculation

    /// <summary>
    /// Return list of the columns calculations
    /// </summary>
    /// <returns></returns>
    private IEnumerable<CalculationOption> GetCalculationOptions()
    {
        foreach (GridColumnState state in ColumnList)
        {
            // Define calculation for the column
            if (state.CalculationType != DataCalculationType.None)
            {
                yield return new CalculationOption()
                {
                    Field = state.UniqueKey,
                    ParametrizedValueExpression = state.ParameterizedValueExpression,
                    CalculationType = state.CalculationType
                };
            }
        }
    }

    /// <summary>
    /// Return columns calculations result
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal async Task<bool> GetCalculationAsync(CancellationToken cancellationToken)
    {
        CustomDataPageLoader<TItem> dataLoader = GetDataLoader();
        if (dataLoader is null)
        {
            return false;
        }
        else
        {
            dataLoader.CalculationOptions = GetCalculationOptions();
            if (dataLoader.CalculationOptions.Any())
            {
                CalculationValues = await dataLoader.GetCalculationAsync(cancellationToken);
            }
            return true;
        }
    }

    /// <summary>
    ///  Format the value with the default culture.
    /// </summary>
    /// <param name="value">the current value</param>
    /// <param name="column">the current column</param>
    /// <returns>Return the value formatted with the default culture</returns>
    private static string GetValueFormatted(IFormattable value, GridColumnState column)
    {
        if (GridViewHelper.GetColumnType(column.CellValueType) is GridColumnType.Numeric && string.IsNullOrEmpty(column.CalculationDisplayFormat))
        {
            column.CalculationDisplayFormat = $"N{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits:D}";
        }

        if (GridViewHelper.GetColumnType(column.CellValueType) is GridColumnType.DateTime && string.IsNullOrEmpty(column.CalculationDisplayFormat))
        {
            column.CalculationDisplayFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }

        return value.ToString(column.CalculationDisplayFormat, null);
    }

    #endregion

    #region ExportData

    /// <summary>
    /// Export datagrid data to a file.
    /// </summary>
    /// <param name="e">The export option event args.</param>
    private async Task ExportDataFromProviderAsync(GridViewExportEventArgs e)
    {
        IExportProvider exportProvider = e.Options.ExportProvider;
        using (WaitingContext context = GetNewWaitingContext())
        {
            string fileName = $"{ExportFileName}{exportProvider.FileExtension}";
            byte[] data = exportProvider.SerializeToByteArray(await GetExportableDataAsync(e.Options, context.CancellationToken),
                GetExportableColumns(e.Options.ExportColumnsMode == ExportColumnsMode.DisplayedColumns));
            App.SaveAsFile(fileName, data, exportProvider.ContentType);
        }
    }

    /// <summary>
    /// Gets the ordered list of exportable columns.
    /// </summary>
    /// <returns>The ordered list of exportable columns.</returns>
    protected ExportColumnCollection<TItem> GetExportableColumns(bool displayedColumns)
    {
        IEnumerable<GridColumnState> source;
        if (displayedColumns)
        {
            source = ColumnList.Where(x => x.Visible && x.Exportable).OrderBy(c => c.Order);
        }
        else
        {

            source = ExportableColumns?.Count > 0 ? ExportableColumns.Select(k => GetColumnOrDefault(k)).Where(c => c is not null) : ColumnList;
            source = source.Where(c => c.Exportable).OrderBy(c => c.Order);
        }

        return new(source.Select(c => c.GetExportColumn<TItem>(GetExportColumnTitle(c), GetExportGroupTitle(c))).Where(c => c is not null));
    }

    /// <summary>
    /// Search gridview export column title from the property name.
    /// </summary>
    /// <param name="columnState">Column state.</param>
    /// <returns>The export column title.</returns>
    internal string GetExportColumnTitle(GridColumnState columnState)
    {
        // Get the user defined ExportTitle property
        if (columnState.ExportTitle is not null)
        {
            return columnState.ExportTitle.CheckTranslate();
        }
        // Return the display columntitle with the group name
        string groupName = columnState.GetGroupName();
        if (ExportGroupName && !string.IsNullOrEmpty(groupName))
        {
            return string.Format(ExportGroupNameTitleFormat, columnState.GetTitle(), groupName);
        }
        // Return the display column title
        return columnState.GetTitle();
    }

    /// <summary>
    /// Search gridview export group title from the property name.
    /// </summary>
    /// <param name="columnState">Column state.</param>
    /// <returns>The export group title.</returns>
    internal static string GetExportGroupTitle(GridColumnState columnState)
    {
        // Get the user defined ExportTitle property
        return columnState.GroupName is not null ? columnState.GroupName.CheckTranslate() : string.Empty;
    }

    /// <summary>
    /// Gets the expotable data list.
    /// </summary>
    /// <param name="exportOptions">The export options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The expotable data list.</returns>
    private async Task<IEnumerable<TItem>> GetExportableDataAsync(ExportOptions exportOptions, CancellationToken cancellationToken = default)
    {
        CustomDataPageLoader<TItem> dataLoader = GetDataLoader(exportOptions is not null && !(exportOptions.ExportRowMode == ExportRowMode.FilteredRows), false, true);
        if (dataLoader is null)
        {
            return Enumerable.Empty<TItem>();
        }
        else
        {
            DataPage<TItem> dataPage = await dataLoader.GetDataPageAsync(GetFieldsUsedInData(), cancellationToken);
            return dataPage.Data;
        }
    }

    #endregion

    #region Filters Summary

    /// <summary>
    /// Change data filter
    /// </summary>
    /// <param name="args"></param>
    internal override async Task OnColumnFilterChangeAsync(GridViewFilterChangeEventArgs args)
    {
        await FilterStateChangeAsync();
        await MoveToPageAsync(1);
        await UpdateCurrentProfileAsync();
        await LoadDataAsync(true, true);
    }

    #endregion

    #region Filters

    /// <summary>
    /// Return the filtered data
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public async Task<IEnumerable<TItem>> GetFilteredDataAsync(CancellationToken cancellationToken = default)
    {
        CustomDataPageLoader<TItem> dataLoader = GetDataLoader(false, false, true);
        if (dataLoader is null)
        {
            return Enumerable.Empty<TItem>();
        }
        else
        {
            DataPage<TItem> dataPage = await dataLoader.GetDataPageAsync(GetFieldsUsedInData(), cancellationToken);
            return dataPage.Data;
        }
    }

    #endregion

    #endregion

}
