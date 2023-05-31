namespace Lagoon.UI.Components;

/// <summary>
/// Arguments of the pinned tab event
/// </summary>
public class PinTabEventArgs
{
    /// <summary>
    /// Gets unique id of the active tab
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Initialise new instance.
    /// </summary>
    /// <param name="key">Tab identifier.</param>
    public PinTabEventArgs(string key)
    {
        Key = key;
    }

}
