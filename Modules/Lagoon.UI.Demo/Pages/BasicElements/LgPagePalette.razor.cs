using Lagoon.UI.Components;
using Lagoon.UI.Demo.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Lagoon.UI.Demo.Pages;

/// <summary>
/// Page showing the color palette.
/// </summary>
[AllowAnonymous]
[Route(ROUTE)]
public partial class LgPagePalette : DemoPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    public const string ROUTE = "lg/colors";


    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "#pgColorPalette", IconNames.All.PaletteFill);
    }

    #endregion

    #region Initialization

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "431";
    }

    #endregion
}

