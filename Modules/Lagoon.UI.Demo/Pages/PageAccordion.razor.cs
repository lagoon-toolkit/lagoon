namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageAccordion : DemoPage
{
    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/accordion";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Accordion", IconNames.All.ArrowDown);
    }
    #endregion

    #region Method
    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "107";
    }

    #endregion
}
