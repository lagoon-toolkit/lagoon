namespace Lagoon.UI.Components;

/// <summary>
/// Class used to store grouping data
/// </summary>
public class GroupItem<TItem>
{

    /// <summary>
    /// Group key
    /// </summary>
    public string Key { get; internal set; }

    /// <summary>
    /// Group level
    /// </summary>
    public int GroupLevel { get; internal set; }

    /// <summary>
    /// Group state (<c>true</c> for collapsed)
    /// </summary>
    public bool Closed { get; internal set; }

    /// <summary>
    /// Data in this group
    /// </summary>
    public IEnumerable<TItem> Items { get; internal set; }

}
