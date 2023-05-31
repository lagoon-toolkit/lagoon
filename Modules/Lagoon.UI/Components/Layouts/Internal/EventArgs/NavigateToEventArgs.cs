namespace Lagoon.UI.Components.Internal;

internal class NavigateToEventArgs : EventArgs
{
    #region properties

    /// <summary>
    /// Information about the page to be displayed.
    /// </summary>
    public RouteData RouteData { get; }

    /// <summary>
    /// Target URI.
    /// </summary>
    public string Uri { get; }

    #endregion

    #region constructor

    /// <summary>
    /// Initialise a new instance.
    /// </summary>
    public NavigateToEventArgs(string uri, RouteData routeData)
    {
        Uri = uri;
        RouteData = routeData;
    }

    #endregion

}
