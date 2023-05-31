using System.Collections.ObjectModel;
using System.Drawing;
using Lagoon.UI.Leaflet.Components.Models.Events;
using Lagoon.UI.Leaflet.Components.Models;
using System.Collections.Specialized;
//using BlazorLeaflet.Exceptions;
using Microsoft.AspNetCore.Components.Web;
using Lagoon.UI.Leaflet.Helpers;

namespace Lagoon.UI.Leaflet.Components;

/// <summary>
/// The Map class
/// </summary>
public class Map
{
    #region privates

    /// <summary>
    /// Runtime JS.
    /// </summary>
    private readonly IJSInProcessRuntime _jsRuntime;

    /// <summary>
    /// Map is initialized
    /// </summary>
    private bool _isInitialized;

    /// <summary>
    /// Get the list of layer
    /// </summary>
    private ObservableCollection<Layer> _layers = new();

    #endregion

    #region properties

    /// <summary>
    /// Initial geographic center of the map
    /// </summary>
    public LatLng Center { get; set; } = new();

    /// <summary>
    /// Initial map zoom level
    /// </summary>
    public float Zoom { get; set; }

    /// <summary>
    /// Minimum zoom level of the map. If not specified and at least one 
    /// GridLayer or TileLayer is in the map, the lowest of their minZoom
    /// options will be used instead.
    /// </summary>
    public float? MinZoom { get; set; }

    /// <summary>
    /// Maximum zoom level of the map. If not specified and at least one
    /// GridLayer or TileLayer is in the map, the highest of their maxZoom
    /// options will be used instead.
    /// </summary>
    public float? MaxZoom { get; set; }

    /// <summary>
    /// When this option is set, the map restricts the view to the given
    /// geographical bounds, bouncing the user back if the user tries to pan
    /// outside the view.
    /// </summary>
    public Tuple<LatLng, LatLng> MaxBounds { get; set; }

    /// <summary>
    /// Whether a zoom control is added to the map by default.
    /// <para/>
    /// Defaults to true.
    /// </summary>
    public bool ZoomControl { get; set; } = true;

    /// <summary>
    /// Event raised when the component has finished its first render.
    /// </summary>
    public event Action OnInitialized;

    /// <summary>
    /// Get the Id
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// MapEventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MapEventHandler(object sender, Event e);
    /// <summary>
    /// MapResizeEventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MapResizeEventHandler(object sender, ResizeEvent e);
    /// <summary>
    /// MouseEventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MouseEventHandler(Map sender, MouseEvent e);

    /// <summary>
    /// OnZoomLevelsChange
    /// </summary>
    public event MapEventHandler OnZoomLevelsChange;
    /// <summary>
    /// OnResize
    /// </summary>
    public event MapResizeEventHandler OnResize;
    /// <summary>
    /// OnUnload
    /// </summary>
    public event MapEventHandler OnUnload;
    /// <summary>
    /// OnViewReset
    /// </summary>
    public event MapEventHandler OnViewReset;
    /// <summary>
    /// OnLoad
    /// </summary>
    public event MapEventHandler OnLoad;
    /// <summary>
    /// OnZoomStart
    /// </summary>
    public event MapEventHandler OnZoomStart;
    /// <summary>
    /// OnMoveStart
    /// </summary>
    public event MapEventHandler OnMoveStart;
    /// <summary>
    /// OnZoom
    /// </summary>
    public event MapEventHandler OnZoom;
    /// <summary>
    /// OnMove
    /// </summary>
    public event MapEventHandler OnMove;
    /// <summary>
    /// OnZoomEnd
    /// </summary>
    public event MapEventHandler OnZoomEnd;
    /// <summary>
    /// OnMoveEnd
    /// </summary>
    public event MapEventHandler OnMoveEnd;
    /// <summary>
    /// OnMouseMove
    /// </summary>
    public event MouseEventHandler OnMouseMove;
    /// <summary>
    /// OnKeyPress
    /// </summary>
    public event MapEventHandler OnKeyPress;
    /// <summary>
    /// OnKeyDown
    /// </summary>
    public event MapEventHandler OnKeyDown;
    /// <summary>
    /// OnKeyUp
    /// </summary>
    public event MapEventHandler OnKeyUp;
    /// <summary>
    /// OnPreClick
    /// </summary>
    public event MouseEventHandler OnPreClick;
    /// <summary>
    /// OnClick
    /// </summary>
    public event MouseEventHandler OnClick;
    /// <summary>
    /// OnDblClick
    /// </summary>
    public event MouseEventHandler OnDblClick;
    /// <summary>
    /// OnMouseDown
    /// </summary>
    public event MouseEventHandler OnMouseDown;
    /// <summary>
    /// OnMouseUp
    /// </summary>
    public event MouseEventHandler OnMouseUp;
    /// <summary>
    /// OnMouseOver
    /// </summary>
    public event MouseEventHandler OnMouseOver;
    /// <summary>
    /// OnMouseOut
    /// </summary>
    public event MouseEventHandler OnMouseOut;
    /// <summary>
    /// OnContextMenu
    /// </summary>
    public event MouseEventHandler OnContextMenu;

    #endregion

    #region Methods

    /// <summary>
    /// This method MUST be called only once by the Blazor component upon rendering, and never by the user.
    /// </summary>
    public void RaiseOnInitialized()
    {
        _isInitialized = true;
        OnInitialized?.Invoke();
    }

    /// <summary>
    /// Add a layer to the map.
    /// </summary>
    /// <param name="layer">The layer to be added.</param>
    /// <exception cref="System.ArgumentNullException">Throws when the layer is null.</exception>
    /// <exception cref="Exception">Throws when the map has not been yet initialized.</exception>
    public void AddLayer(Layer layer)
    {
        if (layer is null)
        {
            throw new ArgumentNullException(nameof(layer));
        }

        if (!_isInitialized)
        {
            //throw new UninitializedMapException();
            throw new Exception();
        }

        _layers.Add(layer);
    }

    /// <summary>
    /// Remove a layer from the map.
    /// </summary>
    /// <param name="layer">The layer to be removed.</param>
    /// <exception cref="System.ArgumentNullException">Throws when the layer is null.</exception>
    public void RemoveLayer(Layer layer)
    {
        if (layer is null)
        {
            throw new ArgumentNullException(nameof(layer));
        }

        if (!_isInitialized)
        {
            //throw new UninitializedMapException();
            throw new Exception();
        }

        _layers.Remove(layer);
    }

    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="jsRuntime">JS runtime</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Map(IJSInProcessRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        Id = StringHelper.GetRandomString(10);

        _layers.CollectionChanged += OnLayersChanged;
    }

    /// <summary>
    /// Get a read only collection of the current layers.
    /// </summary>
    /// <returns>A read only collection of layers.</returns>
    public IReadOnlyCollection<Layer> GetLayers()
    {
        return _layers.ToList().AsReadOnly();
    }

    private void OnLayersChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var item in args.NewItems)
            {
                var layer = item as Layer;
                LgLeafletInterops.AddLayer(_jsRuntime, Id, layer);
            }
        }
        else if (args.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var item in args.OldItems)
            {
                if (item is Layer layer)
                {
                     LgLeafletInterops.RemoveLayer(_jsRuntime, Id, layer.Id);
                }
            }
        }
        else if (args.Action == NotifyCollectionChangedAction.Replace
                 || args.Action == NotifyCollectionChangedAction.Move)
        {
            foreach (var oldItem in args.OldItems)
                if (oldItem is Layer layer)
                    LgLeafletInterops.RemoveLayer(_jsRuntime, Id, layer.Id);

            foreach (var newItem in args.NewItems)
                LgLeafletInterops.AddLayer(_jsRuntime, Id, newItem as Layer);
        }
    }

    /// <summary>
    /// Increases the zoom level by one notch.
    /// 
    /// If <c>shift</c> is held down, increases it by three.
    /// </summary>
    public void ZoomIn(MouseEventArgs e)
    {
        LgLeafletInterops.ZoomIn(_jsRuntime, Id, e);
    }

    /// <summary>
    /// Decreases the zoom level by one notch.
    /// 
    /// If <c>shift</c> is held down, decreases it by three.
    /// </summary>
    public void ZoomOutAsync(MouseEventArgs e)
    {
        LgLeafletInterops.ZoomOut(_jsRuntime, Id, e);
    }

    /// <summary>
    /// FitBounds
    /// </summary>
    /// <param name="corner1"></param>
    /// <param name="corner2"></param>
    /// <param name="padding"></param>
    /// <param name="maxZoom"></param>
    public void FitBounds(PointF corner1, PointF corner2, PointF? padding = null, float? maxZoom = null)
    {
        LgLeafletInterops.FitBounds(_jsRuntime, Id, corner1, corner2, padding, maxZoom);
    }

    /// <summary>
    /// PanTo
    /// </summary>
    /// <param name="position"></param>
    /// <param name="animate"></param>
    /// <param name="duration"></param>
    /// <param name="easeLinearity"></param>
    /// <param name="noMoveStart"></param>
    public void PanTo(PointF position, bool animate = false, float duration = 0.25f, float easeLinearity = 0.25f, bool noMoveStart = false)
    {
        LgLeafletInterops.PanTo(_jsRuntime, Id, position, animate, duration, easeLinearity, noMoveStart);
    }

    /// <summary>
    /// Center
    /// </summary>
    /// <returns></returns>
    public LatLng GetCenter()
    {
        return LgLeafletInterops.GetCenter(_jsRuntime, Id);
    }

    /// <summary>
    /// Zoom
    /// </summary>
    /// <returns></returns>
    public float GetZoom()
    {
        return LgLeafletInterops.GetZoom(_jsRuntime, Id);
    }

    #region events

    /// <summary>
    /// NotifyZoomLevelsChange
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyZoomLevelsChange(Event e)
    {
        OnZoomLevelsChange?.Invoke(this, e);
    }

    /// <summary>
    /// NotifyResize
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyResize(ResizeEvent e)
    {
        OnResize?.Invoke(this, e);
    }

    /// <summary>
    /// NotifyUnload
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyUnload(Event e)
    {
        OnUnload?.Invoke(this, e);
    }

    /// <summary>
    /// NotifyViewReset
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyViewReset(Event e)
    {
        OnViewReset?.Invoke(this, e);
    }

    /// <summary>
    /// NotifyLoad
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyLoad(Event e)
    {
        OnLoad?.Invoke(this, e);
    }

    /// <summary>
    /// NotifyZoomStart
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyZoomStart(Event e)
    {
        OnZoomStart?.Invoke(this, e);
    }

    /// <summary>
    /// NotifyMoveStart
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyMoveStart(Event e)
    {
        OnMoveStart?.Invoke(this, e);
    }

    /// <summary>
    /// NotifyZoom
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyZoom(Event e)
    {
        OnZoom?.Invoke(this, e);
    }

    /// <summary>
    /// NotifyMove
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyMove(Event e)
    {
        OnMove?.Invoke(this, e);
    }

    /// <summary>
    /// NotifyZoomEnd
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyZoomEnd(Event e)
    {
        OnZoomEnd?.Invoke(this, e);
    }

    /// <summary>
    /// NotifyMoveEnd
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyMoveEnd(Event e)
    {
        OnMoveEnd?.Invoke(this, e);
    }

    /// <summary>
    /// NotifyMouseMove
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseMove(MouseEvent eventArgs)
    {
        OnMouseMove?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// NotifyKeyPress
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyKeyPress(Event eventArgs)
    {
        OnKeyPress?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// NotifyKeyDown
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyKeyDown(Event eventArgs)
    {
        OnKeyDown?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// NotifyKeyUp
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyKeyUp(Event eventArgs)
    {
        OnKeyUp?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// NotifyPreClick
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyPreClick(MouseEvent eventArgs)
    {
        OnPreClick?.Invoke(this, eventArgs);
    }

    #endregion events

    #region InteractiveLayerEvents
    // Has the same events as InteractiveLayer, but it is not a layer. 
    // Could place this code in its own class and make Layer inherit from that, but not every layer is interactive...
    // Is there a way to not duplicate this code?

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
    /// NotifyDblClick
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyDblClick(MouseEvent eventArgs)
    {
        OnDblClick?.Invoke(this, eventArgs);
    }

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
    /// NotifyMouseUp
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseUp(MouseEvent eventArgs)
    {
        OnMouseUp?.Invoke(this, eventArgs);
    }

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
    /// NotifyMouseOut
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseOut(MouseEvent eventArgs)
    {
        OnMouseOut?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// NotifyContextMenu
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyContextMenu(MouseEvent eventArgs)
    {
        OnContextMenu?.Invoke(this, eventArgs);
    }

    #endregion InteractiveLayerEvents
}