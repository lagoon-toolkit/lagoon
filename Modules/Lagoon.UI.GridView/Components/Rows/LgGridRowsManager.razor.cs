namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Gridview rows build manager
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class LgGridRowsManager<TItem> : LgBaseGridViewRowManager
{
    #region fields

    /// <summary>
    /// Row data provider
    /// </summary>
    private GridViewRowProvider<TItem> _rowProvider;

    /// <summary>
    /// Group level and columns
    /// </summary>
    private Dictionary<int, List<string>> _groupRowData = new();

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets data to display
    /// </summary>
    [Parameter]
    public IEnumerable<TItem> Data { get; set; }

    /// <summary>
    /// Gets or sets item used to add 
    /// </summary>
    [Parameter]
    public TItem AddItem { get; set; }

    /// <summary>
    /// Gets or sets if the add line is shown on top or not
    /// </summary>
    [Parameter]
    public bool AddItemOnTop { get; set; }

    /// <summary>
    /// Gets or sets columns definitions
    /// </summary>
    [Parameter]
    public RenderFragment<TItem> Columns { get; set; }

    /// <summary>
    /// Gets or sets content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets editing cell
    /// </summary>
    internal object EditingCell { get; set; }

    /// <summary>
    /// Gets or sets if the row is being edited
    /// </summary>
    internal bool IsEditing { get; set; }

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets or sets gridview owner
    /// </summary>
    [CascadingParameter]
    public LgGridView<TItem> GridView { get; set; }

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // Build group data row
        InitializeGroup();
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (RefreshState)
        {
            InitializeGroup();
        }
    }

    ///<inheritdoc/>
    protected override bool ShouldRender()
    {
        bool render = RefreshState;
        GridView.RebuildRows = false;
        if (!render)
        {
            GridView.RefreshEditRow();
        }
        return render;
    }

    /// <summary>
    /// Initialize group by key list
    /// </summary>
    private void InitializeGroup()
    {
        if (GridView.GroupByColumnKeyList is not null)
        {
            _groupRowData.Clear();
            _groupRowData = GridView.GroupByColumnKeyList.ToDictionary(
                entry => entry.Key,
                entry => entry.Value);
        }
    }

    /// <summary>
    /// Return group name
    /// </summary>
    /// <param name="item"></param>
    /// <param name="groupLevel"></param>
    /// <returns></returns>
    private string GroupName(TItem item, int groupLevel)
    {
        StringBuilder groupName = new();
        foreach (string key in _groupRowData[groupLevel])
        {
            if (GridView.TryGetColumn(key, out GridColumnState column))
            {
                object itemValue = column.GetGroupValue(item);
                if (groupName.Length > 0)
                {
                    groupName.Append('|');
                }
                groupName.Append(key);
                groupName.Append(':');
                groupName.Append(itemValue);
            }                
        }
        return groupName.ToString();
    }

    /// <summary>
    /// Return all groups
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private List<string> AllGroups(TItem item)
    {
        List<string> allGroups = new();
        foreach (int key in _groupRowData.Keys.ToList())
        {
            allGroups.Add(GroupName(item, key));
        }
        return allGroups;
    }

    /// <summary>
    /// Build groups with rows
    /// </summary>
    /// <returns></returns>
    private List<GridGroupLevel<TItem>> GetGroupsLevels()
    {
        List<GridGroupLevel<TItem>> groupsLevels = new();
        while (_rowProvider.TryGetRow(out TItem item))
        {
            List<string> allGroups = AllGroups(item);
            string groupName = allGroups.First();
            GridGroupLevel<TItem> groupLevel = new()
            {
                Key = groupName
            };
            GridGroupLevel<TItem> existingGroupLevel = groupsLevels.Find(g => g.Key.Equals(groupName));
            if (existingGroupLevel != null)
            {
                groupLevel = existingGroupLevel;
            }
            else
            {
                foreach (string key in _groupRowData[0].ToList())
                {
                    if (GridView.TryGetColumn(key, out GridColumnState column))
                    {
                        object columnValue = column.GetGroupValue(item);
                        GridGroupingColumn groupParam = new()
                        {
                            Value = columnValue,
                            Title = column.GetTitle()
                        };
                        groupLevel.Params.Add(column.UniqueKey, groupParam);
                    }
                }

                groupsLevels.Add(groupLevel);
            }

            if (allGroups.Count > 1)
            {
                AddSubGroupLevel(groupLevel, allGroups, item, 1);
            }
            else
            {
                groupLevel.Items.Add(item);
            }
        }
        return groupsLevels;
    }

    /// <summary>
    /// Build sub groups with rows
    /// </summary>
    /// <param name="groupLevel"></param>
    /// <param name="allGroups"></param>
    /// <param name="item"></param>
    /// <param name="groupIndex"></param>
    private void AddSubGroupLevel(GridGroupLevel<TItem> groupLevel, List<string> allGroups, TItem item, int groupIndex)
    {
        string newGroupName = allGroups[groupIndex];
        GridGroupLevel<TItem> newGroupLevel = new()
        {
            Key = newGroupName,
            Level = groupIndex
        };

        GridGroupLevel<TItem> existingGroupLevel = groupLevel.SubLevels.Find(g => g.Key.Equals(newGroupName));
        if (existingGroupLevel != null)
        {
            newGroupLevel = existingGroupLevel;
        }
        else
        {
            foreach (string key in _groupRowData[groupIndex])
            {
                if (GridView.TryGetColumn(key, out GridColumnState column))
                {
                    object columnValue = column.GetGroupValue(item);
                    GridGroupingColumn groupParam = new()
                    {
                        Value = columnValue,
                        Title = column.GetTitle()
                    };
                    newGroupLevel.Params.Add(column.UniqueKey, groupParam);
                }
            }

            groupLevel.SubLevels.Add(newGroupLevel);
        }

        if (allGroups.Count > groupIndex + 1)
        {
            AddSubGroupLevel(newGroupLevel, allGroups, item, groupIndex + 1);
        }
        else
        {
            newGroupLevel.Items.Add(item);
        }
    }

    /// <summary>
    /// Return row state of the current line
    /// </summary>
    /// <param name="group">group name</param>        
    private GridViewGroupState GetGroupRowState(string group)
    {
        if (!GridView.State.GroupsState.TryGetValue(group, out GridViewGroupState groupState))
        {
            groupState = new GridViewGroupState { IsCollapsed = GridView.State.GroupCollapsed };
            GridView.State.GroupsState.Add(group, groupState);
        }
        return groupState;
    }

    /// <summary>
    /// Update collapse row state
    /// </summary>
    /// <param name="group">group name</param>        
    /// <param name="state">group open state</param>
    private void ChangeGroupCollapse(string group, bool state)
    {
        GridView.State.GroupsState[group].IsCollapsed = state;
        GridView.State.GroupCollapsed = false;
    }

    /// <summary>
    /// Return group name
    /// </summary>
    /// <param name="groupData">group data</param>
    /// <returns></returns>
    private static string GroupLabel(Dictionary<string, GridGroupingColumn> groupData)
    {
        IEnumerable<string> labels = groupData.Select(g => "GridViewGroupLabel".Translate(g.Value.Title, g.Value.Value?.ToString()));
        return string.Join(", ", labels);
    }

    #endregion

}
