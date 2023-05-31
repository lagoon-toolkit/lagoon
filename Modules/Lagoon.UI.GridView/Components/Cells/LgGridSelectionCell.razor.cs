namespace Lagoon.UI.Components;

/// <summary>
/// Selection cell render
/// </summary>
/// <typeparam name="TItem">The type of data in the list bound to the grid.</typeparam>
public class LgGridSelectionCell<TItem> : LgGridBaseCell<TItem>
{
    #region cascading parameter

    /// <summary>
    /// Selection change indicator
    /// </summary>
    [CascadingParameter]
    public Guid SelectionState { get; set; }

    #endregion

    #region Properties

    ///<inheritdoc/>
    ///<remarks>Is set to false to fix double click selection problem</remarks>
    protected override bool CanFocus => false;

    #endregion

    #region methods        

    /// <inheritdoc/>
    private async Task OnChangeAsync(ChangeEventArgs args)
    {
        if ((bool)args.Value)
        {
            await GridView.OnAddSelectionAsync(Row.Item);
        }
        else
        {
            await GridView.OnRemoveSelectionAsync(Row.Item);
        }
        Row.Refresh();
    }

    ///<inheritdoc/>
    protected override RenderFragment GetCellContent()
    {
        return builder =>
        {
            if (!IsAdd && !Row.RowEditing)
            {
                builder.OpenComponent<LgCheckBox<bool>>(10);
                builder.AddAttribute(11, nameof(LgCheckBox<bool>.Value), GridView.IsSelected(Row.Item));
                builder.AddAttribute(12, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, OnChangeAsync));
                builder.AddAttribute(13, nameof(LgCheckBox<bool>.PolicyEdit), "*");
                builder.AddAttribute(14, nameof(LgCheckBox<bool>.PolicyVisible), "*");
                builder.CloseComponent();
            }
        };
    }

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("gridview-col-sel");
    }

    #endregion
}
