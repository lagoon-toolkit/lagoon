namespace Lagoon.UI.Components;

/// <summary>
/// Frame
/// </summary>
public partial class LgFrame : LgAriaComponentBase, ICollapsable, ICommandListener
{
    #region fields

    /// <summary>
    /// Css class for header cursor
    /// </summary>
    private string _cssCursor = "";

    /// <summary>
    /// Frame tooltip displayed on the "collapse" icon
    /// </summary>
    private string _tooltipTitle;

    /// <summary>
    /// Gets or sets the arrow icon (collapse / expand icon)
    /// </summary>
    private string _collapseIcon;

    /// <summary>
    /// Gets or sets the toolbar frame button size
    /// </summary>
    private ButtonSize _toolbarButtonSize;

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    protected ElementReference ElementHeaderContentRef;

    #endregion

    #region render fragments

    /// <summary>
    /// Gets or sets the content (render)
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the footer(render)
    /// </summary>
    [Parameter]
    public RenderFragment FooterContent { get; set; }

    /// <summary>
    /// Gets or sets the header (render)
    /// </summary>
    [Parameter]
    public RenderFragment HeaderContent { get; set; }

    /// <summary>
    /// Gets or sets the summmary content (render)
    /// </summary>
    [Parameter]
    public RenderFragment SummaryContent { get; set; }

    /// <summary>
    /// Gets or sets the frame toolbar
    /// </summary>
    [Parameter]
    public RenderFragment Toolbar { get; set; }

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
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the frame state (true for collapsable)
    /// </summary>
    [Parameter]
    public ButtonSize? ButtonSize { get; set; }

    /// <summary>
    /// Gets or sets the child content class
    /// </summary>
    [Parameter]
    public string ChildCssClass { get; set; }

    /// <summary>
    /// Gets or sets if the frame can be collapsed
    /// </summary>
    [Parameter]
    public bool Collapsable { get; set; }

    /// <summary>
    /// Gets or sets the frame state (true for collapsed)
    /// </summary>
    [Parameter]
    public bool Collapsed { get; set; }

    /// <summary>
    /// Handle the binding of the frame collapsed.
    /// </summary>
    [Parameter]
    public EventCallback<bool> CollapsedChanged { get; set; }

    /// <summary>
    /// Gets or sets the frame css class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the default collapsed value for the frame
    /// </summary>
    [Parameter]
    public bool? DefaultCollapsed { get; set; }

    /// <summary>
    /// Gets or sets the header class
    /// </summary>
    [Parameter]
    public string HeaderCssClass { get; set; }

    /// <summary>
    /// Gets or sets the frame icon name
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Gets or sets the kind of the frame
    /// </summary>
    [Parameter]
    public Kind Kind { get; set; } = Kind.Primary;

    /// <summary>
    /// Gets or sets the method to call when a command is received.
    /// </summary>
    [Parameter]
    public EventCallback<CommandEventArgs> OnCommand { get; set; }

    /// <summary>
    /// Event callable on toggle frame for client app
    /// </summary>
    [Parameter]
    public EventCallback<ToggleFrameEventArgs> OnToggle { get; set; }

    /// <summary>
    /// Gets or sets the frame collapsable value
    /// </summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>
    /// Gets or sets the frame title
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Custom frame initialisation
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (DefaultCollapsed.HasValue)
        {
            Collapsed = DefaultCollapsed.Value;
        }

        if (ButtonSize == null)
        {
            _toolbarButtonSize = App.BehaviorConfiguration.FrameToolbarButtonSize;
        }

        if (!Collapsable)
        {
            Collapsed = false;
        }
        _cssCursor = (Collapsable ? "cursorPointer" : "");
        SetIconState();
    }

    /// <summary>
    /// Remark ElementReference will be available only after OnAfterRender/OnAfterRenderAsync
    /// </summary>
    /// <param name="firstRender">Is it the first render for this component ?</param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && HeaderContent != null)
        {
            await JS.InvokeAsync<object>("Lagoon.JsUtils.InitClickControl", ElementHeaderContentRef);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Component parameter update
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        SetIconState();
    }


    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        string _cssFrameKind = Kind switch
        {
            Kind.Primary => "frame-primary",
            Kind.Secondary => "frame-secondary",
            Kind.Default => "frame-default",
            Kind.Error => "frame-danger",
            Kind.Info => "frame-info",
            Kind.Success => "frame-success",
            Kind.Warning => "frame-warning",
            _ => "",
        };
        builder.Add("frameRoot", _cssFrameKind, CssClass);
    }

    /// <summary>
    /// Return the title to be rendered.
    /// </summary>
    /// <returns>The title to be rendered.</returns>
    protected virtual string GetRenderTitle()
    {
        return Title;
    }

    /// <summary>
    /// Frame Toolbar render
    /// </summary>
    /// <returns></returns>
    protected virtual RenderFragment RenderToolbarContent()
    {
        return Toolbar;
    }

    /// <summary>
    /// Frame header render
    /// </summary>
    /// <returns></returns>
    protected RenderFragment RenderHeaderContent()
    {
        return HeaderContent;
    }

    /// <summary>
    /// Frame body render
    /// </summary>
    /// <returns></returns>
    protected virtual RenderFragment RenderBodyContent()
    {
        return ChildContent;
    }

    /// <summary>
    /// Frame footer render
    /// </summary>
    /// <returns></returns>
    public virtual RenderFragment RenderFooterContent()
    {
        return FooterContent;
    }

    /// <summary>
    /// Frame summary render
    /// </summary>
    /// <returns></returns>
    protected RenderFragment RenderSummaryContent()
    {
        return SummaryContent;
    }
    /// <summary>
    /// Toggle frame
    /// </summary>
    private async Task ToggleFrameAsync()
    {
        if (Collapsable)
        {
            if (CollapsedChanged.HasDelegate)
            {
                await CollapsedChanged.TryInvokeAsync(App, !Collapsed);
            }
            else
            {
                Collapsed = !Collapsed;
            }
            await OnToggle.TryInvokeAsync(App, new ToggleFrameEventArgs() { Collapsed = Collapsed });
        }
        SetIconState();
    }

    /// <summary>
    /// Return the collapse tooltip depending on the frame state
    /// </summary>
    /// <returns></returns>
    private string SetFrameCollapseTooltip()
    {
        if (Collapsed)
        {
            return "frameExpandTooltip".Translate();
        }
        else
        {
            return "frameCollapseTooltip".Translate();
        }
    }

    /// <summary>
    /// Define the icon and tooltip following opening state
    /// </summary>
    protected void SetIconState()
    {
        if (Collapsable)
        {
            _collapseIcon = (Collapsed ? IconNames.Expand : IconNames.Collapse);
        }
        _tooltipTitle = SetFrameCollapseTooltip();
    }

    #endregion

    #region methods - ICommandListener

    /// <summary>
    /// The parent command listener.
    /// </summary>
    ICommandListener ICommandListener.ParentCommandListener => ParentCommandListener;

    /// <summary>
    /// Method invoked when a command is received from a sub component.
    /// </summary>
    /// <param name="args">The command event args.</param>
    public virtual Task BubbleCommandAsync(CommandEventArgs args)
    {
        return CommandHandler.InvokeAsync(OnCommand, args);
    }

    #endregion

}
