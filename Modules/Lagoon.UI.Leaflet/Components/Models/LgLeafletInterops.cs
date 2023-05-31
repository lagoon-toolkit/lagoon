using Lagoon.UI.Leaflet.Components.Models;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Concurrent;
using System.Drawing;
using Rectangle = Lagoon.UI.Leaflet.Components.Models.Rectangle;

namespace Lagoon.UI.Leaflet.Components;

/// <summary>
/// Lg Leaflet Interops
/// </summary>
public static class LgLeafletInterops
{
    private static readonly string _baseObjectContainer = "window.leafletBlazor";

    private static ConcurrentDictionary<string, (IDisposable, string, Layer)> LayerReferences { get; } = new ConcurrentDictionary<string, (IDisposable, string, Layer)>();

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    public static void Create(IJSInProcessRuntime jsRuntime, Map map)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.create", map, DotNetObjectReference.Create(map));
    }

    private static DotNetObjectReference<T> CreateLayerReference<T>(string mapId, T layer) where T : Layer
    {
        var result = DotNetObjectReference.Create(layer);
        LayerReferences.TryAdd(layer.Id, (result, mapId, layer));
        return result;
    }

    private static void DisposeLayerReference(string layerId)
    {
        if (LayerReferences.TryRemove(layerId, out var value))
            value.Item1.Dispose();
    }

    /// <summary>
    /// AddLayer
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static void AddLayer(IJSInProcessRuntime jsRuntime, string mapId, Layer layer)
    {
        _ = layer switch
        {
            TileLayer tileLayer => jsRuntime.SwitchInvokeVoid($"{_baseObjectContainer}.addTilelayer", mapId, tileLayer, CreateLayerReference(mapId, tileLayer)),
            MbTilesLayer mbTilesLayer => jsRuntime.SwitchInvokeVoid($"{_baseObjectContainer}.addMbTilesLayer", mapId, mbTilesLayer, CreateLayerReference(mapId, mbTilesLayer)),
            ShapefileLayer shapefileLayer => jsRuntime.SwitchInvokeVoid($"{_baseObjectContainer}.addShapefileLayer", mapId, shapefileLayer, CreateLayerReference(mapId, shapefileLayer)),
            Marker marker => jsRuntime.SwitchInvokeVoid($"{_baseObjectContainer}.addMarker", mapId, marker, CreateLayerReference(mapId, marker)),
            Rectangle rectangle => jsRuntime.SwitchInvokeVoid($"{_baseObjectContainer}.addRectangle", mapId, rectangle, CreateLayerReference(mapId, rectangle)),
            Circle circle => jsRuntime.SwitchInvokeVoid($"{_baseObjectContainer}.addCircle", mapId, circle, CreateLayerReference(mapId, circle)),
            Polygon polygon => jsRuntime.SwitchInvokeVoid($"{_baseObjectContainer}.addPolygon", mapId, polygon, CreateLayerReference(mapId, polygon)),
            Polyline polyline => jsRuntime.SwitchInvokeVoid($"{_baseObjectContainer}.addPolyline", mapId, polyline, CreateLayerReference(mapId, polyline)),
            ImageLayer image => jsRuntime.SwitchInvokeVoid($"{_baseObjectContainer}.addImageLayer", mapId, image, CreateLayerReference(mapId, image)),
            GeoJsonDataLayer geo => jsRuntime.SwitchInvokeVoid($"{_baseObjectContainer}.addGeoJsonLayer", mapId, geo, CreateLayerReference(mapId, geo)),
            _ => throw new NotImplementedException($"The layer {typeof(Layer).Name} has not been implemented."),
        };
    }

    /// <summary>
    /// RemoveLayer
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="layerId"></param>
    /// <returns></returns>
    public static void RemoveLayer(IJSInProcessRuntime jsRuntime, string mapId, string layerId)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.removeLayer", mapId, layerId);
        DisposeLayerReference(layerId);
    }

    /// <summary>
    /// UpdatePopupContent
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static void UpdatePopupContent(IJSInProcessRuntime jsRuntime, string mapId, Layer layer)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.updatePopupContent", mapId, layer.Id, layer.Popup?.Content);
    }

    /// <summary>
    /// UpdateTooltipContent
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static void UpdateTooltipContent(IJSInProcessRuntime jsRuntime, string mapId, Layer layer)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.updateTooltipContent", mapId, layer.Id, layer.Tooltip?.Content);
    }

    /// <summary>
    /// UpdateShape
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static void UpdateShape(IJSInProcessRuntime jsRuntime, string mapId, Layer layer)
    {
        _ = layer switch
        {
            Rectangle rectangle => jsRuntime.SwitchInvokeVoid($"{_baseObjectContainer}.updateRectangle", mapId, rectangle),
            Circle circle => jsRuntime.SwitchInvokeVoid($"{_baseObjectContainer}.updateCircle", mapId, circle),
            Polygon polygon => jsRuntime.SwitchInvokeVoid($"{_baseObjectContainer}.updatePolygon", mapId, polygon),
            Polyline polyline => jsRuntime.SwitchInvokeVoid($"{_baseObjectContainer}.updatePolyline", mapId, polyline),
            _ => throw new NotImplementedException($"The layer {typeof(Layer).Name} has not been implemented."),
        };
    }

    /// <summary>
    /// FitBounds
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="corner1"></param>
    /// <param name="corner2"></param>
    /// <param name="padding"></param>
    /// <param name="maxZoom"></param>
    /// <returns></returns>
    public static void FitBounds(IJSInProcessRuntime jsRuntime, string mapId, PointF corner1, PointF corner2, PointF? padding, float? maxZoom)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.fitBounds", mapId, corner1, corner2, padding, maxZoom);
    }

    /// <summary>
    /// PanTo
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="position"></param>
    /// <param name="animate"></param>
    /// <param name="duration"></param>
    /// <param name="easeLinearity"></param>
    /// <param name="noMoveStart"></param>
    /// <returns></returns>
    public static void PanTo(IJSInProcessRuntime jsRuntime, string mapId, PointF position, bool animate, float duration, float easeLinearity, bool noMoveStart)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.panTo", mapId, position, animate, duration, easeLinearity, noMoveStart);
    }

    /// <summary>
    /// GetCenter
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <returns></returns>
    public static LatLng GetCenter(IJSInProcessRuntime jsRuntime, string mapId)
    {
        return jsRuntime.Invoke<LatLng>($"{_baseObjectContainer}.getCenter", mapId);
    }

    /// <summary>
    /// GetZoom
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <returns></returns>
    public static float GetZoom(IJSInProcessRuntime jsRuntime, string mapId)
    {
        return jsRuntime.Invoke<float>($"{_baseObjectContainer}.getZoom", mapId);
    }

    /// <summary>
    /// ZoomIn
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public static void ZoomIn(IJSInProcessRuntime jsRuntime, string mapId, MouseEventArgs e)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.zoomIn", mapId, e);
    }

    /// <summary>
    /// ZoomOut
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public static void ZoomOut(IJSInProcessRuntime jsRuntime, string mapId, MouseEventArgs e)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.zoomOut", mapId, e);
    }

    /// <summary>
    /// Tries to locate the user using the Geolocation API, firing a locationfound event with location data on success or a locationerror event on failure, and optionally sets the map view to the user's location with respect to detection accuracy (or to the world view if geolocation failed). Note that, if your page doesn't use HTTPS, this method will fail in modern browsers (Chrome 50 and newer) See Locate options for more details.
    /// </summary>
    /// <param name="jsRuntime">IJSInProcessRuntime</param>
    /// <param name="mapId">Map Id</param>
    /// <param name="watch">If true, starts continuous watching of location changes (instead of detecting it once) using W3C watchPosition method. You can later stop watching using map.stopLocate() method.</param>
    /// <param name="setView">If true, automatically sets the map view to the user location with respect to detection accuracy, or to world view if geolocation failed.</param>
    /// <param name="maxZoom">The maximum zoom for automatic view setting when using setView option.</param>
    /// <param name="timeout">Number of milliseconds to wait for a response from geolocation before firing a locationerror event.</param>
    /// <param name="maximumAge">Maximum age of detected location. If less than this amount of milliseconds passed since last geolocation response, locate will return a cached location.</param>
    /// <param name="enableHighAccuracy">Enables high accuracy, see description in the W3C spec.</param>
    public static void Locate(IJSInProcessRuntime jsRuntime, string mapId, bool watch, bool setView, float? maxZoom, float timeout, float maximumAge, bool enableHighAccuracy)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.locate", mapId, watch, setView, maxZoom, timeout, maximumAge, enableHighAccuracy);
    }

    /// <summary>
    /// Stops watching location previously initiated by map.locate({watch: true}) and aborts resetting the map view if map.locate was called with {setView: true}.
    /// </summary>
    internal static void StopLocate(IJSInProcessRuntime jsRuntime, string mapId)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.stopLocate", mapId);
    }

    /// <summary>
    /// To add the draw toolbar.
    /// </summary>
    internal static void AddDrawControl(IJSInProcessRuntime jsRuntime, string mapId)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.addDrawControl", mapId);
    }

    /// <summary>
    /// Returns a GeoJSON representation of the layer
    /// </summary>
    /// <param name="jsRuntime">IJSInProcessRuntime</param>
    /// <param name="mapId">Map Id</param>
    /// <param name="layerId">layer id</param>
    /// <returns>return geo json</returns>
    internal static string ToGeoJSON(IJSInProcessRuntime jsRuntime, string mapId, string layerId)
    {
        return jsRuntime.Invoke<string>($"{_baseObjectContainer}.toGeoJSON", mapId, layerId);
    }

    /// <summary>
    /// To add search control.
    /// </summary>
    internal static void AddSearchControl(IJSInProcessRuntime jsRuntime, string mapId)
    {
        jsRuntime.Invoke<string>($"{_baseObjectContainer}.addSearchControl", mapId);
    }

    /// <summary>
    /// To add Osrm routing control.
    /// </summary>
    internal static void AddOsrmRoutingControl(IJSInProcessRuntime jsRuntime, string mapId, RoutingControlOptions routingControlOptions)
    {
        jsRuntime.Invoke<string>($"{_baseObjectContainer}.addOsrmRoutingControl", mapId, routingControlOptions);
    }

    /// <summary>
    /// To add MapBox routing control.
    /// </summary>
    internal static void AddMapBoxRoutingControl(IJSInProcessRuntime jsRuntime, string mapId, string accessToken, RoutingControlOptions routingControlOptions)
    {
        jsRuntime.Invoke<string>($"{_baseObjectContainer}.addMapBoxRoutingControl", mapId, accessToken, routingControlOptions);
    }

    /// <summary>
    /// Restricts the map view to the given bounds (see the maxBounds option).
    /// </summary>
    /// <param name="jsRuntime">IJSInProcessRuntime</param>
    /// <param name="mapId">Map Id</param>
    /// <param name="bounds">max Bounds</param>
    internal static void SetMaxBounds(IJSInProcessRuntime jsRuntime, string mapId, Tuple<LatLng, LatLng> bounds)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.setMaxBounds", mapId, bounds);
    }

    /// <summary>
    /// Adds a base layer (radio button entry) with the given name to the control.
    /// </summary>
    /// <param name="jsRuntime">IJSInProcessRuntime</param>
    /// <param name="mapId">Map Id</param>
    /// <param name="layerId">layer Id</param>
    /// <param name="name">name</param>
    internal static void AddBaseLayer(IJSInProcessRuntime jsRuntime, string mapId, string layerId, string name)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.addBaseLayer", mapId, layerId, name);
    }

    /// <summary>
    /// Add over pass layer.
    /// </summary>
    /// <param name="jsRuntime">IJSInProcessRuntime</param>
    /// <param name="mapId">Map Id</param>
    /// <param name="query">query</param>
    /// <param name="endPoint">End point</param>
    internal static void AddOverPassLayer(IJSInProcessRuntime jsRuntime, string mapId, string query, string endPoint)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.addOverPassLayer", mapId, query, endPoint);
    }

    /// <summary>
    /// Adds an overlay (checkbox entry) with the given name to the control.
    /// </summary>
    /// <param name="jsRuntime">IJSInProcessRuntime</param>
    /// <param name="mapId">Map Id</param>
    /// <param name="layerId">layer Id</param>
    /// <param name="name">name</param>
    internal static void AddOverlay(IJSInProcessRuntime jsRuntime, string mapId, string layerId, string name)
    {
        jsRuntime.InvokeVoid($"{_baseObjectContainer}.addOverlay", mapId, layerId, name);
    }


    /// <summary>
    /// Invokes the specified JavaScript function synchronously and return <c>true</c> value to be used in a switch expression.
    /// </summary>
    /// <param name="jsRuntime">The <see cref="IJSInProcessRuntime"/>.</param>
    /// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
    /// <param name="args">JSON-serializable arguments.</param>
    private static bool SwitchInvokeVoid(this IJSInProcessRuntime jsRuntime, string identifier, params object[] args)
    {
        jsRuntime.InvokeVoid(identifier, args);
        return true;
    }

}
