using Lagoon.UI.Demo.Pages;

namespace Lagoon.UI.Demo.Pages;

/// <summary>
/// The home page of the application.
/// </summary>
[Route(ROUTE)]
//[AllowAnonymous()]
public partial class Index : LgPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "./";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "#pageHomeTitle", @IconNames.All.HouseFill);
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
        public bool IsTabbed { get; set;}
    }

    #endregion

    #region Fields

    /// <summary>
    /// Settings
    /// </summary>
    private readonly Settings _settings = new Settings();

    #endregion

    #region dependencies injections

    /// <summary>
    /// Navigation mode handler.
    /// </summary>
    [Inject]
    public NavigationModeService Navigation { get; set; } = default!;

    #endregion

    #region Initialization

    /// <summary>
    /// Method invoked when the component is ready to start,
    /// having received its initial parameters from its parent in the render tree.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // Set the page title
        SetTitle(Link());
        _settings.IsTabbed = Navigation.IsTabbed;
    }

    #endregion

    #region Methods

    private void ChangeNavigationHandler(ChangeEventArgs e)
    {
        Navigation.ChangeNavigationMode((bool)e.Value);
    }

    #endregion
}

