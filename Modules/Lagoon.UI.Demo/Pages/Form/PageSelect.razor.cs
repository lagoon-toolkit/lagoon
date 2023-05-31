namespace Lagoon.UI.Demo.Pages.Form;

[Route(ROUTE)]
public partial class PageSelect : DemoPageForm
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/select";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Select", IconNames.All.ChatLeftTextFill);
    }

    #endregion

    #region parameters

    public bool ShowSearchBox { get; set; }

    public int? VisibleItemCount { get; set; }

    #endregion

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "348";
        // GG TODO TO REMOVE
        formData.ColorsMultiple = formData.Colors.Select(x => x.value).ToList();
    }

}
