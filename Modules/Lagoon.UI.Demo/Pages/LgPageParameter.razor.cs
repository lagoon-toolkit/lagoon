using Lagoon.Core;
using Lagoon.UI.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Lagoon.UI.Demo.Pages;

/// <summary>
/// Page to display application informations
/// </summary>
[AllowAnonymous]
[Route(ROUTE)]
public partial class LgPageParameter : LgPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    public const string ROUTE = "parameter";

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
        public bool IsTabbed
        {
            get;
            set;
        }
    }

    #endregion

    #region "injections"

    /// <summary>
    /// Navigation mode handler.
    /// </summary>
    [Inject]
    public NavigationModeService Navigation { get; set; } = default!;

    #endregion

    #region parameters

    /// <summary>
    /// is tabbed boolean
    /// </summary>
    [Parameter]
    public bool IsTabbed { get; set; } = true;

    #endregion

    #region Fields

    /// <summary>
    /// Settings
    /// </summary>
    private readonly Settings _settings = new Settings();

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await SetTitleAsync(Link());
        Navigation.OnNavigationModeChanged += OnNavigationModeChanged;
        _settings.IsTabbed = Navigation.IsTabbed;
    }

    protected override void Dispose(bool disposing)
    {
        Navigation.OnNavigationModeChanged -= OnNavigationModeChanged;
        base.Dispose(disposing);
    }

    private void OnNavigationModeChanged()
    {
        StateHasChanged();
    }

    private void ChangeNavigationHandler(ChangeEventArgs e)
    {
        Navigation.ChangeNavigationMode((bool)e.Value);
    }

    #endregion
}
