using Lagoon.UI.Application;

namespace Lagoon.UI.Components;

/// <summary>
/// Select column
/// </summary>
/// <typeparam name="TColumnValue">Type of column values</typeparam>
/// <typeparam name="TDataSourceItem">Type of select values</typeparam>
public class LgGridSelectColumn<TColumnValue, TDataSourceItem> : LgGridBaseValueColumn<TColumnValue>
{
    #region parameters

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    [Parameter]
    public ListDataSource<TDataSourceItem, TColumnValue> DataSource { get; set; }

    /// <summary>
    /// Gets or sets if the reset button is diplayed
    /// </summary>
    [Parameter]
    public bool? ResetButton { get; set; }

    /// <summary>
    /// Get the format of the Select
    /// </summary>
    [Parameter]
    public ExportFormat? ExportFormat { get; set; }

    #endregion        

    #region methods

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        var column = (GridSelectColumnState<TDataSourceItem, TColumnValue>)State;
        column.DataSource = DataSource;
        // Manage reset button displaying
        if (!ResetButton.HasValue)
        {
            ResetButton = !GridView.App.BehaviorConfiguration.Select.HideAlwaysResetButton;
        }
        column.ResetButton = ResetButton;
        column.ExportFormat = ExportFormat ?? GridViewBehaviour.Options.ExportConfiguration.ExportFormat;
    }

    ///<inheritdoc/>
    internal override Type OnGetCellComponentType(Type itemType, Type columnValueType, Type cellValueType)
    {
        return typeof(LgGridSelectCell<,,,>).MakeGenericType(itemType, columnValueType, cellValueType, typeof(TDataSourceItem));
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterType(Type cellValueType)
    {
        return typeof(SelectFilter<>).MakeGenericType(cellValueType);
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterBoxType(Type cellValueType)
    {
        return typeof(LgSelectFilterBox<>).MakeGenericType(cellValueType);
    }

    ///<inheritdoc/>
    internal override GridColumnState CreateState()
    {
        return new GridSelectColumnState<TDataSourceItem, TColumnValue>();
    }

    ///<inheritdoc/>
    internal override string FormatValueAsString<TCellValue>(TCellValue cellValue)
    {
        return ((GridSelectColumnState<TDataSourceItem, TCellValue>)State).GetItemLabel(cellValue);
    }

    #endregion        

}
