namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Custom tab container
/// </summary>
public partial class LgCustomTabContainer : LgAriaComponentBase, IDisposable, IPageTitleHandler, IPageCloser, IPageReloader, IPageDefaultLayoutProvider
{

    #region fields

    /// <summary>
    /// Active tab
    /// </summary>
    internal LgTabRenderData _activeTab;

    private bool _ignoreRefresh;

    #endregion

    #region properties

    internal LgTabRenderDataCollection LoadingTabDataList { get; set; } = new LgTabRenderDataCollection();

    internal LgTabRenderDataCollection TabDataList { get; set; }

    /// <summary>
    /// Indicate if the tab constainer is still loading is data.
    /// </summary>
    protected internal bool IsLoading => TabDataList is null;

    /// <summary>
    /// Indicate if the tab constainer has loaded is data.
    /// </summary>
    protected internal bool IsLoaded => !IsLoading;

    /// <summary>
    /// Determines an unique identifier for each tab data changes.
    /// </summary>
    internal Guid TabDataUpdateState { get; set; }

#if DEBUG

    /// <summary>
    /// ShowDebug informations
    /// </summary>
    /// <returns>Return debug informations.</returns>
    public string DebugState()
    {
        if (IsLoading)
        {
            return "Loading...";
        }
        else
        {
            return TabDataList.Count.ToString() + " tabs";
        }
    }

#endif

    #endregion region

    #region render fragments

    /// <summary>
    /// Gets or sets content of the tab container
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets Active tab
    /// </summary>
    [Parameter]
    public string ActiveTabKey { get; set; }

    /// <summary>
    /// Allow binding on "ActiveTabKey".
    /// This allows the developer to manage the tab to select by manipulating the ActiveTabKey property
    /// </summary>
    [Parameter]
    public EventCallback<string> ActiveTabKeyChanged { get; set; }

    /// <summary>
    /// Gets or sets tab container id
    /// </summary>
    [Parameter]
    [Obsolete("Unused parameter", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the tab activated by default.
    /// </summary>
    [Parameter]
    public string DefaultActiveTabKey { get; set; }

    /// <summary>
    /// Gets or sets css class of the tab container
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets if tab contents must be preload by default.
    /// </summary>
    [Parameter]
    public bool PreloadTabContent { get; set; }

    /// <summary>
    /// Tooltip position.
    /// </summary>
    public TooltipPosition TooltipPosition { get; set; }

    /// <summary>
    /// Gets or sets the type of a layout to be used if the page does not declare any layout.
    /// If specified, the type must implement IComponent and accept a parameter named Body.
    /// </summary>
    [Parameter]
    public Type DefaultLayout { get; set; }

    #endregion

    #region events

    /// <summary>
    /// Gets or sets event rise when tab is activated
    /// </summary>
    [Parameter]
    public EventCallback<ActivateTabEventArgs> OnActivateTab { get; set; }

    /// <summary>
    /// Gets or sets event rise before tab is removed
    /// </summary>
    [Parameter]
    public EventCallback<ClosingTabEventArgs> OnClosingTab { get; set; }

    /// <summary>
    /// Gets or sets event rise when tab is removed
    /// </summary>
    [Parameter]
    public EventCallback<CloseTabEventArgs> OnCloseTab { get; set; }

    /// <summary>
    /// Get or sets if a new clic on the selected tab reload the tab content.
    /// </summary>
    [Parameter]
    public bool ReloadEnabled { get; set; }

    #endregion

    #region actions

    /// <summary>
    /// Gets or sets event rise when header must be refresh
    /// </summary>
    internal Action OnRefreshHeader { get; set; }

    /// <summary>
    /// Gets or sets event rise when header must be refresh
    /// </summary>
    internal Action OnRefreshBody { get; set; }

    #endregion

    #region dependencies

    /// <summary>
    /// Gets or sets javascript runtime dependency
    /// </summary>
    [Inject]
    protected IJSRuntime JSRuntime { get; set; }

    #endregion

    #region page template

    /// <summary>
    /// Return the default layout to use.
    /// </summary>
    /// <returns></returns>
    Type IPageDefaultLayoutProvider.GetDefaultLayout()
    {
        return DefaultLayout;
    }

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (!string.IsNullOrEmpty(DefaultActiveTabKey))
        {
            ActiveTabKey = DefaultActiveTabKey;
        }
    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _ignoreRefresh = false;
    }

    ///<inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        // Change the current selected tab by binding
        if ((_activeTab is not null) && !string.IsNullOrEmpty(ActiveTabKey) && (ActiveTabKey != _activeTab.Key))
        {
            await ActivateTabAsync(ActiveTabKey);
        }
    }

    ///<inheritdoc/>
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (_activeTab is null && LoadingTabDataList.Count != 0)
        {
            return ActivateTabAsync(LoadingTabDataList[0]);
        }
        else
        {
            return Task.CompletedTask;
        }
    }

    ///<inheritdoc/>
    protected override bool ShouldRender()
    {
        bool result = !_ignoreRefresh;
        _ignoreRefresh = true;
        return result;
    }

    /// <summary>
    /// Return tab list if loaded
    /// </summary>
    /// <returns></returns>
    internal IEnumerable<LgTabRenderData> GetLoadedTabs()
    {
        if (TabDataList is null)
        {
            return Enumerable.Empty<LgTabRenderData>();
        }

        return TabDataList;
    }

    /// <summary>
    /// Return tab list if uri loaded
    /// </summary>
    /// <returns></returns>
    internal IEnumerable<Tab> GetLoadedTabsAsModel()
    {
        if (TabDataList is null)
        {
            return Enumerable.Empty<Tab>();
        }

        return TabDataList.Select(x => new Tab { 
            Uri = x.Uri,
            IconName = x.IconName,
            Title = x.Title
        });
    }


    /// <summary>
    /// Open component in an tab
    /// </summary>
    /// <param name="uri">URI of the page to open.</param>
    /// <param name="routeData">Informations to display the page.</param>
    /// <param name="closable">Indicate if the table is closable.</param>
    internal virtual Task OpenTabAsync(string uri, RouteData routeData = null, bool closable = true)
    {
        LgTabData tabData = new()
        {
            Closable = closable,
            Uri = uri
        };
        return OpenTabAsync(tabData, routeData);
    }

    /// <summary>
    /// Add new tab used by children.
    /// </summary>
    /// <param name="tabData">Tab informations.</param>
    /// <param name="routeData">Informations to display the page.</param>
    internal virtual Task OpenTabAsync(LgTabData tabData, RouteData routeData)
    {
        // Ensure tab is added to the collection
        LgTabRenderData tab = RegisterTabWithUri(tabData, routeData);
        // Activate the tab
        return ActivateTabAsync(tab);
    }

    /// <summary>
    /// Register the new tab component and activate it if its necessary
    /// </summary>
    /// <param name="tabData">Tab informations.</param>
    /// <param name="defaultKey"></param>
    /// <param name="routeData">Informations to display the page.</param>
    /// <param name="tabRenderData"></param>
    /// <returns></returns>
    internal bool TryRegisterTabComponent(LgTabData tabData, string defaultKey, RouteData routeData, out LgTabRenderData tabRenderData)
    {
        string activeKey = ActiveTabKey;
        // Ensure tab is registred in tab container
        bool result = TryRegisterTab(tabData, defaultKey, routeData, out tabRenderData);
        if (result)
        {
            // Active the tab corresponding the active key
            if (tabRenderData.Key == activeKey)
            {
                // Deactivate current active tab
                if (_activeTab != null && _activeTab.Key != tabRenderData.Key)
                {
                    _activeTab.Leave();
                }
                tabRenderData.IsActive = true;
                _activeTab = tabRenderData;
            }
        }
        else
        {
            // Update tab data
            tabRenderData.TabData = tabData;
        }
        return result;
    }

    /// <summary>
    /// Add new tab used by children.
    /// </summary>
    /// <param name="tabData">Tab informations.</param>
    /// <param name="routeData">Informations to display the page.</param>
    internal LgTabRenderData RegisterTabWithUri(LgTabData tabData, RouteData routeData)
    {
        _ = TryRegisterTab(tabData, null, routeData, out LgTabRenderData tabRenderData);
        return tabRenderData;
    }

    /// <summary>
    /// Add new tab used by children.
    /// </summary>
    /// <param name="tabData">Tab informations.</param>
    /// <param name="defaultKey">Key to use if it's can't be generated from URI.</param>
    /// <param name="routeData">Informations to display the page.</param>
    /// <param name="tabRenderData">Tab render informations.</param>
    internal bool TryRegisterTab(LgTabData tabData, string defaultKey, RouteData routeData, out LgTabRenderData tabRenderData)
    {
        bool result = false;

        // Initialise tab key if it's necessary
        if (string.IsNullOrEmpty(tabData.Key))
        {
            if (tabData.Uri is null)
            {
                if (defaultKey is null)
                {
                    throw new ArgumentNullException("tabData.Key");
                }
                tabData.Key = defaultKey;
            }
            else
            {
                tabData.Key = LgRouter.GenerateTabKey(tabData.Uri);
            }
        }
        // Check if tab already exists in collection
        if (!LoadingTabDataList.TryGet(tabData.Key, out tabRenderData))
        {
            // Create render state object for tab
            tabRenderData = new LgTabRenderData(tabData);
            // Get route if an uri is defined
            if ((tabData.Uri is not null) && routeData is null)
            {
                routeData = App.GetRouteData(tabData.Uri);
            }
            tabRenderData.RouteData = routeData;
            // Add tab to collection
            LoadingTabDataList.Add(tabData.Key, tabRenderData);
            // Indicate a new tab has been registred
            result = true;
        }
        //// Indicate the data for tab construction have been changed
        TabDataUpdateState = Guid.NewGuid();
        // Indicate if a new tab has been registred
        return result;
    }

    /// <summary>
    /// Remove the tab data from the collection.
    /// </summary>
    /// <param name="tabRenderData">Tab to remove.</param>
    internal virtual void UnregisterTab(LgTabRenderData tabRenderData)
    {
        if (_activeTab == tabRenderData)
        {
            _activeTab = null;
        }
        LoadingTabDataList.Remove(tabRenderData.Key);
    }

    /// <summary>
    /// Return a list key of all opened tabs
    /// </summary>
    public IEnumerable<string> GetOpenTabsKeys()
    {
        if (TabDataList is null)
        {
            return Enumerable.Empty<string>();
        }

        return TabDataList.GetKeys();
    }

    /// <summary>
    /// Ask the selection of a a specific tab.
    /// </summary>
    /// <param name="tab">Tab.</param>
    internal virtual Task GoToTabAsync(LgTabRenderData tab)
    {
        return ActivateTabAsync(tab);
    }

    /// <summary>
    /// Activate tab by its key
    /// </summary>
    /// <param name="key">tab id</param>
    internal async Task<LgTabRenderData> ActivateTabAsync(string key)
    {
        if (LoadingTabDataList.TryGet(key, out LgTabRenderData tab))
        {
            await ActivateTabAsync(tab);
            return tab;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Activate the tab and refresh display.
    /// </summary>
    /// <param name="toActivateTab">Tab to activate.</param>
    /// <returns></returns>
    internal virtual async Task ActivateTabAsync(LgTabRenderData toActivateTab)
    {
        if (_activeTab?.Key != toActivateTab.Key)
        {
            // Deactivate current active tab
            if (_activeTab != null && _activeTab.Key != toActivateTab.Key)
            {
                _activeTab.Leave();
            }
            // Activate the target tab
            _activeTab = toActivateTab;
            await _activeTab.RaiseActivatedEventAsync();
            await ActiveTabKeyChanged.TryInvokeAsync(App, _activeTab.Key);
            // Update display
            if (OnActivateTab.HasDelegate)
            {
                await OnActivateTab.TryInvokeAsync(App, new ActivateTabEventArgs(toActivateTab));
            }
            // Indicate that the component state has changed
            OnRefreshHeader?.Invoke();
            OnRefreshBody?.Invoke();
        }
    }

    #region Change Tab

    /// <summary>
    /// Change active tab
    /// </summary>
    /// <param name="tab">Tab.</param>
    internal async Task OnTabButtonClickAsync(LgTabRenderData tab)
    {
        if (ReloadEnabled && tab.HasRoute && tab == _activeTab)
        {
            // Reload the tab
            await ReloadTabAsync(tab, false);
        }
        else
        {
            // Activate the tab
            await GoToTabAsync(tab);
        }
    }

    #endregion

    #region Reload tab

    /// <summary>
    /// Method called when the method "Reload" is called in a LgPage.
    /// </summary>
    /// <param name="page">Page.</param>
    /// <param name="force">Indicate if we must bypass user confirmations.</param>
    Task IPageReloader.ReloadPageAsync(LgPage page, bool force)
    {
        return ReloadTabAsync((LgTabRenderData)page.Tab, force);
    }

    /// <summary>
    /// Reload the content page in the tab.
    /// </summary>
    /// <param name="tab">Tab.</param>
    /// <param name="force">Bypass the OnClosingTab event.</param>
    internal async Task ReloadTabAsync(LgTabRenderData tab, bool force = false)
    {
        // Local function definition
        async Task action()
        {
            await tab.RaiseCloseEventAsync();
            ReloadTab(tab);
        }
        // Execute the close reload the closing is confirmed
        await ExecuteAfterClosingAsync(PageClosingSourceEvent.PageReload, action, tab, force);
    }

    #endregion

    #region Add and remove Tab

    /// <summary>
    /// Method called when the method "Close" is called in a LgPage.
    /// </summary>
    /// <param name="page">Page to close.</param>
    /// <param name="force">Indicate if we must bypass user confirmations.</param>
    Task IPageCloser.ClosePageAsync(LgPage page, bool force)
    {
        return CloseTabAsync((LgTabRenderData)page.Tab, force);
    }

    /// <summary>
    /// Remove tab by its id
    /// </summary>
    /// <param name="tab">Tab.</param>
    /// <param name="force">Bypass the OnClosingTab event.</param>
    internal virtual async Task CloseTabAsync(LgTabRenderData tab, bool force = false)
    {
        // Local function definition
        async Task action()
        {
            bool isActive = tab.IsActive;
            int index = LoadingTabDataList.IndexOf(tab.Key);
            LoadingTabDataList.Remove(tab.Key);
            await tab.RaiseCloseEventAsync();
            if (isActive)
            {
                // Active previous tab
                if (LoadingTabDataList.Count > 0)
                {
                    int prevTab = Math.Max(0, index - 1);
                    await GoToTabAsync(LoadingTabDataList[prevTab]);
                }
            }
            else
            {
                // Refresh the button list (required when they're a confirmation message on close)
                OnRefreshHeader?.Invoke();
            }
            // Raise events
            if (OnCloseTab.HasDelegate)
            {
                await OnCloseTab.TryInvokeAsync(App, new CloseTabEventArgs(tab.Key));
            }
        }
        // Execute the close after the closing is confirmed
        await ExecuteAfterClosingAsync(PageClosingSourceEvent.PageClose, action, tab, force);
    }

    /// <summary>
    /// Execute the action after the closing is confirmed.
    /// </summary>
    /// <param name="sourceEvent">Source indicator for the "OnClosing" event.</param>
    /// <param name="action">Action to execute.</param>
    /// <param name="tab">Tab selected.</param>
    /// <param name="force">If <c>true</c>, bypass the confirmation.</param>
    /// <returns></returns>
    private async Task ExecuteAfterClosingAsync(PageClosingSourceEvent sourceEvent, Func<Task> action, LgTabRenderData tab, bool force = false)
    {
        if (await IsClosingConfirmedAsync(sourceEvent, action, tab, force))
        {
            await action();
        }
    }

    /// <summary>
    /// Get the tab data if the page can be closed.
    /// </summary>
    /// <param name="sourceEvent">Event source.</param>
    /// <param name="confirmationMethod">Method to call when the closing is confirmed.</param>
    /// <param name="tab">Tab.</param>
    /// <param name="force">Bypass the OnClosingTab event.</param>
    /// <returns>The tab to close.</returns>
    private async Task<bool> IsClosingConfirmedAsync(PageClosingSourceEvent sourceEvent, Func<Task> confirmationMethod, LgTabRenderData tab, bool force = false)
    {
        if (force)
        {
            return true;
        }
        // Raise event
        if (OnClosingTab.HasDelegate)
        {
            ClosingTabEventArgs e = new(tab.Key);
            await OnClosingTab.TryInvokeAsync(App, e);
            if (e.Cancel)
            {
                return false;
            }
        }
        // Checkif tab can be closed
        return await tab.RaiseClosingEventAsync(sourceEvent, confirmationMethod);
    }

    #endregion

    /// <summary>
    /// Reload the content of the tab.
    /// </summary>
    internal void ReloadTab(LgTabRenderData tab)
    {
        tab.TabContentHasChanged();
        OnRefreshHeader?.Invoke();
        OnRefreshBody?.Invoke();
    }

    /// <summary>
    /// Render the header
    /// </summary>
    internal void RefreshHeader()
    {
        OnRefreshHeader?.Invoke();
    }

    /// <summary>
    /// Change the spin state of an tab
    /// </summary>
    protected virtual void PinTab(PinTabEventArgs args)
    {
        LoadingTabDataList.PinTab(args.Key);
    }

    ///<inheritdoc/>
    Task IPageTitleHandler.SetPageErrorTitleAsync(ITab tab, string title, string iconName)
    {
        return Task.FromResult(SetTabTitle(tab.Key, title, iconName));
    }

    ///<inheritdoc/>
    Task IPageTitleHandler.SetPageTitleAsync(LgPage page)
    {
        return OnPageSetTitleAsync(page);
    }

    /// <summary>
    /// Method called when page title is defined.
    /// </summary>
    /// <param name="page">Page instance.</param>
    /// <returns><c>false</c> if the tab is not found.</returns>
    protected virtual Task<bool> OnPageSetTitleAsync(LgPage page)
    {
        return Task.FromResult(SetTabTitle(page.Tab.Key, page.Title, page.LoadingIconName ?? page.IconName, page.TabCssClass));
    }

    /// <summary>
    /// Method called to change the tab title.
    /// </summary>
    /// <param name="tabKey">The tab identifier.</param>
    /// <param name="title">The tab title.</param>
    /// <param name="iconName">The icon name.</param>
    /// <param name="tabCssClass">The tab button CssClass.</param>
    /// <returns><c>true</c> if the tab is found.</returns>
    private bool SetTabTitle(string tabKey, string title, string iconName, string tabCssClass = null)
    {
        if (LoadingTabDataList.TryGet(tabKey, out LgTabRenderData tab))
        {
            title ??= App.ApplicationInformation.Name;
            if (tab.PageTitle != title || tab.PageIconName != iconName || tab.TabData.CssClass != tabCssClass)
            {
                tab.PageTitle = title;
                tab.PageIconName = iconName;
                tab.TabData.CssClass = tabCssClass;          
                // Refresh a vertical tab button of the SideTab
                tab.StateHasChanged();
                // Refresh the horizontal tab buttons
                OnRefreshHeader?.Invoke();
            }
            return true;
        }
        return false;
    }

    #endregion

}
