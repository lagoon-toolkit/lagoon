namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageSearchNavigation : DemoPage
{
    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/searchNavigation";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "#SearchNavigation", IconNames.All.Search);
    }

    #endregion

    #region fields

    /// <summary>
    /// List of FilterTextSearchMode
    /// </summary>
    private List<FilterTextSearchMode> _filterTextSearchModes;

    /// <summary>
    /// List of search navigation result items
    /// </summary>
    private List<LgPageLink> _items;

    #endregion

    #region properties

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

    #region Methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "2571";

        // Default filter search mode
        ModeFilterTextSearch = FilterTextSearchMode.Contains;
        
        // Init source list
        _items ??= new List<LgPageLink>()
            {
                Pages.PageButton.Link(),
                Pages.PageAccordion.Link(),
                Pages.PageCard.Link(),
                Pages.Gridviews.PageGridViewBasic.Link(),
                Pages.Gridviews.PageGridViewEditable.Link(),
                Pages.Gridviews.PageGridViewFilterSort.Link(),
                Pages.Gridviews.PageGridViewSelection.Link(),
                Pages.Form.PageColorPickerBox.Link(),
                Pages.Form.PageDateBox.Link(),
                Pages.PageTitle.Link(),
                Pages.PageTab.Link()
            };
        // Init list of filter text search mode
        _filterTextSearchModes = new List<FilterTextSearchMode> { FilterTextSearchMode.Contains, FilterTextSearchMode.EndsWith, FilterTextSearchMode.StartsWith };
    }

    #endregion
}
