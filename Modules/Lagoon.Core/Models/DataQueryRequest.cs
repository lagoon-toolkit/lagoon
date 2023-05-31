using Lagoon.Helpers.Data;

namespace Lagoon.Shared.Model;

/// <summary>
/// Data query request
/// </summary>
public class DataQueryRequest<TArg> : IDataQueryRequest
{

    #region properties

    /// <summary>
    /// Gets or sets the filter field
    /// </summary>
    public string DistinctProperty { get; set; }

    /// <summary>
    /// Gets or sets selected properties
    /// </summary>
    public IEnumerable<string> VisibleProperties { get; set; }

    /// <summary>
    /// Gets or sets calculation operations
    /// </summary>
    public IEnumerable<CalculationOption> Calculations { get; set; }

    /// <summary>
    /// Gets or sets sorts parameters
    /// </summary>
    public IEnumerable<SortOption> Sorts { get; set; }

    /// <summary>
    /// Gets or sets filters parameters
    /// </summary>
    public IEnumerable<ValueFilter> Filters { get; set; }

    /// <summary>
    /// Gets or sets the lines count
    /// </summary>
    public bool TotalCount { get; set; }
    /// <summary>
    /// Gets or sets index page
    /// </summary>
    public int CurrentPage { get; set; }
    /// <summary>
    /// Gets or sets number of lines to return
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets if the values are distinct
    /// </summary>
    public bool Disctinct { get; set; }

    /// <summary>
    /// Gets or sets if the response return only calculations results
    /// </summary>
    public bool CalculationOnly { get; set; }

    /// <summary>
    /// Gets or set the serialized complementary argument
    /// </summary>
    public TArg ControllerQueryArg { get; set; }

    /// <summary>
    /// Gets the request argument
    /// </summary>
    object IDataQueryRequest.ControllerQueryArgValue => ControllerQueryArg;

    #endregion

}
