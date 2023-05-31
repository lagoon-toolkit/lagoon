using Lagoon.UI.Leaflet.Components.Models.Events;

namespace Lagoon.UI.Leaflet.Components.Models;

/// <summary>
/// InteractiveLayer
/// </summary>
public abstract class InteractiveLayer : Layer
{

    /// <summary>
    /// If false, the layer will not emit mouse events and will act as a part of the underlying map. (events currently not implemented in BlazorLeaflet)
    /// </summary>
    public bool IsInteractive { get; set; } = true;

    /// <summary>
    /// When true, a mouse event on this layer will trigger the same event on the map (unless L.DomEvent.stopPropagation is used).
    /// </summary>
    public virtual bool IsBubblingMouseEvents { get; set; } = true;

    #region events
    /// <summary>
    /// MouseEventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MouseEventHandler(InteractiveLayer sender, MouseEvent e);

    /// <summary>
    /// OnClick Event
    /// </summary>
    public event MouseEventHandler OnClick;

    /// <summary>
    /// NotifyClick
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyClick(MouseEvent eventArgs)
    {
        OnClick?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// OnDblClick Event
    /// </summary>
    public event MouseEventHandler OnDblClick;

    /// <summary>
    /// NotifyDblClick
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyDblClick(MouseEvent eventArgs)
    {
        OnDblClick?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// OnMouseDown Event
    /// </summary>
    public event MouseEventHandler OnMouseDown;

    /// <summary>
    /// NotifyMouseDown
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseDown(MouseEvent eventArgs)
    {
        OnMouseDown?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// OnMouseUp Event
    /// </summary>
    public event MouseEventHandler OnMouseUp;

    /// <summary>
    /// NotifyMouseUp
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseUp(MouseEvent eventArgs)
    {
        OnMouseUp?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// OnMouseOver Event
    /// </summary>
    public event MouseEventHandler OnMouseOver;

    /// <summary>
    /// NotifyMouseOver
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseOver(MouseEvent eventArgs)
    {
        OnMouseOver?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// OnMouseOut Event
    /// </summary>
    public event MouseEventHandler OnMouseOut;

    /// <summary>
    /// NotifyMouseOut 
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseOut(MouseEvent eventArgs)
    {
        OnMouseOut?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// OnContextMenu
    /// </summary>
    public event MouseEventHandler OnContextMenu;

    /// <summary>
    /// NotifyContextMenu
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyContextMenu(MouseEvent eventArgs)
    {
        OnContextMenu?.Invoke(this, eventArgs);
    }

    #endregion

}
