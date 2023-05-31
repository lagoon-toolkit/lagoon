namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Base cell
/// </summary>
public abstract class LgGridBaseCell<TItem> : ComponentBase, IDisposable, IActiveCell
{

    #region parameters

    /// <summary>
    /// Gets or sets Css class of the cell
    /// </summary>
    [Parameter]
    public virtual string CssClass { get; set; }

    /// <summary>
    /// Gets or sets local style
    /// </summary>
    [Parameter]
    public string Style { get; set; }

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets column of the cell
    /// </summary>

    [CascadingParameter]
    public LgGridBaseColumn Column { get; set; }

    /// <summary>
    /// Gets row parent
    /// </summary>
    [CascadingParameter]
    protected LgGridRow<TItem> Row { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// The GridView containing the cell.
    /// </summary>
    internal LgGridView<TItem> GridView => (LgGridView<TItem>)Column.GridView;

    /// <summary>
    /// Gets or sets if cell is always in edit mode
    /// </summary>
    protected bool FullEdit => IsAdd && CanAdd;

    /// <summary>
    /// Gets or sets if an cell for adding
    /// </summary>
    public bool IsAdd => Row?.IsAddRow ?? false;

    /// <summary>
    /// Indicate if the cell is in edit mode
    /// </summary>
    protected bool IsEditing { get; set; }

    /// <summary>
    /// Gets if the cell can be edited 
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanEdit => !IsAdd && Column.CanEdit && (GridView.EditMode != GridEditMode.None) && GridView.IsEditable;

    /// <summary>
    /// Gets if the cell can be added 
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanAdd => IsAdd && Column.CanAdd &&
                (GridView.EditMode != GridEditMode.None);

    /// <summary>
    /// Gets indicator of the cell editing mode
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsInEditMode => (IsEditing || FullEdit || RowEditing) && (CanAdd || CanEdit || FullEdit);


    /// <summary>
    /// Gets cell can enter in edit mode by focus
    /// </summary>
    protected virtual bool CanFocus => !IsAdd && !IsEditing && CanEdit && GridView.CellCanFocus();

    /// <summary>
    /// Gets edit row state
    /// </summary>
    protected bool RowEditing => Row.RowEditing;

    /// <summary>
    /// Gets or sets Input used in edit mode
    /// </summary>
    public InputBusyReference InputBusyReference { get; set; }

    /// <summary>
    /// Gets or sets if the cell has custom edit content
    /// </summary>
    protected bool HasCustomEditContent { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "div");
        builder.AddAttribute(2, "class", GetClassAttribute());
        builder.AddAttribute(3, "role", "gridcell");
        builder.AddAttribute(4, "style", Style);
        builder.AddAttribute(5, "tabindex", "-1");
        builder.AddAttribute(6, "data-row", Row?.Index);
        builder.AddAttribute(7, "data-col", Column.State.Index);
        builder.AddAttribute(8, "aria-colindex", Column.State.Order);
        builder.AddAttribute(9, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, OnCellClickAsync));
        builder.AddAttribute(10, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, OnKeyDownAsync));
        if (CanEdit || IsAdd)
        {
            builder.AddAttribute(11, "onfocus", EventCallback.Factory.Create<FocusEventArgs>(this, OnCellFocus));
            builder.AddAttribute(12, "onfocusout", EventCallback.Factory.Create<FocusEventArgs>(this, OnCellFocusOut));
        }
        BuildCell(13, builder);
        if (!Column.HideContent)
        {
            if (IsInEditMode)
            {
                RenderFragment editFragment = RenderEditContent();
                if (HasCustomEditContent)
                {
                    builder.AddCascadingValueComponent(30, InputBusyReference, editFragment, true);
                }
                else
                {
                    builder.AddContent(30, editFragment);
                }
            }
            else if (!IsAdd)
            {
                builder.AddContent(40, RenderCellContent());
            }
        }
        builder.CloseElement();
    }

    /// <summary>
    /// Call to add attributes to cell
    /// </summary>
    /// <param name="index"></param>
    /// <param name="builder"></param>
    protected virtual void BuildCell(int index, RenderTreeBuilder builder)
    {
    }

    /// <summary>
    /// Force cell rendering
    /// </summary>
    public void Update()
    {
        StateHasChanged();
    }

    /// <summary>
    /// Return the content to set in "class" attribute.
    /// </summary>
    /// <returns></returns>
    protected string GetClassAttribute()
    {
        LgCssClassBuilder builder = new(Column.State.GetCellCssClass(), CssClass);
        OnBuildClassAttribute(builder);
        return builder.ToString();
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected virtual void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        if (Column.State.Frozen)
        {
            builder.Add("gridview-cell-frozen");
        }
        if (CanEdit)
        {
            builder.Add("gridview-cell-editable");
        }
    }

    /// <summary>
    /// Render the content of a readonly cell.
    /// </summary>
    /// <returns>The content of a readonly cell.</returns>        
    internal RenderFragment RenderCellContent()
    {
        RenderFragment customRender = Column.GetCustomCellContent();
        if (customRender is not null)
        {
            return customRender;
        }
        else
        {
            return GetCellContent();
        }
    }

    /// <summary>
    /// Gets the content of the cell in edit mode.
    /// </summary>
    /// <returns>The content of the cell in edit mode.</returns>
    internal RenderFragment RenderEditContent()
    {
        HasCustomEditContent = false;
        RenderFragment customRender = Column.GetCustomEditCellContent();
        if (customRender is not null)
        {
            HasCustomEditContent = true;
            return customRender;
        }
        else
        {
            return GetEditContent();
        }
    }

    /// <summary>
    /// Gets the content of a readonly cell.
    /// </summary>
    /// <returns></returns>        
    protected virtual RenderFragment GetCellContent()
    {
        return null;
    }

    /// <summary>
    /// Gets the content of the edit cell
    /// </summary>
    /// <returns></returns>
    protected virtual RenderFragment GetEditContent()
    {
        return GetCellContent();
    }

    /// <summary>
    /// Event on cell lost focus
    /// </summary>  
    private Task OnCellFocusOut()
    {
        if (IsEditing)
        {
            return OnLeaveEditModeAsync(false);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Open cell in editing on focus
    /// </summary>
    private void OnCellFocus()
    {
        if (CanFocus)
        {
            OnEnterEditModeAsync();
        }
    }

    /// <summary>
    /// Manage cancel value on Ctrl+Z
    /// </summary>
    /// <param name="args"></param>
    protected virtual async Task OnKeyDownAsync(KeyboardEventArgs args)
    {
        if (IsEditing)
        {
            // Set value to previous data
            //object prevValue;
            // TODO return to previous value
            //if (args.CtrlKey && args.Code == "KeyW")
            //{
            //    prevValue = GetPreviousValue();
            //    if (prevValue is not null && column.ValueChanged.HasDelegate)
            //    {
            //        column.ValueChanged.TryInvokeAsync(App, (TColumnData)prevValue);
            //    }
            //}
            // Cancel current change                
            if (args.Code == "Escape")
            {
                await OnLeaveEditModeAsync(true);
            }
        }
    }

    /// <summary>
    /// Propagate cell click
    /// </summary>
    protected virtual async Task OnCellClickAsync()
    {
        if (!IsAdd)
        {
            // Active cell editing                
            if (!CanEdit)
            {
                // Raise the cell click event
                await Column.CellClickAsync(Row.Item);
            }
            // Raise the row grid event                                
            await Row.RowClickAsync(Column.State.UniqueKey);
        }
    }

    /// <summary>
    /// Method called when we enter in edit mode for the cell.
    /// </summary>
    protected virtual Task OnEnterEditModeAsync()
    {
        if (GridView.ActiveCell is not null)
        {
            GridView.ActiveCell.OnLeaveEdit();
        }
        GridView.ActiveCell = this;
        IsEditing = true;
        Row.SetFocusedCell(Column.State.Index);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Method called when we leave the edit mode.
    /// </summary>
    /// <param name="cancelEdition"></param>
    protected virtual Task OnLeaveEditModeAsync(bool cancelEdition)
    {        
        if (GridView.ActiveCell is not null)
        {
            GridView.ActiveCell.OnLeaveEdit();
        }
        if (IsEditing)
        {
            OnLeaveEdit();
        }
        return Task.CompletedTask;
    }



    ///<inheritdoc/>
    public virtual void Dispose()
    {
    }

    /// <summary>
    /// Manage leaving of the active cell
    /// </summary>
    /// <returns></returns>
    public void OnLeaveEdit()
    {
        GridView.ActiveCell = null;
        IsEditing = false;
    }

    #endregion

}
