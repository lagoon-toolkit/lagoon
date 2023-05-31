namespace Lagoon.UI.Components;

/// <summary>
/// Defines a popover container.
/// </summary>
public partial class LgPopover : LgComponentBase
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    private string Id { get; } = GetNewElementId();

    /// <summary>
    /// Gets or sets the event raised when the popover loses the focus.
    /// </summary>
    [Parameter]
    public EventCallback<ActionEventArgs> OnBlur { get; set; }

    /// <summary>
    /// Gets or sets the content that needs to be handled.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets visibility flag.
    /// </summary>
    [Parameter]
    public bool Visible { get; set; }

    /// <summary>
    /// Gets or sets a collection of additional attributes.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Visible)
        {
            await JS.InvokeVoidAsync("Lagoon.JsUtils.focusElement", $"#{Id}");
        }
    }

    /// <summary>
    /// The method is invoked when the component loses the focus.
    /// </summary>
    /// <returns>A <see cref="Task"/>.</returns>
    private Task OnBlurInternalAsync()
    {
        return ExecuteActionAsync(OnBlur);
    }
}