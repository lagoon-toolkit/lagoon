namespace Lagoon.UI.Components;

/// <summary>
/// Numeric cell
/// </summary>
/// <typeparam name="TItem">The type of data in the list bound to the grid.</typeparam>
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
/// <typeparam name="TCellValue">The type of value bound to the cell.</typeparam>
public class LgGridNumericCell<TItem, TColumnValue, TCellValue> : LgGridCell<TItem, TColumnValue, TCellValue> where TCellValue : TColumnValue
{
    #region properties

    /// <inheritdoc/>
    internal override Type InputType => typeof(LgNumericBox<TCellValue>);

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void LoadInputAdditionalAttributes(Dictionary<string, object> attributes)
    {
        if (!string.IsNullOrEmpty(Column.InputDisplayFormat))
        {
            attributes.Add(nameof(LgNumericBox<TCellValue>.DisplayFormat), Column.InputDisplayFormat);
        }
    }

    ///<inheritdoc/>
    protected override RenderFragment GetCellContent()
    {
        return builder =>
        {
            LgGridNumericColumn<TColumnValue> numericColumn = Column as LgGridNumericColumn<TColumnValue>;

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "grid-numeric-content");

            // Prefix content
            if (!string.IsNullOrEmpty(numericColumn.Prefix))
            {
                builder.OpenElement(10, "span");
                builder.AddAttribute(11, "class", "grid-numeric-prefix");

                if (numericColumn.PrefixType == InputLabelType.IconName)
                {
                    builder.OpenComponent<LgIcon>(12);
                    builder.AddAttribute(13, nameof(LgIcon.IconName), numericColumn.Prefix);
                    builder.CloseComponent(); // 12
                }
                else
                {
                    builder.AddContent(12, numericColumn.Prefix);
                }

                builder.CloseElement(); // 10
            }
            // content
            builder.OpenElement(20, "span");
            builder.AddAttribute(21, "class", $"grid-numeric-text {(GridView.WrapContent ? "" : "text-truncate")}");
            builder.AddContent(22, FormatValueAsString(CellValue));
            builder.CloseElement(); // 20
            // suffix content
            if (!string.IsNullOrEmpty(numericColumn.Suffix))
            {
                builder.OpenElement(30, "span");
                builder.AddAttribute(31, "class", "grid-numeric-suffix");

                if (numericColumn.SuffixType == InputLabelType.IconName)
                {
                    builder.OpenComponent<LgIcon>(32);
                    builder.AddAttribute(33, nameof(LgIcon.IconName), numericColumn.Suffix);
                    builder.CloseComponent(); // 32
                }
                else
                {
                    builder.AddContent(32, numericColumn.Suffix);
                }

                builder.CloseElement(); // 30
            }

            builder.CloseElement(); // 0
        };
    }

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("gridview-col-num");
    }

    #endregion
}
