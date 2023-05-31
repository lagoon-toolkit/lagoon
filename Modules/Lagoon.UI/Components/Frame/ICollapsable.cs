namespace Lagoon.UI.Components;

/// <summary>
/// Should be implemented by component which have an Collapse/Expand behaviour
/// </summary>
public interface ICollapsable
{

    /// <summary>
    /// Get or set a collapse state form collapsable component
    /// </summary>
    public bool Collapsed { get; set; }

}
