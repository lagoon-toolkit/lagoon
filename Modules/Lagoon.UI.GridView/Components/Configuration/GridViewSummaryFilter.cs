
namespace Lagoon.UI.GridView.Components.Internal;

/// <summary>
/// Summary filters
/// </summary>
public class GridViewSummaryFilter
{
   
    /// <summary>
    /// Filter name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Filter value
    /// </summary>
    public string Value { get; set; }


    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="value">The value of the filter.</param>
    public GridViewSummaryFilter(string name, string value)
    {
        Name = name;
        Value = value;
    }
}
