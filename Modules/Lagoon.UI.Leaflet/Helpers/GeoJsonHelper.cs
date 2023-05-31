using GeoJSON.Text.Geometry;
using Lagoon.UI.Leaflet.Components;
using Lagoon.UI.Leaflet.Components.Models;
using System.Drawing;

namespace Lagoon.UI.Leaflet.Helpers;

/// <summary>
/// GeoJson helper
/// </summary>
public static class GeoJsonHelper
{
    /// <summary>
    /// Convert GeoJson to layer
    /// </summary>
    /// <param name="geoJson">geoJson</param>
    /// <returns>return layer from geojson</returns>
    public static Layer GetLayerFromGeoJsonObject(this GeoJsonObject geoJson)
    {
        switch (geoJson.Geometry.Type)
        {
            case GeoJSON.Text.GeoJSONObjectType.LineString:
                LineString lineString = geoJson.Geometry as LineString;
                return new Polyline() { Shape = new[] { lineString.Coordinates.Select(x => new PointF((float)x.Latitude, (float)x.Longitude)).ToArray() } };

            case GeoJSON.Text.GeoJSONObjectType.Polygon:
                GeoJSON.Text.Geometry.Polygon polygon = geoJson.Geometry as GeoJSON.Text.Geometry.Polygon;
                //if (geoJson.properties.layerType.Equals("rectangle"))
                //    return new BlazorLeaflet.Models.Rectangle() { Shape = polygon.Coordinates.Select(x => x.Coordinates.Select(x => new PointF((float)x.Latitude, (float)x.Longitude)).ToArray()).ToArray() };
                return new Components.Models.Polygon() { Shape = polygon.Coordinates.Select(x => x.Coordinates.Select(x => new PointF((float)x.Latitude, (float)x.Longitude)).ToArray()).ToArray() };

            case GeoJSON.Text.GeoJSONObjectType.Point:
                GeoJSON.Text.Geometry.Point point = geoJson.Geometry as GeoJSON.Text.Geometry.Point;
                if (!string.IsNullOrEmpty(geoJson.Properties.LayerType))
                {
                    if (geoJson.Properties.LayerType.Equals("circle") || geoJson.Properties.LayerType.Equals("circlemarker"))
                    {
                        return new Circle
                        {
                            Position = new LatLng((float)point.Coordinates.Latitude, (float)point.Coordinates.Longitude),
                            Radius = geoJson.Properties.Radius
                        };
                    }
                }

                return new Marker(new LatLng { Lat = (float)point.Coordinates.Latitude, Lng = (float)point.Coordinates.Longitude });
                ;
        }

        throw new NotImplementedException();
        ;
    }

    /// <summary>
    /// Update layer From GeoJson Object
    /// </summary>
    /// <param name="layer">layer to update</param>
    /// <param name="geoJson">geoJson</param>
    public static void UpdateLayerFromGeoJsonObject(this Layer layer, GeoJsonObject geoJson)
    {
        Layer geoJsonLayer = geoJson.GetLayerFromGeoJsonObject();

        if (layer is Polyline polyline)
        {
            polyline.Shape = (geoJsonLayer as Polyline).Shape;
        }
        if (layer is Circle circle)
        {
            circle.Position = (geoJsonLayer as Circle).Position;
            circle.Radius = (geoJsonLayer as Circle).Radius;
        }

        if (layer is Marker marker)
        {
            marker.Position = (geoJsonLayer as Marker).Position;
        }
    }
}
