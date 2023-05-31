namespace Lagoon.UI.Leaflet.Components.Models;

/// <summary>
/// OSRM Options
/// </summary>
public class Router
{
    /// <summary>
    /// Router service URL
    /// </summary>
    public string ServiceUrl{ get; set; } = "https://router.project-osrm.org/route/v1";

    /// <summary>
    /// Number of milliseconds before a route calculation times out, returning an error to the routing callback
    /// </summary>
    public float Timeout { get; set; } = 30000;

    /// <summary>
    /// The OSRM profile to use in requests
    /// </summary>
    public string Profile { get; set; } = "driving";

    /// <summary>
    /// The precision to use when decoding polylines in responses from OSRM
    /// </summary>
    public float PolylinePrecision { get; set; } = 5;

    /// <summary>
    /// Whether hints should be included in server requests
    /// </summary>
    public bool UseHints { get; set; } = false;
}