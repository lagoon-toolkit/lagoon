namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Cell containing the filter for the column.
/// </summary>
public partial class LgGridFilterCellValue<TColumnValue, TCellValue> : LgGridFilterCellBase
{

    #region fields

    private Func<TCellValue, string> _formatValue;

    #endregion

    #region constructor

    /// <summary>
    /// New instance.
    /// </summary>
    public LgGridFilterCellValue()
    {
        _formatValue = FormatValue;
        RenderFilterContent = GetFilterContent;
    }

    #endregion

    #region methods

    /// <summary>
    /// Gets the content of the filter cell
    /// </summary>
    /// <returns></returns>
    protected virtual RenderFragment GetFilterContent(FilterContext filterContext)
    {
        if (Column.State.FilterBoxType is null)
        {
            return null;
        }
        return builder =>
        {
            builder.OpenComponent(0, Column.State.FilterBoxType);
            builder.AddAttribute(1, nameof(ILgFilterBox.RawFilter), filterContext.Filter);
            builder.AddAttribute(2, nameof(ILgFilterBox.RawFilterChanged),
                EventCallback.Factory.Create<Filter>(this, OnFilterChangedAsync));
            if (Column is LgGridBaseValueColumn<TColumnValue>)
            {
                builder.AddAttribute(3, nameof(FormatValue), _formatValue);
            }
            Func<CancellationToken, Task<IEnumerable<TCellValue>>> getItems = null;
            // The boolean and enum columns always show all the values, for other columns we show only used values
            if (Column.State.FilterShowAllItems)
            {
                if (Column.State is GridColumnState<TCellValue> state)
                {
                    getItems = state.GetFilterAllItems();
                }
            }
            else
            {
                getItems = GetItemsAsync;
            }
            if (getItems is not null)
            {
                builder.AddAttribute(4, nameof(LgFilterBoxBase<string, TextFilter>.GetItems), getItems);
            }
            // Allowing to hid a tab in filters
            if (Column.State.FilterDisableSelection)
            {
                builder.AddAttribute(5, nameof(ILgFilterBox.ActiveTabs), FilterTab.Rules);
            }
            if (Column.InputMaskOptions is not null)
            {
                builder.AddAttribute(6, nameof(LgTextFilterBox.InputMaskOptions), Column.InputMaskOptions);
            }
            builder.CloseComponent();
        };
    }

    /// <summary>
    /// Method called to format a value of the column.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private string FormatValue(TCellValue value)
    {
        return ((LgGridBaseValueColumn<TColumnValue>)Column).FormatValueAsString(value);
    }

    /// <summary>
    /// Method called when the filter value is changed.
    /// </summary>
    /// <param name="value">The new filter value.</param>
    private Task OnFilterChangedAsync(Filter value)
    {
        return Column.State.SetCurrentFilterAsync(value, true);
    }

    /// <summary>
    /// Return the items to show in the filter box.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    private Task<IEnumerable<TCellValue>> GetItemsAsync(CancellationToken cancellationToken)
    {
        return Column.GridView.GetColumnValuesAsync<TCellValue>(Column.State);
    }

    #endregion

}
