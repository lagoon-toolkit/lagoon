namespace Lagoon.UI.Components.Internal;

internal class LgTabRenderData : ITab, IPageManager, IFormTrackerHandler
{

    #region properties

    /// <summary>
    /// DOM element ID for the tab button.
    /// </summary>
    public string ButtonElementId { get; } = LgComponentBase.GetNewElementId();

    /// <summary>
    /// DOM element ID for the content of the tab.
    /// </summary>
    public string ContentElementId { get; } = LgComponentBase.GetNewElementId();

    /// <summary>
    /// Gets or sets if the tab is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets if component must be render even if it's already rendered.
    /// </summary>
    public bool IsTabContentLoaded { get; set; }

    /// <summary>
    /// Gets or sets the tab icon name initialized by the page.
    /// </summary>
    public string PageIconName { get; set; }

    /// <summary>
    /// Gets or sets the tab title initialized by the page.
    /// </summary>
    public string PageTitle { get; set; }

    /// <summary>
    /// Route handler for the uri.
    /// </summary>
    public RouteData RouteData { get; set; }

    /// <summary>
    /// Tab informations.
    /// </summary>
    public LgTabData TabData { get; set; }

    /// <summary>
    /// Gets or sets unique key used to refresh tab
    /// </summary>
    public Guid DynamicRenderKey { get; set; } = Guid.NewGuid();

    #endregion

    #region shortcuts

    /// <summary>
    /// Gets or sets aria label
    /// </summary>
    public string AriaLabel => TabData.AriaLabel;

    /// <summary>
    /// Gets if the tab is closable
    /// </summary>
    public bool Closable => TabData.Closable;

    /// <summary>
    /// Gets if the tab is enabled
    /// </summary>
    public bool Disabled => TabData.Disabled;

    /// <summary>
    /// Gets if the tab can be moved
    /// </summary>
    public bool Draggable => TabData.Draggable;

    /// <summary>
    /// Gets if tab content is loaded from route.
    /// </summary>
    public bool HasRoute => RouteData is not null;

    /// <summary>
    /// Gets the tab icon name.
    /// </summary>
    public string IconName => TabData.FixedTitleAndIconName ? TabData.IconName : PageIconName ?? TabData.IconName;

    /// <summary>
    /// Gets the tab unique identifier.
    /// </summary>
    public string Key => TabData.Key;

    /// <summary>
    /// Gets the tab panel content.
    /// </summary>
    public RenderFragment PanelContent => TabData.PanelContent;

    /// <summary>
    /// Gets if the tab can be pinned at the begining of the tab list
    /// </summary>
    public bool Pinned => TabData.Pinned;

    /// <summary>
    /// Gets or sets if tab content must be preload when created. Else tab content is load on first activation.
    /// if undifined, use the "PreloadTabContent" property of the parent "TabContainer".
    /// </summary>
    public bool PreloadContent => TabData.PreloadContent;

    /// <summary>
    /// Gets if the tab content is reloaded when it's activate
    /// </summary>
    public bool RefreshOnActivate => TabData.RefreshOnActivate;

    /// <summary>
    /// Gets the tab title.
    /// </summary>
    public string Title => TabData.FixedTitleAndIconName ? TabData.Title : PageTitle ?? TabData.Title ?? Key;

    /// <summary>
    /// Get the tab tooltip.
    /// </summary>
    public string Tooltip => TabData.Tooltip;

    /// <summary>
    /// Gets or sets if the tooltip contents use the HTML format; else, the content use a raw text format.
    /// </summary>
    public bool TooltipIsHtml => TabData.TooltipIsHtml;

    /// <summary>
    /// Gets Uri of the content
    /// </summary>
    public string Uri => TabData.Uri;

    /// <summary>
    /// Gets the content in an IFrame.
    /// </summary>
    public bool UseIFrame => TabData.UseIFrame;

    #endregion

    #region events

    /// <summary>
    /// Event called when the tab is leaving for another tab.
    /// </summary>
    public event Action OnStateChanged;

    /// <summary>
    /// Event called when the tab is selected.
    /// </summary>
    public event Func<Task> OnRaiseActivatedEventAsync;

    /// <summary>
    /// Event called when the tab is closing.
    /// </summary>
    public event Func<PageClosingSourceEvent, Func<Task>, Task<bool>> OnRaiseClosingEventAsync;

    /// <summary>
    /// Event called when the tab is closed.
    /// </summary>
    public event Func<Task> OnRaiseCloseEventAsync;

    /// <summary>
    /// Event called when a tab is moved.
    /// </summary>
    internal event Func<Task> OnRaiseMoveEventAsync;

    #endregion

    #region constructor

    /// <summary>
    /// Initialize new instance.
    /// </summary>
    public LgTabRenderData(LgTabData tabData)
    {
        TabData = tabData;
    }

    /// <summary>
    /// Initialize new instance.
    /// </summary>
    public LgTabRenderData(string key, string uri, RenderFragment panelContent = null)
    {
        TabData = new LgTabData()
        {
            Key = key,
            Uri = uri,
            PanelContent = panelContent
        };
    }

    /// <summary>
    /// Methods called when the tab is activated
    /// </summary>
    internal async Task RaiseActivatedEventAsync()
    {
        IsActive = true;
        if (OnRaiseActivatedEventAsync != null)
        {
            await OnRaiseActivatedEventAsync?.Invoke();
        }

        StateHasChanged();
    }

    /// <summary>
    /// Methods called when the tab is leaving for a new tab.
    /// </summary>
    internal void Leave()
    {
        IsActive = false;
        StateHasChanged();
    }

    internal void StateHasChanged()
    {
        OnStateChanged?.Invoke();
    }

    /// <summary>
    /// Indicate if a tab can be closed.
    /// </summary>
    /// <param name="sourceEvent">Event source of the closing event.</param>
    /// <param name="confirmationMethod">Method to call when the closing is confirmed.</param>
    internal async Task<bool> RaiseClosingEventAsync(PageClosingSourceEvent sourceEvent, Func<Task> confirmationMethod)
    {
        if (OnRaiseClosingEventAsync is null)
        {
            return true;
        }
        return await OnRaiseClosingEventAsync.Invoke(sourceEvent, confirmationMethod);
    }

    /// <summary>
    /// Raise the page "OnClose" event.
    /// </summary>
    internal async Task RaiseCloseEventAsync()
    {
        if (OnRaiseCloseEventAsync != null)
        {
            await OnRaiseCloseEventAsync.Invoke();
        }
    }

    /// <summary>
    /// Raise the page "OnMove" event.
    /// </summary>
    internal async Task RaiseMoveEventAsync()
    {
        if (OnRaiseMoveEventAsync != null)
        {
            await OnRaiseMoveEventAsync.Invoke();
        }
    }
    #endregion

    #region IFormTrackerHandler implementation

    ///// <summary>
    ///// List des form trackers
    ///// </summary>
    private readonly List<LgFormTracker> _formTrackerList = new();

    /// <summary>
    /// Event fired when the model of an LgEditForm has been changed
    /// </summary>
    internal event Action OnFieldChanged;

    /// <summary>
    /// Gets or sets form tracker parent component 
    /// </summary>
    public LgFormTracker FormTracker { get; set; }

    /// <summary>
    /// Parent "FormTrackerHandler".
    /// </summary>
    public IFormTrackerHandler ParentFormTrackerHandler { get; set; }

    /// <summary>
    /// The FormTracker list
    /// </summary>
    List<LgFormTracker> IFormTrackerHandler.FormTrackerList => _formTrackerList;

    /// <summary>
    /// Parent FormTracker
    /// </summary>
    IFormTrackerHandler IFormTrackerHandler.ParentFormTracker => ParentFormTrackerHandler;

    #endregion

    #region methods

    /// <summary>
    ///  Notifies the tab content has changed. When applicable, this will
    ///   cause the tab content to be re-loaded.
    /// </summary>
    public void TabContentHasChanged()
    {
        DynamicRenderKey = Guid.NewGuid();
    }

    /// <summary>
    /// Return the tootip and Aria aditional attributes.
    /// </summary>
    /// <param name="ignoreAria">If <c>true</c>, the ARIA attributes aren't added.</param>
    /// <param name="tooltipPosition">Tooltip position.</param>
    /// <param name="ignoreTooltip">If <c>true</c>, the tooltip attributes aren't added.</param>
    /// <returns>The attribute collection.</returns>
    public IEnumerable<KeyValuePair<string, object>> GetButtonAdditionalAttributes(bool ignoreAria = false,
        TooltipPosition tooltipPosition = TooltipPosition.None, bool ignoreTooltip = false)
    {
        return LgComponentBase.GetAdditionalAttributes(GetButtonAriaAttributes(ignoreAria),
            LgComponentBase.GetTooltipAttributes(Tooltip, TooltipIsHtml, tooltipPosition, ignoreTooltip));
    }

    /// <summary>
    /// Get the list of Aria attributes to add to render.
    /// </summary>
    /// <param name="ignoreAria">If <c>true</c> the method do nothing.</param>

    /// <returns>The list of Aria attributes to add to render a tooltip.</returns>
    public IEnumerable<KeyValuePair<string, object>> GetButtonAriaAttributes(bool ignoreAria = false)
    {
#if DEBUG
        ignoreAria = false;
#endif
        if (!ignoreAria)
        {
            yield return new KeyValuePair<string, object>("id", ButtonElementId);
            yield return new KeyValuePair<string, object>("aria-label", AriaLabel.CheckTranslate());
            yield return new KeyValuePair<string, object>("aria-controls", ContentElementId);
            yield return new KeyValuePair<string, object>("role", "tab");
            yield return new KeyValuePair<string, object>("aria-selected", IsActive.JsonEncode());
        }
    }

    /// <inheritdoc />
    public void RaiseFieldChanged()
    {
        OnFieldChanged?.Invoke();

        IFormTrackerHandler tracker = ParentFormTrackerHandler;
        while (tracker != null)
        {
            tracker.RaiseFieldChanged();
            tracker = tracker.ParentFormTracker;
        }
    }

    public string GetClassAttribute()
    {
        LgCssClassBuilder builder = new();
        builder.AddIf(IsActive, "active");
        builder.AddIf(Disabled, "disabled");
        // Add an indicator on the SideTab if FormTracker has pending modification
        builder.AddIf(((IFormTrackerHandler)this).HasModification(), "field-change");
        builder.Add(TabData.CssClass);
        return builder.ToString();
    }

    #endregion

}
