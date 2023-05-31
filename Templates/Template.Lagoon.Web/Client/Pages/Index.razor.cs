namespace TemplateLagoonWeb.Client.Pages;

/// <summary>
/// The home page of the application.
/// </summary>
[Route(ROUTE)]
//[AllowAnonymous()]
public partial class Index : LgPage
{

    #region URL, icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "./";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "#pageHomeTitle", @IconNames.All.HouseFill);
    }

    #endregion

    #region methods

    /// <summary>
    /// Method to load the content of the page.
    /// </summary>
    /// <param name="e">Contains the "CancellationToken" to manage the loading cancellation.</param>
    protected override async Task OnLoadAsync(PageLoadEventArgs e)
    {
        await base.OnLoadAsync(e);
        // Initialize the title and the icon of the page
        await SetTitleAsync(Link());
    }

    #endregion

}
