using Lagoon.UI.Leaflet.Components.Models.Events;
using System.Drawing;

namespace Lagoon.UI.Leaflet.Components.Models;

/// <summary>
/// Marker
/// </summary>
public class Marker : InteractiveLayer
{
    /// <summary>
    /// The position of the marker on the map.
    /// </summary>
    public LatLng Position { get; set; }

    /// <summary>
    /// Icon instance to use for rendering the marker. See <see href="https://leafletjs.com/reference-1.5.0.html#icon">Icon documentation</see> for details on how to customize the marker icon. If not specified, a common instance of <see href="https://leafletjs.com/reference-1.5.0.html#icon-default">L.Icon.Default</see> is used.
    /// </summary>
    public Icon Icon { get; set; }

    /// <summary>
    /// Whether the marker can be tabbed to with a keyboard and clicked by pressing enter.
    /// </summary>
    public bool IsKeyboardAccessible { get; set; } = true;

    /// <summary>
    /// Text for the browser tooltip that appear on marker hover (no tooltip by default).
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Text for the alt attribute of the icon image (useful for accessibility).
    /// </summary>
    public string Alt { get; set; } = string.Empty;

    /// <summary>
    /// By default, marker images zIndex is set automatically based on its latitude. Use this option if you want to put the marker on top of all others (or below), specifying a high value like 1000 (or high negative value, respectively).
    /// </summary>
    public int ZIndexOffset { get; set; }

    /// <summary>
    /// The opacity of the marker.
    /// </summary>
    public double Opacity { get; set; } = 1.0;

    /// <summary>
    /// If true, the marker will get on top of others when you hover the mouse over it.
    /// </summary>
    public bool RiseOnHover { get; set; }

    /// <summary>
    /// The z-index offset used for the riseOnHover feature.
    /// </summary>
    public int RiseOffset { get; set; } = 250;

    /// <inheritdoc/>
    public override string Pane { get; set; } = "markerPane";

    /// <inheritdoc/>
    public override bool IsBubblingMouseEvents { get; set; } = false;

    /// <summary>
    /// Whether the marker is draggable with mouse/touch or not.
    /// </summary>
    public bool Draggable { get; set; }

    /// <summary>
    /// Whether to pan the map when dragging this marker near its edge or not.
    /// </summary>
    public bool UseAutoPan { get; set; }

    /// <summary>
    /// Distance (in pixels to the left/right and to the top/bottom) of the map edge to start panning the map.
    /// </summary>
    public Point AutoPanPadding { get; set; } = new Point(50, 50);

    /// <summary>
    /// Number of pixels the map should pan by.
    /// </summary>
    public int AutoPanSpeed { get; set; } = 10;

    /// <summary>
    /// Contructor
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Marker(float x, float y) : this(new LatLng(x, y)) { }

    /// <summary>
    /// Contructor
    /// </summary>
    /// <param name="position"></param>
    public Marker(PointF position) : this(position.X, position.Y) { }

    /// <summary>
    /// Contructor
    /// </summary>
    /// <param name="latLng"></param>
    public Marker(LatLng latLng)
    {
        Position = latLng;
    }

    #region events

    /// <summary>
    /// DragEventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DragEventHandler(Marker sender, DragEvent e);

    /// <summary>
    /// OnMove event
    /// </summary>
    public event DragEventHandler OnMove;

    /// <summary>
    /// NotifyMove
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMove(DragEvent eventArgs)
    {
        OnMove?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// EventHandlerMarker
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void EventHandlerMarker(Marker sender, Event e);

    /// <summary>
    /// OnDragStart event
    /// </summary>
    public event EventHandlerMarker OnDragStart;

    /// <summary>
    /// NotifyDragStart
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyDragStart(Event eventArgs)
    {
        OnDragStart?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// OnMoveStart event
    /// </summary>
    public event EventHandlerMarker OnMoveStart;

    /// <summary>
    /// NotifyMoveStart
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMoveStart(Event eventArgs)
    {
        OnMoveStart?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// OnDrag event
    /// </summary>
    public event DragEventHandler OnDrag;

    /// <summary>
    /// NotifyDrag
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyDrag(DragEvent eventArgs)
    {
        OnDrag?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// DragEndEventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DragEndEventHandler(Marker sender, DragEndEvent e);

    /// <summary>
    /// OnDragEnd event
    /// </summary>
    public event DragEndEventHandler OnDragEnd;

    /// <summary>
    /// NotifyDragEnd
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyDragEnd(DragEndEvent eventArgs)
    {
        OnDragEnd?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// OnMoveEnd event
    /// </summary>
    public event EventHandlerMarker OnMoveEnd;

    /// <summary>
    /// NotifyMoveEnd
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMoveEnd(Event eventArgs)
    {
        OnMoveEnd?.Invoke(this, eventArgs);
    }

    #endregion

}
