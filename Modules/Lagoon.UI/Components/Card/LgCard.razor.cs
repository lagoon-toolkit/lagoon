namespace Lagoon.UI.Components;

/// <summary>
/// LgCard component
/// </summary>
public partial class LgCard : LgComponentBase, IActionComponent
{

    #region cascading parameters

    /// <summary>
    /// The parent command listener.
    /// </summary>
    [CascadingParameter]
    private ICommandListener ParentCommandListener { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets active card state
    /// </summary>
    [Parameter]
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets child card content 
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// The argument value associated to the command.
    /// </summary>
    /// <remarks>Use primitive types or string to optimise the render performances.</remarks>
    [Parameter]
    public object CommandArgument { get; set; }

    /// <summary>
    /// The name of the command to send to the handler of commands.
    /// </summary>
    [Parameter]
    public string CommandName { get; set; }

    /// <summary>
    /// Gets or sets the confirmation message to show before doing the action.
    /// Warning : Don't work with the CommandName parameter.
    /// </summary>
    [Parameter]
    public string ConfirmationMessage { get; set; }

    /// <summary>
    /// Gets or sets card css class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Informations to create link to specific page.
    /// </summary>
    [Parameter]
    public LgPageLink Link { get; set; }

    /// <summary>
    /// Gets or sets the card Onclick eventCallback
    /// </summary>
    [Parameter]
    public EventCallback<ActionEventArgs> OnClick { get; set; }

    /// <summary>
    /// Gets or sets the card stop propagation
    /// </summary>
    [Parameter]
    public bool StopPropagation { get; set; }

    /// <summary>
    /// The name of the browser window in which the URI should be opened.
    /// </summary>
    [Parameter]
    public string Target { get; set; }

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
    /// Gets or sets the card uri navigation
    /// </summary>
    [Parameter]
    public string Uri { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    protected ElementReference ElementContentRef;

    #endregion

    #region ILgActionComponent implementation

    ICommandListener IActionComponent.ParentCommandListener => ParentCommandListener;

    #endregion

    #region methods

    /// <summary>
    /// Remark ElementReference will be available only after OnAfterRender/OnAfterRenderAsync
    /// </summary>
    /// <param name="firstRender">Is it the first render for this component ?</param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && ChildContent != null)
        {
            await JS.InvokeAsync<object>("Lagoon.JsUtils.InitClickControl", ElementContentRef);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.AddIf(((IActionComponent)this).HasAction(), "card-clickable");
        builder.AddIf(Active, "active");
        builder.Add("card", CssClass);
    }

    /// <summary>
    /// Get the list of additional attributes to add to component.
    /// </summary>
    /// <returns>The list of additional attributes to add to component.</returns>
    protected IEnumerable<KeyValuePair<string, object>> GetAdditionalAttributes()
    {
        return GetAdditionalAttributes(GetTooltipAttributes(Tooltip, TooltipIsHtml));
    }

    #endregion 

    #region Events

    /// <summary>
    /// Method invoked when the card is clicked.
    /// </summary>
    internal Task OnCardClickAsync()
    {
        return ExecuteActionAsync(this);
    }

    #endregion

}
