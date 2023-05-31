namespace Lagoon.UI.Components;


/// <summary>
/// Cell with text content.
/// </summary>
/// <typeparam name="TItem">The type of data in the list bound to the grid.</typeparam>
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
/// <typeparam name="TCellData">The type of value bound to the cell.</typeparam>
public class LgGridTextCell<TItem, TColumnValue, TCellData> : LgGridCell<TItem, TColumnValue, TCellData> where TCellData:TColumnValue
{

    /// <inheritdoc />
    protected override RenderFragment GetCellContent()
    {
        // If WrapContent is set to false add the 'text-truncate' class which truncate the text if not enought space
        // and enable the tooltip visualization only if required (eg. if the text is truncated)
        if (!GridView.WrapContent)
        {
            string value = FormatValueAsString(CellValue);
            return builder =>
            {
                builder.OpenElement(1, "span");
                builder.AddAttribute(2, "class", "text-truncate");
                builder.AddContent(3, value);
                builder.CloseElement();
            };
        }
        return base.GetCellContent();
    }

}
