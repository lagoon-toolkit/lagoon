namespace Lagoon.UI.Components;

/// <summary>
/// Breadcrumb trail item.
/// </summary>
public partial class LgBreadcrumbTrailItem : LgAriaComponentBase
{

    #region parameters

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the button Onclick eventCallback
    /// </summary>
    [Parameter]
    public EventCallback<ActionEventArgs> OnClick { get; set; }

    /// <summary>
    /// Gets or sets the confirmation message to show before doing the action.
    /// </summary>
    [Parameter]
    public string ConfirmationMessage { get; set; }

    /// <summary>
    /// Gets or sets the CSS class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the button icon name
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Gets or sets the BreadcrumbTrailItem object is active.
    /// </summary>
    [Parameter]
    public bool IsActive { get; set; }

    /// <summary>
    /// Informations to create link to specific page.
    /// </summary>
    [Parameter]
    public LgPageLink Link { get; set; }

    /// <summary>
    /// Gets or sets the target window name to navigate to.
    /// </summary>
    [Parameter]
    public string Target { get; set; }

    /// <summary>
    /// Gets or sets the button label
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// The page URI to open when button is clicked.
    /// </summary>
    [Parameter]
    public string Uri { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Text to display.
    /// </summary>
    protected string TextRendering { get; set; }

    /// <summary>
    /// Icon name to use.
    /// </summary>
    protected string IconNameRendering { get; set; }

    /// <summary>
    /// URI to use.
    /// </summary>
    protected string UriRendering { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        TextRendering = (Text ?? Link?.Title).CheckTranslate();
        IconNameRendering = (IconName ?? Link?.IconName).CheckTranslate();
        UriRendering = Uri ?? Link?.Uri ?? string.Empty;
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add(CssClass);
        builder.Add("breadcrumb-item");
        builder.AddIf(IsActive, "active");
    }

    #endregion region

}
