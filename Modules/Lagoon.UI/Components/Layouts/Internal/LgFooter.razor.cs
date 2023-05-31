namespace Lagoon.UI.Components.Layouts.Internal;


/// <summary>
/// Internal component used to render application footer
/// </summary>
public partial class LgFooter : LgComponentBase
{

    #region dependencies injection

    /// <summary>
    /// Get the current application informations.
    /// </summary>
    [Inject]
    public Lagoon.Core.IApplicationInformation IApplicationInformation { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets footer content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

}
