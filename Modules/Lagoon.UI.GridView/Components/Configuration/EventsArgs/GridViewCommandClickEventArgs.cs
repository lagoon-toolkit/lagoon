namespace Lagoon.UI.Components;

/// <summary>
/// Command click event arguments
/// </summary>
public class GridViewCommandClickEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Gets command's name of the cell
    /// </summary>
    /// <value></value>
    public string Command { get; }

    /// <summary>
    /// Gets object of the row
    /// </summary>
    /// <value></value>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TItem Data => Item;

    /// <summary>
    /// Gets object of the row
    /// </summary>
    /// <value></value>
    public TItem Item { get; }

    /// <summary>
    /// Gets or sets if gridview data are reloaded
    /// </summary>
    public bool Refresh { get; set; }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="command">Command</param>
    /// <param name="item">Item</param>
    public GridViewCommandClickEventArgs(string command, TItem item)
    {
        Command = command;
        Item = item;
    }

}
