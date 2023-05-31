namespace Lagoon.UI.Components.Toastr.Internal;

/// <summary>
/// Toast message component.
/// </summary>
public partial class LgToastr : LgComponentBase, IDisposable
{

    #region methods

    /// <summary>
    /// LgToastr initialization
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        App.OnShowToastr += OnApplicationShowToastrAsync;
    }

    /// <summary>
    /// Display toastr
    /// </summary>
    /// <param name="message">Toastr message</param>
    /// <param name="title">Toastr title</param>
    /// <param name="level">Toastr level</param>
    /// <param name="escapeHtml">Indicate if the message and the title are using raw text or HTML format.</param>
#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
    private async void OnApplicationShowToastrAsync(string message, string title, Level level, bool escapeHtml)
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
    {
        try
        {
            await ShowToastAsync(message, title, level, escapeHtml);
        }
        catch (Exception ex)
        {
            App.TraceException(ex);
        }
    }

    /// <summary>
    /// Show a toastr message.
    /// </summary>
    /// <param name="message">Toastr title</param>
    /// <param name="title">Toastr message</param>
    /// <param name="level">Toastr level</param>
    /// <param name="escapeHtml">Indicate if the message and the title are using raw text or HTML format.</param>
    public async Task ShowToastAsync(string message, string title, Level level, bool escapeHtml = true)
    {
        StringBuilder toastrFunc = new("toastr.", 14);
        LgToastrCustomOptions options = null;
        if(escapeHtml)
        {
            message = message.CheckTranslate();
            title = title.CheckTranslate();
        }
        switch (level)
        {
            case Level.Info:
                toastrFunc.Append("info");
                break;
            case Level.Success:
                toastrFunc.Append("success");
                break;
            case Level.Warning:
                toastrFunc.Append("warning");
                break;
            case Level.Error:
                toastrFunc.Append("error");
                options = new LgToastrErrorOptions();
                break;
        }
        options ??= new LgToastrCustomOptions();
        options.EscapeHtml = escapeHtml;
        await JS.InvokeVoidAsync(toastrFunc.ToString(), message, title, options);
    }

    /// <summary>
    /// Free event handler(s)
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        App.OnShowToastr -= OnApplicationShowToastrAsync;
        base.Dispose(disposing);
    }

    #endregion

}
