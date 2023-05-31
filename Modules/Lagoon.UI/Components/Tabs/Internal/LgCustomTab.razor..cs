namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Custom component to declare a tab.
/// </summary>
public abstract class LgCustomTab : LgAriaComponentBase
{

    #region fields

    private LgTabRenderData _tab;
    private bool? _hasModification;

    #endregion

    #region properties

    /// <summary>
    /// Get the state data of the tab.
    /// </summary>
    internal LgTabRenderData Tab => _tab;

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets or sets tab container parent
    /// </summary>
    [CascadingParameter]
    protected LgCustomTabContainer TabContainer { get; set; }

    /// <summary>
    /// Gets or sets form tracker parent component 
    /// </summary>
    [CascadingParameter]
    public LgFormTracker FormTracker { get; set; }

    /// <summary>
    /// Parent "FormTrackerHandler".
    /// </summary>
    [CascadingParameter]
    public IFormTrackerHandler ParentFormTrackerHandler { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// Gets or sets tab included in tabs container
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets if the tab is enabled
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets icon name
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Gets or sets unique key
    /// </summary>
    [Parameter]
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets direct link
    /// </summary>
    [Parameter]
    public LgPageLink Link { get; set; }

    /// <summary>
    /// Gets or sets if tab content must be preload when created. Else tab content is load on first activation.
    /// if undifined, use the "PreloadTabContent" property of the parent "TabContainer".
    /// </summary>
    [Parameter]
    public bool? PreloadContent { get; set; }

    /// <summary>
    /// Gets or sets if the tab content is reloaded when it's activate
    /// </summary>
    [Parameter]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool RefreshOnActivate { get; set; }

    /// <summary>
    /// Gets or sets the title
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the tooltip
    /// </summary>
    [Parameter]
    public string Tooltip { get; set; }

    /// <summary>
    /// Gets or sets if the tooltip contents use the HTML format; else, the content use a raw text format.
    /// </summary>
    [Parameter]
    public bool TooltipIsHtml { get; set; }

    /// <summary>
    /// Gets or sets Uri of the content
    /// </summary>
    [Parameter]
    public string Uri { get; set; }

    /// <summary>
    /// Gets or sets the content in an IFrame.
    /// </summary>
    [Parameter]
    public bool UseIFrame { get; set; }

    /// <summary>
    /// Gets or sets css class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        // Ensure tab is registred into tab container
        LgTabData tabData = new()
        {
            AriaLabel = AriaLabel,
            Closable = false,
            Disabled = Disabled,
            Draggable = false,
            IconName = IconName ?? Link?.IconName,
            Key = Key,
            PanelContent = ChildContent,
            Pinned = false,
            PreloadContent = PreloadContent ?? TabContainer.PreloadTabContent,
            RefreshOnActivate = RefreshOnActivate,
            Title = Title ?? Link?.Title,
            FixedTitleAndIconName = IconName is not null || Title is not null,
            Tooltip = Tooltip,
            TooltipIsHtml = TooltipIsHtml,
            Uri = Uri ?? Link?.Uri,
            UseIFrame = UseIFrame
        };
        OnInitTabData(tabData);
        if (TabContainer.TryRegisterTabComponent(tabData, GetNewElementId('k'), null, out _tab))
        {
            OnRegisterTab(_tab);
        }
    }

    /// <summary>
    /// Initialise tab data from parameters.
    /// </summary>
    /// <param name="tabData">Tab state data.</param>
    internal virtual void OnInitTabData(LgTabData tabData) { }

    /// <summary>
    /// Register the new tab pane.
    /// </summary>
    /// <param name="tab">Tab</param>
    internal virtual void OnRegisterTab(LgTabRenderData tab) 
    {
        // Associate the FormTracker to the TabRenderTab
        tab.FormTracker = FormTracker;
        tab.ParentFormTrackerHandler = ParentFormTrackerHandler;
        tab.OnFieldChanged += OnFieldChanged;
        // StateChanged subscribtion
        tab.OnStateChanged += OnStateChanged;
    }

    /// <summary>
    /// Unregister the tab pane.
    /// </summary>
    /// <param name="tab">Tab.</param>
    internal virtual void OnUnregisterTab(LgTabRenderData tab) 
    {
        tab.OnFieldChanged -= OnFieldChanged;
        tab.OnStateChanged -= OnStateChanged;
        tab.FormTracker = null;
        tab.ParentFormTrackerHandler = null;
    }

    /// <summary>
    /// Open the side tab
    /// </summary>
    protected Task OnTabButtonClickAsync()
    {
        return TabContainer.OnTabButtonClickAsync(_tab);
    }

    /// <inheritdoc />
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.AddIf(Tab.IsActive, "active");
        builder.AddIf(Tab.Disabled, "disabled");
        // Add an indicator on the SideTab if FormTracker has pending modification
        builder.AddIf(((IFormTrackerHandler)Tab).HasModification(), "field-change");
        builder.Add(CssClass);
        base.OnBuildClassAttribute(builder);
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (_tab != null)
        {
            OnUnregisterTab(_tab);
            TabContainer.UnregisterTab(_tab);
        }
        base.Dispose(disposing);
    }

    #region IFormTrackerHandler for Tab

    /// <summary>
    /// The view need to be updated
    /// </summary>
    private void OnStateChanged()
    {
        StateHasChanged();
    }

    /// <summary>
    /// EditForm fields change, update the view
    /// </summary>
    private void OnFieldChanged()
    {
        var hasModif = ((IFormTrackerHandler)Tab).HasModification();
        if (!_hasModification.HasValue || _hasModification != hasModif)
        {
            if (TabContainer is LgTabContainer)
            {
                TabContainer.RefreshHeader();
            }
            else if (TabContainer is LgSideTabContainer)
            {
                StateHasChanged();
            }
            _hasModification = hasModif;
        }
    }

    #endregion

    #endregion

}
