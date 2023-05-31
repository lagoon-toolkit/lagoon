using Lagoon.UI.Demo.ViewModel;

namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageButton : DemoPage
{
    protected FormData formData = new() { };

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/button";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Buttons", IconNames.All.MenuButton);
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "43";
    }

    #endregion

    #region parameters
    public bool IsDisabled { get; set; } = false;
    public bool HaveIcon { get; set; } = true;
    public string Text { get; set; } = "label";
    public string Icon { get; set; } = IconNames.Admin;
    public ButtonSize Size { get; set; } = ButtonSize.Medium;
    public ButtonKind Kind { get; set; } = ButtonKind.Primary;

    public void OnUpdateText(ChangeEventArgs args)
    {
        Text = (string)args.Value;
    }

    #endregion

    public void OnChangeButtonSize(ButtonSize size)
    {
        Size = size;
    }
    public void OnChangeButtonKind(ButtonKind kind)
    {
        Kind = kind;
    }

    public string GetIcon()
    {
        if (HaveIcon)
        {
            return Icon;
        }

        return null;
    }
}