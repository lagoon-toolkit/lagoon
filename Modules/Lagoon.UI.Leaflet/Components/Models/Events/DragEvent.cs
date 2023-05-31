namespace Lagoon.UI.Leaflet.Components.Models.Events;

/// <summary>
/// DragEvent
/// </summary>
public class DragEvent : Event
{
    /// <summary>
    /// LatLng
    /// </summary>
    public LatLng LatLng { get; set; }

    /// <summary>
    /// OldLatLng
    /// </summary>
    public LatLng OldLatLng { get; set; }
}
