namespace Lagoon.UI.Components;

/// <summary>
/// Date column
/// </summary>
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
public class LgGridDateColumn<TColumnValue> : LgGridBaseValueColumn<TColumnValue>
{

    #region methods

    ///<inheritdoc/>
    internal override Type OnGetCellComponentType(Type itemType, Type columnValueType, Type cellValueType)
    {
        return typeof(LgGridDateCell<,,>).MakeGenericType(itemType, columnValueType, cellValueType);
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterType(Type cellValueType)
    {
        return typeof(DateFilter<>).MakeGenericType(cellValueType);
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterBoxType(Type cellValueType)
    {
        return typeof(LgDateFilterBox<>).MakeGenericType(cellValueType);
    }

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        DisplayFormat ??= CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    }

    #endregion

}
