namespace Lagoon.UI.Leaflet.Components.Models;

/// <summary>
/// Circle.
/// </summary>
public class Circle : Path
{
    /// <summary>
    /// Center of the circle.
    /// </summary>
    public LatLng Position { get; set; }

    /// <summary>
    /// Radius of the circle, in meters.
    /// </summary>
    public float Radius { get; set; }

}
