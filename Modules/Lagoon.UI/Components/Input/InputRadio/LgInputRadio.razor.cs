namespace Lagoon.UI.Components;

/// <summary>
/// Input radio component.
/// </summary>
/// <typeparam name="TValue">Values type.</typeparam>
public partial class LgInputRadio<TValue> : LgAriaComponentBase
{

    #region Public properties

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the input element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the value of this input.
    /// </summary>
    [Parameter]
    public TValue Value { get; set; }

    /// <summary>
    /// Gets or sets the name of the parent input radio group.
    /// </summary>
    [Parameter] 
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the radio button label
    /// </summary>
    [Parameter]
    public string Text { get; set; } = "";

    /// <summary>
    /// Gets or sets the input disabled attribute
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the radio button Class css
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

    /// <summary>
    /// Gets the DOM element id.
    /// </summary>
    protected string ElementId { get; } = GetNewElementId();

    /// <summary>
    ///  Gets or sets the radio button label content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region Private properties

    /// <summary>
    /// Gets context for this <see cref="LgInputRadio{TValue}"/>.
    /// </summary>
    internal LgInputRadioContext Context { get; private set; }

    private string _cssDisplayOrientation;

    private string CssDisplayKind;

    [CascadingParameter]
    private LgInputRadioContext CascadedContext { get; set; }

    #endregion

    #region methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        Context = string.IsNullOrEmpty(Name) ? CascadedContext : CascadedContext?.FindContextInAncestors(Name);

        if (Context == null)
        {
            throw new InvalidOperationException($"{GetType()} must have an ancestor {typeof(LgInputRadioGroup<TValue>)} " +
                $"with a matching 'Name' property, if specified.");
        }

        _cssDisplayOrientation = Context.DisplayOrientation.Equals(DisplayOrientation.Vertical) ? "d-block" : "d-inline-block";

        if (Context.Disabled)
        {
            Disabled = true;
        }

        CssDisplayKind = Context.DisplayKind == RadioButtonDisplayKind.Classic ? "rbtClassic" : "rbtModern";

    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("custom-control custom-radio", _cssDisplayOrientation, CssClass);
    }

    /// <summary>
    /// Get the list of additional attributes to add to component.
    /// </summary>
    /// <returns>The list of additional attributes to add to component.</returns>
    protected IEnumerable<KeyValuePair<string, object>> GetAdditionalAttributes()
    {
        return GetAdditionalAttributes(AdditionalAttributes, GetTooltipAttributes(Tooltip, TooltipIsHtml));
    }

    #endregion

}