namespace Lagoon.UI.Leaflet.Components.Models;

/// <summary>
/// Routing control options
/// </summary>
public class RoutingControlOptions
{

    /// <summary>
    /// Initial waypoints for the control
    /// </summary>
    public LatLng[] Waypoints { get; set; }

    /// <summary>
    /// The router to use to calculate routes between waypoints
    /// </summary>
    public Router Router { get; set; } = new Router();

    /// <summary>
    /// If true, a collapse button is added, if false, no button is added, if undefined, a button is added if the screen width is small (typically mobile devices)
    /// </summary>
    public bool? Collapsible { get; set; } = null;

    /// <summary>
    /// If true, alternative polyline[s] will be shown on the map when available at the same time as the primary polyline
    /// </summary>
    public bool ShowAlternatives { get; set; } = false;
}
