namespace Lagoon.UI.Components;

/// <summary>
/// This component should be used inside a collapsable component (like a <see cref="LgFrame"/>)
/// to have different behaviour according to parent collapse state. Useful to display toolbar button conditionnaly.
/// </summary>
public class LgCollapsableView : ComponentBase
{

    #region cascading parameters

    /// <summary>
    /// Get the collapse state of an ancestor
    /// </summary>
    [CascadingParameter]
    public ICollapsable CollapseState { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Get or set the ViewMode
    /// </summary>
    [Parameter]
    public ViewMode ViewMode { get; set; } = ViewMode.Always;

    /// <summary>
    /// Get or set the ChildContent
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region methods

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (IsVisible())
        {
            builder.AddContent(0, ChildContent);
        }
    }

    /// <summary>
    /// Indicate if the view should be visible
    /// </summary>
    /// <returns></returns>
    private bool IsVisible()
    {
        return ViewMode == ViewMode.Always ||
                CollapseState is null ||
                (ViewMode == ViewMode.OnlyExpanded && !CollapseState.Collapsed) ||
                (ViewMode == ViewMode.OnlyCollapsed && CollapseState.Collapsed);
    }

    #endregion

}

/// <summary>
/// Behavior of <see cref="LgCollapsableView"/> component
/// </summary>
public enum ViewMode
{
    /// <summary>
    /// Always visible
    /// </summary>
    Always,

    /// <summary>
    /// Visible only if the parent collapsable component is collapsed
    /// </summary>
    OnlyCollapsed,

    /// <summary>
    /// Visible only if the parent collapsable component is expanded
    /// </summary>
    OnlyExpanded
}
