using System.Drawing;

namespace Lagoon.UI.Leaflet.Components.Models.Events;

/// <summary>
/// ResizeEvent
/// </summary>
public class ResizeEvent : Event
{
    /// <summary>
    /// OldSize
    /// </summary>
    public PointF OldSize { get; set; }
    /// <summary>
    /// NewSize
    /// </summary>
    public PointF NewSize { get; set; }
}
