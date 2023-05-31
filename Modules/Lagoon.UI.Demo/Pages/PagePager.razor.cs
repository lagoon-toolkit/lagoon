namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PagePager : DemoPage
{

    public bool DisplayText { get; set; } = true;
    public int TotalPages { get; set; } = 7;
    public int CurrentPage { get; set; } = 5;
    public int MaxPageToDisplay { get; set; } = 5;

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/pager";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Pager", IconNames.All.ArrowLeftRight);
    }

    #endregion

    #region Method
    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "401";
    }

    #endregion
}
