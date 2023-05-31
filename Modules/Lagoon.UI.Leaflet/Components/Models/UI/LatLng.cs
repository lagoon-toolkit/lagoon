using System.Drawing;

namespace Lagoon.UI.Leaflet.Components.Models;

/// <summary>
/// LatLng
/// </summary>
public class LatLng
{
    /// <summary>
    /// Lat
    /// </summary>
    public float Lat { get; set; }

    /// <summary>
    /// Lng
    /// </summary>
    public float Lng { get; set; }

    /// <summary>
    /// Alt
    /// </summary>
    public float Alt { get; set; }

    /// <summary>
    /// ToPointF
    /// </summary>
    /// <returns></returns>
    public PointF ToPointF()
    {
        return new PointF(Lat, Lng);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public LatLng() { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="position"></param>
    public LatLng(PointF position) : this(position.X, position.Y) { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="lat"></param>
    /// <param name="lng"></param>
    public LatLng(float lat, float lng)
    {
        Lat = lat;
        Lng = lng;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="lat"></param>
    /// <param name="lng"></param>
    /// <param name="alt"></param>
    public LatLng(float lat, float lng, float alt) : this(lat, lng)
    {
        Alt = alt;
    }
}
