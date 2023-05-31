using Lagoon.UI.Components.Input.Internal;

namespace Lagoon.UI.Components;

/// <summary>
/// Select cell
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TColumnValue"></typeparam>
/// <typeparam name="TCellValue"></typeparam>
/// <typeparam name="TDataSourceItemValue"></typeparam>
public class LgGridSelectCell<TItem, TColumnValue, TCellValue, TDataSourceItemValue> : LgGridCell<TItem, TColumnValue, TCellValue> where TCellValue : TColumnValue
{
    #region properties

    /// <inheritdoc/>
    internal override Type InputType => typeof(LgSelect<TCellValue>);

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override string FormatValueAsString(TCellValue cellValue)
    {            
        return ((LgGridSelectColumn<TColumnValue, TDataSourceItemValue>)Column).FormatValueAsString(cellValue);
    }

    ///<inheritdoc/>
    protected override RenderFragment GetEditContent()
    {            
        return builder =>
        {
            var column = (LgGridSelectColumn<TColumnValue, TDataSourceItemValue>)Column;
            builder.OpenComponent<LgSelect<TCellValue>>(0);
            builder.AddAttribute(1, nameof(LgSelect<TCellValue>.Value), CellValue);
            builder.AddAttribute(2, nameof(LgSelect<TCellValue>.ValueExpression), CellValueExpression);
            builder.AddAttribute(3, nameof(LgSelect<TCellValue>.ValueChanged), CellValueChanged);
            builder.AddAttribute(4, nameof(LgSelect<TCellValue>.DataSource), column.DataSource);
            builder.AddAttribute(5, nameof(LgSelect<TCellValue>.OnChange), EventCallback.Factory.Create<ChangeEventArgs>(this, SafeOnChangeAsync));
            Func<TCellValue, TCellValue, Task<bool>> onValueChange = OnValueChange;
            builder.AddAttribute(6, nameof(LgSelect<TCellValue>.ValueChangeCallback), onValueChange);
            builder.AddAttribute(7, nameof(LgSelect<TCellValue>.ConfirmationMessage), Column.Confirmation);
            builder.AddAttribute(8, nameof(LgSelect<TCellValue>.ResetButton), column.ResetButton);
            builder.AddComponentReferenceCapture(9, cp => InputBusyReference.Reference = (IInputBusy)cp);                
            builder.CloseComponent();
        };
    }

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("gridview-col-select");
    }

    ///<inheritdoc/>
    protected override async Task OnChangeAsync(ChangeEventArgs args)
    {
        await base.OnChangeAsync(args);
        // Force leave is the cell is locked in edit mode
        if(GridView.EditMode == GridEditMode.Cell && IsEditing)
        {
            await OnLeaveEditModeAsync(false);
        }
    }

    #endregion
}
