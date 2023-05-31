using System.Drawing;

namespace Lagoon.UI.Leaflet.Components.Models;

/// <summary>
/// DivOverlay
/// </summary>
public abstract class DivOverlay : Layer
{

    /// <summary>
    /// The offset of the popup position. Useful to control the anchor of the popup when opening it on some overlays.
    /// </summary>
    public virtual Point Offset { get; set; } = new Point(0, 7);

    /// <summary>
    /// A custom CSS class name to assign to the popup.
    /// </summary>
    public virtual string ClassName { get; set; } = string.Empty;

    /// <summary>
    /// Map pane where the popup will be added.
    /// </summary>
    public override string Pane { get; set; } = "popupPane";

}
