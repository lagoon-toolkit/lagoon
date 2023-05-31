using Lagoon.Helpers.Data;

namespace Lagoon.UI.Components.Internal;

internal class GridViewPaginationState<T> : GridViewDataCounter
{
    #region fields

    private IEnumerable<T> _data;
    private int? _activeRows;

    #endregion

    #region properties

    /// <summary>
    /// Gets displayed rows number
    /// </summary>
    /// <value></value>
    public override int ActiveRows
    {
        get
        {
            // We do the Count only the first time the property is used.
            if (!_activeRows.HasValue)
            {
                _activeRows = _data.Count();
            }
            return _activeRows.Value;
        }
    }

    /// <summary>
    /// Gets or sets current active page
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets if the last page is selected.
    /// </summary>
    public bool IsLastPage { get; }

    /// <summary>
    /// True if total pages is unknown
    /// </summary>
    internal bool IsTotalPagesUnknown => TotalRows < 0;

    /// <summary>
    /// Gets or sets current page size
    /// </summary>
    public int PageSize { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance
    /// </summary>
    public GridViewPaginationState() : base(-1, -1)
    {
        _activeRows = -1;
        CurrentPage = 1;
    }

    public GridViewPaginationState(DataPage<T> dataPage) : base(dataPage.PageCount, dataPage.RecordCount)
    {
        CurrentPage = dataPage.CurrentPage;
        PageSize = dataPage.PageSize;
        IsLastPage = dataPage.IsLastPage;
        _data = dataPage.Data;
    }

    #endregion

}
