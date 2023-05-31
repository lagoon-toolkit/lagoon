namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageBigEmpTyMessage : DemoPage
{
    public string Text { get; set; } = "label";
    public string Description { get; set; } = "Description";
    public string Icon { get; set; } = IconNames.Error;

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/BigEmptyMessage";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Big/Empty message", IconNames.All.CardText);
    }

    #endregion

    #region Method
    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
    }

    #endregion

    public void TextHasChanged(string text)
    {
        Text = text;
    }

    public void DescriptionHasChanged(string description)
    {
        Description = description;
    }

}
