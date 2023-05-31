namespace Lagoon.UI.Components.Internal;

/// <summary>
/// List of column auto generated
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class LgGridAutoColumnList<TItem> : ComponentBase
{
    #region cascading parameters

    /// <summary>
    /// Parent grid view.
    /// </summary>
    [CascadingParameter]
    public LgGridView<TItem> GridView { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets item to render
    /// </summary>
    [Parameter]
    public TItem Item { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        int sequence = 0;
        foreach (LgGridValueDefinition<TItem> fd in GridView.FieldDefinitionList)
        {
            builder.OpenRegion(sequence);
            builder.OpenComponent(0, fd.ColumnType);
            builder.AddAttribute(1, nameof(LgGridColumn<object>.State.Title), fd.Name);
            builder.AddAttribute(2, LgGridBaseColumn.COLUMN_KEY_PROPERTY_NAME, fd.Name);
            if (Item != null)
            {
                builder.AddAttribute(4, nameof(LgGridColumn<object>.Value), fd.GetValue(Item));
                builder.AddAttribute(5, nameof(LgGridColumn<object>.ValueExpression), fd.GetValueExpression(Item));
                fd.AddValueChangedAttribute(builder, 6, this, Item);
            }
            builder.CloseComponent();
            builder.CloseRegion();
            sequence++;
        }
    }

    #endregion

}
