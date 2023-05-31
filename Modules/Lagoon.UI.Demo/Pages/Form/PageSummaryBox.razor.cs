namespace Lagoon.UI.Demo.Pages.Form;

[Route(ROUTE)]
public partial class PageSummaryBox : DemoPageForm
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/summarybox";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "SummaryBox", IconNames.All.ChatLeftTextFill);
    }

    #endregion

    #region parameters
    public bool HasPrefix { get; set; } = true;
    public bool HasSuffix { get; set; } = true;
    public string IconPrefix { get; set; } = IconNames.All.Search;
    public string IconSuffix { get; set; } = IconNames.All.Search;
    public bool Wrapped { get; set; }

    public string GetPrefix()
    {
        if (HasPrefix)
        {
            return IconPrefix;
        }

        return null;
    }
    public string GetSuffix()
    {
        if (HasSuffix)
        {
            return IconSuffix;
        }

        return null;
    }

    #endregion

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        formData.LongText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam rhoncus maximus faucibus. Morbi rhoncus aliquam lorem convallis pharetra. Proin arcu ligula, sodales vel hendrerit ac, porta at est. Vestibulum dignissim, sem quis scelerisque aliquam, risus velit imperdiet lectus, eget lobortis est diam vel risus. Fusce fermentum euismod arcu et consequat. Nullam sed eros lobortis, mollis augue imperdiet, eleifend quam. Donec in ipsum massa. Sed dictum aliquam neque, in suscipit neque ultrices eget. Nulla facilisi. Praesent ultrices, arcu eget hendrerit condimentum, lorem sapien vestibulum turpis, eu egestas massa neque ac dolor. Pellentesque vehicula maximus viverra. Phasellus imperdiet eros sit amet turpis viverra, eu vulputate orci luctus. Donec facilisis felis vitae laoreet rhoncus.";
        SetTitle(Link());
        DocumentationComponent = "1476";
    }

}
