namespace Lagoon.UI.Components;

/// <summary>
/// Arguments of the dropping tab event
/// </summary>
public class DropTabEventArgs
{
    /// <summary>
    /// Gets unique id of the active tab
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets or sets index of the dropped tab
    /// </summary>
    public int DropIndex { get; }

    /// <summary>
    /// Initialise new instance.
    /// </summary>
    /// <param name="key">Tab identifier.</param>
    /// <param name="dropIndex">Drop position.</param>
    public DropTabEventArgs(string key, int dropIndex)
    {
        Key = key;
        DropIndex = dropIndex;
    }

}
