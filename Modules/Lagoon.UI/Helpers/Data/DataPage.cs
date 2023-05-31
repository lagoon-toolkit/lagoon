namespace Lagoon.Helpers.Data;

/// <summary>
/// Data and page context informations.
/// </summary>
/// <typeparam name="TItem">Data type.</typeparam>
public class DataPage<TItem>
{

    /// <summary>
    /// Gets the curent page index. (1 for the first page)
    /// </summary>
    public int CurrentPage { get; }

    /// <summary>
    /// Gets the item list for the current page.
    /// </summary>
    public IEnumerable<TItem> Data { get; }

    /// <summary>
    /// Gets the calculation values
    /// </summary>
    public Dictionary<string, object> CalculationValues { get; }

    /// <summary>
    /// Gets if the current page is the last page.
    /// </summary>
    public bool IsLastPage { get; }

    /// <summary>
    /// Gets last page index.
    /// </summary>
    [Obsolete("Use the PageCount property.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int LastPage => PageCount;

    /// <summary>
    /// Gets the page count.
    /// </summary>
    public int PageCount { get; }

    /// <summary>
    /// Gets the page count.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of items.
    /// </summary>
    public int RecordCount { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="currentPage"></param>
    /// <param name="data"></param>
    /// <param name="pageSize"></param>
    /// <param name="isLastPage"></param>
    /// <param name="pageCount"></param>
    /// <param name="recordCount"></param>
    /// <param name="calculationValues"></param>
    public DataPage(int currentPage, IEnumerable<TItem> data, int pageSize, bool isLastPage, int pageCount, int recordCount, Dictionary<string,object> calculationValues)
    {
        CurrentPage = currentPage;
        Data = data;
        PageSize = pageSize;
        IsLastPage = isLastPage;
        PageCount = pageCount;
        RecordCount = recordCount;
        CalculationValues = calculationValues;
    }
}
