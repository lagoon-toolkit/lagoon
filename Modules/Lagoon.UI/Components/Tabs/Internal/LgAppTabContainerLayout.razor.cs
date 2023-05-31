using Microsoft.AspNetCore.Components.Authorization;

namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Overload of TabContainer for tabbed application
/// </summary>
public partial class LgAppTabContainerLayout : LgTabContainer
{
    #region constants

    /// <summary>
    /// Key of the default tab
    /// </summary>
    internal const string TABHOMEKEY = "tabHome";

    #endregion

    #region fields

    /// <summary>
    /// Uri of page to open when application is openned with the default URL.
    /// </summary>
    private string _defaultUri;

    /// <summary>
    /// Container for full screen pages.
    /// </summary>
    internal LgFullScreenPageContainer _fullScreenPageContainer;

    /// <summary>
    /// List of page tab top open when application is launched.
    /// </summary>
    internal List<LgTabData> _startupTabs;

    /// <summary>
    /// Tab is initialized ?
    /// </summary>
    private bool _tabInitialized;

    #endregion

    #region dependencies injection

    /// <summary>
    /// Client tab service
    /// </summary>
    [Inject]
    public ClientTabService ClientTabService { get; set; }

    #endregion

    #region cascading parameter

    /// <summary>
    /// LgApp instance.
    /// </summary>
    [CascadingParameter]
    public LgApp AppComponent { get; set; }

    /// <summary>
    /// Class handling the SetTitle method.
    /// </summary>
    [CascadingParameter]
    private IPageTitleHandler PageTitleHandler { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Change parameter default values
    /// </summary>
    public LgAppTabContainerLayout()
    {
        AllowClose = true;
        AllowDragDrop = true;
        ReloadEnabled = true;
        ShowTabList = true;
        CssClass = "app-nav-tabs";
    }

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;

    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        AppComponent.OnNavigateTo -= NavigateToAsync;
        AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        base.Dispose(disposing);
    }

    ///<inheritdoc/>
    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        if (!_tabInitialized && App.BehaviorConfiguration.TabSavingMethod != TabSavingMethod.None)
        {
            TryReloadSavedTabsAsync();
        }
    }

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            // Load saved tabs from Database or IndexedDb
            if (!_tabInitialized && App.BehaviorConfiguration.TabSavingMethod != TabSavingMethod.None)
            {
                if (App.BehaviorConfiguration.TabSavingMethod != TabSavingMethod.Remote || await IsUserAuthenticatedAsync())
                {
                    await ReloadSavedTabsAsync();
                }
            }
            // Wait authentication if startup tab use "PolicyVisible"
            List<LgTabData> startupTabs = await GetVisibleStartupTabsAsync();
            // Register startup pages
            if (startupTabs.Count == 0)
            {
                startupTabs.Add(AddStartupPage(true, string.Empty, string.Empty, IconNames.Home, false));
            }
            foreach (LgTabData tabData in startupTabs)
            {
                RegisterTabWithUri(tabData, null);
            }
            // Open the initial tab
            if (string.IsNullOrEmpty(AppComponent.CurrentUri) && !string.IsNullOrEmpty(_defaultUri))
            {
                await OpenTabAsync(_defaultUri, null);
            }
            else
            {
                await OpenInTabOrFullScreenAsync(AppComponent.CurrentUri, AppComponent.CurrentRouteData);
            }
            // Set the default configuration to reload active tab
            ReloadEnabled = App.BehaviorConfiguration.TabReloadEnabled;
            // Follow next navigation
            AppComponent.OnNavigateTo += NavigateToAsync;
        }
    }

    /// <summary>
    /// Gets the list of tabs that have no "PolicyVisible" or a valid policy visible.
    /// </summary>
    /// <returns>The list of tabs that have no "PolicyVisible" or a valid policy visible.</returns>
    private async Task<List<LgTabData>> GetVisibleStartupTabsAsync()
    {
        List<LgTabData> startupTabs = new();
        Dictionary<string, bool> cache = null;
        foreach (LgTabData tabData in _startupTabs)
        {
            bool visible = true;
            if (tabData is LgStartupTabData std)
            {
                if (!string.IsNullOrEmpty(std.PolicyVisible))
                {
                    cache ??= new();
                    if (!cache.TryGetValue(std.PolicyVisible, out visible))
                    {
                        visible = await IsInPolicyAsync(std.PolicyVisible);
                        cache.Add(std.PolicyVisible, visible);
                    }
                }
            }
            if (visible)
            {
                startupTabs.Add(tabData);
            }
        }
        return startupTabs;
    }

    /// <summary>
    /// Reload saved tabs after authentication
    /// </summary>
#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
    private async void TryReloadSavedTabsAsync()
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
    {
        try
        {
            _startupTabs.Clear();

            _startupTabs = TabDataList.Select(x => x.TabData).ToList();
            // Clear pre-loaded tabs
            foreach (LgTabData tabData in _startupTabs)
            {
                LoadingTabDataList.Remove(tabData.Key);
            }

            _tabInitialized = true;

            Dictionary<string, LgTabData> dicoStartupUri = new();
            foreach (LgTabData tabData in _startupTabs)
            {
                dicoStartupUri.TryAdd(tabData.Uri, tabData);
            }
            _startupTabs.Clear();
            IEnumerable<Tab> tabsDB = await ClientTabService.GetTabsAsync(App.BehaviorConfiguration.TabSavingMethod);
            if (tabsDB is not null)
            {
                foreach (Tab tab in tabsDB)
                {
                    if (dicoStartupUri.TryGetValue(tab.Uri, out LgTabData startupTab))
                    {
                        _startupTabs.Add(startupTab);
                        dicoStartupUri.Remove(tab.Uri);
                    }
                    else
                    {
                        // Add tab to startup page
                        AddStartupPage(false, tab.Uri, tab.Title, tab.IconName, true);
                    }
                }
            }
            int idx = _startupTabs.FindLastIndex(tab => tab.Closable);

            foreach (LgTabData tab in dicoStartupUri.Values.Reverse())
            {
                _startupTabs.Insert(idx < 0 ? 0 : idx, tab);
            }
            foreach (LgTabData tabData in _startupTabs)
            {
                RegisterTabWithUri(tabData, null);
            }
            OnRefreshHeader?.Invoke();
        }
        catch (Exception ex)
        {
            App.TraceException(ex);
        }
    }

    /// <summary>
    /// Reaload saved tabs 
    /// </summary>
    /// <returns></returns>
    private async Task ReloadSavedTabsAsync()
    {
        _tabInitialized = true;
        Dictionary<string, LgTabData> dicoStartupUri = new();
        foreach (LgTabData tabData in _startupTabs)
        {
            dicoStartupUri.TryAdd(tabData.Uri, tabData);
        }
        _startupTabs.Clear();
        IEnumerable<Tab> tabsDB = await ClientTabService.GetTabsAsync(App.BehaviorConfiguration.TabSavingMethod);
        if (tabsDB is not null)
        {
            foreach (Tab tab in tabsDB)
            {
                if (dicoStartupUri.TryGetValue(tab.Uri, out LgTabData startupTab))
                {
                    _startupTabs.Add(startupTab);
                    dicoStartupUri.Remove(tab.Uri);
                }
                else
                {
                    // Add tab to startup page
                    AddStartupPage(false, tab.Uri, tab.Title, tab.IconName, true);
                }
            }
        }
        int idx = _startupTabs.FindLastIndex(tab => tab.Closable);
        foreach (LgTabData tab in dicoStartupUri.Values.Reverse())
        {
            _startupTabs.Insert(idx < 0 ? 0 : idx, tab);
        }
    }

    /// <summary>
    /// Load the page from the route data.
    /// </summary>
    /// <param name="e">Informstions about the page to display.</param>
    private async Task NavigateToAsync(NavigateToEventArgs e)
    {
        await OpenInTabOrFullScreenAsync(e.Uri, e.RouteData);
    }

    /// <summary>
    /// Open tab by its URI and routeData.
    /// </summary>
    /// <param name="uri">URI of the page to open</param>
    /// <param name="routeData">Informations to display the page.</param>
    private async Task OpenInTabOrFullScreenAsync(string uri, RouteData routeData)
    {
        // Check if the page implements the IFullScreenPage interface
        if (typeof(IFullScreenPage).IsAssignableFrom(routeData.PageType))
        {
            _fullScreenPageContainer.Refresh(routeData);
        }
        else
        {
            _fullScreenPageContainer.Refresh(null);
            await OpenTabAsync(uri, routeData, true);
        }
    }

    ///<inheritdoc/>
    internal override async Task OpenTabAsync(string uri, RouteData routeData, bool closable = true)
    {
        // Initialize LgTabData
        LgTabData tabData = new()
        {
            Closable = closable,
            Draggable = true,
            Uri = uri
        };
        // Get a registred tab from the container
        LgTabRenderData tab = RegisterTabWithUri(tabData, routeData);

        // Save tab to bdd or local storage
        tab.OnRaiseMoveEventAsync += OnTabMoveAsync;

        // Check if tab found
        if (tab is not null)
        {
            await ActivateTabAsync(tab);
        }
    }

    ///<inheritdoc/>
    internal override Task GoToTabAsync(LgTabRenderData tab)
    {
        // Open or activate the tab
        App.NavigationManager.NavigateTo(tab.Uri);
        return Task.CompletedTask;
    }

    ///<inheritdoc/>
    protected override async Task<bool> OnPageSetTitleAsync(LgPage page)
    {
        // Change tab title
        bool found = await base.OnPageSetTitleAsync(page);
        // Change window title
        if (found)
        {
            await PageTitleHandler?.SetPageTitleAsync(page);
        }
        // Indicate if the tab has been found
        return found;
    }

    /// <summary>
    /// Load the list of the pages to open when application start.
    /// </summary>
    /// <returns>The list of pages to open when application start.</returns>
    private RenderFragment LoadStartupPages()
    {
        if (_startupTabs is null)
        {
            _startupTabs = new();
            return AppComponent.StartupTabs;
        }
        return null;
    }

    /// <summary>
    /// Register a page to always open when application start.
    /// </summary>
    /// <param name="active">Indicate that this page will be active when application start on the root URL.</param>
    /// <param name="uri">URI of the page.</param>
    /// <param name="title">Title of the tab.</param>
    /// <param name="iconName">Icon name of the tab.</param>
    /// <param name="closable">Indicate if the tab can be closed.</param>
    /// <param name="policyVisible">The name of the policy to show the tab.</param>
    internal LgStartupTabData AddStartupPage(bool active, string uri, string title, string iconName, bool closable, string policyVisible = null)
    {
        bool isFirst = _startupTabs.Count == 0;
        if (isFirst)
        {
            closable = false;
        }
        LgStartupTabData tabData = new()
        {
            Closable = closable,
            Draggable = closable,
            Uri = uri,
            Title = title.CheckTranslate(),
            IconName = iconName.CheckTranslate(),
            Key = isFirst ? TABHOMEKEY : null,
            PolicyVisible = policyVisible
        };
        _startupTabs.Add(tabData);
        if (active)
        {
            _defaultUri = uri;
        }
        return tabData;
    }

    /// <summary>
    /// On move tab event
    /// </summary>
    /// <returns></returns>
    private Task OnTabMoveAsync()
    {
        SaveTabsUriAsync();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Save opened tabs to database or indexeddb depending on app configuration
    /// </summary>
    /// <returns></returns>
#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
    internal async void SaveTabsUriAsync()
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
    {
        try
        {
            await ClientTabService.SaveTabsAsync(App.BehaviorConfiguration.TabSavingMethod, GetLoadedTabsAsModel());
        }
        catch (Exception ex)
        {
            App.TraceException(ex);
        }
    }

}

#endregion
