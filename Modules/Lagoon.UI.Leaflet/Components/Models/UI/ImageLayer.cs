﻿using System.Drawing;

namespace Lagoon.UI.Leaflet.Components.Models;

/// <summary>
/// ImageLayer
/// </summary>
public class ImageLayer : InteractiveLayer
{
    /// <summary>
    /// The opacity of the image overlay.
    /// </summary>
    public float Opacity { get; set; } = 1.0f;

    /// <summary>
    /// Text for the alt attribute of the image (useful for accessibility).
    /// </summary>
    public string Alt { get; set; } = string.Empty;

    /// <summary>
    /// Whether the crossOrigin attribute will be added to the image. If a String is provided, the image will have its crossOrigin attribute set to the String provided. This is needed if you want to access image pixel data. Refer to CORS Settings for valid String values.
    /// </summary>
    public string CrossOrigin { get; set; }

    /// <summary>
    /// URL to the overlay image to show in place of the overlay that failed to load.
    /// </summary>
    public string ErrorOverlayUrl { get; set; } = string.Empty;

    /// <summary>
    /// The explicit zIndex of the overlay layer.
    /// </summary>
    public int ZIndex { get; set; } = 1;

    /// <summary>
    /// A custom class name to assign to the image. Empty by default.
    /// </summary>
    public string ClassName { get; set; } = string.Empty;

    /// <summary>
    ///  Image url.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Corner1
    /// </summary>
    public PointF Corner1 { get; }

    /// <summary>
    /// Corner2
    /// </summary>
    public PointF Corner2 { get; }

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="url"></param>
    /// <param name="corner1"></param>
    /// <param name="corner2"></param>
    public ImageLayer(string url, PointF corner1, PointF corner2)
    {
        Url = url;
        Corner1 = corner1;
        Corner2 = corner2;
    }

}
