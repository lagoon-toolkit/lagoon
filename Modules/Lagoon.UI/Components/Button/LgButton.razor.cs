using Microsoft.AspNetCore.Components.Authorization;

namespace Lagoon.UI.Components;

/// <summary>
/// Component button.
/// </summary>
public partial class LgButton : LgAriaComponentBase, ILgComponentPolicies, IActionComponent
{
    #region private fields

    // Component id
    private string _elementId;

    // Indicate the state of component after policies applied
    private PolicyState _policyState;

    // Indicate if the user policies must be checked
    private bool _updatePolicyState;

    // Internal flag used to track button action excecution state
    private bool _isExecuting = false;

    #endregion

    #region properties

    /// <summary>
    /// Button size to render.
    /// </summary>
    protected ButtonSize ButtonSizeRendering { get; set; }

    /// <summary>
    /// Text to display.
    /// </summary>
    protected string TextRendering { get; set; }

    /// <summary>
    /// Icon name to use.
    /// </summary>
    protected string IconNameRendering { get; set; }

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    protected ElementReference ElementRef { get; set; }

    ///<inheritdoc/>
    protected ElementReference FocusElementRef => ElementRef;

    #endregion

    #region cascading parameters

    /// <summary>
    /// Provides information about the currently authenticated user.
    /// </summary>
    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationState { get; set; }

    /// <summary>
    /// Potential policies defined by an ancestor 
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    public LgAuthorizeView ParentPolicy { get; set; }

    /// <summary>
    /// The parent command listener.
    /// </summary>
    [CascadingParameter]
    private ICommandListener ParentCommandListener { get; set; }

    /// <summary>
    /// An optional parent LgEditForm
    /// </summary>
    [CascadingParameter]
    public LgEditForm EditForm { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// Button custom content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets if the component received the focus on first render
    /// </summary>
    [Parameter]
    public bool AutoFocus { get; set; }

    /// <summary>
    /// Gets or sets the button size.
    /// </summary>
    [Parameter]
    public ButtonSize ButtonSize { get; set; } = ButtonSize.Medium;

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
    /// Gets or sets the CSS class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the disabled attribute.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets the DOM element id.
    /// </summary>
    protected string ElementId
    {
        get
        {
            _elementId ??= GetNewElementId();

            return _elementId;
        }
    }

    /// <summary>
    /// Gets or sets the button icon name
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Get or set if the button type is "submit".
    /// </summary>
    [Parameter]
    public bool IsSubmit { get; set; }

    /// <summary>
    /// Gets or sets the kind of the button
    /// </summary>
    [Parameter]
    public ButtonKind Kind { get; set; } = ButtonKind.Primary;

    /// <summary>
    /// Informations to create link to specific page.
    /// </summary>
    [Parameter]
    public LgPageLink Link { get; set; }

    /// <summary>
    /// Gets or sets the button Onclick eventCallback
    /// </summary>
    [Parameter]
    public EventCallback<ActionEventArgs> OnClick { get; set; }

    /// <summary>
    /// Gets or sets the button prevent default
    /// </summary>
    [Parameter]
    public bool PreventDefault { get; set; }

    /// <summary>
    /// Disable button when executing an action. 
    /// </summary>
    /// <value><c>true</c> by default</value>
    [Parameter]
    public bool PreventDoubleClick { get; set; } = true;

    /// <summary>
    /// Gets or sets the character associated with CTRL that trigger the button click.
    /// </summary>
    [Parameter]
    public string ShortcutKey { get; set; }

    /// <summary>
    /// Gets or sets the button stop propagation
    /// </summary>
    [Parameter]
    public bool StopPropagation { get; set; } = true;

    /// <summary>
    /// The name of the browser window in which the URI should be opened.
    /// </summary>
    [Parameter]
    public string Target { get; set; }

    /// <summary>
    /// Gets or sets the button label
    /// </summary>
    [Parameter]
    public string Text { get; set; }

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
    /// Position de la bulle d'aide.
    /// </summary>
    [Parameter]
    public TooltipPosition TooltipPosition { get; set; }

    /// <summary>
    /// The URI to open.
    /// </summary>
    [Parameter]
    public string Uri { get; set; }

    #endregion

    #region IComponentPolicy implementation

    /// <summary>
    /// Policy required to view the Button
    /// </summary>
    [Parameter]
    public string PolicyVisible { get; set; }

    /// <summary>
    /// Not implemented for button
    /// </summary>
    [Parameter]
    public string PolicyEdit { get; set; }

    #endregion

    #region ILgActionComponent implementation

    ICommandListener IActionComponent.ParentCommandListener => ParentCommandListener;

    #endregion

    #region intialisation

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (IsSubmit && EditForm is not null)
        {
            EditForm.OnSubmitting += EditForm_OnSubmitting;
        }
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing && IsSubmit && EditForm is not null)
        {
            EditForm.OnSubmitting -= EditForm_OnSubmitting;
        }
        base.Dispose(disposing);
    }

    #endregion

    #region methods



    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ButtonSizeRendering = ButtonSize;
        TextRendering = (Text ?? Link?.Title).CheckTranslate();
        IconNameRendering = (IconName ?? Link?.IconName).CheckTranslate();
        // Initialize the component visibility if it's depend of policies
        _updatePolicyState = InitPolicyState(ref _policyState, ParentPolicy, !string.IsNullOrEmpty(PolicyVisible), !string.IsNullOrEmpty(PolicyEdit));

        // Confirmation message must be on LgEditForm and not on button when button is submitting a form
        if (IsSubmit && !String.IsNullOrEmpty(ConfirmationMessage))
        {
            throw new InvalidOperationException($"When supplying an {nameof(IsSubmit)} parameter to " +
                $"{nameof(LgButton)}, do not also supply {nameof(ConfirmationMessage)}.");
        }

    }

    /// <summary>
    /// Update policy
    /// </summary>
    protected override Task OnParametersSetAsync()
    {
        if (_updatePolicyState)
        {
            return ((ILgComponentPolicies)this).UpdatePolicyStateAsync(AuthenticationState, _policyState);
        }
        else
        {
            return base.OnParametersSetAsync();
        }
    }

    ///<inheritdoc/>
    protected override bool ShouldRender()
    {
        return _policyState.Visible;
    }

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && AutoFocus)
        {
            await JS.FocusAsync(FocusElementRef);
        }
    }

    /// <summary>
    /// Get the list of additional attributes to add to component.
    /// </summary>
    /// <returns>The list of additional attributes to add to component.</returns>
    protected IEnumerable<KeyValuePair<string, object>> GetAdditionalAttributes()
    {
        return GetAdditionalAttributes(AdditionalAttributes, GetTooltipAttributes(GetTooltipWithShortcutKey(), TooltipIsHtml, TooltipPosition));
    }

    /// <summary>
    /// Gets the tooltip with Shortcut Key
    /// </summary>
    private string GetTooltipWithShortcutKey()
    {
        // Only tooltip
        if (string.IsNullOrEmpty(ShortcutKey))
        {
            return Tooltip;
        }
        // Only shortcutkey
        if (string.IsNullOrEmpty(Tooltip))
        {
            return string.Format("(Ctrl + {0})", ShortcutKey);
        }
        // Tooltip + ShortcutKey
        return string.Format("{0} (Ctrl + {1})", Tooltip, ShortcutKey);
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        string _cssBtnSize = ButtonSizeRendering switch
        {
            ButtonSize.Small => "btn-sm",
            ButtonSize.Large => "btn-lg",
            _ => "",
        };
        string _cssBtnKind = Kind switch
        {
            ButtonKind.Primary => "btn-primary",
            ButtonKind.Secondary => "btn-secondary",
            ButtonKind.Error => "btn-danger",
            ButtonKind.Info => "btn-info",
            ButtonKind.Success => "btn-success",
            ButtonKind.Warning => "btn-warning",
            ButtonKind.Link => "btn-link",
            ButtonKind.Ghost => "btn-ghost",
            _ => "",
        };
        builder.Add("btn", _cssBtnSize, _cssBtnKind, CssClass);
        builder.AddIf(Disabled, "disabled");
    }

    /// <summary>
    /// Button should be disabled if explicitly set or by a policy edit
    /// </summary>
    /// <returns>true if disabled, false otherwise</returns>
    private bool IsDisabled()
    {
        return Disabled || !_policyState.Editable || (PreventDoubleClick && _isExecuting);
    }

    #endregion

    #region event

    /// <summary>
    /// Capture the key pressed events.
    /// </summary>
    /// <param name="args">Key pressed informations.</param>
    private async Task OnKeyPressAsync(KeyboardEventArgs args)
    {
        if (args.CtrlKey && string.Equals(args.Key, ShortcutKey))
        {
            await OnClickInternalAsync();
        }
    }

    /// <summary>
    /// invoked when on click button
    /// </summary>
    /// <returns></returns>
    protected async Task OnClickInternalAsync()
    {
        try
        {
            _isExecuting = true;
            await ExecuteActionAsync(this);
        }
        finally
        {
            _isExecuting = false;
        }
    }

    /// <summary>
    /// On LgEditForm submission change, refresh the executing flag
    /// </summary>
    /// <param name="isSubmitting"></param>
    private void EditForm_OnSubmitting(bool isSubmitting)
    {
        if (isSubmitting != _isExecuting)
        {
            _isExecuting = isSubmitting;
            StateHasChanged();
        }
    }

    #endregion

}
