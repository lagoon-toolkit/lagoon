namespace Lagoon.UI.Components;

/// <summary>
/// Toolbar component.
/// </summary>
public partial class LgToolbar : LgComponentBase, ICommandListener
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
    /// Gets or sets the child content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the Root container class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Get or sets the default button kind for the buttons.
    /// </summary>
    [Parameter]
    public ButtonKind? ButtonKind { get; set; }

    /// <summary>
    /// Gets or sets the buttons size into toolbar.
    /// </summary>
    [Parameter]
    public ButtonSize? ButtonSize { get; set; }

    /// <summary>
    /// Get or sets the default button kind for the buttons.
    /// </summary>
    [Parameter]
    public ToolbarGroupKind GroupKind { get; set; }

    /// <summary>
    /// Gets or sets the method to call when a command is received.
    /// </summary>
    [Parameter]
    public EventCallback<CommandEventArgs> OnCommand { get; set; }

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // Default button size from configuration
        ButtonSize ??= App.BehaviorConfiguration.ToolbarButtonSize;
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("toolbar", CssClass);
    }

    #endregion region

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
