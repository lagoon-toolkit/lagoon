namespace Lagoon.UI.Components;

/// <summary>
/// Navigate link for the LgRouter
/// </summary>
public partial class LgHyperlink : LgComponentBase, IActionComponent
{

    #region render fragments

    /// <summary>
    /// Gets or sets the child content of the component.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region cascading parameters

    /// <summary>
    /// The parent command listener.
    /// </summary>
    [CascadingParameter]
    private ICommandListener ParentCommandListener { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be added to the generated
    /// <c>a</c> element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

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
    /// Gets or sets the CSS class.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Informations to create link to specific page.
    /// </summary>
    [Parameter]
    public LgPageLink Link { get; set; }

    /// <summary>
    /// The name of the browser window in which the URI should be opened.
    /// </summary>
    [Parameter]
    public string Target { get; set; }

    /// <summary>
    /// Gets or sets the URI to navigate to.
    /// </summary>
    [Parameter]
    public string Uri { get; set; }

    #endregion

    #region events

    /// <summary>
    /// Gets or sets the link Onclick eventCallback
    /// </summary>
    [Parameter]
    public EventCallback<ActionEventArgs> OnClick { get; set; }

    #endregion

    #region ILgActionComponent implementation

    ICommandListener IActionComponent.ParentCommandListener => ParentCommandListener;

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        string uri = Uri ?? Link?.Uri ?? "#";
        builder.OpenElement(0, "a");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "class", GetClassAttribute());
        builder.AddAttribute(3, "href", uri);
        // Check if there is no target, or if the auth token must be passed as temp cookie (for api)
        if (string.IsNullOrEmpty(Target) || uri.StartsWith("api/"))
        {
            // We handle the click on the link
            builder.AddAttribute(4, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, OnClickInternalAsync));
            builder.AddEventPreventDefaultAttribute(5, "onclick", true);
        }
        else
        {
            // We create a simple link
            builder.AddAttribute(6, "target", Target);
        }
        builder.AddContent(7, ChildContent);
        builder.CloseElement();
    }

    /// <summary>
    /// Invoked when link clicked
    /// </summary>
    private Task OnClickInternalAsync()
    {
        return ExecuteActionAsync(this);
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add(CssClass);
    }

    #endregion
}
