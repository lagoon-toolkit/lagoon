namespace Lagoon.UI.Leaflet.Components.Models.Events;

/// <summary>
/// ErrorEvent
/// </summary>
public class ErrorEvent : Event
{
    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// Code
    /// </summary>
    public int Code { get; set; }
}
