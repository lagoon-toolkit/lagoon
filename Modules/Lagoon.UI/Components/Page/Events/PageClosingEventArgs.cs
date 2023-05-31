namespace Lagoon.UI.Components;

/// <summary>
/// Information about the page closing.
/// </summary>
public class PageClosingEventArgs : EventArgs
{

    #region properties

    /// <summary>
    /// Gets or sets if the close of the page must be cancelled.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Show a confirmation box if this message is not empty.
    /// </summary>
    public string ConfirmationMessage { get; set; }

    #endregion

}
