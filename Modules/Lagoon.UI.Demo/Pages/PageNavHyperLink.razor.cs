namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageNavHyperLink : DemoPage
{
    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/NavHyperLink";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "NavLink/HyperLink", IconNames.All.Link);
    }

    #endregion

    #region Method
    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
    }

    #endregion
}
