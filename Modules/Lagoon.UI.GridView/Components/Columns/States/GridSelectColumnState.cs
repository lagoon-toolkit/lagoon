using Lagoon.UI.GridView.Components.Internal;

namespace Lagoon.UI.Components.Internal;

/// <summary>
/// The state of select column.
/// </summary>
internal class GridSelectColumnState<TDataSourceItem, TColumnValue> : GridColumnState<TColumnValue>
{

    #region properties

    /// <summary>
    /// Gets or sets select items datasource
    /// </summary>
    public ListDataSource<TDataSourceItem, TColumnValue> DataSource { get; set; }

    /// <summary>
    /// Gets or sets if the reset button is displayed
    /// </summary>
    public bool? ResetButton { get; set; }

    public ExportFormat ExportFormat { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Return item label following item selector
    /// </summary>
    /// <param name="value"></param>        
    /// <returns></returns>
    internal string GetItemLabel(TColumnValue value)
    {
        IEnumerable<TDataSourceItem> items = DataSource?.Items;
        if (items is not null)
        {
            TDataSourceItem item = items.FirstOrDefault(i => DataSource.GetItemValue(i).Equals(value));
            if (item is not null)
            {
                return DataSource?.GetItemText(item);
            }
        }
        return value?.ToString();
    }

    /// <summary>
    /// Method to return all the existing items for the filter.
    /// </summary>
    /// <returns>All the existing items for the filter</returns>
    internal override System.Func<CancellationToken, Task<IEnumerable<TColumnValue>>> GetFilterAllItems()
    {
        return GetItemsAsync;
    }

    /// <summary>
    ///  Method to return all the existing items for the filter.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All the existing items for the filter</returns>
    private Task<IEnumerable<TColumnValue>> GetItemsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(DataSource.Items.Select(d => DataSource.GetItemValue(d)));
    }

    ///<inheritdoc/>
    internal override GridViewSummaryFilter GetSummaryFilter()
    {
        return new GridViewSummaryFilter(GetTitle(), ((Filter<TColumnValue>)Filter).ToString(GetItemLabel));
    }

    /// <inheritdoc/>
    internal override IExportColumn<TItem> GetExportColumn<TItem>(string columnTitle, string groupTitle)
    {
        if (CellValueType is null || ParameterizedValueExpression is null)
        {
            return null;
        }
        return (IExportColumn<TItem>)Activator
            .CreateInstance(typeof(ExportSelectColumn<,,>)
            .MakeGenericType(typeof(TItem), CellValueType, typeof(TDataSourceItem)), columnTitle, ParameterizedValueExpression, DataSource, ExportFormat, groupTitle);
    }
    #endregion

}
