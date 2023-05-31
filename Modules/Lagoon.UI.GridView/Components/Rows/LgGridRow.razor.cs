namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Gridview row render
/// </summary>
public partial class LgGridRow<TItem> : LgComponentBase
{

    #region fields

    /// <summary>
    /// Form of the line
    /// </summary>
    private LgEditForm _editForm;

    /// <summary>
    /// Cell focused in the row
    /// </summary>
    private int? _cellFocusedIndex;

    /// <summary>
    /// Reference to this for JS
    /// </summary>
    private ElementReference _rowReference;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets row class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or set index of the row
    /// </summary>
    [Parameter]
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets if the render is for the add line.
    /// </summary>
    [Parameter]
    public bool IsAddRow { get; set; }

    /// <summary>
    /// Gets or sets row datasource item
    /// </summary>
    [Parameter]
    public TItem Item { get; set; }

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets the parent gridview
    /// </summary>
    [CascadingParameter]
    protected LgGridView<TItem> GridView { get; set; }

    #endregion

    #region properties                     

    /// <summary>
    /// Gets or sets edit state of the line
    /// </summary>
    internal bool RowEditing { get; set; }

    /// <summary>
    /// Gets or sets column span in the current row
    /// </summary>        
    public int ActiveColSpan { get; set; }

    /// <summary>
    /// Gets or sets row selector indicator
    /// </summary>        
    public bool Selected => GridView.IsSelected(Item);

    #endregion

    #region events        

    /// <summary>
    /// Row edit cancellation event
    /// </summary>
    internal List<Func<Task>> RowEditingCancellation = new();

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (IsAddRow)
        {
            GridView.SetAddRow(this);
        }
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (_cellFocusedIndex is not null)
        {
            await JS.InvokeAsync<bool>("Lagoon.GridView.SetFocus", new object[] { _rowReference, _cellFocusedIndex, GridView.EditMode });
            _cellFocusedIndex = null;
        }
    }

    /// <summary>
    /// Gets the CSS class to apply to the row.
    /// </summary>
    /// <returns>The CSS class.</returns>
    private string GetRowCssClass()
    {
        return GridView.RowCssClassSelector?.Invoke(Index, Item);
    }

    /// <summary>
    /// Return style for the current row
    /// </summary>
    /// <returns></returns>
    protected string GetRowStyle()
    {
        return GridView.RowCssClassSelector is null
            ? Index > 1 ? "grid-template-rows: repeat(" + Index + ", auto);" : ""
            : null;
    }

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("gridview-row");
        builder.Add("gridview-body");
        builder.Add(GetRowCssClass());
        if (RowEditing)
        {
            builder.Add("gridview-row-edit");
        }
        else if (Selected)
        {
            builder.Add("gridview-row-selected");
        }
        builder.Add(CssClass);
    }

    /// <summary>
    /// Return current edit form validator
    /// </summary>
    /// <returns></returns>
    internal LgValidator GetValidator()
    {
        return _editForm?.Validator;
    }

    /// <inheritdoc/>
    internal bool ValidateForm()
    {
        if (_editForm is null)
        {
            return false;
        }
        return _editForm.EditContext.Validate();
    }

    /// <summary>
    /// Set row in edit mode on cell focusing
    /// </summary>
    /// <param name="columnIndex"></param>
    internal void SetFocusedCell(int? columnIndex)
    {
        if (columnIndex is not null)
        {
            _cellFocusedIndex = columnIndex;                
        }
        if (GridView.EditMode == GridEditMode.Row && !GridView.HasEditRowActive())
        {
            SetRowInEdit();
        }
        else
        {
            StateHasChanged();
        }         
    }

    /// <summary>
    /// Pass row in edit mode
    /// </summary>
    internal void SetRowInEdit()
    {
        RowEditing = true;
        GridView.SetEditRow(this);
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _editForm?.Dispose();
    }

    /// <summary>
    /// Cancel edit modification
    /// </summary>
    /// <param name="resetData">Undo value indicator</param>
    internal async Task CancelEditAsync(bool resetData)
    {
        if (resetData && _editForm is not null)
        {
            _editForm.ClearValidationMessages(true);
            // Undo row change
            List<Func<Task>> rowEditingCancellation = RowEditingCancellation;
            RowEditingCancellation = new();
            foreach (Func<Task> task in rowEditingCancellation)
            {
                await task();
            }
        }
        RowEditing = false;
        StateHasChanged();
    }

    /// <inheritdoc/>
    internal void ValidateEditContext()
    {
        _editForm?.EditContext?.Validate();
        StateHasChanged();
    }

    /// <summary>
    /// Refresh render of the row clicked
    /// </summary>
    /// <param name="uniqueKey"></param>
    /// <returns></returns>
    internal async Task RowClickAsync(string uniqueKey)
    {
        await GridView.RowClickAsync(this, uniqueKey);
        StateHasChanged();
    }

    /// <summary>
    /// Force row rebuild
    /// </summary>
    public void Refresh()
    {
        StateHasChanged();
    }

    #endregion

}