namespace Lagoon.UI.Components;

/// <summary>
/// Gridview column 
/// </summary>
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
public class LgGridColumn<TColumnValue> : LgGridBaseValueColumn<TColumnValue>
{

    #region methods

    ///<inheritdoc/>
    internal override Type OnGetCellComponentType(Type itemType, Type columnValueType, Type cellValueType)
    {
        return typeof(LgGridTextCell<,,>).MakeGenericType(itemType, columnValueType, cellValueType);
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterType(Type cellValueType)
    {
        return typeof(TextFilter);
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterBoxType(Type cellValueType)
    {
        return typeof(LgTextFilterBox);
    }

    ///<inheritdoc/>
    internal override string FormatValueAsString<TCellValue>(TCellValue value)
    {
        return value?.ToString();
    }

    #endregion

}