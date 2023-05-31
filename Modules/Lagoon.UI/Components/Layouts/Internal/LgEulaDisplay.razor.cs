namespace Lagoon.UI.Components.Layouts.Internal;

/// <summary>
/// Page to display EULA.
/// </summary>
[Route(ROUTE)]
public partial class LgEulaDisplay : LgPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "LgEulaDisplay";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "#EulaTitle");
    }        

    #endregion

    #region Initialization

    /// <summary>
    /// Method to load data for the current page.
    /// </summary>
    /// <param name="e"></param>
    protected override async Task OnLoadAsync(PageLoadEventArgs e)
    {
        try
        {
            await base.OnLoadAsync(e);
            // Initialize the title and the icon of the page
            await SetTitleAsync(Link());                                
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    #endregion

}
