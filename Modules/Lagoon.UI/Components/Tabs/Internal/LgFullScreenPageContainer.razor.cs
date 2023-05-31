namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Component to open URL that should not be in a tab.
/// </summary>
public partial class LgFullScreenPageContainer : ComponentBase
{
    #region parameters

    /// <summary>
    /// Route informations about the page to show.
    /// </summary>
    private RouteData RouteData { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Raise a state has changed event.
    /// </summary>
    internal void Refresh(RouteData routeData)
    {
        if (RouteData != routeData)
        {
            RouteData = routeData;
            StateHasChanged();
        }
    }

    #endregion

}
