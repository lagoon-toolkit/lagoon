namespace Lagoon.UI.Components.Internal;


/// <summary>
/// Component used to display End User Licence Agreement
/// </summary>
public partial class LgEula : LgPage, IFullScreenPage
{

    #region fields

    /// <summary>
    /// Show/Hide 
    /// </summary>
    private bool _eulaToAccept;
    /// <summary>
    /// LocalStorage key for eula definition state
    /// </summary>
    private string _eulaDicoKey;
    /// <summary>
    /// LocalStorage key for eula validation state
    /// </summary>
    private string _eulaStateKey;

    #endregion

    #region methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _eulaDicoKey = App.GetLocalStorageKey("EulaDico");
        _eulaStateKey = App.GetLocalStorageKey("EulaState");

        // Show modal if consent is enable and user does not have accept the EULA
        _eulaToAccept = App.BehaviorConfiguration.RequireEulaConsent && !JS.HasAcceptedCgu(_eulaStateKey, _eulaDicoKey);
    }

    /// <summary>
    /// Accept EULA
    /// </summary>
    private void AcceptCguAsync()
    {
        JS.AcceptCgu(_eulaStateKey);
        _eulaToAccept = false;
    }

    #endregion        

}
