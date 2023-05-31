namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageInputFile : DemoPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/inputfile";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "InputFile", IconNames.All.ChatLeftTextFill);
    }

    #endregion

    private readonly string _uploadUri = "api/InputFile/Upload";
    private readonly string _linkedFileListUrl = "api/InputFile/Files";

    #region parameters
    public UploadMode UploadMode { get; set; } = UploadMode.Automatic;
    public bool AllowMultipleFiles { get; set; } = true;
    public bool ShowFileList { get; set; } = true;
    public bool ShowUpload { get; set; } = true;
    public bool ShowUploadSpeed { get; set; } = true;
    public bool AllowDirectories { get; set; } = true;

    #endregion

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "440";
    }

}
