namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageFrame : DemoPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/frame";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Frames", IconNames.All.Square);
    }

    #endregion

    #region Classes

    private class FrameState
    {
        /// <summary>
        /// Search ter into list
        /// </summary>
        public bool Enabled { get; set; }
    }

    #endregion

    #region Initialization

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "47";
    }

    #endregion

    #region parameters
    public bool HaveTitle { get; set; } = true;
    public bool HaveIcon { get; set; } = true;
    public bool HaveCollapsed { get; set; } = true;
    public bool HaveToolbar { get; set; } = true;
    public string ContentTitle { get; set; } = "Title";
    public string Icon { get; set; } = IconNames.Info;
    public Kind Kind { get; set; } = Kind.Default;
    public string ContentCard { get; set; } = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";

    public string GetTitle()
    {
        if (HaveTitle)
        {
            return ContentTitle;
        }

        return null;
    }

    public string GetIcon()
    {
        if (HaveIcon)
        {
            return Icon;
        }

        return null;
    }
    public void OnUpdateTitle(ChangeEventArgs args)
    {
        ContentTitle = (string)args.Value;
    }

    public void OnChangeKind(Kind kind)
    {
        Kind = kind;
    }

    #endregion

}
