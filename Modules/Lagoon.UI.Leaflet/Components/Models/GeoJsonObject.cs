using System.Text.Json.Serialization;
using GeoJSON.Text.Converters;
using GeoJSON.Text.Geometry;

namespace Lagoon.UI.Leaflet.Components;

/// <summary>
/// Geo Json Object
/// </summary>
public class GeoJsonObject
{
    /// <summary>
    /// Type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// Properties
    /// </summary>
    [JsonPropertyName("properties")]
    public Properties Properties { get; set; }

    /// <summary>
    /// Geometry
    /// </summary>
    [JsonConverter(typeof(GeometryConverter))]
    [JsonPropertyName("geometry")]
    public IGeometryObject Geometry { get; set; }

}

/// <summary>
/// Properties
/// </summary>
public class Properties
{
    /// <summary>
    /// Layer type
    /// </summary>
    [JsonPropertyName("layerType")]
    public string LayerType { get; set; }

    /// <summary>
    /// Radius
    /// </summary>
    [JsonPropertyName("radius")]
    public float Radius { get; set; }
}
