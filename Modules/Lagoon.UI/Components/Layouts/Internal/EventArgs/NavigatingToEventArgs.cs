namespace Lagoon.UI.Components.Internal;

internal class NavigatingToEventArgs : EventArgs
{
    #region properties

    /// <summary>
    /// Indicate if the navigation must be cancelled.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Target URI.
    /// </summary>
    public string Uri { get; }

    #endregion

    #region constructor

    /// <summary>
    /// Initialise a new instance.
    /// </summary>
    public NavigatingToEventArgs(string uri)
    {
        Uri = uri;
    }

    #endregion

}
