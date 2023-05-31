namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Group parameters
/// </summary>
public class GridGroupLevel<TItem>
{
    /// <summary>
    /// Gets or sets group level name
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets group level items
    /// </summary>
    public List<TItem> Items { get; } = new List<TItem>();

    /// <summary>
    /// Gets weither the group level has items
    /// </summary>
    public bool HasItems => Items.Count > 0;

    /// <summary>
    /// Level number of the group
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets group level's sub levels
    /// </summary>
    public List<GridGroupLevel<TItem>> SubLevels { get; } = new List<GridGroupLevel<TItem>>();

    /// <summary>
    /// Gets weither the group level has sub levels
    /// </summary>
    public bool HasSubLevels => SubLevels.Count > 0;

    /// <summary>
    /// Gets or sets group level's params
    /// </summary>
    public Dictionary<string, GridGroupingColumn> Params { get; } = new Dictionary<string, GridGroupingColumn>();

    /// <summary>
    /// Gets the Items count from a level and its sub level
    /// </summary>
    public int GetItemsCount()
    {
        int count = 0;
        foreach (var subLevel in SubLevels)
        {
            count += subLevel.GetItemsCount();
        }
        count += Items.Count;

        return count;
    }
}
