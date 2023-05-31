namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageBreadcrumb : DemoPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/breadcrumb";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Breadcrumb", IconNames.All.Wrench);
    }

    #endregion

    private readonly BreadcrumbTrailItemList _breadcrumbItems = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "411";

        _breadcrumbItems.AddUri("./", "Home", IconNames.Home);
        _breadcrumbItems.Add(PageShowcase.Link());
        _breadcrumbItems.AddAction(() => { ShowInformation("Ok"); }, "OK", IconNames.All.ChatSquareDots);
        _breadcrumbItems.Add("Label");
        _breadcrumbItems.Add(Link());
    }
}
