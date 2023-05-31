namespace Lagoon.UI.Components;

/// <summary>
/// Component GroupOptions.
/// </summary>
public partial class LgGroupOptions : LgAriaComponentBase
{
    /// <summary>
    /// The dragged Group
    /// </summary>
    private GroupOption _draggedGroup;

    /// <summary>
    /// Lg Modal ref
    /// </summary>
    public LgModal _lgModal;

    #region Parameters

    /// <summary>
    /// Gets or sets list of columns
    /// </summary>
    [Parameter]
    public List<ColumnOption> Columns { get; set; }

    /// <summary>
    /// Gets or sets list of groups by order
    /// </summary>
    [Parameter]
    public List<GroupOption> GroupsByOrder { get; set; }

    /// <summary>
    /// Gets or sets if the drag and drop is active
    /// </summary>
    [Parameter]
    public bool AllowDragDrop { get; set; } = true;

    #endregion Parameters

    #region events

    /// <summary>
    /// Gets or sets on close modal event
    /// </summary>
    [Parameter]
    public EventCallback<SaveGroupOptionsEventArgs> OnSave { get; set; }


    /// <summary>
    /// Gets or sets on close modal event
    /// </summary>
    [Parameter]
    public EventCallback<CloseModalEventArgs> OnClose { get; set; }

    #endregion

    #region Properties

    /// <summary>
    /// Return true if user can not add column
    /// Can not have more than 5 groups
    /// </summary>
    /// <returns></returns>
    public bool DisableAddButton => GroupsByOrder.Count >= 5;

    /// <summary>
    /// Return true if user can not save
    /// Can not have an empty group
    /// </summary>
    /// <returns></returns>
    public bool DisableSaveButton => GroupsByOrder.Any(x => !x.Columns.Any());

    #endregion Properties

    #region Private Methods

    /// <summary>
    /// Remove the group
    /// </summary>
    /// <param name="group"></param>
    private void RemoveGroup(GroupOption group)
    {
        GroupsByOrder.Remove(group);
    }

    /// <summary>
    /// Add a group
    /// </summary>
    private void AddGroup()
    {
        groupOptionToFocus = new GroupOption() { };
        GroupsByOrder.Add(groupOptionToFocus);
    }

    /// <summary>
    /// Get a list of available columns to choose from
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    private List<ColumnOption> GetAvailableColumns(List<string> columns)
    {
        return Columns.Where(x =>
        {
            if (!x.IsGroupable)
            {
                return false;
            }
            if (columns != null && columns.Any(c => c.Equals(x.Key)))
            {
                return true;
            }                    
            if (GroupsByOrder.Any(g => g.Columns.Contains(x.Key)))
            {
                return false;
            }                    
            return true;
        }).ToList();
    }

    /// <summary>
    /// Save group options
    /// </summary>
    /// <returns></returns>
    private async Task SaveGroupOptionsAsync()
    {
        if (OnSave.HasDelegate)
        {
            var args = new SaveGroupOptionsEventArgs(GroupsByOrder);
            await OnSave.TryInvokeAsync(App, args);
        }

        await _lgModal.CloseAsync();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Get a group line CSS class.
    /// </summary>
    /// <param name="groupOption">The grup</param>
    /// <returns>The CSS class.</returns>
    private string GetItemCssClass(GroupOption groupOption)
    {
        LgCssClassBuilder builder = new("group");
        builder.AddIf(groupOption == _draggedGroup, "dragging");
        return builder.ToString();
    }

    #endregion Private Methods

    #region Drag and drop

    /// <summary>
    /// Drag and drop Start
    /// </summary>
    /// <param name="args">Drag event</param>
    /// <param name="draggedGroup">The dragged group</param>
    private void HandleDragStart(DragEventArgs args, GroupOption draggedGroup)
    {
        if (AllowDragDrop)
        {
            args.DataTransfer.DropEffect = "move";
            args.DataTransfer.EffectAllowed = "move";
            _draggedGroup = draggedGroup;
        }
    }

    /// <summary>
    /// Drag and drop End
    /// </summary>
    /// <param name="droppedGroup">the dropped group</param>
    private void HandleDrop(GroupOption droppedGroup)
    {
        if (_draggedGroup != null)
        {
            int dropIndex = GroupsByOrder.IndexOf(droppedGroup);
            GroupsByOrder.Remove(_draggedGroup);
            GroupsByOrder.Insert(dropIndex, _draggedGroup);
            _draggedGroup = null;
        }
    }

    #endregion Drag and drop

    #region keyboard shortcuts
    /// <summary>
    /// Flag used to focus moved group after Render
    /// </summary>
    private GroupOption groupOptionToFocus = null;

    #endregion keyboard shortcuts

    #region Protected Methods

    /// <summary>
    /// Remark ElementReference will be available only after OnAfterRender/OnAfterRenderAsync
    /// </summary>
    /// <param name="firstRender">Is it the first render for this component ?</param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (groupOptionToFocus != null)
        {
            await JS.ScriptGetNewRefAsync("Lagoon.JsUtils.focusElementById", groupOptionToFocus.ElementId);
            groupOptionToFocus = null;
        }
    }

    #endregion Protected Methods

    #region private methods

    /// <summary>
    /// Fill list of selected columns of a group
    /// </summary>
    /// <param name="args"></param>
    /// <param name="columns"></param>
    private static void ChangeColumn(ChangeEventArgs args, List<string> columns)
    {
        if (args.Value is not null)
        {
            columns = (List<string>)args.Value;
        }
        else
        {
            columns = new();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Invoked to display the group options modal
    /// </summary>
    /// <returns></returns>
    public Task ShowAsync()
    {
        return _lgModal.ShowAsync();
    }

    #endregion Public Methods

}
