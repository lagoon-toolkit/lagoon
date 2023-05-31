namespace Lagoon.UI.Components;

/// <summary>
/// Component to show icon from library.
/// </summary>
public partial class LgIcon : LgAriaComponentBase
{
    #region fields

    private bool _isLocalized;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the icon Onclick eventCallback
    /// </summary>
    [Parameter]
    public EventCallback<ActionEventArgs> OnClick { get; set; }

    /// <summary>
    /// Gets or sets the icon name
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Gets or sets the icon class css
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the icon tooltip.
    /// </summary>
    [Parameter]
    public string Tooltip { get; set; }

    /// <summary>
    /// Gets or sets if the tooltip contents use the HTML format; else, the content use a raw text format.
    /// </summary>
    [Parameter]
    public bool TooltipIsHtml { get; set; }


    #endregion

    #region properties

    /// <summary>
    /// Gets or sets the icon name to be rendered.
    /// </summary>
    protected string IconNameRendering { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _isLocalized = IconName.TranslationNeeded();
        IconNameRendering = _isLocalized ? IconName.CheckTranslate() : IconName;
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("bi", IconNameRendering, CssClass);
    }

    /// <summary>
    /// Get the list of additional attributes to add to component.
    /// </summary>
    /// <returns>The list of additional attributes to add to component.</returns>
    protected IEnumerable<KeyValuePair<string, object>> GetAdditionalAttributes()
    {
        return GetAdditionalAttributes(AdditionalAttributes, GetTooltipAttributes(Tooltip, TooltipIsHtml));
    }

    /// <summary>
    /// Gets the SVG content.
    /// </summary>
    private MarkupString GetSvgUse()
    {
        return GetSvgUse(IconNameRendering);
    }

    /// <summary>
    /// Return then "use" part to include into SVG node.
    /// </summary>
    /// <param name="iconName"></param>
    /// <returns></returns>
    public static MarkupString GetSvgUse(string iconName)
    {
        return new MarkupString("<use xlink:href=\"#" + iconName + "\"/>");
    }

    /// <summary>
    /// Handler the icon click.
    /// </summary>
    /// <returns></returns>
    private async Task OnClickInternalAsync()
    {
        if (OnClick.HasDelegate)
        {
            await ExecuteActionAsync(OnClick);
        }
    }

    #endregion

}
