namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageBanner : DemoPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/banner";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Banner", IconNames.All.ChatLeftTextFill);
    }

    #endregion

    #region params
    public string Text { get; set; } = "Info Banner";
    public string BannerIconName { get; set; } = IconNames.About;
    public bool Closable { get; set; } = false;
    public bool Show { get; set; } = true;
    #endregion

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "410";
    }

    public void TextHasChanged(string text)
    {
        Text = text;
    }

    public void IconChanged(string icon)
    {
        BannerIconName = icon;
    }

    public void ShowHasChanged(bool show)
    {
        Show = show;
    }

    public void ClosableHasChanged(bool closable)
    {
        Closable = closable;
    }

}
