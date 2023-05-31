namespace Lagoon.UI.Demo.Pages.Form;

[Route(ROUTE)]
public partial class PageNumericBox : DemoPageForm
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/numericbox";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "NumericBox", IconNames.All.ChatLeftTextFill);
    }

    #endregion

    #region parameters

    private int Decimals { get; set; } = 2;

    public int IncrementStep { get; set; } = 1;

    public PrefixSuffix PrefixSuffix { get; set; } = new();

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "342";
    }

    #endregion

}
