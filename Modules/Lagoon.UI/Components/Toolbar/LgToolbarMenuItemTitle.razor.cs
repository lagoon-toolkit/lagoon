namespace Lagoon.UI.Components;

/// <summary>
/// Toolbar menu items.
/// </summary>
public partial class LgToolbarMenuItemTitle : LgComponentBase
{
    #region public properties

    /// <summary>
    /// Gets or sets the CSS classes.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the toolabr menu item title object
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("dropdown-header", CssClass);
    }

    #endregion region

}
