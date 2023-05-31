using Lagoon.UI.Leaflet.Helpers;
using Lagoon.UI.Leaflet.Components.Models;
using System.Text.Json;

namespace Lagoon.UI.Leaflet.Components;

/// <summary>
/// Leaflet Map
/// </summary>
public class LeafletMap : Map
{
    private readonly IJSInProcessRuntime _jsRuntime;

    /// <summary>
    /// Leaflet Map
    /// </summary>
    /// <param name="jsRuntime"></param>
    public LeafletMap(IJSInProcessRuntime jsRuntime) : base(jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Tries to locate the user using the Geolocation API, firing a locationfound event with location data on success or a locationerror event on failure, and optionally sets the map view to the user's location with respect to detection accuracy (or to the world view if geolocation failed). Note that, if your page doesn't use HTTPS, this method will fail in modern browsers (Chrome 50 and newer) See Locate options for more details.
    /// </summary>
    /// <param name="watch">If true, starts continuous watching of location changes (instead of detecting it once) using W3C watchPosition method. You can later stop watching using map.stopLocate() method.</param>
    /// <param name="setView">If true, automatically sets the map view to the user location with respect to detection accuracy, or to world view if geolocation failed.</param>
    /// <param name="maxZoom">The maximum zoom for automatic view setting when using setView option.</param>
    /// <param name="timeout">Number of milliseconds to wait for a response from geolocation before firing a locationerror event.</param>
    /// <param name="maximumAge">Maximum age of detected location. If less than this amount of milliseconds passed since last geolocation response, locate will return a cached location.</param>
    /// <param name="enableHighAccuracy">Enables high accuracy, see description in the W3C spec.</param>
    public void Locate(bool watch = false, bool setView = false, float? maxZoom = float.MaxValue, float timeout = 10000, float maximumAge = 0, bool enableHighAccuracy = false)
    {
        LgLeafletInterops.Locate(_jsRuntime, Id, watch, setView, maxZoom, timeout, maximumAge, enableHighAccuracy);
    }

    /// <summary>
    /// Stops watching location previously initiated by map.locate({watch: true}) and aborts resetting the map view if map.locate was called with {setView: true}.
    /// </summary>
    public void StopLocate()
    {
        LgLeafletInterops.StopLocate(_jsRuntime, Id);

    }

    /// <summary>
    /// To add the draw toolbar.
    /// </summary>
    public void AddDrawControl()
    {
        LgLeafletInterops.AddDrawControl(_jsRuntime, Id);
    }

    /// <summary>
    /// To add search control.
    /// </summary>
    public void AddSearchControl()
    {
        LgLeafletInterops.AddSearchControl(_jsRuntime, Id);
    }

    /// <summary>
    /// To add Osrm routing control.
    /// </summary>
    public void AddOsrmRoutingControl(RoutingControlOptions routingControlOptions)
    {
        LgLeafletInterops.AddOsrmRoutingControl(_jsRuntime, Id, routingControlOptions);
    }

    /// <summary>
    /// To add MapBox routing control.
    /// </summary>
    public void AddMapBoxRoutingControl(string accessToken, RoutingControlOptions routingControlOptions)
    {
        LgLeafletInterops.AddMapBoxRoutingControl(_jsRuntime, Id, accessToken, routingControlOptions);
    }

    /// <summary>
    /// Returns a GeoJSON representation of the layer
    /// </summary>
    /// <param name="layerId">layer id</param>
    /// <returns>return geo json</returns>
    public string ToGeoJSON(string layerId)
    {
        return LgLeafletInterops.ToGeoJSON(_jsRuntime, Id, layerId);
    }

    /// <summary>
    /// Restricts the map view to the given bounds (see the maxBounds option).
    /// </summary>
    /// <param name="bounds">max Bounds</param>
    public void SetMaxBounds(Tuple<LatLng, LatLng> bounds)
    {
        LgLeafletInterops.SetMaxBounds(_jsRuntime, Id, bounds);
    }

    /// <summary>
    /// Adds a base layer (radio button entry) with the given name to the control.
    /// </summary>
    /// <param name="layerId">layer Id</param>
    /// <param name="name">name</param>
    public void AddBaseLayer(string layerId, string name)
    {
        LgLeafletInterops.AddBaseLayer(_jsRuntime, Id, layerId, name);
    }

    /// <summary>
    /// Adds an overlay (checkbox entry) with the given name to the control.
    /// </summary>
    /// <param name="layerId">layer Id</param>
    /// <param name="name">name</param>
    public void AddOverlay(string layerId, string name)
    {
        LgLeafletInterops.AddOverlay(_jsRuntime, Id, layerId, name);
    }

    /// <summary>
    /// Add over pass layer.
    /// </summary>
    /// <param name="query">query</param>
    /// <param name="endPoint">by default : https://overpass-api.de/api/</param>
    public void AddOverPassLayer(string query, string endPoint = "https://overpass-api.de/api/")
    {
        LgLeafletInterops.AddOverPassLayer(_jsRuntime, Id, query, endPoint);
    }

    /// <summary>
    /// Send a query to the Overpass API and gets back the data set that corresponds to the query
    /// </summary>
    /// <param name="query">query</param>
    /// <returns>return json string</returns>
    public static async Task<string> GetDataFromOverpassAPIAsync(string query)
    {
        var url = string.Format("https://overpass-api.de/api/interpreter?data={0}", query);
        var response = await new HttpClient().GetAsync(url);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();

        return responseBody;
    }

    /// <summary>
    /// Fired when geolocation (using the locate method) went successfully.
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">event</param>
    public delegate void LocationEventHandler(LeafletMap sender, LocationEvent e);

    /// <summary>
    /// Location Event Handler
    /// </summary>
    public event LocationEventHandler OnLocationFound;

    /// <summary>
    /// Notify when Location Found
    /// </summary>
    /// <param name="eventArgs">Event args</param>
    [JSInvokable]
    public void NotifyLocationFound(LocationEvent eventArgs)
    {
        OnLocationFound?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// Fired when a new vector or marker has been created.
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="createdLayer">created layer</param>
    public delegate void DrawCreatedEventHandler(LeafletMap sender, Layer createdLayer);

    /// <summary>
    /// DrawCreated Event Handler
    /// </summary>
    public event DrawCreatedEventHandler OnDrawCreated;

    /// <summary>
    /// Notify when draw created
    /// </summary>
    /// <param name="eventArgs">event Args</param>
    [JSInvokable]
    public void NotifyDrawCreated(string eventArgs)
    {
        OnDrawCreatedHandler(this, eventArgs);
    }

    /// <summary>
    /// Fired when a new vector or marker has been edited.
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="editedLayers">event</param>
    /// <returns>edited layers</returns>
    public delegate void DrawEditedEventHandler(LeafletMap sender, List<Layer> editedLayers);

    /// <summary>
    /// DrawEdited Event Handler
    /// </summary>
    public event DrawEditedEventHandler OnDrawEdited;

    /// <summary>
    /// Notify when draw edited
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyDrawEdited(Dictionary<string, string> eventArgs)
    {
        OnDrawEditedHandler(this, eventArgs);
    }

    /// <summary>
    /// Fired when a new vector or marker has been deleted.
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="deletedLayers">event</param>
    public delegate void DrawDeletedEventHandler(LeafletMap sender, List<Layer> deletedLayers);

    /// <summary>
    /// DrawDeleted Event Handler
    /// </summary>
    public event DrawDeletedEventHandler OnDrawDeleted;

    /// <summary>
    /// Notify when draw deleted
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyDrawDeleted(List<string> eventArgs)
    {
        OnDrawDeletedHandler(this, eventArgs);
    }

    #region Protected Methods
    /// <summary>
    /// Delete selected draw
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="removedLayersIds">List of Layers Ids to remove</param>
    protected virtual void OnDrawDeletedHandler(LeafletMap sender, List<string> removedLayersIds)
    {
        var removedLayers = GetLayers().Where(x => removedLayersIds.Contains(x.Id)).ToList();
        removedLayers.ForEach(y => RemoveLayer(y));
        OnDrawDeleted?.Invoke(sender, removedLayers);
    }

    /// <summary>
    /// Edit selected draw
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="editedLayers">List of edited Layers</param>
    protected virtual void OnDrawEditedHandler(LeafletMap sender, Dictionary<string, string> editedLayers)
    {
        var layers = new List<Layer>();
        foreach (var editedLayer in editedLayers)
        {
            var layerToEdit = GetLayers().FirstOrDefault(x => x.Id == editedLayer.Key);
            if (layerToEdit != null)
            {
                GeoJsonObject geoJsonObject = JsonSerializer.Deserialize<GeoJsonObject>(editedLayer.Value);
                layerToEdit.UpdateLayerFromGeoJsonObject(geoJsonObject);
                layers.Add(layerToEdit);
            }
        }
        
        OnDrawEdited?.Invoke(sender, layers);
    }

    /// <summary>
    /// Create layer from draw control 
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="geoJson">geo Json</param>
    protected virtual void OnDrawCreatedHandler(LeafletMap sender, string geoJson)
    {
        GeoJsonObject geoJsonObject = JsonSerializer.Deserialize<GeoJsonObject>(geoJson);
        var layer = geoJsonObject.GetLayerFromGeoJsonObject();
        AddLayer(layer);
        OnDrawCreated?.Invoke(sender, layer);
    }

    #endregion Protected Methods
}
