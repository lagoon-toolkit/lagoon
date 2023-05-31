namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageToastr : DemoPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/toastr";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Toaster", IconNames.All.MegaphoneFill);
    }

    #endregion

    #region parameters

    public ToastrKind ToastrKind { get; set; }

    public void GetOnClickEvent()
    {
        if (ToastrKind == ToastrKind.Error)
        {
            ShowError("Message", "Title");
        }

        if (ToastrKind == ToastrKind.Success)
        {
            ShowSuccess("Message", "Title");
        }

        if (ToastrKind == ToastrKind.Warning)
        {
            ShowWarning("Message", "Title");
        }

        if (ToastrKind == ToastrKind.Info)
        {
            ShowInformation("Message", "Title");
        }
    }
    #endregion

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "825";
    }

}

public enum ToastrKind
{
    /// <summary>
    /// Success kind.
    /// </summary>
    Success,
    /// <summary>
    /// Error kind.
    /// </summary>
    Error,
    /// <summary>
    /// Warning kind.
    /// </summary>
    Warning,
    /// <summary>
    /// Information kind.
    /// </summary>
    Info
}
