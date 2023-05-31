namespace Lagoon.UI.Components;

/// <summary>
/// Arguments of the remove tab event
/// </summary>
public class ClosingTabEventArgs
{

    /// <summary>
    /// Gets or sets if the tab close must be cancelled.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Gets unique id of the active tab
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Initialise new instance.
    /// </summary>
    /// <param name="key">Tab identifier.</param>
    public ClosingTabEventArgs(string key)
    {
        Key = key;
    }

}
