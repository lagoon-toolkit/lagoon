namespace Lagoon.UI.Components;

/// <summary>
/// Menu provider service.
/// </summary>
public class MenuService
{

    #region fields

    /// <summary>
    /// A unique key for each MenuDataChanged event.
    /// </summary>
    private string _menuRenderKey;

    /// <summary>
    /// Menu configuration
    /// </summary>
    private LgMenuConfiguration _menuConfiguration;

    #endregion

    #region dependencies injections

    /// <summary>
    /// Logger for menu service.
    /// </summary>
    private ILogger<MenuService> Logger { get; }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets if the menu configuration is loading.
    /// </summary>
    public bool IsLoading { get; set; }

    #endregion

    #region events

    /// <summary>
    /// Even fired when the <see cref="LgMenuConfiguration"/> is loaded or explicitly refreshed.
    /// </summary>
    internal event Action OnMenuDataChange;

    /// <summary>
    /// Event used to explicitly refresh the menu 
    /// </summary>
    internal event Action<MenuPosition> OnRefresh;

    #endregion

    #region constructors

    /// <summary>
    /// Create a new <see cref="MenuService" /> instance.
    /// </summary>
    public MenuService(ILogger<MenuService> logger)
    {
        Logger = logger;
    }

    #endregion

    #region methods

    /// <summary>
    /// Associated to the OnMenuDataChange event, new menu data available
    /// </summary>
    internal void MenuDataChanged()
    {
        _menuRenderKey = Guid.NewGuid().ToString("N");
        Logger.LogDebug($"MenuDataChanged {_menuRenderKey}");
        OnMenuDataChange?.Invoke();
    }

    /// <summary>
    /// Can be called to refresh all existing menus.
    /// </summary>
    public void RefreshAll()
    {
        Logger.LogDebug($"RefreshAll");
        // Refresh all menus
        Refresh(~MenuPosition.None);
    }

    /// <summary>
    /// Can be called to refresh a combination of menu.
    /// </summary>
    /// <param name="menuPositionCombinaison"></param>
    public void Refresh(MenuPosition menuPositionCombinaison)
    {
        Logger.LogDebug($"Refresh : {menuPositionCombinaison}");
        OnRefresh?.Invoke(menuPositionCombinaison);
    }

    /// <summary>
    /// Remove the reference to the menu configuration.
    /// </summary>
    /// <param name="menuConfiguration">Menu configuration.</param>
    internal void RegisterMenuConfiguration(LgMenuConfiguration menuConfiguration)
    {
        _menuConfiguration = menuConfiguration;
    }

    /// <summary>
    /// Keep a reference to the menu configuration.
    /// </summary>
    /// <param name="menuConfiguration">Menu configuration.</param>
    internal void UnregisterMenuConfiguration(LgMenuConfiguration menuConfiguration)
    {
        if (_menuConfiguration == menuConfiguration)
        {
            _menuConfiguration = null;
        }
    }

    /// <summary>
    /// Return a unique key for each MenuDataChanged event.
    /// (To force refresh when a state as change is done on the menu configuration)
    /// </summary>
    /// <param name="menuPosition">Menu ID.</param>
    /// <returns>A unique key for each MenuDataChanged event.</returns>
    internal string RenderKey(MenuPosition menuPosition)
    {
        return _menuRenderKey + ((int)menuPosition).ToString();
    }

    /// <summary>
    /// Indicate if the menu content is defined.
    /// </summary>
    /// <param name="menuPosition">The menu to retreive.</param>
    internal bool IsMenuDefined(MenuPosition menuPosition)
    {
        return _menuConfiguration is not null && GetMenuContent(menuPosition) is not null;
    }

    /// <summary>
    /// Return the render fragment for the menu.
    /// </summary>
    /// <param name="menuPosition">The menu to retreive.</param>
    internal RenderFragment GetMenuContent(MenuPosition menuPosition)
    {
        Logger.LogDebug($"RenderMenu {menuPosition}, IsLoading: {IsLoading}");
        return menuPosition switch
        {
            MenuPosition.MenuTop => _menuConfiguration.Menu,
            MenuPosition.MenuSidebar => _menuConfiguration.Sidebar,
            MenuPosition.MenuToolbar => _menuConfiguration.Toolbar,
            MenuPosition.MenuToolbarTabContainer => _menuConfiguration.TabContainerToolbar,
            _ => throw new InvalidOperationException($"Unknown MenuPosition : {menuPosition}")
        };
    }

    /// <summary>
    /// Return the render fragment for the menuset.
    /// </summary>
    /// <param name="menuSetId">ID of the menu set.</param>
    internal RenderFragment RenderMenuSet(string menuSetId)
    {
        LgCustomMenuSet menuSet = _menuConfiguration.TryGetMenuSet(menuSetId);
        if (menuSet is null)
        {
            Logger.LogWarning($"RenderMenuSet : Searched Id \"{menuSetId}\": NOT FOUND");
            return null;
        }
        else
        {
            Logger.LogDebug($"RenderMenuSet : {menuSet.Id}");
            return menuSet.GetContent();
        }
    }

    #endregion

}
