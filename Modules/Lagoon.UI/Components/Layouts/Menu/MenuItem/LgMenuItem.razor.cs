using Lagoon.UI.Components.Layouts.Internal;

namespace Lagoon.UI.Components;

/// <summary>
/// Render menu item.
/// </summary>
public partial class LgMenuItem : LgCustomMenuItem, IActionComponent
{

    #region cascading parameters

    /// <summary>
    /// Header component.
    /// </summary>
    [CascadingParameter]
    private LgHeader Header { get; set; }

    /// <summary>
    /// Menu position (MenuTop, MenuToolbar, MenuSidebar, MenuAppTabContainer)
    /// </summary>
    [CascadingParameter]
    public LgMenuRender MenuRender { get; set; }

    /// <summary>
    /// The parent command listener.
    /// </summary>
    [CascadingParameter]
    private ICommandListener ParentCommandListener { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// Get or set the sub menu items inside this menu item. 
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Get or set the custom render for this item.
    /// </summary>
    [Parameter]
    public RenderFragment CustomContent { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets label for accessibility
    /// </summary>
    /// <value>label</value>
    [Parameter]
    public string AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the auto hide property (hide dropdown when no subcontent displayed)
    /// </summary>
    [Parameter]
    public bool AutoHide { get; set; }

    /// <summary>
    /// The argument value associated to the command.
    /// </summary>
    /// <remarks>Use primitive types or string to optimise the render performances.</remarks>
    [Parameter]
    public object CommandArgument { get; set; }

    /// <summary>
    /// The name of the command to send to the handler of commands.
    /// </summary>
    [Parameter]
    public string CommandName { get; set; }

    /// <summary>
    /// Gets or sets the confirmation message to show before doing the action.
    /// Warning : Don't work with the CommandName parameter.
    /// </summary>
    [Parameter]
    public string ConfirmationMessage { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for the menu item.
    /// </summary>
    /// <value>The CSS class for the menu item.</value>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets if the menu item render a separator before himself.
    /// </summary>
    [Parameter]
    public bool HasSeparator { get; set; }

    /// <summary>
    /// Gets or sets if the drown down have an arrow icon.
    /// </summary>
    [Parameter]
    public bool? HideDropDownArrow { get; set; }

    /// <summary>
    /// By default, the text display depends of the menu container : The text is not displayed in toolbar by default.
    /// </summary>
    [Parameter]
    public bool? HideRootLevelText { get; set; }

    /// <summary>
    /// Name of the icon.
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Direct link
    /// </summary>
    [Parameter]
    public LgPageLink Link { get; set; }

    /// <summary>
    /// Get or set the action executer when user click on the menu
    /// </summary>
    [Parameter]
    public EventCallback<ActionEventArgs> OnClick { get; set; }

    /// <summary>
    /// By default, the tooltip display depends of the menu container.
    /// </summary>
    [Parameter]
    public bool? ShowTextAsTooltip { get; set; }

    /// <summary>
    /// By default, the text display depends of the menu container : The text is not displayed in toolbar by default.
    /// (Obsolete : Use HideRootLevelText)
    /// </summary>
    [Parameter]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the HideRootLevelText property.")]
    public bool? ShowText { get => !HideRootLevelText; set => HideRootLevelText = !value; }

    /// <summary>
    /// By default, the text display depends of the menu container : The text is not displayed in toolbar by default.
    /// (Obsolete : Use HideRootLevelText)
    /// </summary>
    [Parameter]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the HideRootLevelText property.")]
    public bool? ShowLabel { get => !HideRootLevelText; set => HideRootLevelText = !value; }

    /// <summary>
    /// Gets or sets if the menu have a vertical scroll bar when the list of sub-items is too large.
    /// WARNING ! Scrollable item list can't have sub-menus. 
    /// </summary>
    [Parameter]
    public bool Scrollable { get; set; }

    /// <summary>
    /// Get or set the tag of the menu
    /// </summary>
    [Parameter]
    public string Tag { get; set; }

    /// <summary>
    /// The name of the browser window in which the URI should be opened.
    /// </summary>
    [Parameter]
    public string Target { get; set; }

    /// <summary>
    /// Get or set the text of the menu
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// The URI to open.
    /// </summary>
    [Parameter]
    public string Uri { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Icon name to use.
    /// </summary>
    protected string IconNameRendering { get; set; }

    /// <summary>
    /// The parent command listener.
    /// </summary>
    ICommandListener IActionComponent.ParentCommandListener => ParentCommandListener;

    /// <summary>
    /// Text to display.
    /// </summary>
    protected string TagRendering { get; set; }

    /// <summary>
    /// Text to display.
    /// </summary>
    protected string TextRendering { get; set; }

    /// <summary>
    /// Uri to use.
    /// </summary>
    protected string UriRendering { get; set; }

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    protected ElementReference ElementRef;

    #endregion

    #region initialization

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (AutoHide)
        {
            App.JS.InvokeVoid("Lagoon.autoHideDropdownMenuItem", ElementRef);
        }
    }
    
    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (AutoHide)
        {
            App.JS.InvokeVoid("Lagoon.autoHideDropdownMenuItemDispose", ElementRef);
        }
        base.Dispose(disposing);
    }

    #endregion

    #region methods

    /// <summary>
    /// Handle click on menu item.
    /// </summary>
    private Task MenuItemClickAsync()
    {
        // Toggle nav menu from a menu item (for mobile / tablet render)
        if (Header is not null && !Header.CollapsedNavMenu)
        {
            Header.ToggleNavMenu();
        }
        // Raise the click event
        return ExecuteActionAsync(this);
    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        TagRendering = Tag.CheckTranslate();
        TextRendering = (Text ?? Link?.Title).CheckTranslate();
        IconNameRendering = (IconName ?? Link?.IconName).CheckTranslate();
        UriRendering = Uri ?? Link?.Uri;
    }

    /// <summary>
    /// Check if the label must be shown.
    /// </summary>
    /// <returns><c>true</c> if the label must be shown.</returns>
    private bool IsRootLevelTextHidden()
    {
        if (HideRootLevelText.HasValue)
        {
            // Set the visibility defined on the menu item
            return HideRootLevelText.Value;
        }
        else if (Level == 0)
        {
            // Set the visibility defined on the menu
            return MenuRender?.HideRootLevelText ?? false;
        }
        else
        {
            // Always visible
            return false;
        }
    }

    /// <summary>
    /// Check the dropdown arrow must be show
    /// </summary>
    /// <returns><c>true</c> if the dropdown arrow must be showtooltip must be hidden.</returns>
    private bool IsDropDownArrowVisible()
    {
        if (HideDropDownArrow.HasValue)
        {
            // Set the visibility defined on the menu item
            return !HideDropDownArrow.Value;
        }
        else
        {
            // Set the visibility defined on the menu
            return !(MenuRender?.HideDropDownArrow ?? false);
        }
    }

    /// <summary>
    /// Check if the tooltip must be hidden.
    /// </summary>
    /// <returns><c>true</c> if the tooltip must be hidden.</returns>
    private bool IsTextAsTooltipVisible()
    {
        if (ShowTextAsTooltip.HasValue)
        {
            // Set the visibility defined on the menu item
            return ShowTextAsTooltip.Value;
        }
        else
        {
            // Set the visibility defined on the menu
            return MenuRender.GetIsTooltipAsText(Level);
        }
    }

    /// <summary>
    /// Get CSS classes for the drop down menu.
    /// </summary>
    /// <returns>CSS classes.</returns>
    private string GetDropDownMenuCssClassAttribute()
    {
        LgCssClassBuilder builder = new(MenuRender?.GetDropDownMenuCssClass(Level));
        // Add the scrollable class on first level only if they is no sub level (Else sub dropdown is cropped)
        builder.AddIf(Scrollable, "scrollable");
        return builder.ToString();
    }

    /// <summary>
    /// Get CSS classes for the nav item.
    /// </summary>
    /// <param name="dropdown">Indicate if the item have a drop down menu.</param>
    /// <returns>CSS classes.</returns>
    private string GetNavItemCssClass(bool dropdown)
    {
        LgCssClassBuilder builder = new("nav-item", CssClass);
        bool hideRootLevelText = Level == 0 && IsRootLevelTextHidden();
        builder.AddIf(hideRootLevelText, "hrlt");
        if (CustomContent is null)
        {
            builder.AddIf(hideRootLevelText || string.IsNullOrEmpty(TextRendering), "noText");
        }
        if (dropdown)
        {
            builder.Add(MenuRender?.GetNavItemCssClass(Level));
            builder.AddIf(Level == 1, "dropdown-submenu");
        }
        return builder.ToString();
    }

    private string GetNavLinkCssClass()
    {
        LgCssClassBuilder builder = new("nav-link d-flex");
        builder.AddIf(Level > 0, "dropdown-item");
        return builder.ToString();
    }

    /// <summary>
    /// Define css class on navlink having submenu
    /// </summary>
    /// <returns></returns>
    private string GetDropdownToggleCssClass()
    {
        LgCssClassBuilder builder = new("nav-link d-flex");
        builder.AddIf(IsDropDownArrowVisible(), " dropdown-toggle");
        return builder.ToString();
    }

    #endregion

}
