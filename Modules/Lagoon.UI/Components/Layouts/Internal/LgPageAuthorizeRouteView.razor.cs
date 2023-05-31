namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Displays the content of a route by checking the user's rights.
/// </summary>
public partial class LgPageAuthorizeRouteView : ComponentBase
{

    #region cascading parameters

    /// <summary>
    /// LgApp instance.
    /// </summary>
    [CascadingParameter]
    public LgApp AppComponent { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Information about the page to display.
    /// </summary>
    [Parameter]
    public RouteData RouteData { get; set; }

    #endregion

}
