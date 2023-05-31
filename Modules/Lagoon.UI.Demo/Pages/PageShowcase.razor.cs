namespace Lagoon.UI.Demo.Pages;

[AllowAnonymous]
[Route(ROUTE)]
[Route(ROUTE + "/{ActiveTab}")]
public partial class PageShowcase
{

    [Parameter]
    public string ActiveTab { get; set; }

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/showcase";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "#pgShowCase");
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
