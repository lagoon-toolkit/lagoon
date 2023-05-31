namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Header cell
/// </summary>
public partial class LgGridHeaderCell : ComponentBase
{
    #region properties

    /// <summary>
    /// Gets sort icon
    /// </summary>
    private string SortIcon
    {
        get
        {
            return Column.State.SortDirection switch
            {
                DataSortDirection.Asc => IconNames.All.ArrowUp,
                DataSortDirection.Desc => IconNames.All.ArrowDown,
                _ => null,
            };
        }
    }

    /// <summary>
    /// Gets value for aria-sort attribut
    /// </summary>
    /// <returns></returns>
    private string AriaSort => Column.State.SortDirection switch
    {
        DataSortDirection.Asc => "ascending",
        DataSortDirection.Desc => "descending",
        _ => "none",
    };

    /// <summary>
    /// Gets if column can be move
    /// </summary>        
    /// <returns></returns>
    private bool IsMovable => Column.GridView.Features.HasFlag(GridFeature.Move);

    /// <summary>
    /// Gets or sets is column is dragged
    /// </summary>
    private bool IsDragged { get; set; }

    /// <summary>
    /// Gets or sets is drag is over
    /// </summary>
    private int DragEnterCount { get; set; }

    /// <summary>
    /// Gets or sets can be dropped indicator
    /// </summary>
    public bool CanDrop { get; set; }

    /// <summary>
    /// Gets drag and drop running indicator
    /// </summary>
    private bool DragRunning => GridView.State.DraggedColumn is not null;

    /// <summary>
    /// Return <c>true</c> if a custom tooltip is defined, <c>false</c> otherwise
    /// </summary>
    private bool IsCustomTooltip
    {
        get
        {
            return Column.State.HeaderTooltip != Column.State.Title;
        }
    }

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets column of the cell
    /// </summary>

    [CascadingParameter]
    public LgGridBaseColumn Column { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// The GridView containing the cell.
    /// </summary>
    internal LgBaseGridView GridView => Column.GridView;

    #endregion

    #region methods

    /// <summary>
    /// Force cell rendering
    /// </summary>
    public void Update()
    {
        StateHasChanged();
    }

    /// <summary>
    /// Change sort direction and raise event
    /// </summary>
    private async Task SortChangeAsync()
    {
        if (Column.State.IsSortable())
        {
            if (Column.State.SortDirection == DataSortDirection.Asc)
            {
                await Column.OnSortChangeAsync(DataSortDirection.Desc);
            }
            else
            {
                await Column.OnSortChangeAsync(DataSortDirection.Asc);
            }
        }
    }

    /// <summary>
    /// Return the content to set in "class" attribute.
    /// </summary>
    /// <returns></returns>
    protected string GetClassAttribute()
    {
        GridColumnState state = Column.State;
        LgCssClassBuilder builder = new(state.GetCellCssClass(), state.HeaderCssClass);
        if (state.IsSortable())
        {
            builder.Add("gridview-sort");
        }
        if (IsDragged)
        {
            builder.Add("gridview-header-dragged");
            if (CanDrop)
            {
                builder.Add("gridview-header-drop");
            }
            else
            {
                builder.Add("gridview-header-no-drop");
            }
        }
        if (DragEnterCount > 0)
        {
            builder.Add("gridview-header-dragover");
        }
        return builder.ToString();
    }

    #region Drag & drop

    /// <summary>
    /// Drag and drop start
    /// </summary>
    /// <param name="args">event arguments</param>        
    private void HandleDragStart(DragEventArgs args)
    {
        if (IsMovable)
        {
            //args.DataTransfer.DropEffect = "move";
            //args.DataTransfer.EffectAllowed = "move"; 
            GridView.State.DraggedColumn = Column.State;
            IsDragged = true;
        }
    }

    /// <summary>
    /// Drag enter cell event
    /// </summary>
    private void HandleDragEnter(DragEventArgs args)
    {
        DragEnterCount++;
        //args.DataTransfer.DropEffect = IsMovable ? "move" : "none";
        //args.DataTransfer.EffectAllowed = IsMovable ? "move" : "none";
    }

    /// <summary>
    /// Drag leave cell event
    /// </summary>
    private void HandleDragLeave()
    {
        DragEnterCount--;
    }

    /// <summary>
    /// Drag stop
    /// </summary>
    /// <param name="args"></param>
    private void HandleDragEnd(DragEventArgs args)
    {
        IsDragged = false;
        DragEnterCount = 0;
    }

    /// <summary>
    /// Drag and drop drop
    /// </summary>        
    private async Task HandleDropAsync()
    {
        GridColumnState draggedColumn = GridView.State.DraggedColumn;
        //TODO check same group ?
        //var sameGroup = string.IsNullOrEmpty(draggedColumn.Options.GroupName) 
        //    || draggedColumn.Options.GroupName == Column.State.Options.GroupName;
        if (draggedColumn != null && IsMovable)
        {
            await GridView.OnColumnMove(new GridViewMoveColumnEventArgs
            {
                DraggedColumn = draggedColumn,
                DroppedColumn = Column.State
            });
        }
        DragEnterCount = 0;
    }

    #endregion

    ///<inheritdoc/>
    private IEnumerable<KeyValuePair<string, object>> GetAdditionalAttributes()
    {
        if (IsCustomTooltip)
        {
            return LgComponentBase.GetTooltipAttributes(Column.State.HeaderTooltip, Column.State.HeaderTooltipIsHtml);
        }
        return null;
    }

    #endregion
}
