namespace Lagoon.UI.Components;

/// <summary>
/// Toolbar menu item separators.
/// </summary>
public partial class LgToolbarMenuItemSeparator : LgComponentBase
{
    #region public properties

    /// <summary>
    /// Gets or sets the CSS classes.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("dropdown-divider", CssClass);
    }

    #endregion region
}
