namespace Lagoon.UI.Components;

/// <summary>
/// Menu set including a <see cref="LgSearchMenuItem"/>
/// </summary>
public partial class LgSearchMenuSet : ComponentBase
{
    #region parameters

    /// <summary>
    /// Id of this toolbar instance.
    /// </summary>
    [Parameter]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the items.
    /// </summary>
    [Parameter]
    public LgGlobalSearchItemCollection Items { get; set; }
    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the CSS classes.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the input placeholder
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the items.
    /// </summary>
    [Parameter]
    public string SearchUri { get; set; }

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

}
