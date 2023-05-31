namespace Lagoon.UI.Components;

/// <summary>
/// Column option class
/// </summary>
public class ColumnOption
{
    /// <summary>
    /// Gets the DOM element id.
    /// </summary>
    internal string ElementId { get; } = LgComponentBase.GetNewElementId();

    /// <summary>
    /// Gets or sets column id
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets if the column is removable
    /// </summary>
    public bool IsRemovable { get; set; } = true;

    /// <summary>
    /// Gets or sets if the column is displayed
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets if the column is frozen.
    /// </summary>
    public bool IsFrozen { get; set; }

    /// <summary>
    /// Gets or sets if the column can be grouped
    /// </summary>
    public bool IsGroupable { get; set; }

}
