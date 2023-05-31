namespace Lagoon.UI.Components;


/// <summary>
/// Component used by the layout to configure application menu
/// </summary>
public partial class LgMenuConfiguration : LgComponentBase
{

    #region dependencies injection

    /// <summary>
    /// Service allowing communication with the menu.
    /// Two purposes:
    /// - Drive the menu rendering component
    /// - Refresh explicitly called by the developper
    /// </summary>
    [Inject]
    private MenuService MenuService { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// Main node for adding <see cref="LgMenuSet" />
    /// </summary>
    [Parameter]
    public RenderFragment MenuSetDeclarations { get; set; }

    /// <summary>
    /// Get or set the menu items for the main menu.
    /// </summary>
    [Parameter]
    public RenderFragment Menu { get; set; }

    /// <summary>
    /// Get or set the menu items for the side menu.
    /// </summary>
    [Parameter]
    public RenderFragment Sidebar { get; set; }

    /// <summary>
    /// Get or set the menu items for the main toolbar.
    /// </summary>
    [Parameter]
    public RenderFragment Toolbar { get; set; }

    /// <summary>
    /// Get or set the menu items for the application tab container.
    /// </summary>
    [Parameter]
    public RenderFragment TabContainerToolbar { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// The complete list of <see cref="LgCustomMenuSet" /> inside <see cref="MenuSetDeclarations"/>
    /// </summary>
    private readonly Dictionary<string, LgCustomMenuSet> _menuSets = new();

    #endregion

    #region initialization

    /// <summary>
    /// When initializing, subcribe to explict menu refresh event
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        MenuService.IsLoading = true;
        MenuService.RegisterMenuConfiguration(this);
        MenuService.OnRefresh += RefreshMenu;
    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        MenuService.IsLoading = true;
    }

    #endregion

    #region dispose

    /// <summary>
    /// Unsubscribe event listenner
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        MenuService.OnRefresh -= RefreshMenu;
        MenuService.UnregisterMenuConfiguration(this);
        base.Dispose(disposing);
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override bool ShouldRender()
    {
        //We don't need a second pass, menuset registrations are done in OnInitialize events.
        return MenuService.IsLoading;
    }

    /// <summary>
    /// When the <see cref="LgMenuConfiguration"/> is fully loaded : Raise an event to ask the rendered.
    /// </summary>
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        MenuService.IsLoading = false;
        // Notify that the menu content have been reevaluated
        MenuService.MenuDataChanged();
    }

    /// <summary>
    /// Fired by <see cref="MenuService" /> to update menu explicitly
    /// </summary>
    internal void RefreshMenu(MenuPosition menuPositionCombinaison)
    {
        // To refresh menu config
        StateHasChanged();
    }

    #endregion

    #region internal methods used by children component

    /// <summary>
    /// Used by children to declare itself to the parent element
    /// </summary>
    internal void RegisterMenuSet(LgCustomMenuSet menuSet)
    {
        if (!_menuSets.TryAdd(menuSet.Id, menuSet))
        {
            throw new InvalidOperationException($"Duplicate LgCustomMenuSet's Id: {menuSet.Id}");
        }
    }

    /// <summary>
    /// Used by children to declare itself to the parent element
    /// </summary>
    internal void UnregisterMenuSet(LgCustomMenuSet menuSet)
    {

        if (_menuSets.TryGetValue(menuSet.Id, out LgCustomMenuSet oldMenuSet))
        {
            if (menuSet == oldMenuSet) _menuSets.Remove(menuSet.Id);
        }
    }

    /// <summary>
    /// Try to return the menu set with the specified id.
    /// </summary>
    /// <param name="menuSetId">Id of the menu set.</param>
    /// <returns>The menu set with the specified id.</returns>
    internal LgCustomMenuSet TryGetMenuSet(string menuSetId)
    {
        if (_menuSets.TryGetValue(menuSetId, out LgCustomMenuSet menuSet))
        {
            return menuSet;
        }
        else
        {
            return null;
        }
    }

    #endregion       

}
