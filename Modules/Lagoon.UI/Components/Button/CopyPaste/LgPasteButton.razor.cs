using Microsoft.AspNetCore.Components.Forms;

namespace Lagoon.UI.Components;

/// <summary>
/// A button to get the text from the clipboard.
/// </summary>
public partial class LgPasteButton : LgButton
{

    #region fields

    /// <summary>
    /// DotNet Object reference
    /// </summary>
    private DotNetObjectReference<LgPasteButton> _dotNetObjRef;

    /// <summary>
    /// The paste window reference.
    /// </summary>
    private LgModal _modalReference;

    /// <summary>
    /// Indicate if the paste window is visible.
    /// </summary>
    private bool _showPasteModal;

    #endregion


    #region cascading parameter

    /// <summary>
    /// Gets the associated <see cref="Microsoft.AspNetCore.Components.Forms.EditContext"/>.
    /// This property is uninitialized if the input does not have a parent <see cref="EditForm"/>.
    /// </summary>
    [CascadingParameter]
    protected EditContext EditContext { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets a callback when pasting content.
    /// </summary>
    [Parameter]
    public EventCallback<ClipboardTextEventArg> OnPaste { get; set; }

    /// <summary>
    /// Gets the value from the clipboard after the button is clicked.
    /// </summary>
    [Parameter]
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets a callback that updates the bound value.
    /// </summary>
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the bound value.
    /// </summary>
    [Parameter]
    public Expression<Func<string>> ValueExpression { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public LgPasteButton()
    {
        IconName = IconNames.All.Clipboard;
        Tooltip = "#lgPasteButtonTooltip";
        OnClick = EventCallback.Factory.Create<ActionEventArgs>(this, PasteAsync);
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _dotNetObjRef = DotNetObjectReference.Create(this);
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _dotNetObjRef.Dispose();
        }
        base.Dispose(disposing);
    }

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (_showPasteModal)
        {
            await JS.InvokeVoidAsync("Lagoon.LgCopyPaste.AddPasteHandler", _modalReference.Id, _dotNetObjRef);
        }
    }

    /// <summary>
    /// Try to paste without the modal, if fail, open the modal.
    /// </summary>
    private async Task PasteAsync()
    {
        await JS.InvokeAsync<string>("Lagoon.LgCopyPaste.TryPaste", _dotNetObjRef);
    }

    /// <summary>
    /// Invoked from JS when the value is changed.
    /// </summary>
    /// <param name="value">The pasted value</param>
    /// <returns></returns>
    [JSInvokable]
    public async Task OnPasteAsync(string value)
    {
        // Close the modal
        _showPasteModal = false;
        if (value is not null)
        {
            ClipboardTextEventArg arg = new() { Value = value };
            // Raise the event
            if (OnPaste.HasDelegate)
            {
                await OnPaste.TryInvokeAsync(App, arg);
            }
            if (Value != arg.Value)
            {
                Value = arg.Value;
                if (ValueChanged.HasDelegate)
                {
                    await ValueChanged.TryInvokeAsync(App, Value);
                }
            }
            if (EditContext is not null && ValueExpression is not null)
            {
                // Clear/Show validation message
                EditContext?.NotifyFieldChanged(FieldIdentifier.Create(ValueExpression));
            }
        }
        StateHasChanged();
    }

    /// <summary>
    /// Invoked from JS when the brwoser doesn't allow Paste.
    /// </summary>
    /// <returns></returns>
    [JSInvokable]
    public void ShowPasteModal()
    {
        // Show the modal
        _showPasteModal = true;
        StateHasChanged();
    }

    #endregion

}
