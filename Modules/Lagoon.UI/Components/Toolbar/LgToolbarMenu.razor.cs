namespace Lagoon.UI.Components;

/// <summary>
/// Toolbar component.
/// </summary>
public partial class LgToolbarMenu : LgAriaComponentBase
{

    #region public properties & variables

    /// <summary>
    /// Gets or sets the CSS classes.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the menu button kind
    /// </summary>
    [Parameter]
    public ButtonKind Kind { get; set; } = ButtonKind.Secondary;

    /// <summary>
    /// Gets or sets the button label
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the button icon name
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Gets or sets the child content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region private properties

    /// <summary>
    /// Gets or sets parent toolbar object
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    private LgToolbar Toolbar { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("btn-group", CssClass);
    }

    #endregion region

}
