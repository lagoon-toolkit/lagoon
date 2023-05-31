namespace Lagoon.UI.Components;

/// <summary>
/// Column option class
/// </summary>
public class GroupOption
{
    /// <summary>
    /// Gets the DOM element id.
    /// </summary>
    internal string ElementId { get; } = LgComponentBase.GetNewElementId();

    /// <summary>
    /// Gets or sets columns keys
    /// </summary>
    public List<string> Columns { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets if the group is removable
    /// </summary>
    public bool IsRemovable { get; set; } = true;

}
