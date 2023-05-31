using System.Drawing;

namespace Lagoon.UI.Leaflet.Components.Models;

/// <summary>
/// the Gridlayer 
/// </summary>
public abstract class GridLayer : Layer
{
    /// <summary>
    /// Width and height of tiles in the grid.
    /// </summary>
    public Size TileSize { get; set; } = new Size(256, 256);

    /// <summary>
    /// Gets or sets the opacity.
    /// </summary>
    public double Opacity { get; set; } = 1.0;

    /// <summary>
    /// Load new tiles only when panning ends.
    /// </summary>
    public bool UpdateWhenIdle { get; set; }

    /// <summary>
    /// By default, a smooth zoom animation will update grid layers every integer zoom level. Setting this option to false will update the grid layer only when the smooth animation ends.
    /// </summary>
    public bool UpdateWhenZooming { get; set; } = true;

    /// <summary>
    /// Tiles will not update more than once every updateInterval milliseconds when panning.
    /// </summary>
    public int UpdateInterval { get; set; } = 200;

    /// <summary>
    /// Gets or sets the z-index.
    /// </summary>
    public int ZIndex { get; set; } = 1;

    /// <summary>
    /// If set, tiles will only be loaded inside the set.
    /// </summary>
    public Tuple<LatLng, LatLng> Bounds { get; set; }

    /// <summary>
    /// The minimum zoom level down to which this layer will be displayed
    /// </summary>
    public float MinZoom { get; set; }

    /// <summary>
    /// The maximum zoom level up to which this layer will be displayed.
    /// </summary>
    public float? MaxZoom { get; set; }

    /// <summary>
    /// Maximum zoom number the tile source has available.
    /// </summary>
    public float? MaxNativeZoom { get; set; }

    /// <summary>
    /// Minimum zoom number the tile source has available.
    /// </summary>
    public float? MinNativeZoom { get; set; }

    /// <summary>
    /// Whether the layer is wrapped around the antimeridian.
    /// </summary>
    public bool NoWrap { get; set; }

    /// <summary>
    /// A custom class name to assign to the tile layer.
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// When panning the map, keep this many rows and columns of tiles before unloading them.
    /// </summary>
    public int KeepBuffer { get; set; } = 2;

}
