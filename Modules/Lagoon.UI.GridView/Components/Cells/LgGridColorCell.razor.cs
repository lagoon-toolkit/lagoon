using Lagoon.UI.Components.Input.Internal;

namespace Lagoon.UI.Components;

/// <summary>
/// Color picker cell
/// </summary>
/// <typeparam name="TItem">The type of data in the list bound to the grid.</typeparam>
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
/// <typeparam name="TCellValue">The type of value bound to the cell.</typeparam>
public class LgGridColorCell<TItem, TColumnValue, TCellValue> : LgGridCell<TItem, TColumnValue, TCellValue> where TCellValue : TColumnValue
{

    #region properties

    /// <inheritdoc/>
    internal override Type InputType => typeof(LgColorPickerBox);

    #endregion

    #region methods         

    /// <inheritdoc/>
    protected override RenderFragment GetCellContent()
    {
        return builder =>
        {
            LgGridColorColumn<TColumnValue>colorColumn = Column as LgGridColorColumn<TColumnValue>;
            builder.OpenComponent<LgGridColorReadOnlyCell>(0);
            builder.AddAttribute(1, nameof(LgGridColorReadOnlyCell.CellValue), CellValue);
            builder.CloseComponent();
        };
    }

    ///<inheritdoc/>
    protected override RenderFragment GetEditContent()
    {
        return builder =>
        {
            LgGridColorColumn<TColumnValue> colorColumn = Column as LgGridColorColumn<TColumnValue>;
            builder.OpenComponent<LgColorPickerBox>(0);
            builder.AddAttribute(1, nameof(LgColorPickerBox.Value), CellValue);
            builder.AddAttribute(2, nameof(LgColorPickerBox.ValueExpression), CellValueExpression);
            builder.AddAttribute(3, nameof(LgColorPickerBox.ValueChanged), CellValueChanged);
            builder.AddAttribute(5, nameof(LgColorPickerBox.Palette), colorColumn.Palette);
            builder.AddAttribute(6, nameof(LgColorPickerBox.OnChange), EventCallback.Factory.Create<ChangeEventArgs>(this, SafeOnChangeAsync));
            builder.AddAttribute(7, nameof(LgColorPickerBox.OpenOnFocus), true);
            builder.AddAttribute(8, nameof(LgColorPickerBox.ConfirmationMessage), Column.Confirmation);
            Func<TCellValue, TCellValue, Task<bool>> onValueChange = OnValueChange;
            builder.AddAttribute(9, nameof(LgColorPickerBox.ValueChangeCallback), onValueChange);
            builder.AddComponentReferenceCapture(10, cp => InputBusyReference.Reference = (IInputBusy)cp);
            builder.CloseComponent();
        };
    }

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("gridview-col-color");

    }

    #endregion

}
