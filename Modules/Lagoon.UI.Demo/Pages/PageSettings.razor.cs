namespace Lagoon.UI.Demo.Pages;

/// <summary>
/// Page to display application informations
/// </summary>
[AllowAnonymous]
[Route(ROUTE)]
public partial class PageSettings : LgPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    public const string ROUTE = "settings";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "#pgParameterTitle");
    }

    #endregion

    #region classes

    /// <summary>
    /// Settings object 
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Define if the application is in tabbed navigation
        /// </summary>
        public bool IsTabbed { get; set; }

    }

    #endregion

    #region Fields

    /// <summary>
    /// Settings
    /// </summary>
    private readonly Settings _settings = new();

    #endregion

    #region injections

    /// <summary>
    /// Navigation mode handler.
    /// </summary>
    [Inject]
    public NavigationModeService NavigationMode { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await SetTitleAsync(Link());
        NavigationMode.OnNavigationModeChanged += OnNavigationModeChanged;
        _settings.IsTabbed = NavigationMode.IsTabbed;
    }

    protected override void Dispose(bool disposing)
    {
        NavigationMode.OnNavigationModeChanged -= OnNavigationModeChanged;
        base.Dispose(disposing);
    }

    private void OnNavigationModeChanged()
    {
        StateHasChanged();
    }

    private void ChangeNavigationHandler(ChangeEventArgs e)
    {
        NavigationMode.ChangeNavigationMode((bool)e.Value);
    }

    #endregion
}