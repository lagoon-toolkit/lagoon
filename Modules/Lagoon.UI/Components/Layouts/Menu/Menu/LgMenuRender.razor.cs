namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Component to deport the menu rendering.
/// </summary>
public partial class LgMenuRender : LgComponentBase
{

    #region fields

    /// <summary>
    /// Content of the menu.
    /// </summary>
    private RenderFragment _content;

    /// <summary>
    /// Maximum level supported by the menu.
    /// </summary>
    private readonly int _maxSupportedLevel;

    /// <summary>
    /// Indicate if the menu contains too much level for sub items.
    /// </summary>
    private bool _hasUnsupportedLevel;

    #endregion

    #region dependencies injection

    /// <summary>
    /// Menu service.
    /// </summary>
    [Inject]
    public MenuService MenuService { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Menu position (MenuTop, MenuToolbar, MenuSidebar, MenuAppTabContainer)
    /// </summary>
    [Parameter]
    public MenuPosition MenuPosition { get; set; }

    /// <summary>
    /// Gets or sets if the label are hidden.
    /// </summary>
    [Parameter]
    public bool HideRootLevelText { get; set; }

    /// <summary>
    /// Gets or sets if the dropdown menu arrow are hidden.
    /// </summary>
    [Parameter]
    public bool HideDropDownArrow { get; set; }

    /// <summary>
    /// Gets or sets if the menu items separators are hidden.
    /// </summary>
    [Parameter]
    public bool HideSeparatorItems { get; set; }

    /// <summary>
    /// Gets or sets if the title menu items are hidden.
    /// </summary>
    [Parameter]
    public bool HideTitleItems { get; set; }

    /// <summary>
    /// Gets or sets if the menu item text is shown in tooltip.
    /// </summary>
    [Parameter]
    public bool ShowTextAsTooltip { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    public LgMenuRender()
    {
        _maxSupportedLevel = 2;
    }

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    public LgMenuRender(int maxSupportedLevel)
    {
        _maxSupportedLevel = maxSupportedLevel;
    }

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        MenuService.OnMenuDataChange += OnMenuDataChange;
    }

    /// <summary>
    /// Refresh the menu render.
    /// </summary>
    private void OnMenuDataChange()
    {
        _content = MenuService.GetMenuContent(MenuPosition);
        StateHasChanged();
    }

    /// <summary>
    /// Get the dropdown menu additional CSS class.
    /// </summary>
    /// <param name="level">Level of the dropdown menu item.</param>
    /// <returns>The dropdown menu additional CSS class.</returns>
    internal virtual string GetDropDownMenuCssClass(int level)
    {
        return "dropdown-menu";
    }

    internal bool IsSupportedLevel(int level)
    {
        bool isSupported = level < _maxSupportedLevel;
        if (!isSupported && !_hasUnsupportedLevel) {
            _hasUnsupportedLevel = true;
            ShowError("menuErrorMaxLevelReached".Translate(_maxSupportedLevel + 1), MenuPosition.ToString());
        }
        return isSupported;
    }

    /// <summary>
    /// Get the dropdown menu item CSS class.
    /// </summary>
    /// <param name="level">Level of the dropdown menu item.</param>
    /// <returns>The dropdown menu item CSS class.</returns>
    internal virtual string GetNavItemCssClass(int level)
    {
        return "dropdown";
    }

    /// <summary>
    /// Return an additional content specified by an overriding class.
    /// </summary>
    /// <returns>An additional content specified by an overriding class.</returns>
    protected virtual RenderFragment GetAdditionnalContent()
    {
        return null;
    }

    /// <summary>
    /// Return the tooltip and text visibility
    /// </summary>
    /// <param name="level">Menu level</param>
    internal virtual bool GetIsTooltipAsText(int level)
    {
        return ShowTextAsTooltip;
    }

    #endregion

}
