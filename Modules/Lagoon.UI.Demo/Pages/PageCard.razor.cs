namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageCard : DemoPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/card";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Cards", IconNames.All.CardChecklist);
    }

    #endregion

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "365";
    }

    #region parameters

    public string Content { get; set; } = "Title";
    public string ContentSeparator { get; set; } = "Content";
    public bool HaveColor { get; set; } = true;
    public string Color { get; set; } = "#B60022";

    public bool HaveIcon { get; set; } = true;
    public string Icon { get; set; } = IconNames.Warning;

    public bool HaveSeparator { get; set; } = true;

    public bool IsClickable { get; set; } = true;


    public void OnUpdateContent(ChangeEventArgs args)
    {
        Content = (string)args.Value;
    }

    public void OnUpdateContentSeparator(ChangeEventArgs args)
    {
        ContentSeparator = (string)args.Value;
    }

    public string GetIcon()
    {
        if (HaveIcon)
        {
            return Icon;
        }

        return null;
    }

    #endregion
}
