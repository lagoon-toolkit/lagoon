namespace Lagoon.UI.Components;

/// <summary>
/// Information displayer for screen reader
/// </summary>
public partial class LgSrInformation : LgComponentBase, IDisposable
{

    #region Properties

    /// <summary>
    /// Gets or sets removing text timer
    /// </summary>
    /// <value>timer in millisecond</value>
    private readonly int _removeTimerValue = 5000;

    #endregion

    #region Services
    /// <summary>
    /// JS Runtime
    /// </summary>
    /// <value></value>
    [Inject]
    private IJSRuntime JSRuntime { get; set; }
    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        App.OnShowScreenReaderInformation += ReadText;
    }

    /// <summary>
    /// Display text
    /// </summary>
    /// <param name="text">Data to add</param>
    private void ReadText(string text)
    {
        JS.InvokeVoid("Lagoon.JsUtils.ChangeSrText", text.CheckTranslate(), true, _removeTimerValue);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        App.OnShowScreenReaderInformation -= ReadText;
        base.Dispose(disposing);
    }

    #endregion

}