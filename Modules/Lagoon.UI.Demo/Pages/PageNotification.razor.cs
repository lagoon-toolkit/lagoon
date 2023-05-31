namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageNotification : DemoPage
{
    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/notifications";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Notifications", IconNames.All.BellFill);
    }

    #endregion


    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        DocumentationComponent = "1208";
        SetTitle(Link());
    }
}
