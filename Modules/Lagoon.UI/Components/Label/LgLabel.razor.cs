namespace Lagoon.UI.Components;

/// <summary>
/// Label component.
/// </summary>
public partial class LgLabel : LgComponentBase
{

    #region Public properties

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the child content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the label text
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the label text
    /// </summary>
    [Parameter]
    public string For { get; set; }

    /// <summary>
    /// Gets or sets the label css class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

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

    #endregion

    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("lbl", CssClass);
    }

    /// <summary>
    /// Get the list of additional attributes to add to component.
    /// </summary>
    /// <returns>The list of additional attributes to add to component.</returns>
    protected IEnumerable<KeyValuePair<string, object>> GetAdditionalAttributes()
    {
        return GetAdditionalAttributes(AdditionalAttributes, GetTooltipAttributes(Tooltip, TooltipIsHtml));
    }

    #endregion region
}
