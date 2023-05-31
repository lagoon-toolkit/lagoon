namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageActionPanel : DemoPage
{
    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/ActionPanel";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "ActionPanel", IconNames.All.Palette);
    }

    #endregion

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        DocumentationComponent = "176";
        SetTitle(Link());
    }

    private void Save()
    {
        ShowInformation("#lblSaveTostr");
    }

    private void Delete()
    {
        ShowSuccess("#lblDeleteTostr");
    }

    private void Cancel()
    {
        ShowWarning("#lblCancelTostr");
    }

}
