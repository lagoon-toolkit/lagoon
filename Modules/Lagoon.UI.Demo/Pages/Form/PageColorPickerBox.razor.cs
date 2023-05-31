namespace Lagoon.UI.Demo.Pages.Form;

[Route(ROUTE)]
public partial class PageColorPickerBox : DemoPageForm
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/colorpickerbox";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "ColorPickerBox", IconNames.All.ChatLeftTextFill);
    }

    #endregion

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "381";
    }

    #region parameters

    private bool ShowInput { get; set; } = true;
    #endregion

}
