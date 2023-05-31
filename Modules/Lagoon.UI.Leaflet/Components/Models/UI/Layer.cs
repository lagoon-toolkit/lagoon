using Lagoon.UI.Leaflet.Components.Models.Events;
using Lagoon.UI.Leaflet.Helpers;

namespace Lagoon.UI.Leaflet.Components.Models;

/// <summary>
/// Layer.
/// </summary>
public abstract class Layer
{
    /// <summary>
    /// Unique identifier used by the interoperability service on the client side to identify layers.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// By default the layer will be added to the map's overlay pane. Overriding this option will cause the layer to be placed on another pane by default.
    /// </summary>
    public virtual string Pane { get; set; } = "overlayPane";

    /// <summary>
    /// String to be shown in the attribution control, e.g. "© OpenStreetMap contributors". It describes the layer data and is often a legal obligation towards copyright holders and tile providers.
    /// </summary>
    public string Attribution { get; set; }

    /// <summary>
    /// The tooltip assigned to this marker.
    /// </summary>
    public Tooltip Tooltip { get; set; }

    /// <summary>
    /// The popup shown when the marker is clicked.
    /// </summary>
    public Popup Popup { get; set; }

    /// <summary>
    /// Construcor
    /// </summary>
    protected Layer()
    {
        Id = StringHelper.GetRandomString(20);
    }

    #region events
    /// <summary>
    /// EventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void EventHandler(Layer sender, Event e);
    /// <summary>
    /// OnAdd event
    /// </summary>
    public event EventHandler OnAdd;

    /// <summary>
    /// NotifyAdd
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyAdd(Event eventArgs)
    {
        OnAdd?.Invoke(this, eventArgs);
    }
    /// <summary>
    /// OnRemove event
    /// </summary>
    public event EventHandler OnRemove;

    /// <summary>
    /// NotifyRemove
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyRemove(Event eventArgs)
    {
        OnRemove?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// PopupEventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PopupEventHandler(Layer sender, PopupEvent e);

    /// <summary>
    /// OnPopupOpen event
    /// </summary>
    public event PopupEventHandler OnPopupOpen;

    /// <summary>
    /// NotifyPopupOpen
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyPopupOpen(PopupEvent eventArgs)
    {
        OnPopupOpen?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// OnPopupClose event
    /// </summary>
    public event PopupEventHandler OnPopupClose;

    /// <summary>
    /// NotifyPopupClose
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyPopupClose(PopupEvent eventArgs)
    {
        OnPopupClose?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// TooltipEventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TooltipEventHandler(Layer sender, TooltipEvent e);

    /// <summary>
    /// TooltipEventHandler event
    /// </summary>
    public event TooltipEventHandler OnTooltipOpen;

    /// <summary>
    /// NotifyTooltipOpen
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyTooltipOpen(TooltipEvent eventArgs)
    {
        OnTooltipOpen?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// OnTooltipClose event
    /// </summary>
    public event TooltipEventHandler OnTooltipClose;

    /// <summary>
    /// NotifyTooltipClose
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyTooltipClose(TooltipEvent eventArgs)
    {
        OnTooltipClose?.Invoke(this, eventArgs);
    }

    #endregion
}
