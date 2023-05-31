namespace Lagoon.UI.Components;

/// <summary>
/// Label with action component.
/// </summary>
public partial class LgLabelAction : LgComponentBase
{
    /// <summary>
    /// Gets or sets the value of this input.
    /// </summary>
    [Parameter]
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the label
    /// </summary>
    [Parameter]
    public string Label { get; set; } = "";

    /// <summary>
    /// Gets or sets the component css class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the component Tooltip
    /// </summary>
    [Parameter]
    public string Tooltip { get; set; }

    /// <summary>
    /// Gets or sets if the tooltip contents use the HTML format; else, the content use a raw text format.
    /// </summary>
    [Parameter]
    public bool TooltipIsHtml { get; set; }

    /// <summary>
    /// Gets or sets the kind of the button
    /// </summary>
    [Parameter]
    public ButtonKind ButtonKind { get; set; } = ButtonKind.Primary;

    /// <summary>
    /// Gets or sets the button label
    /// </summary>
    [Parameter]
    public string ButtonText { get; set; } = "";

    /// <summary>
    /// Gets or sets the button icon name
    /// </summary>
    [Parameter]
    public string ButtonIconName { get; set; }

    /// <summary>
    /// Gets or sets input text mode.
    /// </summary>
    [Parameter]
    public bool MultiLine { get; set; }

    /// <summary>
    /// Gets or sets the input disabled attribute
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the input readonly attribute
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the button Onclick eventCallback
    /// </summary>
    [Parameter]
    public EventCallback<ActionEventArgs> OnClick { get; set; }

    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("labelCaption", CssClass);
    }

    #endregion region
}
