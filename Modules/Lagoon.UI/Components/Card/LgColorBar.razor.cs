namespace Lagoon.UI.Components;

/// <summary>
/// LgColorBar (left border with or without icon) component for LgCard
/// </summary>
public partial class LgColorBar : LgComponentBase
{

    /// <summary>
    /// Gets or sets css class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the color for color bar
    /// </summary>
    [Parameter]
    public string Color { get; set; }

    /// <summary>
    /// Gets or sets the button icon name
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary> 
    /// Gets or sets the tooltip. 
    /// </summary> 
    [Parameter]
    public string Tooltip { get; set; }

    /// <summary>
    /// Gets or sets if the tooltip contents use the HTML format; else, the content use a raw text format.
    /// </summary>
    [Parameter]
    public bool TooltipIsHtml { get; set; }


    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add(string.IsNullOrEmpty(IconName) ? "color-bar" : "color-bar-icon", CssClass);
    }

    private string GetStyleAttribute()
    {
        if (string.IsNullOrEmpty(Color)) return null;
        return "background-color:" + Color;
    }

    #endregion region

}
