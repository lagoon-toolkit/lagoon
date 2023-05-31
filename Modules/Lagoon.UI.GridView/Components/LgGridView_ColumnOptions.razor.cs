namespace Lagoon.UI.Components;

/// <summary>
/// Gridview - Column options
/// </summary>
public partial class LgGridView<TItem>
{
    #region fields

    /// <summary>
    /// LgColumnOptions modal ref
    /// </summary>
    private LgColumnOptions _lgColumnOptions;

    #endregion Fields

    #region properties


    #endregion Properties

    /// <summary>
    /// Gets list of columns by order
    /// </summary>
    private List<ColumnOption> ColumnsOptions => ColumnList.OrderBy(x => x.Order).Select(y => new ColumnOption
    {
        Key = y.UniqueKey,
        Title = y.GetFullTitle(),
        IsRemovable = y.AllowHide,
        IsVisible = y.Visible,
        IsFrozen = y.IsFrozen(),
        IsGroupable = y.IsGroupable()

    }).ToList();

    /// <summary>
    /// Gets list of displayed Columns by order
    /// </summary>
    private List<ColumnOption> DisplayedColumnsByOrder => new(ColumnsOptions.Where(c => c.IsVisible));

    #region Private methods

    /// <summary>
    /// Open column options pop-up
    /// </summary>
    /// <returns></returns>
    private Task OpenColumnOptionsModalAsync()
    {
        return _lgColumnOptions.ShowAsync();
    }

    /// <summary>
    /// On save columns options
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async Task OnSaveColumnsOptionsAsync(SaveColumnOptionsEventArgs args)
    {
        int index = 1;
        List<string> visibleColumn = new();
        foreach (ColumnOption column in args.DisplayedColumnsByOrder)
        {
            if (TryGetColumn(column.Key, out var state))
            {
                state.Order = index;
                state.Visible = true;
                state.Frozen = column.IsFrozen;
                visibleColumn.Add(column.Key);
                index++;
            }
        }
        // Hide other columns
        foreach (GridColumnState state in ColumnList)
        {
            if (!visibleColumn.Contains(state.UniqueKey))
            {
                state.Order = index;
                state.Visible = false;
                index++;
            }
        }
        await ResizeColumnsWidthsAsync();
        await UpdateCurrentProfileAsync();
        RebuildRows = true;
        await BuildGridViewStyleAsync();
    }

    #endregion Private methods
}
