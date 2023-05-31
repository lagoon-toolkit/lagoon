namespace Lagoon.UI.Components;

/// <summary>
/// Component to group actions.
/// </summary>
public partial class LgActionPanel : LgAriaComponentBase
{
    #region private fields

    /// <summary>
    /// Toolbar button size definition from parameter or application configuration
    /// </summary>
    private ButtonSize _buttonSize;

    #endregion

    #region public properties
    /// <summary>
    /// Gets or sets css class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the action panel left content
    /// </summary>
    [Parameter]
    public RenderFragment ActionContent { get; set; }

    /// <summary>
    /// Gets or sets the action panel Toolbar definition
    /// </summary>
    [Parameter]
    public RenderFragment Toolbar { get; set; }

    /// <summary>
    /// Toolbar button size
    /// </summary>
    [Parameter]
    public ButtonSize? ToolbarButtonSize { get; set; }
    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // Default toolbar button size from configuration
        if (ToolbarButtonSize == null)
            _buttonSize = App.BehaviorConfiguration.ActionPanelToolbarButtonSize;
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("d-flex action-pnl", CssClass);
    }

    #endregion region

}
