namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageModal : DemoPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/modal";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Modal", IconNames.All.Front);
    }

    #endregion

    private bool _firstMdoal = false;

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "44";
    }

    #region Parameters

    public bool HaveTitle { get; set; } = true;
    public bool HaveIcon { get; set; } = true;
    public bool IsClosable { get; set; } = true;
    public bool IsDefaultVisible { get; set; } = true;
    public bool IsDraggable { get; set; } = true;
    public bool IsCentered { get; set; } = true;

    public string ContentTitle { get; set; } = "Title";
    public string Icon { get; set; } = IconNames.Info;
    public ModalSize Size { get; set; } = ModalSize.Medium;

    public void OnChangeModalSize(ModalSize size)
    {
        Size = size;
    }

    #endregion

    /// <summary>
    /// Display custom modal
    /// </summary>
    public void OpenMyModal()
    {
        // Display modal
        _firstMdoal = true;
    }

    public void CloseModal()
    {
        _firstMdoal = false;
    }
}
