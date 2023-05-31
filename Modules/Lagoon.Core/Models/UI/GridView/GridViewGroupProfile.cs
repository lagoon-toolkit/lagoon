namespace Lagoon.UI.Components;

/// <summary>
/// GridView group profile
/// </summary>
public class GridViewGroupProfile
{

    /// <summary>
    /// Gets or sets column list
    /// </summary>
    [JsonPropertyName("columns")]
    public List<string> Columns { get; set; }

    /// <summary>
    /// New instances
    /// </summary>
    /// <param name="columns">A list of column ids.</param>
    public GridViewGroupProfile(List<string> columns)
    {
        Columns = columns;
    }

}
