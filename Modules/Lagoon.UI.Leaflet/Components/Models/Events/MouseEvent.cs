using System.Drawing;

namespace Lagoon.UI.Leaflet.Components.Models.Events;

/// <summary>
/// MouseEvent
/// </summary>
public class MouseEvent : Event
{
    /// <summary>
    /// LatLng
    /// </summary>
    public LatLng LatLng { get; set; }
    /// <summary>
    /// LayerPoint
    /// </summary>
    public PointF LayerPoint { get; set; }
    /// <summary>
    /// ContainerPoint
    /// </summary>
    public PointF ContainerPoint { get; set; }
}
