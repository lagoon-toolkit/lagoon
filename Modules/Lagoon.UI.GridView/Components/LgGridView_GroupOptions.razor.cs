namespace Lagoon.UI.Components;

/// <summary>
/// Gridview - Column options
/// </summary>
public partial class LgGridView<TItem>
{
    #region fields

    /// <summary>
    /// LgGroupOptions modal ref
    /// </summary>
    private LgGroupOptions _lgGroupOptions;

    #endregion Fields

    #region properties


    #endregion Properties

    /// <summary>
    /// Gets list of groups by order
    /// </summary>
    private List<GroupOption> GroupsByOrder => GroupByColumnKeyList.Values.Select(group => new GroupOption
    {
        Columns = group,
    }).ToList();

    #region Private methods

    /// <summary>
    /// Open group options pop-up
    /// </summary>
    /// <returns></returns>
    private Task OpenGroupOptionsModalAsync()
    {
        return _lgGroupOptions.ShowAsync();
    }

    /// <summary>
    /// On save group options
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async Task OnSaveGroupOptionsAsync(SaveGroupOptionsEventArgs args)
    {
        GroupByColumnKeyList.Clear();
        var groupLevel = 0;
        foreach (GroupOption group in args.GroupsByOrder)
        {
            foreach (string field in group.Columns)
            {
                AddGroupField(groupLevel, field.Trim());
            }
            groupLevel++;
        }
        await UpdateCurrentProfileAsync();
        await LoadDataAsync(false);
    }

    /// <summary>
    /// Expand all groups event
    /// </summary>
    /// <param name="args"></param>
    private void OnExpandGroup(ActionEventArgs args)
    {
        State.GroupCollapsed = false;
        State.GroupsState.Clear();
        RebuildRows = true;
    }

    /// <summary>
    /// Collapse all groups event
    /// </summary>
    /// <param name="args"></param>
    private void OnCollapseGroup(ActionEventArgs args)
    {
        State.GroupCollapsed = true;
        State.GroupsState.Clear();
        RebuildRows = true;
    }

    /// <summary>
    /// Remove group event
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async Task OnRemoveGroupAsync(ActionEventArgs args)
    {
        GroupByColumnKeyList.Clear();
        await UpdateCurrentProfileAsync();
        await LoadDataAsync(false);
    }

    #endregion Private methods
}
