namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Tab container header
/// </summary>
public partial class LgTabContainerHeader : LgAriaComponentBase
{
    #region private fields

    /// <summary>
    /// Key of the dragged tab
    /// </summary>
    private string _draggedTab;

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    private ElementReference ElementRef { get; set; }

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets or sets tab container
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    public LgCustomTabContainer TabContainer { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Show in the toolbar, the dropdown menu of the list of openned tabs.
    /// </summary>
    [Parameter]
    public bool ShowTabList { get; set; }

    /// <summary>
    /// Gets or sets the frame toolbar
    /// </summary>
    [Parameter]
    public RenderFragment Toolbar { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets if the drag and drop is active
    /// </summary>
    [Parameter]
    public bool AllowDragDrop { get; set; }

    /// <summary>
    /// Gets or sets if the closing tab is active
    /// </summary>
    [Parameter]
    public bool AllowClose { get; set; }

    #endregion

    #region events

    /// <summary>
    /// Drop tab event
    /// </summary>
    [Parameter]
    public EventCallback<DropTabEventArgs> OnDropTab { get; set; }

    /// <summary>
    /// Pin tab event
    /// </summary>
    [Parameter]
    public EventCallback<PinTabEventArgs> OnPinTab { get; set; }

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        TabContainer.OnRefreshHeader += OnRefresh;
    }

    ///<inheritdoc/>
    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && App.WindowInformation.MediaType != MediaType.Mobile)
        {
            await JS.ScriptGetNewRefAsync("Lagoon.LgTabScroll.init", ElementRef);
        }
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        TabContainer.OnRefreshHeader -= OnRefresh;
        if (JS is IJSInProcessRuntime ijs)
        {
            ijs.InvokeVoid("Lagoon.LgTabScroll.dispose", ElementRef);
        }
        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    protected override bool ShouldRender()
    {
        return !TabContainer.IsLoading;
    }

    /// <summary>
    /// Called when the header need to be render.
    /// </summary>
    private void OnRefresh()
    {
        if (TabContainer is LgAppTabContainerLayout appTabContainer)
        {
            appTabContainer.SaveTabsUriAsync();
        }
        StateHasChanged();
    }

    /// <summary>
    /// Rise change tab event
    /// </summary>
    /// <param name="tab">Tab.</param>
    internal virtual Task ChangeTabAsync(LgTabRenderData tab)
    {
        return TabContainer.OnTabButtonClickAsync(tab);
    }

    /// <summary>
    /// Rise remove tab event
    /// </summary>
    /// <param name="tab">Tab.</param>
    internal virtual async Task CloseTabAsync(LgTabRenderData tab)
    {
        if (tab.Closable)
        {
            await TabContainer.CloseTabAsync(tab, false);
            if (TabContainer is LgAppTabContainerLayout tbc)
            {
                tbc.SaveTabsUriAsync();
            }
        }
    }

    /// <summary>
    /// Close all opened tabs
    /// </summary>
    /// <returns></returns>
    internal async Task CloseAllTabAsync()
    {
        // ToList() required because the collection is modified during process
        foreach (LgTabRenderData tab in TabContainer.LoadingTabDataList.ToList())
        {
            await CloseTabAsync(tab);
        }
    }

    /// <summary>
    /// Raise the pin tab event.
    /// </summary>
    /// <param name="key">The tab key.</param>
    private async Task PinTabAsync(string key)
    {
        if (OnPinTab.HasDelegate)
        {
            await OnPinTab.TryInvokeAsync(App, new PinTabEventArgs(key));
        }
    }

    #region Drag & drop

    /// <summary>
    /// Indicate if an tab is draggable
    /// </summary>
    /// <param name="tab"></param>
    /// <returns></returns>
    internal bool IsDraggable(LgTabRenderData tab)
    {
        return AllowDragDrop && (!tab.Disabled) && tab.Draggable && !tab.Pinned;
    }

    /// <summary>
    /// Drag and drop start
    /// </summary>
    /// <param name="args"></param>
    /// <param name="key"></param>
    private void HandleDragStart(DragEventArgs args, string key)
    {
        if (AllowDragDrop)
        {
            args.DataTransfer.DropEffect = "move";
            args.DataTransfer.EffectAllowed = "move";
            _draggedTab = key;
        }
    }

    /// <summary>
    /// Drag and drop drop
    /// </summary>
    /// <param name="tab"></param>
    private async Task HandleDropAsync(LgTabRenderData tab)
    {
        if (_draggedTab != null && IsDraggable(tab))
        {
            int dropIndex = TabContainer.TabDataList.IndexOf(tab.Key);
            if (OnDropTab.HasDelegate)
            {
                await OnDropTab.TryInvokeAsync(App, new DropTabEventArgs(_draggedTab, dropIndex));
            }
            _draggedTab = null;

            await tab.RaiseMoveEventAsync();
        }
    }

    #endregion

    #region Scroll

    /// <summary>
    /// Scroll Tab header to the left
    /// </summary>
    /// <returns></returns>
    public Task OnClickScrollLeftAsync()
    {
        return JS.ScriptGetNewRefAsync("Lagoon.LgTabScroll.scroll", ElementRef, "left");
    }

    /// <summary>
    /// Scroll Tab header to the right
    /// </summary>
    /// <returns></returns>
    public Task OnClickScrollRightAsync()
    {
        return JS.ScriptGetNewRefAsync("Lagoon.LgTabScroll.scroll", ElementRef, "right");
    }

    #endregion

    #endregion
}
