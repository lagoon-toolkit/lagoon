namespace Lagoon.UI.Components;

/// <summary>
/// Component ColumnOptions.
/// </summary>
public partial class LgColumnOptions : LgAriaComponentBase
{
    #region Fields

    /// <summary>
    /// Lg Modal ref
    /// </summary>
    private LgModal _lgModal;

    /// <summary>
    /// DotNet object reference
    /// </summary>
    private IDisposable _dotnetRef;

    /// <summary>
    /// Columns list reference
    /// </summary>
    private ElementReference _elementReference;

    /// <summary>
    /// Unique list id
    /// </summary>
    private Guid _keyList;

    #endregion        

    #region Parameters

    /// <summary>
    /// Gets or sets list of columns
    /// </summary>
    [Parameter]
    public List<ColumnOption> Columns { get; set; }

    /// <summary>
    /// Gets or sets list of displayed Columns by order
    /// </summary>
    [Parameter]
    public List<ColumnOption> DisplayedColumnsByOrder { get; set; }

    /// <summary>
    /// Gets or sets if the drag and drop is active
    /// </summary>
    [Parameter]
    public bool AllowDragDrop { get; set; } = true;

    /// <summary>
    /// Gets or sets if the frozen is active
    /// </summary>
    [Parameter]
    public bool AllowFrozen { get; set; } = true;

    /// <summary>
    /// Gets or sets if the visibility managment is active
    /// </summary>
    [Parameter]
    public bool AllowVisibility { get; set; } = true;

    #endregion Parameters

    #region events

    /// <summary>
    /// Gets or sets on close modal event
    /// </summary>
    [Parameter]
    public EventCallback<SaveColumnOptionsEventArgs> OnSave { get; set; }

    /// <summary>
    /// Gets or sets on close modal event
    /// </summary>
    [Parameter]
    public EventCallback<CloseModalEventArgs> OnClose { get; set; }

    #endregion

    #region Properties

    /// <summary>
    /// Return true if user cannot add column
    /// </summary>
    /// <returns></returns>
    public bool DisableAddButton => DisplayedColumnsByOrder.Count >= Columns.Count;

    /// <summary>
    /// Return true if user cannot save
    /// </summary>
    /// <returns></returns>
    public bool DisableSaveButton => DisplayedColumnsByOrder.Any(x => string.IsNullOrEmpty(x.Key));

    #endregion Properties

    #region Private Methods

    /// <summary>
    /// Remove the column
    /// </summary>
    /// <param name="column"></param>
    internal void RemoveColumn(ColumnOption column)
    {
        DisplayedColumnsByOrder.Remove(column);
        StateHasChanged();
    }

    /// <summary>
    /// Freeze the column
    /// </summary>
    /// <param name="column"></param>
    internal void FreezeColumn(ColumnOption column)
    {
        column.IsFrozen = !column.IsFrozen;
        int index = DisplayedColumnsByOrder.IndexOf(column);
        int start = column.IsFrozen ? 0 : index + 1;
        int end = column.IsFrozen ? index : DisplayedColumnsByOrder.Count;

        for (int i = start; i < end; i++)
        {
            DisplayedColumnsByOrder[i].IsFrozen = column.IsFrozen;
        }
        StateHasChanged();
    }

    /// <summary>
    /// Add a column
    /// </summary>
    private void AddColumn()
    {
        columnOptionToFocus = new ColumnOption() { Key = null };
        DisplayedColumnsByOrder.Add(columnOptionToFocus);
    }

    /// <summary>
    /// On column change event
    /// </summary>
    /// <param name="args"></param>
    /// <param name="column"></param>
    internal void OnColumnChange(ChangeEventArgs args, ColumnOption column)
    {
        string val = (string)args.Value;

        if (string.IsNullOrEmpty(val))
        {
            return;
        }
        int dropIndex = DisplayedColumnsByOrder.IndexOf(column);
        ColumnOption col = Columns.Find(x => x.Key.Equals(val));
        col.IsVisible = true;
        col.IsFrozen = column.IsFrozen;
        DisplayedColumnsByOrder.Remove(column);
        DisplayedColumnsByOrder.Insert(dropIndex, col);
        StateHasChanged();
    }

    /// <summary>
    /// Get a list of available columns
    /// </summary>
    /// <param name="column"></param>
    /// <returns></returns>
    public List<ColumnOption> GetAvailableColumns(ColumnOption column)
    {
        return Columns.Where(x =>
        {
            return !string.IsNullOrEmpty(column.Key) && x.Key.Equals(column.Key) || !DisplayedColumnsByOrder.Any(y => x.Key.Equals(y.Key));
        }).OrderBy(x => x.Title).ToList();
    }

    /// <summary>
    /// Save column options
    /// </summary>
    /// <returns></returns>
    private async Task SaveColumnOptionsAsync()
    {
        if (OnSave.HasDelegate)
        {
            SaveColumnOptionsEventArgs args = new(DisplayedColumnsByOrder);
            await OnSave.TryInvokeAsync(App, args);
        }

        await _lgModal.CloseAsync();
        await Task.CompletedTask;
    }

    #endregion Private Methods

    #region Drag and drop 

    /// <summary>
    /// Call move column from JS
    /// </summary>
    /// <param name="fromId"></param>
    /// <param name="toId"></param>        
    [JSInvokable]
    public void MoveColumnJS(string fromId, string toId)
    {
        ColumnOption from = DisplayedColumnsByOrder.First(c => c.ElementId == fromId);
        ColumnOption to = DisplayedColumnsByOrder.First(c => c.ElementId == toId);
        MoveColumn(from, to);
        _keyList = Guid.NewGuid();
        StateHasChanged();
    }

    /// <summary>
    /// Change column position
    /// </summary>
    /// <param name="from">column to move</param>
    /// <param name="to">destination column</param>         
    public void MoveColumn(ColumnOption from, ColumnOption to)
    {
        if (from.IsVisible)
        {
            int toIdx = DisplayedColumnsByOrder.IndexOf(to);
            int fromIdx = DisplayedColumnsByOrder.IndexOf(from);
            if (toIdx >= 0 && toIdx < DisplayedColumnsByOrder.Count)
            {
                DisplayedColumnsByOrder.Remove(from);
                DisplayedColumnsByOrder.Insert(toIdx, from);
                int nextIdx = toIdx + 1;
                if (nextIdx < DisplayedColumnsByOrder.Count && DisplayedColumnsByOrder[nextIdx].IsFrozen)
                {
                    from.IsFrozen = true;
                }

                // Freezes the set of columns between the from and to if the column being moved is frozen
                if (from.IsFrozen)
                {
                    for (int i = fromIdx; i < toIdx; i++)
                    {
                        ColumnOption element = DisplayedColumnsByOrder[i];
                        element.IsFrozen = true;
                    }
                }
                columnOptionToFocus = from;
            }
        }
    }

    #endregion Drag and drop

    #region keyboard shortcuts
    /// <summary>
    /// Flag used to focus moved  column after Render
    /// </summary>
    private ColumnOption columnOptionToFocus = null;

    /// <summary>
    /// Invoked when user uses the keyboard shortcuts
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fromId"></param>
    /// <returns></returns>        
    [JSInvokable]
    public void KeyPress(string key, string fromId)
    {
        columnOptionToFocus = null;
        ColumnOption column = DisplayedColumnsByOrder.First(c => c.ElementId == fromId);
        int dropIndex = DisplayedColumnsByOrder.IndexOf(column);
        switch (key)
        {
            case "ArrowUp":
                dropIndex -= 1;
                break;
            case "ArrowDown":
                dropIndex += 1;
                break;
            default:
                break;
        }
        if (dropIndex >= 0 && dropIndex < DisplayedColumnsByOrder.Count)
        {
            MoveColumn(column, DisplayedColumnsByOrder.ElementAt(dropIndex));
            StateHasChanged();
        }
    }

    /// <summary>
    /// Remove column on key Enter and Space
    /// </summary>
    /// <param name="args">Arguments</param>
    /// <param name="column">Column selected</param>
    /// <returns></returns>
    internal void KeyPressDelete(KeyboardEventArgs args, ColumnOption column)
    {
        if (args.Code is "Enter" or "Space")
        {
            RemoveColumn(column);
        }
    }

    #endregion keyboard shortcuts

    #region Protected Methods

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (DisplayedColumnsByOrder != null && DisplayedColumnsByOrder.Any(x => x.IsFrozen))
        {
            int maxIndex = DisplayedColumnsByOrder.Select((x, i) => new { Value = x, Index = i }).Where(y => y.Value.IsFrozen).Max(z => z.Index);
            for (int i = 0; i < maxIndex; i++)
            {
                DisplayedColumnsByOrder[i].IsFrozen = true;
            }
        }
    }

    /// <summary>
    /// Remark ElementReference will be available only after OnAfterRender/OnAfterRenderAsync
    /// </summary>
    /// <param name="firstRender">Is it the first render for this component ?</param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            _dotnetRef = DotNetObjectReference.Create(this);
        }
        // Activate drag and drop JS
        if (AllowDragDrop)
        {
            await JS.InvokeVoidAsync("Lagoon.JsColumnOptions.Init", _elementReference, _dotnetRef);
        }
        if (columnOptionToFocus != null)
        {
            await JS.ScriptGetNewRefAsync("Lagoon.JsUtils.focusElementById", columnOptionToFocus.ElementId);
            columnOptionToFocus = null;
        }
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        _dotnetRef?.Dispose();
        base.Dispose(disposing);
    }

    #endregion Protected Methods

    #region Public Methods

    /// <summary>
    /// invoked to display modal
    /// </summary>
    /// <returns></returns>
    public Task ShowAsync()
    {
        return _lgModal.ShowAsync();
    }

    #endregion Public Methods
}
