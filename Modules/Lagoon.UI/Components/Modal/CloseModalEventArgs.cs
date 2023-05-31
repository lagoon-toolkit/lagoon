namespace Lagoon.UI.Components;

/// <summary>
/// Contains close modal event data.
/// </summary>
public class CloseModalEventArgs : EventArgs
{
    /// <summary>
    /// Indicate if window closing must be cancelled.
    /// </summary>
    public bool Cancel { get; set; }
}
