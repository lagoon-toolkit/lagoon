namespace Lagoon.UI.Components;

/// <summary>
/// Confirm message modal popup.
/// </summary>
public partial class LgConfirm : LgModal
{
    #region public properties

    /// <summary>
    /// Gets or sets the modal message
    /// </summary>
    [Parameter]
    public string ConfirmationMessage { get; set; }

    /// <summary>
    /// Gets or sets confirmation button event
    /// </summary>
    [Parameter]
    public Func<Task> ConfirmCallback { get; set; }

    /// <summary>
    /// Gets or sets cancel button event
    /// </summary>
    [Parameter]
    public Func<Task> CancelCallback { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Centered = true;
        Draggable = false;
        App.OnShowConfirm += OnAppShowConfirmAsync;
    }

    /// <summary>
    /// Free event handler(s)
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        App.OnShowConfirm -= OnAppShowConfirmAsync;
        base.Dispose(disposing);
    }

    ///<inheritdoc/>
    protected override RenderFragment RenderBodyContent()
    {
        return (builder) =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "confirmContent");
            builder.AddContent(2, (MarkupString)System.Net.WebUtility.HtmlEncode(ConfirmationMessage)?.Replace("\n","<br />"));
            builder.CloseElement();
        };
    }

    ///<inheritdoc/>
    protected override RenderFragment RenderFooterContent()
    {
        return (builder) =>
        {
            // Cancel button
            builder.OpenComponent<LgButton>(0);
            builder.AddAttribute(1, nameof(LgButton.Text), "#lblLgConfirmBtnCancel");
            builder.AddAttribute(2, nameof(LgButton.ButtonSize), ButtonSize.Medium);
            builder.AddAttribute(3, nameof(LgButton.Kind), ButtonKind.Secondary);
            builder.AddAttribute(4, nameof(LgButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, CancelAsync));
            builder.AddAttribute(5, "aria-label", ("lblLgConfirmBtnCancel".Translate()));
            builder.CloseComponent();
            // Ok button
            builder.OpenComponent<LgButton>(6);
            builder.AddAttribute(7, nameof(LgButton.Text), "#lblLgConfirmBtnValid");
            builder.AddAttribute(8, nameof(LgButton.Kind), ButtonKind.Primary);
            builder.AddAttribute(9, nameof(LgButton.ButtonSize), ButtonSize.Medium);
            builder.AddAttribute(10, nameof(LgButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, ConfirmAsync));
            builder.AddAttribute(11, "aria-label", ("lblLgConfirmBtnValid".Translate()));
            builder.CloseComponent();
        };
    }

    #endregion

    #region events

    /// <summary>
    /// Show confirmation message
    /// </summary>
    /// <param name="confirmationMessage">confirmation message</param>
    /// <param name="confirmCallBack">confirmation callback</param>
    /// <param name="cancelCallBack">cancellation callback</param>
    /// <param name="title">Modal title</param>
    private async Task OnAppShowConfirmAsync(string confirmationMessage, Func<Task> confirmCallBack, Func<Task> cancelCallBack, string title)
    {
        Title = !string.IsNullOrEmpty(title) ? title : "#lblLgConfirmTitle";
        ConfirmationMessage = confirmationMessage;
        ConfirmCallback = confirmCallBack;
        CancelCallback = cancelCallBack;
        await ShowAsync();
        // Refresh state too display modal
        StateHasChanged();
    }

    /// <summary>
    /// invoked when content change (lost focus)
    /// </summary>
    /// <returns></returns>
    protected virtual async Task ConfirmAsync(ActionEventArgs e)
    {
        // Hide modal
        await ForceCloseAsync();
        // Do the pending action                        
        if (ConfirmCallback is not null)
        {
            await ConfirmCallback();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected virtual async Task CancelAsync(ActionEventArgs e)
    {
        // Hide modal
        await ForceCloseAsync();
        // Do the pending action
        if(CancelCallback is not null)
        {
            await CancelCallback();
        }            
    }

    #endregion

}
