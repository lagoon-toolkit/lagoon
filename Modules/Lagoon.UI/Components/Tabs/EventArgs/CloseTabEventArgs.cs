namespace Lagoon.UI.Components;

/// <summary>
/// Arguments of the remove tab event
/// </summary>
public class CloseTabEventArgs
{
    /// <summary>
    /// Gets unique id of the active tab
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Initialise new instance.
    /// </summary>
    /// <param name="key">Tab identifier.</param>
    public CloseTabEventArgs(string key)
    {
        Key = key;
    }

}
