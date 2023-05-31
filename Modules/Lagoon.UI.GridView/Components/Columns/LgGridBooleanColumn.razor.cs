using Lagoon.Core.Helpers;

namespace Lagoon.UI.Components;

/// <summary>
/// Checkbox column
/// </summary>    
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
public class LgGridBooleanColumn<TColumnValue> : LgGridBaseValueColumn<TColumnValue>
{

    #region methods

    ///<inheritdoc/>
    internal override GridColumnState CreateState()
    {
        return new GridBooleanColumnState();
    }

    ///<inheritdoc/>
    internal override void OnStateInitialized()
    {
        base.OnStateInitialized();
        // We don't filter the rows to show only used
        State.FilterShowAllItems = true;
        // Load a shared formatter
        if (State is GridBooleanColumnState state)
        {
            state.BooleanFormatter = DisplayFormat is null ? BooleanFormatter.Default : new(DisplayFormat);
        }
    }

    ///<inheritdoc/>
    internal override Type OnGetCellComponentType(Type itemType, Type columnValueType, Type cellValueType)
    {
        return typeof(LgGridCheckboxCell<,,>).MakeGenericType(itemType, columnValueType, cellValueType);
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterType(Type cellValueType)
    {
        return typeof(BooleanFilter<>).MakeGenericType(cellValueType);
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterBoxType(Type cellValueType)
    {
        return typeof(LgBooleanFilterBox<>).MakeGenericType(cellValueType);
    }

    ///<inheritdoc/>
    internal override string FormatValueAsString<TCellValue>(TCellValue value)
    {
        return ((GridBooleanColumnState)State).BooleanFormatter.Format(value);
    }

    #endregion

}
