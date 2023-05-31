namespace Lagoon.UI.Custom.Pages;

/// <summary>
/// Page showing the color palette.
/// </summary>
[AllowAnonymous()]
[Route(ROUTE)]
public partial class LgPageColorsPalette : LgPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "lg/colors-palette";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "#pgColorPalette", IconNames.All.Palette);
    }

    #endregion

    #region Initialization

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());


    }

    #endregion
}
