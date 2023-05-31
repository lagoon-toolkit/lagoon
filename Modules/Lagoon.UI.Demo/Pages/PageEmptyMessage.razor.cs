namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageEmptyMessage : DemoPage
{
    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/PageEmptyMessage";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Empty message", IconNames.All.CardText);
    }

    #endregion

    #region Method
    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "413";
    }
    #endregion

    #region parameters
    public bool HaveTitle { get; set; } = true;
    public bool HaveDescription { get; set; } = true;
    public bool HaveIcon { get; set; } = true;
    public string TitleMessage { get; set; } = "No results found";
    public string Description { get; set; } = "Try adjusting filter(s) to find what you are looking for";
    public string Icon { get; set; } = IconNames.All.Box;

    public void OnUpdateTitle(ChangeEventArgs args)
    {
        TitleMessage = (string)args.Value;
    }
    public void OnUpdateDescription(ChangeEventArgs args)
    {
        Description = (string)args.Value;
    }

    public string GetIcon()
    {
        if (HaveIcon)
        {
            return Icon;
        }

        return null;
    }

    public string GetTitle()
    {
        if (HaveTitle)
        {
            return TitleMessage;
        }

        return null;
    }

    public string GetDescription()
    {
        if (HaveDescription)
        {
            return Description;
        }

        return null;
    }

    #endregion
}
