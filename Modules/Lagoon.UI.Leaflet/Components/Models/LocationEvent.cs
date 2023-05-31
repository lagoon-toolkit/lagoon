using Lagoon.UI.Leaflet.Components.Models;
using Lagoon.UI.Leaflet.Components.Models.Events;

namespace Lagoon.UI.Leaflet.Components;

/// <summary>
/// Location Event
/// </summary>
public class LocationEvent : Event
{
    /// <summary>
    /// Detected geographical location of the user.
    /// </summary>
    public LatLng LatLng { get; set; }

    /// <summary>
    /// Accuracy of location in meters.
    /// </summary>
    public float Accuracy { get; set; }

    /// <summary>
    /// Height of the position above the WGS84 ellipsoid in meters.
    /// </summary>
    public float Altitude { get; set; }

    /// <summary>
    /// Accuracy of altitude in meters.
    /// </summary>
    public float AltitudeAccuracy { get; set; }

    /// <summary>
    /// The direction of travel in degrees counting clockwise from true North.
    /// </summary>
    public float Heading { get; set; }

    /// <summary>
    /// Current velocity in meters per second.
    /// </summary>
    public float Speed { get; set; }

    /// <summary>
    /// The time when the position was acquired.
    /// </summary>
    public float Timestamp { get; set; }
}
