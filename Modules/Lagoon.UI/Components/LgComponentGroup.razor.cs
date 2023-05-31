namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Container used to declare component list in variable.
/// </summary>
public partial class LgComponentGroup : ComponentBase
{

    #region render fragments

    /// <summary>
    /// Content of the component.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

}
