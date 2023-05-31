namespace Lagoon.UI.Components.Internal;


/// <summary>
/// Interface for filter boxes.
/// </summary>
public interface ILgFilterBox
{

    /// <summary>
    /// Gets or sets actived tabs in filter
    /// </summary>
    FilterTab ActiveTabs { get; set; }

    /// <summary>
    /// Gets or sets the codition list.
    /// </summary>
    Filter RawFilter { get; }

    /// <summary>
    /// Expression 
    /// </summary>
    EventCallback<Filter> RawFilterChanged { get; set; }

    /// <summary>
    /// Get the filter editor.
    /// </summary>
    ILgFilterEditor FilterEditor { get; }

}
