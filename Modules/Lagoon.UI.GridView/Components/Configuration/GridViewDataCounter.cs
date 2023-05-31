namespace Lagoon.UI.Components;


/// <summary>
/// Rows, pages numbers
/// </summary>
public class GridViewDataCounter
{

    #region properties

    /// <summary>
    /// Gets displayed rows number
    /// </summary>
    /// <value></value>
    public virtual int ActiveRows { get; }

    /// <summary>
    /// Gets displayed pages number
    /// </summary>
    /// <value></value>
    public int TotalPages { get; }

    /// <summary>
    /// Gets total rows number
    /// </summary>
    /// <value></value>
    public int TotalRows { get; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public GridViewDataCounter()
    {
    }

    /// <summary>
    /// New instance
    /// </summary>
    /// <param name="totalPages">The total page count.</param>
    /// <param name="totalRows">The total row count.</param>
    internal GridViewDataCounter(int totalPages, int totalRows)
    {
        TotalPages = totalPages;
        TotalRows = totalRows;
    }

    #endregion

}
