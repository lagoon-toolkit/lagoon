namespace Lagoon.UI.Components;

/// <summary>
/// Date cell
/// </summary>
/// <typeparam name="TItem">The type of data in the list bound to the grid.</typeparam>
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
/// <typeparam name="TCellValue">The type of value bound to the cell.</typeparam>
public class LgGridDateCell<TItem, TColumnValue, TCellValue> : LgGridCell<TItem, TColumnValue, TCellValue> where TCellValue : TColumnValue
{     

    #region properties

    /// <inheritdoc/>
    internal override Type InputType => typeof(LgDateBox<TCellValue>);

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void LoadInputAdditionalAttributes(Dictionary<string, object> attributes)
    {
        if (!string.IsNullOrEmpty(Column.DisplayFormat))
        {
            attributes.Add(nameof(LgDateBox<TCellValue>.DisplayFormat), Column.DisplayFormat);
        }
    }

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("gridview-col-date");
    }

    #endregion 
}
