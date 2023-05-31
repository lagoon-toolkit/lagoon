namespace Lagoon.UI.Demo.Pages.Form;

[Route(ROUTE)]
public partial class PageRadioButton : DemoPageForm
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/radiobutton";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Radio Button", IconNames.All.ChatLeftTextFill);
    }

    #endregion


    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "345";
    }

    #region Parameters
    private RadioButtonDisplayKind _displayKind = RadioButtonDisplayKind.Classic;
    private DisplayOrientation _orientation = DisplayOrientation.Vertical;
    private void OnChangeKind(RadioButtonDisplayKind kind)
    {
        _displayKind = kind;
    }

    private void OnChangeOrientation(DisplayOrientation orientation)
    {
        _orientation = orientation;
    }

    #endregion
}
