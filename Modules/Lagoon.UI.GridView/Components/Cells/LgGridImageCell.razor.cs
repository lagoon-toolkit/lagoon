namespace Lagoon.UI.Components;

/// <summary>
/// Image cell
/// </summary>
/// <typeparam name="TItem">The type of data in the list bound to the grid.</typeparam>
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
/// <typeparam name="TCellValue">The type of value bound to the cell.</typeparam>
public class LgGridImageCell<TItem, TColumnValue, TCellValue> : LgGridCell<TItem, TColumnValue, TCellValue> where TCellValue : TColumnValue
{
    #region methods

    /// <inheritdoc/>
    protected override RenderFragment GetCellContent()
    {
        return builder =>
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TColumnValue));
            LgGridImageColumn<TColumnValue> imageColumn = Column as LgGridImageColumn<TColumnValue>;
            string src = CellValue?.ToString();
            if (!string.IsNullOrEmpty(imageColumn.UrlPrefix))
            {
                src = string.Format("{0}{1}", imageColumn.UrlPrefix, src);
            }

            if (!string.IsNullOrEmpty(imageColumn.UrlSuffix))
            {
                src = string.Format("{0}{1}", src, imageColumn.UrlSuffix);
            }
            builder.OpenElement(0, "img");
            builder.AddAttribute(1, "src", src);
            builder.AddAttribute(2, "width", imageColumn.ImageWidth);
            builder.AddAttribute(3, "height", imageColumn.ImageHeight);
            builder.CloseElement();
        };
    }

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("gridview-col-img");
    }

    #endregion

}
