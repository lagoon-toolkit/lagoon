namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Render the top menu.
/// </summary>
public partial class LgMenuTopRender : LgMenuRender
{

    #region parameters

    /// <summary>
    /// Menu is collapsed ? (Used for mobile mode with hamburger menu mode)
    /// </summary>
    [Parameter]
    public bool Collapsed { get; set; }

    /// <summary>
    /// Gets or sets if menu item that overflow the menu content are moved to a sub menu.
    /// </summary>
    [Parameter]
    public bool DisableWrap { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// Create a new instance of <see cref="LgMenuTopRender"/>
    /// </summary>
    public LgMenuTopRender()
    {
        MenuPosition = MenuPosition.MenuTop;
    }

    #endregion

    #region methods

    /// <summary>
    /// Add the menu wrapper item.
    /// </summary>
    /// <returns>RenderFragment with the menu wrapper item.</returns>
    protected override RenderFragment GetAdditionnalContent()
    {
        return builder =>
            {
                if (!DisableWrap)
                {
                    builder.OpenComponent<LgMenuWrapper>(1);
                    builder.CloseComponent();
                }
            };
    }

    ///<inheritdoc/>
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (!DisableWrap)
        {
            JS.InvokeVoidAsync("Lagoon.wrapTopMenu");
        }
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        JS.InvokeVoid("Lagoon.wrapTopMenuDispose");
        base.Dispose(disposing);
    }

    /// <summary>
    /// Gets CSS classes for this component.
    /// </summary>
    /// <returns>CSS classes for this component.</returns>
    private string GetCssClassAttribute()
    {
        LgCssClassBuilder builder = new("navbar-collapse order-3 order-lg-2");
        builder.AddIf(Collapsed, "collapse");
        // Hide the overflow to avoid blinking when wrapping
        builder.AddIf(!DisableWrap, "overflow-hidden");
        return builder.ToString();
    }

    ///<inheritdoc/>
    internal override string GetNavItemCssClass(int level)
    {
        return level == 0 ? "dropdown" : "dropright";
    }

    #endregion

}
