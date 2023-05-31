namespace Lagoon.UI.Demo.Pages.Form;

[Route(ROUTE)]
public partial class PageToggle : DemoPageForm
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/toggle";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Toggle", IconNames.All.ChatLeftTextFill);
    }

    #endregion


    #region fields

    public ToggleTextPosition ToggleTextPosition = ToggleTextPosition.Left;
    public string TextOn = "#lblTxtOn";
    public string TextOff = "#lblTxtOff";

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "2560";
    }

    /// <summary>
    /// Set toggle text position (left or right)
    /// </summary>
    /// <param name="textPosition">new text position</param>
    public void OnChangeTextPosition(ToggleTextPosition textPosition)
    {
        ToggleTextPosition = textPosition;
    }

    /// <summary>
    /// Set on toggle text
    /// </summary>
    /// <param name="textOn">new text</param>
    public void OnChangeTextOnAsync(string textOn)
    {
        TextOn = textOn;
    }

    /// <summary>
    /// Set off toggle text
    /// </summary>
    /// <param name="textOff">new text</param>
    public void OnChangeTextOffAsync(string textOff)
    {
        TextOff = textOff;
    }

    #endregion

}
