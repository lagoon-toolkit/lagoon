namespace Lagoon.UI.Components;

/// <summary>
/// Title component.
/// </summary>
public partial class LgTitle : LgAriaComponentBase
{

    #region Public properties

    /// <summary>
    /// Gets or sets the label text
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the label css class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the title level (size).
    /// </summary>
    [Parameter]
    public TitleLevel TitleLevel { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("lblTitle", CssClass);
    }

    #endregion region

}
