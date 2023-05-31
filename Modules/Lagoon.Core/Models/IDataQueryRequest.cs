using Lagoon.Helpers.Data;

namespace Lagoon.Shared.Model;

/// <summary>
/// Data query request interface
/// </summary>
public interface IDataQueryRequest
{
    #region constant

    /// <summary>
    /// Uri data query identifier
    /// </summary>
    public const string QueryIdentifier = "$qd";

    /// <summary>
    /// Request header key attribute to identify an data query request
    /// </summary>
    public const string HeaderQueryIdentifier = "isQuery";

    #endregion

    /// <summary>
    /// Gets or sets the filter field
    /// </summary>
    string DistinctProperty { get; set; }

    /// <summary>
    /// Gets or sets selected properties
    /// </summary>
    IEnumerable<string> VisibleProperties { get; set; }

    /// <summary>
    /// Gets or sets calculation operations
    /// </summary>
    IEnumerable<CalculationOption> Calculations { get; set; }

    /// <summary>
    /// Gets or sets sorts parameters
    /// </summary>
    IEnumerable<SortOption> Sorts { get; set; }

    /// <summary>
    /// Gets or sets filters parameters
    /// </summary>
    IEnumerable<ValueFilter> Filters { get; set; }

    /// <summary>
    /// Gets or sets the lines count
    /// </summary>
    bool TotalCount { get; set; }
    /// <summary>
    /// Gets or sets index page
    /// </summary>
    int CurrentPage { get; set; }
    /// <summary>
    /// Gets or sets number of lines to return
    /// </summary>
    int PageSize { get; set; }

    /// <summary>
    /// Gets or sets if the values are distinct
    /// </summary>
    bool Disctinct { get; set; }

    /// <summary>
    /// Gets or sets if the response return only calculations results
    /// </summary>
    bool CalculationOnly { get; set; }

    /// <summary>
    /// Gets the request argument value
    /// </summary>
    object ControllerQueryArgValue { get; }

}
