using Lagoon.UI.Components.Input.Internal;

namespace Lagoon.UI.Components;

/// <summary>
/// Numeric cell
/// </summary>
/// <typeparam name="TItem">The type of data in the list bound to the grid.</typeparam>
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
/// <typeparam name="TCellValue">The type of value bound to the cell.</typeparam>
/// <typeparam name="TEnum">Enum type of the column.</typeparam>
public class LgGridEnumCell<TItem, TColumnValue, TCellValue, TEnum> : LgGridCell<TItem, TColumnValue, TCellValue> where TCellValue : TColumnValue where TEnum : Enum
{
    #region properties

    /// <inheritdoc/>
    internal override Type InputType => typeof(LgTextBox);

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override string FormatValueAsString(TCellValue cellValue)
    {
        if (cellValue is TEnum value)
        {
            return ((GridEnumColumnState<TColumnValue>)Column.State).GetEnumDisplayName(value);
        }
        else
        {
            return string.Empty;
        }
    }

    ///<inheritdoc/>
    protected override RenderFragment GetEditContent()
    {
        return builder =>
        {
            builder.OpenComponent<LgSelect<TCellValue>>(0);
            builder.AddAttribute(1, nameof(LgSelect<TCellValue>.Value), CellValue);
            builder.AddAttribute(2, nameof(LgSelect<TCellValue>.ValueExpression), CellValueExpression);
            builder.AddAttribute(3, nameof(LgSelect<TCellValue>.ValueChanged), CellValueChanged);
            builder.AddAttribute(4, nameof(LgSelect<TCellValue>.OnChange), EventCallback.Factory.Create<ChangeEventArgs>(this, SafeOnChangeAsync));
            builder.AddAttribute(5, nameof(LgSelect<TCellValue>.ConfirmationMessage), Column.Confirmation);
            Func<TCellValue, TCellValue, Task<bool>> onValueChange = OnValueChange;
            builder.AddAttribute(6, nameof(LgSelect<TCellValue>.ValueChangeCallback), onValueChange);
            builder.AddComponentReferenceCapture(7, cp => InputBusyReference.Reference = (IInputBusy)cp);
            builder.CloseComponent();
        };
    }

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("gridview-col-enum");
    }

    #endregion
}
