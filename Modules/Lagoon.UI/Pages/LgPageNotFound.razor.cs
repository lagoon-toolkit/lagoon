namespace Lagoon.UI.Pages;

/// <summary>
/// Page to download application logs.
/// </summary>
public partial class LgPageNotFound : LgPage
{
    #region fields

    private static readonly string _title = "#pgNotFoundTitle";
    private static readonly string _iconName = IconNames.Error;

    #endregion

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    internal const string ROUTE = "404";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, _title, _iconName);
    }

    #endregion

    #region cascading parameter

    /// <summary>
    /// LgApp instance.
    /// </summary>
    [CascadingParameter]
    public LgApp AppComponent { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Initialization
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await SetTitleAsync(Link());
    }

    #endregion

}
