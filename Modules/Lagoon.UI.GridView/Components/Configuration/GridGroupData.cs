namespace Lagoon.UI.Components;

/// <summary>
/// Group data
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class GridGroupData<TItem>
{
    /// <summary>
    /// Gets Level data
    /// </summary>
    public Dictionary<string, GridGroupingColumn> GroupingColumns { get; }

    /// <summary>
    /// Gets first group item
    /// </summary>
    public TItem FirstItem { get; }

    /// <summary>
    /// Gets default group render fragment
    /// </summary>
    public RenderFragment DefaultContent { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="groupData"></param>
    /// <param name="item"></param>
    /// <param name="defaultContent"></param>
    public GridGroupData(Dictionary<string, GridGroupingColumn> groupData, TItem item, RenderFragment defaultContent)
    {
        GroupingColumns = groupData;
        FirstItem = item;
        DefaultContent = defaultContent;
    }
}
