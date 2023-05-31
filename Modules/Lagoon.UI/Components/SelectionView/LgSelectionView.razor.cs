namespace Lagoon.UI.Components;

/// <summary>
/// Component displaying its content if there is a selection.
/// </summary>
public class LgSelectionView : ComponentBase
{
    #region cascading parameters

    /// <summary>
    /// Get the number of selected values from a int cascading value.
    /// </summary>
    [CascadingParameter]
    public int SelectedItemsCount { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Child content of the component if the selected items count is between <see cref="MinimumSelectionCount" /> and <see cref="MaximumSelectionCount" />.
    /// </summary>
    [Parameter]
    public RenderFragment<int> ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the minimum number of items that must be selected to show the component.
    /// Defauly value : 1
    /// </summary>
    [Parameter]
    public int MinimumSelectionCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets the maximum number of selected items before hide the component.
    /// </summary>
    [Parameter]
    public int? MaximumSelectionCount { get; set; }

    #endregion

    #region methods

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if(IsSelectionVisible())
        {
            builder.AddContent(0, ChildContent(SelectedItemsCount));
        }
    }

    /// <summary>
    /// Indicate if the number of selected items allow the display of the component
    /// </summary>
    /// <returns></returns>
    private bool IsSelectionVisible()
    {
        return (SelectedItemsCount >= MinimumSelectionCount)
            && ((!MaximumSelectionCount.HasValue) || SelectedItemsCount <= MaximumSelectionCount.Value);
    }

    #endregion
}
