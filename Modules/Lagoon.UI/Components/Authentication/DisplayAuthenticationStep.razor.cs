namespace Lagoon.UI.Components;


/// <summary>
/// Component used to show authentication state during the authentication workflow
/// </summary>
public partial class DisplayAuthenticationStep : ComponentBase
{

    #region fields

    private string _loadingAnimation;

    #endregion

    #region properties

    /// <summary>
    /// JS interop runtime.
    /// </summary>
    [Inject]
    public Microsoft.JSInterop.IJSRuntime JS { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Label of the authentication state.
    /// </summary>
    [Parameter]
    public string Step { get; set; }

    /// <summary>
    /// Gets or sets if the reconnect button must be visible.
    /// </summary>
    [Parameter]
    public bool ShowReconnect { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        _loadingAnimation = await JS.GetLoadingAnimationAsync();
        await base.OnInitializedAsync();
    }

    #endregion

}
