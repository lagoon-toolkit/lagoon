namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Gridview rows manager
/// </summary>
public class LgBaseGridViewRowManager : LgComponentBase
{

    #region parameters

    /// <summary>
    /// Gets or sets indicator to refresh rows state
    /// </summary>
    [Parameter]
    public bool RefreshState { get; set; }

    /// <summary>
    /// Gets or sets binding of the refresh rows indicator
    /// </summary>
    [Parameter]
    public EventCallback<bool> RefreshStateChanged { get; set; }

    #endregion

}