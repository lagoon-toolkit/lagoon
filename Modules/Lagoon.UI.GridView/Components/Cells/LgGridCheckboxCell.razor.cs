namespace Lagoon.UI.Components;

/// <summary>
/// Checkbox cell
/// </summary>
/// <typeparam name="TItem">The type of data in the list bound to the grid.</typeparam>
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
/// <typeparam name="TCellValue">The type of value bound to the cell.</typeparam>
public class LgGridCheckboxCell<TItem, TColumnValue, TCellValue> : LgGridCell<TItem, TColumnValue, TCellValue> where TCellValue : TColumnValue
{

    #region fields

    private static bool _allowIndeterminateState = Nullable.GetUnderlyingType(typeof(TCellValue)) is not null;

    #endregion

    #region properties

    /// <inheritdoc/>
    internal override Type InputType => typeof(LgCheckBox<TCellValue>);

    #endregion

    #region Render fragments              

    ///<inheritdoc/>
    protected override string FormatValueAsString(TCellValue cellValue)
    {
        return ((GridBooleanColumnState)Column.State).BooleanFormatter.Format(cellValue);
    }

    ///<inheritdoc/>
    protected override RenderFragment GetEditContent()
    {
        return builder =>
        {
            builder.OpenComponent(0, InputType);
            builder.AddAttribute(1, nameof(LgCheckBox<TCellValue>.Value), CellValue);
            builder.AddAttribute(2, nameof(LgCheckBox<TCellValue>.ValueExpression), CellValueExpression);
            builder.AddAttribute(3, nameof(LgCheckBox<TCellValue>.ValueChanged), CellValueChanged);
            builder.AddAttribute(4, nameof(LgCheckBox<TCellValue>.AllowIndeterminateState), _allowIndeterminateState);
            builder.AddAttribute(5, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, SafeOnChangeAsync));

            builder.AddAttribute(7, nameof(LgCheckBox<TCellValue>.ResetButton), !Application.GridViewBehaviour.Options.ResetInputConfiguration.HideAlwaysResetButton);
            builder.AddAttribute(8, nameof(LgCheckBox<TCellValue>.ResetText), Application.GridViewBehaviour.Options.ResetInputConfiguration.ResetText);
            builder.AddAttribute(9, nameof(LgCheckBox<TCellValue>.ResetTextAriaLabel), Application.GridViewBehaviour.Options.ResetInputConfiguration.ResetTextAriaLabel);
            builder.AddAttribute(10, nameof(LgCheckBox<TCellValue>.UseResetGridViewConfiguration), true);

            builder.AddMultipleAttributes(11, GetInputAdditionalAttributes());
            builder.CloseComponent();
        };
    }

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("gridview-col-chk");

    }

    #endregion

}
