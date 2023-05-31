namespace Lagoon.UI.Demo.Pages.Form;

[Route(ROUTE)]
public partial class PageTextBox : DemoPageForm
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/textbox";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "TextBox", IconNames.All.ChatLeftTextFill);
    }

    #endregion

    #region parameters

    public PrefixSuffix PrefixSuffix { get; set; } = new();

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "341";
    }

    #endregion
}
