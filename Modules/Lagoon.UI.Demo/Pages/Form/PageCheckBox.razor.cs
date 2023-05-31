namespace Lagoon.UI.Demo.Pages.Form;

[Route(ROUTE)]
public partial class PageCheckBox : DemoPageForm
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/checkbox";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "CheckBox", IconNames.All.ChatLeftTextFill);
    }

    #endregion

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "346";
    }

    #region parameters
    public CheckBoxKind CheckBoxType { get; set; } = CheckBoxKind.Check;
    public CheckBoxTextPosition CheckBoxTextPosition = CheckBoxTextPosition.Left;
    public DisplayOrientation CheckBoxListOrientation = DisplayOrientation.Vertical;

    public void OnChangeKindCheckbox(CheckBoxKind kind)
    {
        CheckBoxType = kind;
    }

    public void OnChangeTextPosition(CheckBoxTextPosition textPosition)
    {
        CheckBoxTextPosition = textPosition;
    }

    #endregion

}
