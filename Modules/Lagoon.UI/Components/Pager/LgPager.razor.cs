namespace Lagoon.UI.Components;

/// <summary>
/// Pager component
/// </summary>
public partial class LgPager : LgComponentBase
{
    #region Fields 

    private int _totalPages;
    private int _currentPage;
    private int _maxPagesToDisplay = 5;

    private PagerHelper _pagerHelper;

    #endregion Fields

    #region Parameters

    /// <summary>
    /// Gets or sets a callback that updates the bound value.
    /// </summary>
    [Parameter]
    public EventCallback<ChangeEventArgs> OnChange { get; set; }

    /// <summary>
    /// Gets or sets the total pages.
    /// </summary>
    [Parameter]
    public int TotalPages
    {
        get => _totalPages;
        set
        {
            _totalPages = value;
            PagerHelper.CalculatePagination();
        }
    }

    /// <summary>
    /// Gets or sets the current page.
    /// </summary>
    [Parameter]
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            PagerHelper.CalculatePagination();
        }
    }

    /// <summary>
    /// Gets or sets the max pages to display.
    /// </summary>
    [Parameter]
    public int MaxPagesToDisplay
    {
        get => _maxPagesToDisplay;
        set
        {
            _maxPagesToDisplay = value;
            PagerHelper.CalculatePagination();
        }
    }

    /// <summary>
    /// Display text (First,Last,Next,Previous) if true
    /// </summary>
    [Parameter]
    public bool DisplayText { get; set; }

    /// <summary>
    /// Gets or sets the Root container class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// True if total pages is unknown
    /// </summary>
    [Parameter]
    public bool IsTotalPagesUnknown { get; set; }

    /// <summary>
    /// True to disable next button
    /// Only used if total pages is unknown
    /// </summary>
    [Parameter]
    public bool DisableNextButton { get; set; }

    #endregion Parameters

    /// <summary>
    /// Pager helper
    /// </summary>
    private PagerHelper PagerHelper => _pagerHelper ?? (_pagerHelper = new PagerHelper(this));

    /// <summary>
    /// invoked when page change
    /// </summary>
    /// <param name="selectedPage">selected page</param>
    private async Task OnChangeAsync(int selectedPage)
    {
        CurrentPage = selectedPage;
        if (OnChange.HasDelegate)
        {
            await OnChange.TryInvokeAsync(App, new ChangeEventArgs() { Value = selectedPage });
        }
    }
}

/// <summary>
/// Pager helper
/// </summary>
internal class PagerHelper
{
    private readonly LgPager _lgPager;
    internal PagerHelper(LgPager lgPager)
    {
        _lgPager = lgPager;
    }

    internal void CalculatePagination()
    {
        int totalPages = _lgPager.TotalPages;
        int currentPage = _lgPager.CurrentPage;
        int maxPagesToDisplay = _lgPager.MaxPagesToDisplay;

        int startPage;
        int endPage;
        TotalPages = totalPages;
        int maxPages = maxPagesToDisplay - 2;

        if (currentPage < 1)
        {
            currentPage = 1;
        }
        else if (currentPage > totalPages)
        {
            currentPage = totalPages;
        }

        if (maxPages < 1)
        {
            maxPages = 1;
        }
        else if (maxPages > totalPages)
        {
            maxPages = totalPages;
        }

        CurrentPage = currentPage;

        if (maxPages == totalPages)
        {
            StartPage = 1;
            EndPage = totalPages;
        }
        else
        {
            int left = (int)Math.Ceiling(maxPages / (decimal)2);
            int right = (int)Math.Floor(maxPages / (decimal)2);
            if (currentPage <= left)
            {
                startPage = 1;
                endPage = maxPages;
            }
            else if (currentPage + right >= totalPages)
            {
                startPage = totalPages - maxPages + 1;
                endPage = totalPages;
            }
            else
            {
                startPage = currentPage - left + 1;
                endPage = currentPage + right;
            }

            if (startPage <= 1)
            {
                startPage = 2;
                endPage++;
            }
            else if (endPage >= totalPages && startPage > 2)
            {
                startPage--;
            }

            StartPage = startPage;
            EndPage = endPage >= totalPages ? totalPages - 1 : endPage;

            if (HideLeftPages)
            {
                StartPage++;
            }

            if (HideRightPages)
            {
                EndPage--;
            }
        }
    }

    /// <summary>
    /// Start page
    /// </summary>
    public int StartPage { get; set; }

    /// <summary>
    /// End page
    /// </summary>
    public int EndPage { get; set; }

    /// <summary>
    /// Current page
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Total Pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Return true to hide left pages
    /// </summary>
    public bool HideLeftPages => StartPage > 2;

    /// <summary>
    /// Return true to right pages
    /// </summary>
    public bool HideRightPages => EndPage < TotalPages - 1;

}
