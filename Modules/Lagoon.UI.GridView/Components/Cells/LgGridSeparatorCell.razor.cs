namespace Lagoon.UI.Components;

/// <summary>
/// Selection cell render
/// </summary>
/// <typeparam name="TItem">The type of data in the list bound to the grid.</typeparam>
public class LgGridSeparatorCell<TItem> : LgGridBaseCell<TItem>
{
    #region methods

    ///<inheritdoc/>
    protected override RenderFragment GetCellContent()
    {
        return builder =>
        {
            builder.OpenElement(10, "div");
            builder.AddAttribute(11, "class", "gridview-separator-round gridview-separator-round-top");
            builder.CloseElement(); // 10
            builder.OpenElement(20, "div");
            builder.AddAttribute(21, "class", "gridview-separator-dotted-line");
            builder.CloseElement(); // 20
            builder.OpenElement(30, "div");
            builder.AddAttribute(31, "class", "gridview-separator-round gridview-separator-round-bottom");
            builder.CloseElement(); // 30
        };
    }

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("gridview-col-separator");
    }

    #endregion
}
