namespace Lagoon.UI.Demo.Pages.Form;

[Route(ROUTE)]
public partial class PageSuggestBox : DemoPageForm
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/suggestbox";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "SuggestBox", IconNames.All.ChatLeftTextFill);
    }

    #endregion

    #region fields

    /// <summary>
    /// List of FilterTextSearchMode
    /// </summary>
    private List<FilterTextSearchMode> _filterTextSearchModes;

    /// <summary>
    /// List of SUGGEST result items
    /// </summary>
    private List<Color> _items;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the search text mode
    /// </summary>
    public FilterTextSearchMode ModeFilterTextSearch { get; set; }

    /// <summary>
    /// Gets or sets the highlight founded text
    /// </summary>
    public bool HighlightText { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum charters to launch the search
    /// </summary>
    public int MinSearchCharacter { get; set; }

    /// <summary>
    /// Gets or sets the maximum items number to display in the result list
    /// </summary>
    public int MaxItemsInView { get; set; } = 3;

    /// <summary>
    /// Gets or sets the see all data flag (or display short list with see all item)
    /// </summary>
    public bool SeeAllEnabled { get; set; } = true;

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "2644";

        // Default filter search mode
        ModeFilterTextSearch = FilterTextSearchMode.Contains;

        // Init source list
        _items ??= new List<Color>()
            {
            new Color(){Id=1, Label="Orange"},
            new Color(){Id=2, Label="Purple"},
            new Color(){Id=3, Label="Yellow"},
            new Color(){Id=4, Label="Red"},
            new Color(){Id=5, Label="Blue"},
            new Color(){Id=6, Label="Green"},
            new Color(){Id=7, Label="Black"},
            new Color(){Id=8, Label="White"},
            new Color(){Id=9, Label="Pink"},
            new Color(){Id=10, Label="Grey"}
            };
        // Init list of filter text search mode
        _filterTextSearchModes = new List<FilterTextSearchMode> { FilterTextSearchMode.Contains, FilterTextSearchMode.EndsWith, FilterTextSearchMode.StartsWith };

    }

    #endregion

}
public class Color
{
    public int Id;

    public string Label;

    public override string ToString()
    {
        return Label;
    }
}
