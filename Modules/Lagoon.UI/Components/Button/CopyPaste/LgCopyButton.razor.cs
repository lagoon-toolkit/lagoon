namespace Lagoon.UI.Components;

/// <summary>
/// A button to copy the bound value to the clipboard.
/// </summary>
public class LgCopyButton : LgButton
{

    #region parameters

    /// <summary>
    /// Gets or sets a callback when pasting content.
    /// </summary>
    [Parameter]
    public EventCallback<ClipboardTextEventArg> OnCopy { get; set; }

    /// <summary>
    /// Gets or sets the value to save in the clipboard when the button is pressed.
    /// </summary>
    [Parameter]
    public object Value { get; set; }

    #endregion

    #region constructor

    /// <summary>
    /// New instance.
    /// </summary>
    public LgCopyButton()
    {
        Tooltip = "#lgCopyButtonTooltip";
        IconName = IconNames.All.ClipboardPlus;
        OnClick = EventCallback.Factory.Create<ActionEventArgs>(this, CopyAsync);
    }

    #endregion

    #region methods

    /// <summary>
    /// Copy the value into the clipboard.
    /// </summary>
    /// <returns>The value type.</returns>
    private async Task CopyAsync()
    {
        ClipboardTextEventArg arg = new() { Value = Value?.ToString() };
        // Raise the event
        if (OnCopy.HasDelegate)
        {
            await OnCopy.TryInvokeAsync(App, arg);
        }
        if (arg.Value is not null)
        {
            await JS.InvokeAsync<string>("Lagoon.LgCopyPaste.Copy", arg.Value);
            IconNameRendering = IconNames.All.ClipboardCheck;
        }
    }

    #endregion

}
