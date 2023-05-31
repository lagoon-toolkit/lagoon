namespace Lagoon.UI.Components;

/// <summary>
/// Color column
/// </summary>    
/// <typeparam name="TColumnValue">The type of data bound to the column.</typeparam>
public class LgGridColorColumn<TColumnValue> : LgGridBaseValueColumn<TColumnValue>
{

    #region parameters

    /// <summary>
    /// Gets or sets the Palette of availlabled colors.
    /// </summary>
    [Parameter]
    public List<string> Palette { get; set; }

    #endregion Parameters

    #region methods

    ///<inheritdoc/>
    internal override Type OnGetCellComponentType(Type itemType, Type columnValueType, Type cellValueType)
    {
        return typeof(LgGridColorCell<,,>).MakeGenericType(itemType, columnValueType, cellValueType);
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterType(Type cellValueType)
    {
        return null;
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterBoxType(Type cellValueType)
    {
        return null;
    }

    #endregion
}
