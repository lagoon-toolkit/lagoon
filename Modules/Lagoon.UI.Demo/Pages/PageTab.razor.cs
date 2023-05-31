namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageTab : DemoPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/tab";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Tabs", IconNames.All.Wrench);
    }

    #endregion
    #region constants
    #endregion

    #region fields

    private string _activeTabKey;

    #endregion

    /// <summary>
    /// Element reference to second tabcontainer
    /// </summary>
    public LgTabContainer TabContainer2;

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        DocumentationComponent = "51";
        SetTitle(Link());
    }
}
