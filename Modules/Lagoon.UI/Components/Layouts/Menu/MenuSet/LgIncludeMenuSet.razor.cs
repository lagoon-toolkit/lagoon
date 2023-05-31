namespace Lagoon.UI.Components;


/// <summary>
/// Component used to link <see cref="LgMenuSet" /> within a menu.
/// </summary>
public partial class LgIncludeMenuSet : ComponentBase
{

    #region dependencies injection

    /// <summary>
    /// Menu service.
    /// </summary>
    [Inject]
    public MenuService MenuService { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// This id should be related to the id of an <see cref="LgMenuSet" />
    /// </summary>
    /// <value></value>
    [Parameter]
    public string Id { get; set; }

    #endregion

}
